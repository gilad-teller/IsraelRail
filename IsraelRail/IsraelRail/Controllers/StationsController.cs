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
using IsraelRail.RailRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IsraelRail.Controllers
{
    public class StationsController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;

        public StationsController(IHttpClientFactory clientFactory, IConfiguration config)
        {
            _clientFactory = clientFactory;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetStationData(int id)
        {
            E_Station station = (E_Station)id;
            StationsRepository stationsRepository = new StationsRepository(_clientFactory);
            GetStationsInforResponse stationInfo = await stationsRepository.GetStationsInfor(station);
            StationData stationData = new StationData(stationInfo.Data.FirstOrDefault(x => x.StationCode == id.ToString()), E_Language.Hebrew);
            return Json(stationData);
        }

        public async Task<IActionResult> Station(int id)
        {
            E_Station station = (E_Station)id;
            StationsRepository stationsRepository = new StationsRepository(_clientFactory);
            GetStationsInforResponse stationInfo = await stationsRepository.GetStationsInfor(station);
            StationData stationData = new StationData(stationInfo.Data.FirstOrDefault(x => x.StationCode == id.ToString()), E_Language.Hebrew); 
            ViewBag.GoogleMapsUrl = GetGoogleMapsUrl(stationData);
            return PartialView("_Station", stationData);
        }

        private string GetGoogleMapsUrl(StationData stationData)
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["center"] = $"{stationData.Latitude},{stationData.Longitude}";
            parameters.Add("zoom", "15");
            parameters.Add("size", "540x320");
            parameters.Add("key", _config.GetValue<string>("AppSettings:GoogleApiKey"));
            UriBuilder ub = new UriBuilder("https://maps.googleapis.com/maps/api/staticmap")
            {
                Query = HttpUtility.UrlDecode(parameters.ToString())
            };
            return ub.ToString();
        }

        private string GetGoogleMapsUrl2(StationData stationData)
        {
            StringBuilder sb = new StringBuilder("https://maps.googleapis.com/maps/api/staticmap?");
            sb.Append("size=540x320");
            sb.AppendFormat("&markers=color:red%7Clabel:R%7C{0},{1}", stationData.Latitude, stationData.Longitude);
            if (stationData.Parking != null && stationData.Parking.Any())
            {
                sb.Append("&markers=color:blue%7Clabel:P");
                foreach (StationParking sp in stationData.Parking)
                {
                    sb.AppendFormat("%7C{0},{1}", sp.Latitude, sp.Longitude);
                }
            }
            sb.AppendFormat("&key={0}", _config.GetValue<string>("AppSettings:GoogleApiKey"));
            return sb.ToString();
        }
    }
}