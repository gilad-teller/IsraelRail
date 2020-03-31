using IsraelRail.Models;
using IsraelRail.Models.ApiModels;
using IsraelRail.Models.ViewModels;
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
        private List<StationLightData> _stations;
        private readonly IRail _rail;

        public StaticStationsRepository(IRail rail)
        {
            _rail = rail;
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

        public string GetStation(E_Station station)
        {
            StationLightData lightData = _stations.FirstOrDefault(x => x.Station == station);
            if (lightData != null)
            {
                return lightData.Name;
            }
            return null;
        }

        public IEnumerable<StationLightData> GetAllStations()
        {
            return _stations;
        }
    }
}