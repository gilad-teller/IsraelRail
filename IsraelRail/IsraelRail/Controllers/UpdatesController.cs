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
    public class UpdatesController : Controller
    {
        private readonly IRail _rail;
        private readonly IStaticStations _staticStations;

        public UpdatesController(IRail rail, IStaticStations staticStations)
        {
            _rail = rail;
            _staticStations = staticStations;
        }

        public IActionResult Index()
        {
            IEnumerable<StationLightData> allStations = _staticStations.GetAllStations();
            return View(allStations);
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