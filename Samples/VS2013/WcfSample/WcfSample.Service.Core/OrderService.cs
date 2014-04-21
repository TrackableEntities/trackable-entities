using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
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
    public class OrderService : IOrderService, IDisposable
    {
        private readonly NorthwindSlimContext _dbContext;

        public OrderService()
        {
            _dbContext = new NorthwindSlimContext();
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            IEnumerable<Order> orders = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include("OrderDetails.Product")
                .ToListAsync();
            return orders;
        }

        public async Task<IEnumerable<Order>> GetCustomerOrders(string customerId)
        {
            IEnumerable<Order> orders = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include("OrderDetails.Product")
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();
            return orders;
        }

        public async Task<Order> GetOrder(int id)
        {
            Order order = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include("OrderDetails.Product")
                .SingleOrDefaultAsync(o => o.OrderId == id);
            return order;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            // Mark order as added
            order.TrackingState = TrackingState.Added;
            _dbContext.ApplyChanges(order);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException updateEx)
            {
                throw new FaultException(updateEx.Message);
            }

            // Load related entities and accept changes
            await _dbContext.LoadRelatedEntitiesAsync(order);
            order.AcceptChanges();
            return order;
        }

        public async Task<Order> UpdateOrder(Order order)
        {
            // Update entity state
            _dbContext.ApplyChanges(order);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException updateEx)
            {
                throw new FaultException(updateEx.Message);
            }

            // Load related entities, accept changes and return updated order
            await _dbContext.LoadRelatedEntitiesAsync(order);
            order.AcceptChanges();
            return order;
        }

        public async Task<bool> DeleteOrder(int id)
        {
            // Retrieve order to be deleted
            Order order = await _dbContext.Orders
                .Include(o => o.OrderDetails) // Include details
                .SingleOrDefaultAsync(c => c.OrderId == id);
            if (order == null)
                return false;

            // Mark as deleted
            order.TrackingState = TrackingState.Deleted;
            _dbContext.ApplyChanges(order);

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
