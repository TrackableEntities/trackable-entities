using System;
using TrackableEntities.Patterns;
using WebApiSample.Service.Persistence.Repositories;

namespace WebApiSample.Service.Persistence.UnitsOfWork
{
    public interface INorthwindSlimUnitOfWork : IUnitOfWork, IUnitOfWorkAsync
    {
        // TODO: Add read-only properties for each entity repository interface
        IProductRepository ProductRepository { get; }
    }
}
