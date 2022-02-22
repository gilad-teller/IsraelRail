using IsraelRail.Models;
using IsraelRail.Models.ApiModels;
using IsraelRail.Models.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IsraelRail.Repositories
{
    public interface IStaticStations
    {
        string GetStation(E_Station station);
        IEnumerable<StationLightData> GetAllStations();
    }

    public class StaticStationsRepository : IStaticStations
    {
        private readonly List<StationLightData> _stations;
        private readonly IRail _rail;
        private readonly ILogger<StaticStationsRepository> _logger;
        private Exception _exception;

        public StaticStationsRepository(IRail rail, ILogger<StaticStationsRepository> logger)
        {
            _logger = logger;
            _rail = rail;
            try
            {
                GetStationsInforResponse stationsInfo = _rail.GetStationsInfor(Enum.GetValues(typeof(E_Station)).Cast<E_Station>()).Result;
                IEnumerable<GetStationsInforResponseData> stations = stationsInfo.Data.OrderBy(x => x.Hebrew.StationName);
                _stations = new List<StationLightData>();
                foreach (GetStationsInforResponseData s in stations)
                {
                    StationLightData lightData = new StationLightData()
                    {
                        Station = (E_Station)int.Parse(s.StationCode),
                        Name = s.Hebrew.StationName,
                        Latitude = s.General.Lat,
                        Longitude = s.General.Long
                    };
                    _stations.Add(lightData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to initialize static stations");
                _exception = ex;
            }
        }

        public string GetStation(E_Station station)
        {
            if (_stations == null || !_stations.Any())
            {
                throw _exception ?? new Exception("No stations information");
            }
            StationLightData lightData = _stations.FirstOrDefault(x => x.Station == station);
            if (lightData != null)
            {
                return lightData.Name;
            }
            return null;
        }

        public IEnumerable<StationLightData> GetAllStations()
        {
            if (_stations == null || !_stations.Any())
            {
                throw _exception ?? new Exception("No stations information");
            }
            return _stations;
        }
    }
}