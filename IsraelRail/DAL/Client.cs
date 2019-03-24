using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Client
    {
        private static HttpClient httpClient;
        private static object _lock = new object();

        public static HttpClient GetClient
        {
            get
            {
                if (httpClient == null)
                {
                    lock (_lock)
                    {
                        if (httpClient == null)
                        {
                            httpClient = new HttpClient();
                        }
                    }
                }
                return httpClient;
            }
        }

        public static async Task<T> Get<T,K>(K request)
        {
            GetClient.BaseAddress = new Uri("https://www.rail.co.il/apiinfo/api/");
            using (HttpResponseMessage response = await GetClient.GetAsync("Plan/GetRoutes?OId=700&TId=4600&Date=20190325&Hour=2400&isGoing=true"))
            {
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                return default(T);
            }
        }
    }
}
