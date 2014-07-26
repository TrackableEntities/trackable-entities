using System;
using System.Net.Http;

namespace WebApiSample.Mvvm.WpfClient
{
    public class ServiceProxy
    {
        private static volatile HttpClient _instance;
        private static readonly object SyncRoot = new object();

        private ServiceProxy() { }

        public static HttpClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                        {
                            string baseAddress = string.Format("http://{0}:{1}/",
                                Constants.WebClient.ServiceHost, Constants.WebClient.PortNumber);
                            _instance = new HttpClient {BaseAddress = new Uri(baseAddress)};
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
