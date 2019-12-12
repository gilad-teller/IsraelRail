using IsraelRail.Models;
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
        private readonly Dictionary<E_Station, string> _stations;
        private readonly IConfiguration _config;

        public StaticStationsRepository(IConfiguration config)
        {
            _config = config;
            string fileLocation = _config.GetValue<string>("AppSettings:StationsFile");
            string file = File.ReadAllText(fileLocation, Encoding.UTF8);
            _stations = JsonConvert.DeserializeObject<Dictionary<E_Station, string>>(file);
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