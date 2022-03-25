using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IsraelRail.Models;
using IsraelRail.Models.ApiModels;
using IsraelRail.Models.ViewModels;
using IsraelRail.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IsraelRail.Controllers
{
    public class RoutesController : Controller
    {
        private readonly IRail _rail;
        private readonly IRailRouteBuilder _railRouteBuilder;
        private readonly IStaticStations _staticStations;
        private readonly ITime _time;
        private readonly ILogger<RoutesController> _logger;

        public RoutesController(IRail rail, IRailRouteBuilder railRouteBuilder, IStaticStations staticStations, ITime time, ILogger<RoutesController> logger)
        {
            _rail = rail;
            _railRouteBuilder = railRouteBuilder;
            _staticStations = staticStations;
            _time = time;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.Now = _time.NowInLocal();
                IEnumerable<StationLightData> allStations = await _staticStations.GetAllStations();
                return View(allStations);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Failed Routes main page");
                return PartialView("Error", new ErrorViewModel()
                {
                    Exception = ex
                });
            }
        }

        public async Task<IActionResult> Routes(string origin, string destination, DateTime dateTime, bool isDepart)
        {
            try
            {
                GetRoutesResponse getRoutesResponse = await _rail.GetRoutes(origin, destination, dateTime, isDepart);
                IEnumerable<TrainAvailableChairsResponse> chairsResponses = await GetChairsResponses(getRoutesResponse);
                IEnumerable<Models.ViewModels.Route> routes = await _railRouteBuilder.BuildRoutes(getRoutesResponse, chairsResponses);
                if (routes == null || !routes.Any())
                {
                    return PartialView("_NoRoutes");
                }
                Models.ViewModels.Route routeToShow = Tools.SelectRoute(routes, dateTime, isDepart);
                ViewBag.ToShow = routeToShow != null ? routeToShow.Index : 0;
                return PartialView("_Routes", routes);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Failed Routes({origin}, {destination}, {dateTime:yyyy-MM-ddTHH:mm}, {isDepart})");
                return PartialView("Error", new ErrorViewModel()
                {
                    Exception = ex
                });
            }
        }

        public async Task<IActionResult> Advanced(string origin, string destination, DateTime dateTime, bool isDepart)
        {
            try
            {
                DateTime nextWeek = dateTime.AddDays(7);
                GetRoutesResponse getNextWeekRoutesResponse = await _rail.GetRoutes(origin, destination, nextWeek, isDepart);
                IEnumerable<TrainAvailableChairsResponse> nextWeekChairsResponses = await GetChairsResponses(getNextWeekRoutesResponse);
                IEnumerable<Models.ViewModels.Route> nextWeekRoutes = await _railRouteBuilder.BuildRoutes(getNextWeekRoutesResponse, nextWeekChairsResponses);
                if (nextWeekRoutes == null || !nextWeekRoutes.Any())
                {
                    return PartialView("_NoRoutes");
                }
                Models.ViewModels.Route selectedRoute = Tools.SelectRoute(nextWeekRoutes, nextWeek, isDepart);
                DateTime nowNextWeek = _time.NowInLocal().AddDays(7);
                Models.ViewModels.Train selectedTrain = selectedRoute.Trains.FirstOrDefault(x => x.DestinationStop.StopTime.FirstOrDefault() >= nowNextWeek);
                Stop selectedStop = selectedTrain.Stops.FirstOrDefault(x => x.StopTime.FirstOrDefault() >= nowNextWeek);
                string currentOrigin = selectedStop.Station;
                if (currentOrigin == destination)
                {
                    int selectedIndex = selectedTrain.Stops.IndexOf(selectedStop);
                    selectedStop = selectedTrain.Stops.ElementAtOrDefault(selectedIndex - 1);
                    currentOrigin = selectedStop.Station;
                }

                GetRoutesResponse getRoutesResponse = await _rail.GetRoutes(currentOrigin, destination, dateTime, isDepart);
                IEnumerable<TrainAvailableChairsResponse> chairsResponses = await GetChairsResponses(getRoutesResponse);
                IEnumerable<Models.ViewModels.Route> routes = await _railRouteBuilder.BuildRoutes(getRoutesResponse, chairsResponses);
                routes = routes.Where(x => nextWeekRoutes.Contains(x));
                if (routes == null || !routes.Any())
                {
                    return PartialView("_NoRoutes");
                }

                foreach (Models.ViewModels.Route route in routes)
                {
                    Stop ori = route.Trains.FirstOrDefault().Stops.FirstOrDefault(x => x.Station == origin);
                    if (ori != null)
                    {
                        route.Trains.FirstOrDefault().OrigintStop = ori;
                    }
                }

                Models.ViewModels.Route routeToShow = Tools.SelectRoute(routes, dateTime, isDepart);
                ViewBag.ToShow = routeToShow != null ? routeToShow.Index : 0;
                return PartialView("_Routes", routes);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Failed Advanced({origin}, {destination}, {dateTime:yyyy-MM-ddTHH:mm}, {isDepart})");
                return PartialView("Error", new ErrorViewModel()
                {
                    Exception = ex
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRoutes(string origin, string destination, DateTime dateTime, bool isDepart)
        {
            try
            {
                GetRoutesResponse getRoutesResponse = await _rail.GetRoutes(origin, destination, dateTime, isDepart);
                IEnumerable<TrainAvailableChairsResponse> chairsResponses = await GetChairsResponses(getRoutesResponse);
                IEnumerable<Models.ViewModels.Route> routes = await _railRouteBuilder.BuildRoutes(getRoutesResponse, chairsResponses);
                return Json(routes);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Failed GetRoutes({origin}, {destination}, {dateTime:yyyy-MM-ddTHH:mm}, {isDepart})");
                return StatusCode(500, ex);
            }
        }

        private async Task<IEnumerable<TrainAvailableChairsResponse>> GetChairsResponses(GetRoutesResponse getRoutesResponse)
        {
            IEnumerable<TrainAvailableChairsRequest> chairsRequests = _rail.TrainAvailableChairsRequestBuilder(getRoutesResponse);
            List<Task<TrainAvailableChairsResponse>> tasks = new List<Task<TrainAvailableChairsResponse>>();
            foreach (TrainAvailableChairsRequest chairsRequest in chairsRequests)
            {
                tasks.Add(_rail.TrainAvailableChairs(chairsRequest));
            }
            IEnumerable<TrainAvailableChairsResponse> chairsResponses;
            try
            {
                chairsResponses = await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                chairsResponses = tasks.Where(t => t.Status == TaskStatus.RanToCompletion).Select(t => t.Result).ToList();
                _logger.LogError(ex, "GetChairsResponses failed");
            }
            return chairsResponses;
        }
    }
}