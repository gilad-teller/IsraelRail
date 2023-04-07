using IsraelRail.Models.ApiModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;

namespace IsraelRail.Repositories
{
    public interface IRail
    {
        Task<StationInformationResponse> GetStationInformation(int stationId);
        Task<TimetableResponse> Timetable(int fromStation, int toStation, DateTime dateTime, ScheduleType scheduleType);
        Task<StationsResponse> Stations();
    }

    public class RailRepository : IRail
    {
        private readonly IHttpClientFactory _clientFactory;

        public RailRepository(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<StationInformationResponse> GetStationInformation(int stationId)
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["stationId"] = stationId.ToString();
            parameters["systemType"] = "2";
            parameters["languageId"] = "Hebrew";
            UriBuilder ub = new UriBuilder()
            {
                Path = "common/api/v1/Stations/GetStationInformation",
                Query = HttpUtility.UrlDecode(parameters.ToString())
            };
            string uri = ub.Uri.PathAndQuery;
            HttpClient client = _clientFactory.CreateClient("RailApi");
            using (HttpResponseMessage response = await client.GetAsync(uri))
            {
                response.EnsureSuccessStatusCode();
                StationInformationResponse result = await response.Content.ReadAsAsync<StationInformationResponse>();
                if (result.ErrorMessages != null && result.ErrorMessages.Any())
                {
                    throw new Exception(string.Join(", ", result.ErrorMessages));
                }
                return result;
            }
        }

        public async Task<TimetableResponse> Timetable(int fromStation, int toStation, DateTime dateTime, ScheduleType scheduleType)
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["fromStation"] = fromStation.ToString();
            parameters["toStation"] = toStation.ToString();
            parameters["date"] = dateTime.ToString("yyyy-MM-dd");
            parameters["hour"] = dateTime.ToString("HH:mm");
            parameters["scheduleType"] = ((int)scheduleType).ToString();
            parameters["systemType"] = "2";
            parameters["languageId"] = "Hebrew";
            UriBuilder ub = new UriBuilder()
            {
                Path = "rjpa-prod/api/v1/timetable/searchTrainLuzForDateTime",
                Query = HttpUtility.UrlDecode(parameters.ToString())
            };
            string uri = ub.Uri.PathAndQuery;
            HttpClient client = _clientFactory.CreateClient("RailApi");
            using (HttpResponseMessage response = await client.GetAsync(uri))
            {
                response.EnsureSuccessStatusCode();
                TimetableResponse result = await response.Content.ReadAsAsync<TimetableResponse>();
                if (result.ErrorMessages != null && result.ErrorMessages.Any())
                {
                    throw new Exception(string.Join(", ", result.ErrorMessages));
                }
                return result;
            }
        }

        public async Task<StationsResponse> Stations()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["systemType"] = "2";
            parameters["languageId"] = "Hebrew";
            UriBuilder ub = new UriBuilder()
            {
                Path = "common/api/v1/stations",
                Query = HttpUtility.UrlDecode(parameters.ToString())
            };
            string uri = ub.Uri.PathAndQuery;
            HttpClient client = _clientFactory.CreateClient("RailApi");
            using (HttpResponseMessage response = await client.GetAsync(uri))
            {
                response.EnsureSuccessStatusCode();
                StationsResponse result = await response.Content.ReadAsAsync<StationsResponse>();
                if (result.ErrorMessages != null && result.ErrorMessages.Any())
                {
                    throw new Exception(string.Join(", ", result.ErrorMessages));
                }
                return result;
            }
        }
    }
}
