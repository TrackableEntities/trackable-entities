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
    public class CustomerController : ApiController
    {
        private readonly NorthwindTestDbContext _dbContext = new NorthwindTestDbContext();

        // GET api/Customer
        [ResponseType(typeof(IEnumerable<Customer>))]
        public async Task<IHttpActionResult> GetCustomers()
        {
	        IEnumerable<Customer> customers = await _dbContext.Customers
				.Include(c => c.CustomerSetting)
				.ToListAsync();
	
            return Ok(customers);
        }

        // GET api/Customer/5
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> GetCustomer(string id)
        {
	        Customer customer = await _dbContext.Customers
				.Include(c => c.CustomerSetting)
				.SingleOrDefaultAsync(c => c.CustomerId == id);
	
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // POST api/Customer
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> PostCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            customer.TrackingState = TrackingState.Added;
            _dbContext.ApplyChanges(customer);


            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (_dbContext.Customers.Any(c => c.CustomerId == customer.CustomerId))
                {
                    return Conflict();
                }
                throw;
            }

            await _dbContext.LoadRelatedEntitiesAsync(customer);
            customer.AcceptChanges();
            return CreatedAtRoute("DefaultApi", new { id = customer.CustomerId }, customer);
        }

        // PUT api/Customer
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> PutCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.ApplyChanges(customer);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Customers.Any(c => c.CustomerId == customer.CustomerId))
                {
                    return Conflict();
                }
                throw;
            }

			await _dbContext.LoadRelatedEntitiesAsync(customer);
			customer.AcceptChanges();
	        return Ok(customer);
        }

        // DELETE api/Customer/5
        public async Task<IHttpActionResult> DeleteCustomer(string id)
        {
			Customer customer = await _dbContext.Customers
				.SingleOrDefaultAsync(c => c.CustomerId == id);
			if (customer == null)
            {
                return Ok();
            }

			customer.TrackingState = TrackingState.Deleted;
			_dbContext.ApplyChanges(customer);

            try
            {
	            await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Customers.Any(c => c.CustomerId == customer.CustomerId))
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