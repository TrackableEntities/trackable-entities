using System;
using System.Web.Http;
using AspnetWebApi2Helpers.Serialization;

namespace WebApiSample.Mvvm.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configure Json and Xml formatters to handle cycles
            config.Formatters.JsonPreserveReferences();
            config.Formatters.XmlPreserveReferences();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
