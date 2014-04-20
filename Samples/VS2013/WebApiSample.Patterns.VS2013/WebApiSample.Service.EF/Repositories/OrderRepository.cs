using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using TrackableEntities;
using TrackableEntities.Patterns.EF6;
using WebApiSample.Service.EF.Contexts;
using WebApiSample.Service.Entities.Models;
using WebApiSample.Service.Persistence.Repositories;

namespace WebApiSample.Service.EF.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly INorthwindSlimContext _context;

        public OrderRepository(INorthwindSlimContext context) : 
            base(context as DbContext)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            IEnumerable<Order> orders = await _context.Orders
                .Include(o => o.Customer)
                .Include("OrderDetails.Product")
                .ToListAsync();
            return orders;
        }

        public async Task<IEnumerable<Order>> GetOrders(string customerId)
        {
            IEnumerable<Order> orders = await _context.Orders
                .Include(o => o.Customer)
                .Include("OrderDetails.Product")
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();
            return orders;
        }

        public async Task<Order> GetOrder(int id)
        {
            Order order = await _context.Orders
                 .Include(o => o.Customer)
                 .Include("OrderDetails.Product")
                 .SingleOrDefaultAsync(o => o.OrderId == id);
            return order;
        }
    }
}
