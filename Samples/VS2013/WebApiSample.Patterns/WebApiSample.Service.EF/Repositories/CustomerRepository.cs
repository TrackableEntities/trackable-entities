using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using TrackableEntities.Patterns.EF6;
using WebApiSample.Service.EF.Contexts;
using WebApiSample.Service.Entities.Models;
using WebApiSample.Service.Persistence.Repositories;

namespace WebApiSample.Service.EF.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private readonly INorthwindSlimContext _context;

        public CustomerRepository(INorthwindSlimContext context) :
            base(context as DbContext)
        {
            _context = context;
        }

        public async Task<Customer> GetCustomer(string id)
        {
            Customer customer = await FindAsync(id);
            return customer;
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            IEnumerable<Customer> customers = await _context.Customers.ToListAsync();
            return customers;
        }
    }
}
