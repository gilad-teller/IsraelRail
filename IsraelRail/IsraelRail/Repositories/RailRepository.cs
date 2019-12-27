using IsraelRail.Models;
using IsraelRail.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            UriBuilder ub = new UriBuilder("https://www.rail.co.il/apiinfo/api/infor/GetStationsInfor")
            {
                Query = HttpUtility.UrlDecode(parameters.ToString())
            };
            Uri uri = ub.Uri;
            HttpClient client = _clientFactory.CreateClient();
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
            UriBuilder ub = new UriBuilder("https://www.rail.co.il/apiinfo/api/infor/GetStationsInfor")
            {
                Query = parameters
            };
            Uri uri = ub.Uri;
            HttpClient client = _clientFactory.CreateClient();
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
            UriBuilder ub = new UriBuilder("https://www.rail.co.il/apiinfo/api/Info/GetStationsInfo")
            {
                Query = HttpUtility.UrlDecode(parameters.ToString())
            };
            Uri uri = ub.Uri;
            HttpClient client = _clientFactory.CreateClient();
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
            UriBuilder ub = new UriBuilder("https://www.rail.co.il/apiinfo/api/Plan/GetRoutes")
            {
                Query = HttpUtility.UrlDecode(parameters.ToString())
            };
            Uri uri = ub.Uri;
            HttpClient client = _clientFactory.CreateClient();
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
    }
}
