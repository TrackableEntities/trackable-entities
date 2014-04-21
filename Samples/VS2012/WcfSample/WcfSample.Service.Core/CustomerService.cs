using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.ServiceModel;
using System.Threading.Tasks;
using TrackableEntities;
using TrackableEntities.Common;
using TrackableEntities.EF6;
using WcfSample.Service.Entities.Contexts;
using WcfSample.Service.Entities.Models;

namespace WcfSample.Service.Core
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class CustomerService : ICustomerService, IDisposable
    {
        private readonly NorthwindSlimContext _dbContext;

        public CustomerService()
        {
            _dbContext = new NorthwindSlimContext();
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            IEnumerable<Customer> customers = await _dbContext.Customers
                .ToListAsync();
            return customers;
        }

        public async Task<Customer> GetCustomer(string id)
        {
            Customer customer = await _dbContext.Customers
                .SingleOrDefaultAsync(c => c.CustomerId == id);
            return customer;
        }

        public async Task<Customer> CreateCustomer(Customer customer)
        {
            customer.TrackingState = TrackingState.Added;
            _dbContext.ApplyChanges(customer);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException updateEx)
            {
                throw new FaultException(updateEx.Message);
            }

            await _dbContext.LoadRelatedEntitiesAsync(customer);
            customer.AcceptChanges();
            return customer;
        }

        public async Task<Customer> UpdateCustomer(Customer customer)
        {
            _dbContext.ApplyChanges(customer);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException updateEx)
            {
                throw new FaultException(updateEx.Message);
            }

            await _dbContext.LoadRelatedEntitiesAsync(customer);
            customer.AcceptChanges();
            return customer;
        }

        public async Task<bool> DeleteCustomer(string id)
        {
            Customer customer = await _dbContext.Customers
                .SingleOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null)
                return false;

            customer.TrackingState = TrackingState.Deleted;
            _dbContext.ApplyChanges(customer);

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException updateEx)
            {
                throw new FaultException(updateEx.Message);
            }
        }

        public void Dispose()
        {
            var dispose = _dbContext as IDisposable;
            if (dispose != null)
            {
                _dbContext.Dispose();
            }
        }
    }
}
