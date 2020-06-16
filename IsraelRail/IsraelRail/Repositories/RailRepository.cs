using IsraelRail.Models;
using IsraelRail.Models.ApiModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IsraelRail.Repositories
{
    public interface IRail
    {
        Task<GetStationsInforResponse> GetStationsInfor(E_Station station);
        Task<GetStationsInforResponse> GetStationsInfor(IEnumerable<E_Station> stations);
        Task<GetStationsInfoResponse> GetStationsInfo(E_Station origin, E_Station destination);
        Task<GetRoutesResponse> GetRoutes(E_Station origin, E_Station destination, DateTime dateTime, bool isGoing);
        Task<TrainAvailableChairsResponse> TrainAvailableChairs(TrainAvailableChairsRequest request);
        IEnumerable<TrainAvailableChairsRequest> TrainAvailableChairsRequestBuilder(GetRoutesResponse routesResponse);
    }

    public class RailRepository : IRail
    {
        private readonly IHttpClientFactory _clientFactory;

        public RailRepository(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<GetStationsInforResponse> GetStationsInfor(E_Station station)
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["stations"] = ((int)station).ToString();
            UriBuilder ub = new UriBuilder()
            {
                Path = "apiinfo/api/infor/GetStationsInfor",
                Query = HttpUtility.UrlDecode(parameters.ToString())
            };
            string uri = ub.Uri.PathAndQuery;
            HttpClient client = _clientFactory.CreateClient("RailApi");
            using (HttpResponseMessage response = await client.GetAsync(uri))
            {
                response.EnsureSuccessStatusCode();
                GetStationsInforResponse result = await response.Content.ReadAsAsync<GetStationsInforResponse>();
                return result;
            }
        }

        public async Task<GetStationsInforResponse> GetStationsInfor(IEnumerable<E_Station> stations)
        {
            string parameters = string.Join('&', stations.Select(x => $"stations={(int)x}"));
            UriBuilder ub = new UriBuilder()
            {
                Path = "apiinfo/api/infor/GetStationsInfor",
                Query = parameters
            };
            string uri = ub.Uri.PathAndQuery;
            HttpClient client = _clientFactory.CreateClient("RailApi");
            using (HttpResponseMessage response = await client.GetAsync(uri))
            {
                response.EnsureSuccessStatusCode();
                GetStationsInforResponse result = await response.Content.ReadAsAsync<GetStationsInforResponse>();
                return result;
            }
        }

        public async Task<GetStationsInfoResponse> GetStationsInfo(E_Station origin, E_Station destination)
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["OId"] = ((int)origin).ToString();
            parameters["TId"] = ((int)destination).ToString();
            UriBuilder ub = new UriBuilder()
            {
                Path = "apiinfo/api/Info/GetStationsInfo",
                Query = HttpUtility.UrlDecode(parameters.ToString())
            };
            string uri = ub.Uri.PathAndQuery;
            HttpClient client = _clientFactory.CreateClient("RailApi");
            using (HttpResponseMessage response = await client.GetAsync(uri))
            {
                response.EnsureSuccessStatusCode();
                GetStationsInfoResponse result = await response.Content.ReadAsAsync<GetStationsInfoResponse>();
                return result;
            }
        }

        public async Task<GetRoutesResponse> GetRoutes(E_Station origin, E_Station destination, DateTime dateTime, bool isGoing)
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["OId"] = ((int)origin).ToString();
            parameters["TId"] = ((int)destination).ToString();
            parameters["Date"] = dateTime.ToString("yyyyMMdd");
            parameters["Hour"] = dateTime.ToString("HHmm");
            parameters["isGoing"] = isGoing.ToString();
            parameters["c"] = "1574944324761";
            UriBuilder ub = new UriBuilder()
            {
                Path = "apiinfo/api/Plan/GetRoutes",
                Query = HttpUtility.UrlDecode(parameters.ToString())
            };
            string uri = ub.Uri.PathAndQuery;
            HttpClient client = _clientFactory.CreateClient("RailApi");
            using (HttpResponseMessage response = await client.GetAsync(uri))
            {
                response.EnsureSuccessStatusCode();
                GetRoutesResponse result = await response.Content.ReadAsAsync<GetRoutesResponse>();
                if (result.Data.Error != null)
                {
                    throw new Exception(result.Data.Error.Description);
                }
                return result;
            }
        }

        public async Task<TrainAvailableChairsResponse> TrainAvailableChairs(TrainAvailableChairsRequest request)
        {
            string json = JsonConvert.SerializeObject(request);
            string encodedJson = json.Replace("\"", "%22");
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["method"] = "TrainAvailableChairs";
            parameters["jsonObj"] = encodedJson;
            UriBuilder ub = new UriBuilder()
            {
                Path = "_layouts/15/SolBox/TrainAvailableChairsHandler.ashx",
                Query = HttpUtility.UrlDecode(parameters.ToString())
            };
            string uri = ub.Uri.PathAndQuery;
            HttpClient client = _clientFactory.CreateClient("RailApi");
            using (HttpResponseMessage response = await client.PostAsync(uri, null))
            {
                response.EnsureSuccessStatusCode();
                TrainAvailableChairsResponse result = await response.Content.ReadAsAsync<TrainAvailableChairsResponse>();
                if (result.clsResult.returnCode != 1)
                {
                    throw new Exception(result.clsResult.returnDescription);
                }
                return result;
            }
        }

        public IEnumerable<TrainAvailableChairsRequest> TrainAvailableChairsRequestBuilder(GetRoutesResponse routesResponse)
        {
            List<TrainAvailableChairsRequestItem> requestItems = new List<TrainAvailableChairsRequestItem>();
            foreach (Route route in routesResponse.Data.Routes)
            {
                foreach (Train train in route.Train)
                {
                    if (int.TryParse(train.Trainno, out int trainNumber) && DateTime.TryParseExact(train.DepartureTime, "dd/MM/yyyy HH:mm:ss", null, DateTimeStyles.None, out DateTime departureTime) && int.TryParse(train.OrignStation, out int originStation) && int.TryParse(train.DestinationStation, out int destinationStation))
                    {
                        TrainAvailableChairsRequestItem requestItem = new TrainAvailableChairsRequestItem()
                        {
                            trainNumber = trainNumber,
                            trainDate = departureTime.Date,
                            fromStation = originStation,
                            destStation = destinationStation
                        };
                        requestItems.Add(requestItem);
                    }
                }
            }
            List<TrainAvailableChairsRequest> requests = new List<TrainAvailableChairsRequest>();
            for (int i = 0; i < requestItems.Count; i+= 17)
            {
                TrainAvailableChairsRequest request = new TrainAvailableChairsRequest()
                {
                    lstTrainAvailableChairsQuery = requestItems.GetRange(i, Math.Min(17, requestItems.Count - i)).ToArray()
                };
                requests.Add(request);
            }
            return requests;
        }
    }
}
