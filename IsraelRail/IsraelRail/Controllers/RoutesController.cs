﻿using System;
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

        public IActionResult Index()
        {
            ViewBag.Now = _time.NowInLocal();
            Dictionary<E_Station, string> allStations = _staticStations.GetAllStations();
            return View(allStations);
        }

        public async Task<IActionResult> Routes(int origin, int destination, DateTime dateTime, bool isDepart)
        {
            try
            {
                GetRoutesResponse getRoutesResponse = await _rail.GetRoutes((E_Station)origin, (E_Station)destination, dateTime, isDepart);
                IEnumerable<Models.ViewModels.Route> routes = _railRouteBuilder.BuildRoutes(getRoutesResponse);
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
                _logger.LogError(ex, "Error in Routes Search");
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
                GetRoutesResponse getNextWeekRoutesResponse = await _rail.GetRoutes((E_Station)origin, (E_Station)destination, nextWeek, isDepart);
                IEnumerable<Models.ViewModels.Route> nextWeekRoutes = _railRouteBuilder.BuildRoutes(getNextWeekRoutesResponse);
                if (nextWeekRoutes == null || !nextWeekRoutes.Any())
                {
                    return PartialView("_NoRoutes");
                }
                Models.ViewModels.Route selectedRoute = Tools.SelectRoute(nextWeekRoutes, nextWeek, isDepart);
                DateTime nowNextWeek = _time.NowInLocal().AddDays(7);
                Models.ViewModels.Train selectedTrain = selectedRoute.Trains.FirstOrDefault(x => x.DestinationStop.StopTime.FirstOrDefault() >= nowNextWeek);
                Stop selectedStop = selectedTrain.Stops.FirstOrDefault(x => x.StopTime.FirstOrDefault() >= nowNextWeek);
                E_Station currentOrigin = selectedStop.Station;

                GetRoutesResponse getRoutesResponse = await _rail.GetRoutes(currentOrigin, (E_Station)destination, dateTime, isDepart);
                IEnumerable<Models.ViewModels.Route> routes = _railRouteBuilder.BuildRoutes(getRoutesResponse);
                routes = routes.Where(x => nextWeekRoutes.Contains(x));
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
                _logger.LogError(ex, "Error in Advanced Search");
                return PartialView("Error", new ErrorViewModel()
                {
                    Exception = ex
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRoutes(int origin, int destination, DateTime dateTime, bool isDepart)
        {
            GetRoutesResponse getRoutesResponse = await _rail.GetRoutes((E_Station)origin, (E_Station)destination, dateTime, isDepart);
            IEnumerable<Models.ViewModels.Route> routes = _railRouteBuilder.BuildRoutes(getRoutesResponse);
            return Json(routes);
        }
    }
}