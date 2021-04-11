using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace DotnetProxy
{
    public class ProxyService
    {
        private readonly ILogger<ProxyService> _logger;

        public ProxyService(ILogger<ProxyService> logger)
        {
            _logger = logger;
        }

        public string Get(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

                HttpResponseMessage response = client.SendAsync(request).Result;

                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
