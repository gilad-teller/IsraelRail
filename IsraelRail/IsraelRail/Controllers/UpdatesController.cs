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
    public class UpdatesController : Controller
    {
        private readonly IRail _rail;
        private readonly IStaticStations _staticStations;
        private readonly ILogger<UpdatesController> _logger;

        public UpdatesController(IRail rail, IStaticStations staticStations, ILogger<UpdatesController> logger)
        {
            _rail = rail;
            _staticStations = staticStations;
            _logger = logger;
        }

        public IActionResult Index()
        {
            IEnumerable<StationLightData> allStations = _staticStations.GetAllStations();
            return View(allStations);
        }

        [HttpGet]
        public async Task<IActionResult> GetStationUpdates(int oId, int dId)
        {
            try
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
                return Ok(stationUpdates);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Failed GetStationUpdates({oId}, {dId})");
                return StatusCode(500, ex);
            }
        }

        public async Task<IActionResult> StationUpdates(int oId, int dId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Failed StationUpdates({oId}, {dId})");
                return PartialView("Error", new ErrorViewModel()
                {
                    Exception = ex
                });
            }
        }
    }
}