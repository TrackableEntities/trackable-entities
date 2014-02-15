using System;
using System.Web.Http;
using TinyIoC;
using WebApiSample.Service.EF.Contexts;
using WebApiSample.Service.EF.Repositories;
using WebApiSample.Service.EF.UnitsOfWork;
using WebApiSample.Service.Persistence.Repositories;
using WebApiSample.Service.Persistence.UnitsOfWork;
using WebApiSample.Service.WebApi.DependencyResolution;

namespace WebApiSample.Service.WebApi
{
    public static class IoCConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Get IoC container
            var container = TinyIoCContainer.Current;

            // Register unit of work with per request lifetime
            container.Register<INorthwindSlimContext, NorthwindSlimContext>().AsPerRequestSingleton();
            container.Register<INorthwindUnitOfWork, NorthwindUnitOfWork>().AsPerRequestSingleton();
            container.Register<ICustomerRepository, CustomerRepository>().AsPerRequestSingleton();
            container.Register<IOrderRepository, OrderRepository>().AsPerRequestSingleton();

            // Set Web API dep resolver
            config.DependencyResolver = new TinyIoCDependencyResolver(container);
        }
    }
}
