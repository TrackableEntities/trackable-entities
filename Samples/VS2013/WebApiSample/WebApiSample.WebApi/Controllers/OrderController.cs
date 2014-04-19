using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TrackableEntities;
using TrackableEntities.Common;
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
                // Apply changes and save
                _dbContext.ApplyChanges(order);
				await _dbContext.SaveChangesAsync();

                // Accept changes and load related entities
				order.AcceptChanges();
			    await _dbContext.LoadRelatedEntitiesAsync(order);

				// Load Products into added order details
                //var ctx = ((IObjectContextAdapter)_dbContext).ObjectContext;
                //ctx.LoadProperty(order, o => o.Customer);
                //foreach (var detail in order.OrderDetails)
                //    ctx.LoadProperty(detail, od => od.Product);

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

            // Mark as added, apply changes and save
            order.TrackingState = TrackingState.Added;
			_dbContext.ApplyChanges(order);
			await _dbContext.SaveChangesAsync();

            // Accept changes and load related entities
			order.AcceptChanges();
		    await _dbContext.LoadRelatedEntitiesAsync(order);

			// Load related entities
            //var ctx = ((IObjectContextAdapter)_dbContext).ObjectContext;
            //ctx.LoadProperty(order, o => o.Customer);
            //ctx.LoadProperty(order, o => o.OrderDetails);
            //foreach (var detail in order.OrderDetails)
            //    ctx.LoadProperty(detail, od => od.Product);

			return CreatedAtRoute("DefaultApi", new { id = order.OrderId }, order);
		}

		// DELETE api/Order/5
		public async Task<IHttpActionResult> DeleteOrder(int id)
		{
            // Retrieve order with details
			Order order = await _dbContext.Orders
				.Include(o => o.OrderDetails)
				.SingleOrDefaultAsync(o => o.OrderId == id);

            // Deleting non-existent order should have no effect
			if (order == null)
			{
				return Ok();
			}

			// Mark order as deleted and apply changes
			order.TrackingState = TrackingState.Deleted;
			_dbContext.ApplyChanges(order);

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