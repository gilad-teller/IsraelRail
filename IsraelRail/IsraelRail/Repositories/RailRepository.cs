using IsraelRail.Models;
using IsraelRail.Models.ApiModels;
using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace IsraelRail.Repositories
{
    public interface IRail
    {
        Task<GetStationsInforResponse> GetStationsInfor(E_Station station);
        Task<GetStationsInfoResponse> GetStationsInfo(E_Station origin, E_Station destination);
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
            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                GetStationsInforResponse result = await response.Content.ReadAsAsync<GetStationsInforResponse>();
                return result;
            }
            return null;
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
            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                GetStationsInfoResponse result = await response.Content.ReadAsAsync<GetStationsInfoResponse>();
                return result;
            }
            return null;
        }
    }
}
