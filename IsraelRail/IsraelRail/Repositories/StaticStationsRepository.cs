using IsraelRail.Models;
using IsraelRail.Models.ApiModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsraelRail.Repositories
{
    public interface IStaticStations
    {
        string GetStation(E_Station station);
        Dictionary<E_Station, string> GetAllStations();
    }

    public class StaticStationsRepository : IStaticStations
    {
        private Dictionary<E_Station, string> _stations;
        private readonly IRail _rail;

        public StaticStationsRepository(IRail rail)
        {
            _rail = rail;
            GetStationsInforResponse stationsInfo = _rail.GetStationsInfor(Enum.GetValues(typeof(E_Station)).Cast<E_Station>()).Result;
            _stations = stationsInfo.Data.ToDictionary(x => (E_Station)int.Parse(x.StationCode), y => y.Hebrew.StationName);
        }

        public string GetStation(E_Station station)
        {
            if (_stations.ContainsKey(station))
            {
                return _stations[station];
            }
            return null;
        }

        public Dictionary<E_Station, string> GetAllStations()
        {
            return _stations;
        }
    }
}