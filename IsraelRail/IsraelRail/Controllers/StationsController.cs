using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IsraelRail.Models;
using IsraelRail.Models.ApiModels;
using IsraelRail.Models.ViewModels;
using IsraelRail.RailRepository;
using Microsoft.AspNetCore.Mvc;

namespace IsraelRail.Controllers
{
    public class StationsController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public StationsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
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
    }
}