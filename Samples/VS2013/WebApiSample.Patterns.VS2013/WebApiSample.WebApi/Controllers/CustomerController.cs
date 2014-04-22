using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TrackableEntities.Common;
using WebApiSample.Service.Entities.Models;
using WebApiSample.Service.Persistence.Exceptions;
using WebApiSample.Service.Persistence.UnitsOfWork;

namespace WebApiSample.Service.WebApi.Controllers
{
	public class CustomerController : ApiController
	{
		private readonly INorthwindUnitOfWork _unitOfWork;

		public CustomerController(INorthwindUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET api/Customer
		[ResponseType(typeof(IEnumerable<Customer>))]
		public async Task<IHttpActionResult> GetCustomers()
		{
			IEnumerable<Customer> customers = await _unitOfWork.CustomerRepository.GetCustomers();	
			return Ok(customers);
		}

		// GET api/Customer/5
		[ResponseType(typeof(Customer))]
		public async Task<IHttpActionResult> GetCustomer(string id)
		{
			Customer customer = await _unitOfWork.CustomerRepository.GetCustomer(id);	
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

            _unitOfWork.CustomerRepository.Insert(customer);

            try
            {
                // Save and accept changes
                await _unitOfWork.SaveChangesAsync();
            }
            catch (UpdateException)
            {
                if (_unitOfWork.CustomerRepository.Find(customer.CustomerId) == null)
                {
                    return Conflict();
                }
                throw;
            }

            // Load related entities and accept changes
            await _unitOfWork.CustomerRepository.LoadRelatedEntitiesAsync(customer);
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

            _unitOfWork.CustomerRepository.Update(customer);
            
            try
			{
				await _unitOfWork.SaveChangesAsync();
			}
			catch (UpdateConcurrencyException)
			{
				if (_unitOfWork.CustomerRepository.Find(customer.CustomerId) == null)
				{
                    return Conflict();
				}
				throw;
			}

            await _unitOfWork.CustomerRepository.LoadRelatedEntitiesAsync(customer);
            customer.AcceptChanges();
            return Ok(customer);
        }

		// DELETE api/Customer/5
		public async Task<IHttpActionResult> DeleteCustomer(string id)
		{
			bool result = await _unitOfWork.CustomerRepository.DeleteAsync(id);
			if (!result) return Ok();

			try
			{
				await _unitOfWork.SaveChangesAsync();
			}
			catch (UpdateConcurrencyException)
			{
				if (_unitOfWork.CustomerRepository.Find(id) == null)
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
				var disposable = _unitOfWork as IDisposable;
				if (disposable != null)
					disposable.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}