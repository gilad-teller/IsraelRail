using IsraelRail.Models;
using IsraelRail.Models.ApiModels;
using IsraelRail.Models.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IsraelRail.Repositories
{
    public interface IStaticStations
    {
        Task<string> GetStation(string station);
        Task<IEnumerable<StationLightData>> GetAllStations();
    }

    public class StaticStationsRepository : IStaticStations
    {
        private List<StationLightData> _stations;
        private readonly IRail _rail;
        private readonly ILogger<StaticStationsRepository> _logger;

        public StaticStationsRepository(IRail rail, ILogger<StaticStationsRepository> logger)
        {
            _logger = logger;
            _rail = rail;
        }

        private async Task Initialize()
        {
            try
            {
                GetStationsSuggestionResponse stationsSuggestions = await _rail.GetStationsSuggestion();
                IEnumerable<string> stationIds = stationsSuggestions.Data.CustomPropertys.Select(x => x.Id);
                GetStationsInforResponse stationsInfo = await _rail.GetStationsInfor(stationIds);
                IEnumerable<GetStationsInforResponseData> stations = stationsInfo.Data.OrderBy(x => x.Hebrew.StationName);
                _stations = new List<StationLightData>();
                foreach (GetStationsInforResponseData s in stations)
                {
                    StationLightData lightData = new StationLightData()
                    {
                        Id = s.StationCode,
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
                throw;
            }
        }

        public async Task<string> GetStation(string station)
        {
            if (_stations == null || !_stations.Any())
            {
                await Initialize();
            }
            StationLightData lightData = _stations.FirstOrDefault(x => x.Id == station);
            if (lightData != null)
            {
                return lightData.Name;
            }
            return null;
        }

        public async Task<IEnumerable<StationLightData>> GetAllStations()
        {
            if (_stations == null || !_stations.Any())
            {
                await Initialize();
            }
            return _stations;
        }
    }
}