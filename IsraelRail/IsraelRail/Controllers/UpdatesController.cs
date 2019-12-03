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
    public class UpdatesController : Controller
    {
        private readonly IRail _rail;

        public UpdatesController(IRail rail)
        {
            _rail = rail;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetStationUpdates(int oId, int dId)
        {
            E_Station origin = (E_Station)oId;
            E_Station destination = (E_Station)dId;
            GetStationsInfoResponse updates = await _rail.GetStationsInfo(origin, destination);
            List<StationUpdate> stationUpdates = new List<StationUpdate>();
            foreach (GetStationsInfoResponseData update in updates.Data.OrderBy(x => x.Order))
            {
                StationUpdate stationUpdate = new StationUpdate(update, E_Language.Hebrew);
                stationUpdates.Add(stationUpdate);
            }
            return Json(stationUpdates);
        }

        public async Task<IActionResult> StationUpdates(int oId, int dId)
        {
            E_Station origin = (E_Station)oId;
            E_Station destination = (E_Station)dId;
            GetStationsInfoResponse updates = await _rail.GetStationsInfo(origin, destination);
            List<StationUpdate> stationUpdates = new List<StationUpdate>();
            foreach (GetStationsInfoResponseData update in updates.Data.OrderBy(x => x.Order))
            {
                StationUpdate stationUpdate = new StationUpdate(update, E_Language.Hebrew);
                stationUpdates.Add(stationUpdate);
            }
            return PartialView("_Updates", stationUpdates);
        }
    }
}