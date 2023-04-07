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
    public class RoutesController : BaseController
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

        public async Task<IActionResult> Routes(int origin, int destination, DateTime dateTime, bool isDepart)
        {
            try
            {
                TimetableResponse timetableResponse = await _rail.Timetable(origin, destination, dateTime, isDepart ? ScheduleType.OriginTime : ScheduleType.DestinationTime);
                IEnumerable<Route> routes = await _railRouteBuilder.BuildRoutes(timetableResponse.Result);
                if (routes == null || !routes.Any())
                {
                    return PartialView("_NoRoutes");
                }
                ViewBag.ToShow = timetableResponse.Result.StartFromIndex;
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

        public async Task<IActionResult> Advanced(int origin, int destination, DateTime dateTime, bool isDepart)
        {
            try
            {
                DateTime nextWeek = dateTime.AddDays(7);
                TimetableResponse nextWeekTimetableResponse = await _rail.Timetable(origin, destination, dateTime, isDepart ? ScheduleType.OriginTime : ScheduleType.DestinationTime);
                IEnumerable<Route> nextWeekRoutes = await _railRouteBuilder.BuildRoutes(nextWeekTimetableResponse.Result);
                if (nextWeekRoutes == null || !nextWeekRoutes.Any())
                {
                    return PartialView("_NoRoutes");
                }
                Route selectedRoute = Tools.SelectRoute(nextWeekRoutes, nextWeek, isDepart);
                DateTime nowNextWeek = _time.NowInLocal().AddDays(7);
                Models.ViewModels.Train selectedTrain = selectedRoute.Trains.FirstOrDefault(x => x.DestinationStop.StopTime.FirstOrDefault() >= nowNextWeek);
                Stop selectedStop = selectedTrain.Stops.FirstOrDefault(x => x.StopTime.FirstOrDefault() >= nowNextWeek);
                int currentOrigin = selectedStop.Station;
                if (currentOrigin == destination)
                {
                    int selectedIndex = selectedTrain.Stops.IndexOf(selectedStop);
                    selectedStop = selectedTrain.Stops.ElementAtOrDefault(selectedIndex - 1);
                    currentOrigin = selectedStop.Station;
                }

                TimetableResponse timetableResponse = await _rail.Timetable(origin, destination, dateTime, isDepart ? ScheduleType.OriginTime : ScheduleType.DestinationTime);
                IEnumerable<Route> routes = await _railRouteBuilder.BuildRoutes(timetableResponse.Result);
                routes = routes.Where(x => nextWeekRoutes.Contains(x));
                if (routes == null || !routes.Any())
                {
                    return PartialView("_NoRoutes");
                }

                foreach (Route route in routes)
                {
                    Stop ori = route.Trains.FirstOrDefault().Stops.FirstOrDefault(x => x.Station == origin);
                    if (ori != null)
                    {
                        route.Trains.FirstOrDefault().OrigintStop = ori;
                    }
                }

                Route routeToShow = Tools.SelectRoute(routes, dateTime, isDepart);
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
        public async Task<IActionResult> GetRoutes(int origin, int destination, DateTime dateTime, bool isDepart)
        {
            try
            {
                TimetableResponse timetableResponse = await _rail.Timetable(origin, destination, dateTime, isDepart ? ScheduleType.OriginTime : ScheduleType.DestinationTime);
                IEnumerable<Route> routes = await _railRouteBuilder.BuildRoutes(timetableResponse.Result);
                return Json(routes);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Failed GetRoutes({origin}, {destination}, {dateTime:yyyy-MM-ddTHH:mm}, {isDepart})");
                return StatusCode(500, ex);
            }
        }
    }
}