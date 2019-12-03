using IsraelRail.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IsraelRail.Repositories
{
    public interface IGoogle
    {
        string GetGoogleMapsUrl(StationData stationData);
    }

    public class GoogleApiRepository : IGoogle
    {
        private readonly IConfiguration _config;

        public GoogleApiRepository(IConfiguration config)
        {
            _config = config;
        }

        public string GetGoogleMapsUrl(StationData stationData)
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters.Add("center", $"{stationData.Latitude},{stationData.Longitude}");
            parameters.Add("zoom", "15");
            parameters.Add("size", "540x320");
            parameters.Add("key", _config.GetValue<string>("AppSettings:GoogleApiKey"));
            UriBuilder ub = new UriBuilder("https://maps.googleapis.com/maps/api/staticmap")
            {
                Query = HttpUtility.UrlDecode(parameters.ToString())
            };
            return ub.ToString();
        }
    }

    public class GoogleApiRepositoryWithPArking : IGoogle
    {
        private readonly IConfiguration _config;

        public GoogleApiRepositoryWithPArking(IConfiguration config)
        {
            _config = config;
        }

        public string GetGoogleMapsUrl(StationData stationData)
        {
            StringBuilder sb = new StringBuilder("https://maps.googleapis.com/maps/api/staticmap?");
            sb.Append("size=540x320");
            sb.AppendFormat("&markers=color:red%7Clabel:R%7C{0},{1}", stationData.Latitude, stationData.Longitude);
            if (stationData.Parking != null && stationData.Parking.Any())
            {
                sb.Append("&markers=color:blue%7Clabel:P");
                foreach (StationParking sp in stationData.Parking)
                {
                    sb.AppendFormat("%7C{0},{1}", sp.Longitude, sp.Latitude);
                }
            }
            sb.AppendFormat("&key={0}", _config.GetValue<string>("AppSettings:GoogleApiKey"));
            return sb.ToString();
        }
    }
}
