using System;
using TrackableEntities.Patterns;
using WebApiSample.Service.Persistence.Repositories;

namespace WebApiSample.Service.Persistence.UnitsOfWork
{
    public interface INorthwindUnitOfWork : IUnitOfWork
    {
        ICustomerRepository CustomerRepository { get; }
        IOrderRepository OrderRepository { get; }
    }
}
