using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TrackableEntities.EF5;
using WebApiSample.Service.Entities.Models;

namespace WebApiSample.Service.WebApi.Controllers
{
    public class CustomersController : ApiController
    {
        private readonly NorthwindSlimContext _dbContext = new NorthwindSlimContext();

        // GET api/Customers
        public IEnumerable<Customer> GetCustomers()
        {
            IEnumerable<Customer> customers = _dbContext.Customers
                .ToList();
            return customers;
        }

        // GET api/Customers/5
        public Customer GetCustomer(string id)
        {
            Customer customer = _dbContext.Customers
                .SingleOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            return customer;
        }

        // PUT api/Customers
        public HttpResponseMessage PutCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
				// Update object graph entity state
                _dbContext.ApplyChanges(customer);
                _dbContext.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, customer);
                return response;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
        }

        // POST api/Customers
        public HttpResponseMessage PostCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
				_dbContext.Customers.Add(customer);
                _dbContext.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, customer);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = customer.CustomerId }));
                return response;
            }

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        // DELETE api/Customers/5
		// TODO: Accept entity concurrency property (rowversion)
        public HttpResponseMessage DeleteCustomer(string id)
        {
            Customer customer = _dbContext.Customers
                .SingleOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            _dbContext.Customers.Attach(customer);
            _dbContext.Customers.Remove(customer);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
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