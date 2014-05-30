using System.Web.Http;
using System.Web.Http.SelfHost;

namespace TrackableEntities.Tests.Acceptance.WebHost
{
    public class WebHostServer
    {
        private WebHostServer() { }

        public static HttpSelfHostServer Create(int portNumber)
        {
            var config = GetHostConfig(portNumber);
            var server = new HttpSelfHostServer(config);
            return server;
        }

        private static HttpSelfHostConfiguration GetHostConfig(int portNumber)
        {
            var baseAddress = string.Format("http://{0}:{1}",
                Constants.WebHost.HostName, portNumber);
            var config = new HttpSelfHostConfiguration(baseAddress);
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new
                {
                    id = RouteParameter.Optional
                });
            return config;
        }
    }
}
