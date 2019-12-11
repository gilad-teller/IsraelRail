using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IsraelRail.Models;
using IsraelRail.Models.ApiModels;
using IsraelRail.Models.ViewModels;
using IsraelRail.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace IsraelRail.Controllers
{
    public class RoutesController : Controller
    {
        private readonly IRail _rail;

        public RoutesController(IRail rail)
        {
            _rail = rail;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRoutes(int origin, int destination, DateTime dateTime, bool isGoing)
        {

            GetRoutesResponse getRoutesResponse = await _rail.GetRoutes((E_Station)origin, (E_Station)destination, dateTime, isGoing);
            RoutesBuilder routesBuilder = new RoutesBuilder();
            IEnumerable<Models.ViewModels.Route> routes = routesBuilder.BuildRoutes(getRoutesResponse);
            return Json(routes);
        }
    }
}