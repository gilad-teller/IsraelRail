using IsraelRail.Models;
using IsraelRail.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace IsraelRail.RailRepository
{
    public class StationsRepository
    {
        private readonly IHttpClientFactory _clientFactory;

        public StationsRepository(IHttpClientFactory clientFactory)
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
    }
}
