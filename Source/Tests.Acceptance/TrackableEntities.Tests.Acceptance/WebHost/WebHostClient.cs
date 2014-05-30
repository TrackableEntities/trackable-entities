using System;
using System.Net.Http;

namespace TrackableEntities.Tests.Acceptance.WebHost
{
    public class WebHostClient
    {
        private WebHostClient() { }

        public static HttpClient Create()
        {
            string baseAddress = string.Format("http://{0}:{1}/",
                Constants.WebHost.HostName, Constants.WebHost.PortNumber);
            var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
            return client;
        }
    }
}
