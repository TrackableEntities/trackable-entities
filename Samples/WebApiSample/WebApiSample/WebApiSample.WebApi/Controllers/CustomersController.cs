using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TrackableEntities.EF;
using WebApiSample.ServiceEntities.Models;

namespace WebApiSample.WebApi.Controllers
{
    public class CustomersController : ApiController
    {
        private NorthwindSlimContext db = new NorthwindSlimContext();

        // GET api/Customers
        public IEnumerable<Customer> GetCustomers()
        {
            IEnumerable<Customer> customers = db.Customers
				.AsEnumerable();
            return customers;
        }

        // GET api/Customers/5
        public Customer GetCustomer(string id)
        {
            Customer customer = db.Customers
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
                db.ApplyChanges(customer);
                db.SaveChanges();

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
				db.Customers.Add(customer);
                db.SaveChanges();

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
            Customer customer = db.Customers
                .SingleOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            db.Customers.Attach(customer);
            db.Customers.Remove(customer);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}