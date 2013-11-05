using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TrackableEntities;
using TrackableEntities.EF6;
using WebApiSample.Service.Entities.Models;

namespace WebApiSample.Service.WebApi.Controllers
{
    public class OrderController : ApiController
    {
        private readonly NorthwindSlimContext _dbContext = new NorthwindSlimContext();

        // GET api/Order
        [ResponseType(typeof(IEnumerable<Order>))]
        public async Task<IHttpActionResult> GetOrders()
        {
	        IEnumerable<Order> orders = await _dbContext.Orders
				.Include(o => o.Customer)
                .Include("OrderDetails.Product") // include details with products
                .ToListAsync();
	
            return Ok(orders);
        }

        // GET api/Order?customerId=ABCD
        [ResponseType(typeof(IEnumerable<Order>))]
        public async Task<IHttpActionResult> GetOrders(string customerId)
        {
            IEnumerable<Order> orders = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include("OrderDetails.Product") // include details with products
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();

            return Ok(orders);
        }

        // GET api/Order/5
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> GetOrder(int id)
        {
	        Order order = await _dbContext.Orders
			     .Include(o => o.Customer)
                 .Include("OrderDetails.Product") // include details with products
                 .SingleOrDefaultAsync(o => o.OrderId == id);
	
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        // PUT api/Order
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> PutOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
				// Update object graph entity state
                _dbContext.ApplyChanges(order);
                await _dbContext.SaveChangesAsync();

                // Load Products into added order details
                var ctx = ((IObjectContextAdapter)_dbContext).ObjectContext;
                var added = order.OrderDetails
                    .Where(od => od.TrackingState == TrackingState.Added);
                foreach (var detail in added)
                    ctx.LoadProperty(detail, od => od.Product);

                return Ok(order);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Orders.Any(o => o.OrderId == order.OrderId))
                {
                    return NotFound();
                }
                throw;
            }
        }

        // POST api/Order
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            // Load related entities
            var ctx = ((IObjectContextAdapter)_dbContext).ObjectContext;
            ctx.LoadProperty(order, o => o.Customer);
            ctx.LoadProperty(order, o => o.OrderDetails);
            foreach (var detail in order.OrderDetails)
                ctx.LoadProperty(detail, od => od.Product);

            return CreatedAtRoute("DefaultApi", new { id = order.OrderId }, order);
        }

        // DELETE api/Order/5
        public async Task<IHttpActionResult> DeleteOrder(int id)
        {
	        Order order = await _dbContext.Orders
                .Include(o => o.OrderDetails) // include details
                .SingleOrDefaultAsync(o => o.OrderId == id);
	        if (order == null)
            {
                return NotFound();
            }

            _dbContext.Orders.Attach(order);
            _dbContext.Orders.Remove(order);

            try
            {
	            await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Orders.Any(o => o.OrderId == order.OrderId))
                {
                    return NotFound();
                }
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}