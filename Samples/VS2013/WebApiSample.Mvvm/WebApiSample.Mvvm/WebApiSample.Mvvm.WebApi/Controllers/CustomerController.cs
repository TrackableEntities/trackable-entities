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
using WebApiSample.Mvvm.Service.Entities.Models;

namespace WebApiSample.Mvvm.WebApi.Controllers
{
    public class CustomerController : ApiController
    {
        private readonly NorthwindSlimContext _dbContext = new NorthwindSlimContext();

        // GET api/Customer
        [ResponseType(typeof(IEnumerable<Customer>))]
        public async Task<IHttpActionResult> GetCustomers()
        {
            IEnumerable<Customer> entities = await _dbContext.Customers
                .OrderBy(e => e.CompanyName)
                .ToListAsync();

            return Ok(entities);
        }

        // GET api/Customer/5
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> GetCustomer(string id)
        {
            Customer entity = await _dbContext.Customers
                .SingleOrDefaultAsync(e => e.CustomerId == id);

            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        // POST api/Customer
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> PostCustomer(Customer entity)
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
                if (_dbContext.Customers.Any(e => e.CustomerId == entity.CustomerId))
                {
                    return Conflict();
                }
                throw;
            }

            await _dbContext.LoadRelatedEntitiesAsync(entity);
            entity.AcceptChanges();
            return CreatedAtRoute("DefaultApi", new { id = entity.CustomerId }, entity);
        }

        // PUT api/Customer
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> PutCustomer(Customer entity)
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
                if (!_dbContext.Customers.Any(e => e.CustomerId == entity.CustomerId))
                {
                    return Conflict();
                }
                throw;
            }

            await _dbContext.LoadRelatedEntitiesAsync(entity);
            entity.AcceptChanges();
            return Ok(entity);
        }

        // DELETE api/Customer/5
        public async Task<IHttpActionResult> DeleteCustomer(string id)
        {
            Customer entity = await _dbContext.Customers
                .SingleOrDefaultAsync(e => e.CustomerId == id);
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
                if (!_dbContext.Customers.Any(e => e.CustomerId == entity.CustomerId))
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
