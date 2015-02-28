using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using TrackableEntities;
using TrackableEntities.EF6;
using TrackableEntities.Common;
using WebApiSample.Mvvm.Service.Entities.Contexts;
using WebApiSample.Mvvm.Service.Entities.Models;

namespace WebApiSample.Mvvm.WebApi.Controllers
{
    public class OrderController : ApiController
    {
        private readonly NorthwindSlimContext _dbContext = new NorthwindSlimContext();

        // GET api/Order
        [ResponseType(typeof(IEnumerable<Order>))]
        public async Task<IHttpActionResult> GetOrders()
        {
            IEnumerable<Order> entities = await _dbContext.Orders
                .Include(e => e.Customer)
                .Include("OrderDetails.Product")
                .ToListAsync();

            return Ok(entities);
        }

        // GET api/Orders?customerId=ABCD
        public async Task<IEnumerable<Order>> GetOrders(string customerId)
        {
            IEnumerable<Order> orders = await _dbContext.Orders
                .Include(e => e.Customer)
                .Include("OrderDetails.Product")
                .Where(o => o.CustomerId == customerId)
                .OrderBy(e => e.OrderDate)
                .ToListAsync();
            return orders;
        }

        // GET api/Order/5
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> GetOrder(int id)
        {
            Order entity = await _dbContext.Orders
                .Include(e => e.Customer)
                .Include("OrderDetails.Product")
                .SingleOrDefaultAsync(e => e.OrderId == id);

            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        // POST api/Order
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> PostOrder(Order entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            entity.TrackingState = TrackingState.Added;
            _dbContext.ApplyChanges(entity);


            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (_dbContext.Orders.Any(e => e.OrderId == entity.OrderId))
                {
                    return Conflict();
                }
                throw;
            }

            await _dbContext.LoadRelatedEntitiesAsync(entity);
            entity.AcceptChanges();
            return CreatedAtRoute("DefaultApi", new { id = entity.OrderId }, entity);
        }

        // PUT api/Order
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> PutOrder(Order entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.ApplyChanges(entity);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Orders.Any(e => e.OrderId == entity.OrderId))
                {
                    return Conflict();
                }
                throw;
            }

            await _dbContext.LoadRelatedEntitiesAsync(entity);
            entity.AcceptChanges();
            return Ok(entity);
        }

        // DELETE api/Order/5
        public async Task<IHttpActionResult> DeleteOrder(int id)
        {
            Order entity = await _dbContext.Orders
                .Include(e => e.OrderDetails)
                .SingleOrDefaultAsync(e => e.OrderId == id);
            if (entity == null)
            {
                return Ok();
            }

            entity.TrackingState = TrackingState.Deleted;
            _dbContext.ApplyChanges(entity);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Orders.Any(e => e.OrderId == entity.OrderId))
                {
                    return Conflict();
                }
                throw;
            }

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
            base.Dispose(disposing);
        }
    }
}
