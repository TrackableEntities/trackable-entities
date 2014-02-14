using System;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using TrackableEntities.Patterns.EF6;
using WebApiSample.Service.EF.Contexts;
using WebApiSample.Service.EF.Repositories;
using WebApiSample.Service.Persistence.Exceptions;

namespace WebApiSample.Service.EF.UnitsOfWork
{
    public class NorthwindUnitOfWork : UnitOfWork
    {
        private readonly NorthwindSlimContext _context = new NorthwindSlimContext();
        private readonly CustomerRepository _customerRepository;
        private readonly OrderRepository _orderRepository;

        public NorthwindUnitOfWork()
        {
            Context = _context;
            _customerRepository = new CustomerRepository(_context);
            _orderRepository = new OrderRepository(_context);
        }

        public CustomerRepository CustomerRepository
        {
            get { return _customerRepository; }
        }

        public OrderRepository OrderRepository
        {
            get { return _orderRepository; }
        }

        public override Task<int> Save()
        {
            try
            {
                return base.Save();
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
