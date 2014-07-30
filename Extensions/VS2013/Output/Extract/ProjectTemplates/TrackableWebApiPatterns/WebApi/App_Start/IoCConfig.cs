using System.Web.Http;
using TinyIoC;
using $safeprojectname$.DependencyResolution;

//using TrackableWebApiPatterns.Service.EF.Contexts;
//using TrackableWebApiPatterns.Service.EF.Repositories;
//using TrackableWebApiPatterns.Service.EF.UnitsOfWork;
//using TrackableWebApiPatterns.Service.Persistence.Repositories;
//using TrackableWebApiPatterns.Service.Persistence.UnitsOfWork;

namespace $safeprojectname$
{
    public static class IoCConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Get IoC container
            var container = TinyIoCContainer.Current;

            // TODO: Register context, unit of work and repos with per request lifetime
            // For example:
            //container.Register<INorthwindSlimContext, NorthwindSlimContext>().AsPerRequestSingleton();
            //container.Register<INorthwindUnitOfWork, NorthwindUnitOfWork>().AsPerRequestSingleton();
            //container.Register<ICustomerRepository, CustomerRepository>().AsPerRequestSingleton();
            //container.Register<IOrderRepository, OrderRepository>().AsPerRequestSingleton();

            // Set Web API dep resolver
            config.DependencyResolver = new TinyIoCDependencyResolver(container);
        }
    }
}
