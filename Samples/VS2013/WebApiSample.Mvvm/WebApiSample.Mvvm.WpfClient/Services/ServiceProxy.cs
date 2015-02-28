using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using AspnetWebApi2Helpers.Serialization;

namespace WebApiSample.Mvvm.WpfClient
{
    public class ServiceProxy
    {
        private static volatile HttpClient _instance;
        private static volatile MediaTypeFormatter _formatter;
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
                            var formatter = new JsonMediaTypeFormatter();
                            formatter.JsonPreserveReferences();

                            string baseAddress = string.Format("http://{0}:{1}/",
                                Constants.WebClient.ServiceHost, Constants.WebClient.PortNumber);

                            _instance = new HttpClient {BaseAddress = new Uri(baseAddress)};
                        }
                    }
                }
                return _instance;
            }
        }

        public static MediaTypeFormatter Formatter
        {
            get
            {
                if (_formatter == null)
                {
                    lock (SyncRoot)
                    {
                        if (_formatter == null)
                        {
                            var jsonFormatter = new JsonMediaTypeFormatter();
                            jsonFormatter.JsonPreserveReferences();
                            _formatter = jsonFormatter;
                        }
                    }
                }
                return _formatter;
            }
        }
    }
}
