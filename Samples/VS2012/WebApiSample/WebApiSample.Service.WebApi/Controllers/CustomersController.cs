using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TrackableEntities;
using TrackableEntities.EF5;
using TrackableEntities.Common;
using WebApiSample.Service.Entities.Contexts;
using WebApiSample.Service.Entities.Models;

namespace WebApiSample.Service.WebApi.Controllers
{
    public class CustomersController : ApiController
    {
        private readonly NorthwindSlimContext _dbContext = new NorthwindSlimContext();

        // GET api/Customer
        public IEnumerable<Customer> GetCustomers()
        {
            IEnumerable<Customer> customers = _dbContext.Customers
			    .Include(c => c.CustomerSetting)
				.ToList();

            return customers;
        }

        // GET api/Customer/5
        public Customer GetCustomer(string id)
        {
            Customer customer = _dbContext.Customers
			    .Include(c => c.CustomerSetting)
                .SingleOrDefault(c => c.CustomerId == id);

            if (customer == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return customer;
        }

        // POST api/Customer
        public HttpResponseMessage PostCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            customer.TrackingState = TrackingState.Added;
            _dbContext.ApplyChanges(customer);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (_dbContext.Customers.Any(c => c.CustomerId == customer.CustomerId))
                {
	                return Request.CreateErrorResponse(HttpStatusCode.Conflict, ex);
                }
                throw;
            }

            _dbContext.LoadRelatedEntities(customer);
            customer.AcceptChanges();

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, customer);
            response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = customer.CustomerId }));
            return response;
        }

        // PUT api/Customer
        public HttpResponseMessage PutCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            _dbContext.ApplyChanges(customer);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_dbContext.Customers.Any(c => c.CustomerId == customer.CustomerId))
                {
	                return Request.CreateErrorResponse(HttpStatusCode.Conflict, ex);
                }
                throw;
            }

			_dbContext.LoadRelatedEntities(customer);
			customer.AcceptChanges();

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, customer);
            return response;
        }

        // DELETE api/Customer/5
        public HttpResponseMessage DeleteCustomer(string id)
        {
            Customer customer = _dbContext.Customers
                .SingleOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

			customer.TrackingState = TrackingState.Deleted;
			_dbContext.ApplyChanges(customer);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_dbContext.Customers.Any(c => c.CustomerId == customer.CustomerId))
                {
	                return Request.CreateErrorResponse(HttpStatusCode.Conflict, ex);
                }
                throw;
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
            base.Dispose(disposing);
        }
    }
}