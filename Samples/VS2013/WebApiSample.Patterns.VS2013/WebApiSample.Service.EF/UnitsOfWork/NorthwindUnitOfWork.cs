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
    public class NorthwindUnitOfWork : UnitOfWork, INorthwindUnitOfWork
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;

        public NorthwindUnitOfWork(INorthwindSlimContext context,
            ICustomerRepository customerRepository,
            IOrderRepository orderRepository) :
            base(context as DbContext)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
        }

        public ICustomerRepository CustomerRepository
        {
            get { return _customerRepository; }
        }

        public IOrderRepository OrderRepository
        {
            get { return _orderRepository; }
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
