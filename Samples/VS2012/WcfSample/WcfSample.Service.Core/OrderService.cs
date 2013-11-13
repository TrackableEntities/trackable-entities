using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using TrackableEntities.EF6;
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

        public async Task<Order> UpdateOrder(Order order)
        {
            try
            {
                // Update object graph entity state
                _dbContext.ApplyChanges(order);
                await _dbContext.SaveChangesAsync();
                order.AcceptChanges();

                // Load order details
                var ctx = ((IObjectContextAdapter)_dbContext).ObjectContext;
                foreach (var detail in order.OrderDetails)
                    ctx.LoadProperty(detail, od => od.Product);
                return order;
            }
            catch (DbUpdateConcurrencyException updateEx)
            {
                throw new FaultException(updateEx.Message);
            }
        }

        public async Task<Order> CreateOrder(Order order)
        {
            // Save new order
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
            order.AcceptChanges();

            // Load order details
            var ctx = ((IObjectContextAdapter)_dbContext).ObjectContext;
            ctx.LoadProperty(order, o => o.Customer);
            ctx.LoadProperty(order, o => o.OrderDetails);
            foreach (var detail in order.OrderDetails)
                ctx.LoadProperty(detail, od => od.Product);
            return order;
        }

        public async Task<bool> DeleteOrder(int id)
        {
            Order order = await _dbContext.Orders
                .Include(o => o.OrderDetails)
                .SingleOrDefaultAsync(c => c.OrderId == id);
            if (order == null)
                return false;

            try
            {
                // First remove order
                _dbContext.Orders.Attach(order);
                _dbContext.Orders.Remove(order);

                // Then remove order details
                foreach (var detail in order.OrderDetails)
                {
                    _dbContext.OrderDetails.Attach(detail);
                    _dbContext.OrderDetails.Remove(detail);
                }

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
