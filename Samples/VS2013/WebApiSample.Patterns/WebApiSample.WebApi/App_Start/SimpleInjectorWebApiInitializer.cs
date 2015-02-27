using System.Web.Http;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using WebApiSample.Service.EF.Contexts;
using WebApiSample.Service.EF.Repositories;
using WebApiSample.Service.EF.UnitsOfWork;
using WebApiSample.Service.Persistence.Repositories;
using WebApiSample.Service.Persistence.UnitsOfWork;
using WebApiSample.Service.WebApi;

[assembly: WebActivator.PostApplicationStartMethod(typeof(SimpleInjectorWebApiInitializer), "Initialize")]

namespace WebApiSample.Service.WebApi
{
    public static class SimpleInjectorWebApiInitializer
    {
        public static void Initialize()
        {
            // Create IoC container
            var container = new Container();
  
            // Register dependencies
            InitializeContainer(container);
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
       
            // Verify registrations
            container.Verify();
            
            // Set Web API dependency resolver
            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);
        }
     
        private static void InitializeContainer(Container container)
        {
            // Register context, unit of work and repos with per request lifetime
            container.RegisterWebApiRequest<INorthwindSlimContext, NorthwindSlimContext>();
            container.RegisterWebApiRequest<INorthwindUnitOfWork, NorthwindUnitOfWork>();
            container.RegisterWebApiRequest<ICustomerRepository, CustomerRepository>();
            container.RegisterWebApiRequest<IOrderRepository, OrderRepository>();
        }
    }
}