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
        Task<string> GetStationName(int station);
        Task<StationLightData> GetStation(int station);
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
                StationsResponse stationsResponse = await _rail.Stations();
                IEnumerable<StationsResult> stations = stationsResponse.Result.OrderBy(x => x.StationName);
                _stations = new List<StationLightData>();
                foreach (StationsResult s in stations)
                {
                    StationLightData lightData = new StationLightData()
                    {
                        Id = s.StationId,
                        Name = s.StationName,
                        Latitude = s.Location.Latitude,
                        Longitude = s.Location.Lontitude
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

        public async Task<string> GetStationName(int station)
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

        public async Task<StationLightData> GetStation(int station)
        {
            if (_stations == null || !_stations.Any())
            {
                await Initialize();
            }
            StationLightData lightData = _stations.FirstOrDefault(x => x.Id == station);
            if (lightData != null)
            {
                return lightData;
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