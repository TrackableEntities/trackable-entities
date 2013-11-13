using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TrackableEntities.EF6;
using WebApiSample.Service.Entities.Models;

namespace WebApiSample.Service.WebApi.Controllers
{
    public class CustomerController : ApiController
    {
        private readonly NorthwindSlimContext _dbContext = new NorthwindSlimContext();

        // GET api/Customer
        [ResponseType(typeof(IEnumerable<Customer>))]
        public async Task<IHttpActionResult> GetCustomers()
        {
	        IEnumerable<Customer> customers = await _dbContext.Customers.ToListAsync();
	
            return Ok(customers);
        }

        // GET api/Customer/5
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> GetCustomer(string id)
        {
	        Customer customer = await _dbContext.Customers.SingleOrDefaultAsync(c => c.CustomerId == id);
	
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        // PUT api/Customer
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> PutCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
				// Update object graph entity state
                _dbContext.ApplyChanges(customer);
                await _dbContext.SaveChangesAsync();
                customer.AcceptChanges();
	            return Ok(customer);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Customers.Any(c => c.CustomerId == customer.CustomerId))
                {
                    return NotFound();
                }
                throw;
            }
        }

        // POST api/Customer
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> PostCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.Customers.Add(customer);

            try
            {
                await _dbContext.SaveChangesAsync();
                customer.AcceptChanges();
            }
            catch (DbUpdateException)
            {
                if (_dbContext.Customers.Any(c => c.CustomerId == customer.CustomerId))
                {
                    return Conflict();
                }
                throw;
            }

            return CreatedAtRoute("DefaultApi", new { id = customer.CustomerId }, customer);
        }

        // DELETE api/Customer/5
        public async Task<IHttpActionResult> DeleteCustomer(string id)
        {
	        Customer customer = await _dbContext.Customers.SingleOrDefaultAsync(c => c.CustomerId == id);
	        if (customer == null)
            {
                return NotFound();
            }

            _dbContext.Customers.Attach(customer);
            _dbContext.Customers.Remove(customer);

            try
            {
	            await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Customers.Any(c => c.CustomerId == customer.CustomerId))
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