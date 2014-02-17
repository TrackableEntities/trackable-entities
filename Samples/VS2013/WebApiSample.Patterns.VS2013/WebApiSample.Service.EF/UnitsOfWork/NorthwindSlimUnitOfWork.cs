using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using TrackableEntities.Patterns.EF6;
using WebApiSample.Service.EF.Contexts;
using WebApiSample.Service.Persistence.Exceptions;
using WebApiSample.Service.Persistence.Repositories;
using WebApiSample.Service.Persistence.UnitsOfWork;

namespace WebApiSample.Service.EF.UnitsOfWork
{
    // TODO: Add INorthwindSlimUnitOfWorkUnitOfWork to the Persistence project
    public class NorthwindSlimUnitOfWork : UnitOfWork, INorthwindSlimUnitOfWork
    {
        // TODO: Add read-only fields for each entity repository interface
        private readonly IProductRepository _productRepository;

		// TODO: Rename IDatabaseContext to match context interface
		// TODO: Add parameters for each repository interface
        public NorthwindSlimUnitOfWork(INorthwindContext context,
            IProductRepository productRepository) :
            base(context as DbContext)
        {
		    // TODO: Initizlialize each entity repository field
            _productRepository = productRepository;
        }

        // TODO: Add read-only property for each entity repository interface
        public IProductRepository ProductRepository
        {
            get { return _productRepository; }
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException concurrencyException)
            {
                throw new UpdateConcurrencyException(concurrencyException.Message,
                    concurrencyException);
            }
            catch (DbUpdateException updateException)
            {
                throw new UpdateException(updateException.Message,
                    updateException);
            }
        }

        public override Task<int> SaveChangesAsync()
        {
            return SaveChangesAsync(CancellationToken.None);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                return base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException concurrencyException)
            {
                throw new UpdateConcurrencyException(concurrencyException.Message,
                    concurrencyException);
            }
            catch (DbUpdateException updateException)
            {
                throw new UpdateException(updateException.Message, 
                    updateException);
            }
        }
    }
}
