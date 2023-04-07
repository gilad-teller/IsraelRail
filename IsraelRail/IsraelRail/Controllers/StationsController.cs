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
    public class StationsController : BaseController
    {
        private readonly IRail _rail;
        private readonly IGoogle _google;
        private readonly ILogger<StationsController> _logger;
        private readonly IStaticStations _staticStations;

        public StationsController(IRail rail, IGoogle google, IStaticStations staticStations, ILogger<StationsController> logger)
        {
            _rail = rail;
            _google = google;
            _staticStations = staticStations;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<StationLightData> allStations = await _staticStations.GetAllStations();
                return View(allStations);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Failed Stations main page");
                return PartialView("Error", new ErrorViewModel()
                {
                    Exception = ex
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStationData(int id)
        {
            try
            {
                StationLightData stationLightData = await _staticStations.GetStation(id);
                StationInformationResponse stationInfo = await _rail.GetStationInformation(id);
                StationData stationData = new StationData(stationInfo.Result, stationLightData);
                return Ok(stationData);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Failed GetStationData({id})");
                return StatusCode(500, ex);
            }
        }

        public async Task<IActionResult> Station(int id)
        {
            try
            {
                StationLightData stationLightData = await _staticStations.GetStation(id);
                StationInformationResponse stationInfo = await _rail.GetStationInformation(id);
                StationData stationData = new StationData(stationInfo.Result, stationLightData);
                ViewBag.GoogleMapsUrl = _google.GetGoogleMapsUrl(stationData);
                return PartialView("_Station", stationData);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Failed Station({id})");
                return PartialView("Error", new ErrorViewModel()
                {
                    Exception = ex
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStationsDictionary()
        {
            try
            {
                IEnumerable<StationLightData> allStations = await _staticStations.GetAllStations();
                Dictionary<int, string> res = allStations.ToDictionary(x => x.Id, y => y.Name);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Failed GetStationsDictionary()");
                return StatusCode(500, ex);
            }
        }
    }
}