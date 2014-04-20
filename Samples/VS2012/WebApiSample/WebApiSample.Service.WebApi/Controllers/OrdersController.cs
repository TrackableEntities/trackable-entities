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
using WebApiSample.Service.Entities.Models;

namespace WebApiSample.Service.WebApi.Controllers
{
    public class OrdersController : ApiController
    {
        private readonly NorthwindSlimContext _dbContext = new NorthwindSlimContext();

        // GET api/Orders
        public IEnumerable<Order> GetOrders()
        {
            IEnumerable<Order> orders = _dbContext.Orders
			    .Include(o => o.Customer)
                .Include("OrderDetails.Product") // Include details with product
                .ToList();

            return orders;
        }

        // GET api/Order?customerId=ABCD
        public IEnumerable<Order> GetOrders(string customerId)
        {
            IEnumerable<Order> orders = _dbContext.Orders
                .Include(o => o.Customer)
                .Include("OrderDetails.Product") // Include details with product
                .Where(o => o.CustomerId == customerId)
                .ToList();
            return orders;
        }

        // GET api/Orders/5
        public Order GetOrder(int id)
        {
            Order order = _dbContext.Orders
			    .Include(o => o.Customer)
                .Include("OrderDetails.Product") // Include details with product
                .SingleOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return order;
        }

        // POST api/Orders
        public HttpResponseMessage PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            order.TrackingState = TrackingState.Added;
            _dbContext.ApplyChanges(order);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (_dbContext.Orders.Any(o => o.OrderId == order.OrderId))
                {
	                return Request.CreateErrorResponse(HttpStatusCode.Conflict, ex);
                }
                throw;
            }

            _dbContext.LoadRelatedEntities(order);
            order.AcceptChanges();

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, order);
            response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = order.OrderId }));
            return response;
        }

        // PUT api/Orders
        public HttpResponseMessage PutOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            _dbContext.ApplyChanges(order);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_dbContext.Orders.Any(o => o.OrderId == order.OrderId))
                {
	                return Request.CreateErrorResponse(HttpStatusCode.Conflict, ex);
                }
                throw;
            }

			_dbContext.LoadRelatedEntities(order);
			order.AcceptChanges();

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, order);
            return response;
        }

        // DELETE api/Orders/5
        public HttpResponseMessage DeleteOrder(int id)
        {
            Order order = _dbContext.Orders
                .Include(o => o.OrderDetails) // Include details
                .SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

			order.TrackingState = TrackingState.Deleted;
			_dbContext.ApplyChanges(order);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_dbContext.Orders.Any(o => o.OrderId == order.OrderId))
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