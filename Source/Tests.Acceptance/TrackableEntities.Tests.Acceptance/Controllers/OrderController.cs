using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TrackableEntities.Common;
using TrackableEntities.EF.Tests.NorthwindModels;
using TrackableEntities.EF6;
using TrackableEntities.Tests.Acceptance.Contexts;

namespace TrackableEntities.Tests.Acceptance.Controllers
{
    public class OrderController : ApiController
    {
        private readonly NorthwindTestDbContext _dbContext = new NorthwindTestDbContext();

        // GET api/Order
        [ResponseType(typeof(IEnumerable<Order>))]
        public async Task<IHttpActionResult> GetOrders()
        {
	        IEnumerable<Order> orders = await _dbContext.Orders
				.Include(o => o.Customer)
                .Include("OrderDetails.Product") // Include details with products
                .ToListAsync();
	
            return Ok(orders);
        }

        // GET api/Order?customerId=ABCD
        [ResponseType(typeof(IEnumerable<Order>))]
        public async Task<IHttpActionResult> GetOrders(string customerId)
        {
            IEnumerable<Order> orders = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include("OrderDetails.Product") // Include details with products
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
                .Include("OrderDetails.Product") // Include details with products
                .SingleOrDefaultAsync(o => o.OrderId == id);
	
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // POST api/Order
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            order.TrackingState = TrackingState.Added;
            _dbContext.ApplyChanges(order);

            await _dbContext.SaveChangesAsync();

            await _dbContext.LoadRelatedEntitiesAsync(order);
            order.AcceptChanges();
            return CreatedAtRoute("DefaultApi", new { id = order.OrderId }, order);
        }

        // PUT api/Order
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> PutOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.ApplyChanges(order);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Orders.Any(o => o.OrderId == order.OrderId))
                {
                    return Conflict();
                }
                throw;
            }

			await _dbContext.LoadRelatedEntitiesAsync(order);
			order.AcceptChanges();
	        return Ok(order);
        }

        // DELETE api/Order/5
        public async Task<IHttpActionResult> DeleteOrder(int id)
        {
			Order order = await _dbContext.Orders
                .Include(o => o.OrderDetails) // Include details
                .SingleOrDefaultAsync(o => o.OrderId == id);
			if (order == null)
            {
                return Ok();
            }

			order.TrackingState = TrackingState.Deleted;
			_dbContext.ApplyChanges(order);

            try
            {
	            await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Orders.Any(o => o.OrderId == order.OrderId))
                {
                    return Conflict();
                }
                throw;
            }

            return Ok();
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