using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using IsraelRail.Models;
using IsraelRail.Models.ApiModels;
using IsraelRail.Models.ViewModels;
using IsraelRail.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IsraelRail.Controllers
{
    public class StationsController : Controller
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

        public IActionResult Index()
        {
            IEnumerable<StationLightData> allStations = _staticStations.GetAllStations();
            return View(allStations);
        }

        [HttpGet]
        public async Task<IActionResult> GetStationData(int id)
        {
            E_Station station = (E_Station)id;
            GetStationsInforResponse stationInfo = await _rail.GetStationsInfor(station);
            StationData stationData = new StationData(stationInfo.Data.FirstOrDefault(x => x.StationCode == id.ToString()), E_Language.Hebrew);
            return Json(stationData);
        }

        public async Task<IActionResult> Station(int id)
        {
            E_Station station = (E_Station)id;
            GetStationsInforResponse stationInfo = await _rail.GetStationsInfor(station);
            StationData stationData = new StationData(stationInfo.Data.FirstOrDefault(x => x.StationCode == id.ToString()), E_Language.Hebrew); 
            ViewBag.GoogleMapsUrl = _google.GetGoogleMapsUrl(stationData);
            return PartialView("_Station", stationData);
        }

        [HttpGet]
        public async Task<IActionResult> GetStationsDictionary()
        {
            GetStationsInforResponse stationsInfo = await _rail.GetStationsInfor(Enum.GetValues(typeof(E_Station)).Cast<E_Station>());
            Dictionary<int, string> res = stationsInfo.Data.ToDictionary(x => int.Parse(x.StationCode), y => y.Hebrew.StationName);
            return Json(res);
        }
    }
}