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
        private readonly ILogger<RoutesController> _logger;

        public RoutesController(IRail rail, IRailRouteBuilder railRouteBuilder, IStaticStations staticStations, ILogger<RoutesController> logger)
        {
            _rail = rail;
            _railRouteBuilder = railRouteBuilder;
            _staticStations = staticStations;
            _logger = logger;
        }

        public IActionResult Index()
        {
            Dictionary<E_Station, string> allStations = _staticStations.GetAllStations();
            return View(allStations);
        }

        public async Task<IActionResult> Routes(int origin, int destination, DateTime dateTime, bool isDepart)
        {
            GetRoutesResponse getRoutesResponse = await _rail.GetRoutes((E_Station)origin, (E_Station)destination, dateTime, isDepart);
            IEnumerable<Models.ViewModels.Route> routes = _railRouteBuilder.BuildRoutes(getRoutesResponse);
            Models.ViewModels.Route routeToShow = null;
            if (isDepart)
            {
                routeToShow = routes.FirstOrDefault(x => x.Trains.FirstOrDefault().OrigintStop.Departure >= dateTime) ?? routes.LastOrDefault();
            }
            else
            {
                routeToShow = routes.LastOrDefault(x => x.Trains.LastOrDefault().DestinationStop.Arrival <= dateTime) ?? routes.FirstOrDefault();
            }
            ViewBag.ToShow = routeToShow.Index;
            return PartialView("_Routes", routes);
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