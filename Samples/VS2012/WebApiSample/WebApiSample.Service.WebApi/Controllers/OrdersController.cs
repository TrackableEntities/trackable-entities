using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TrackableEntities;
using TrackableEntities.EF5;
using WebApiSample.Service.Entities.Models;

namespace WebApiSample.Service.WebApi.Controllers
{
    public class OrdersController : ApiController
    {
        private readonly NorthwindSlimContext _dbContext = new NorthwindSlimContext();

        // GET api/Order
        public IEnumerable<Order> GetOrders()
        {
            IEnumerable<Order> orders = _dbContext.Orders
                .Include(o => o.Customer)
                .Include("OrderDetails.Product")
                .ToList();
            return orders;
        }

        // GET api/Order?customerId=ABCD
        public IEnumerable<Order> GetOrders(string customerId)
        {
            IEnumerable<Order> orders = _dbContext.Orders
                .Include(o => o.Customer)
                .Include("OrderDetails.Product")
                .Where(o => o.CustomerId == customerId)
                .ToList();
            return orders;
        }

        // GET api/Order/5
        public Order GetOrder(int id)
        {
            Order order = _dbContext.Orders
                .Include(o => o.Customer)
                .Include("OrderDetails.Product")
                .SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            return order;
        }

        // PUT api/Order
        public HttpResponseMessage PutOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
				// Update object graph entity state
                _dbContext.ApplyChanges(order);
                _dbContext.SaveChanges();
                order.AcceptChanges();

                // Load Products into added order details
                var ctx = ((IObjectContextAdapter)_dbContext).ObjectContext;
                foreach (var detail in order.OrderDetails)
                    ctx.LoadProperty(detail, od => od.Product);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, order);
                return response;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
        }

        // POST api/Order
        public HttpResponseMessage PostOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                // Save new order
				_dbContext.Orders.Add(order);
                _dbContext.SaveChanges();
                order.AcceptChanges();

                // Load related entities
                var ctx = ((IObjectContextAdapter)_dbContext).ObjectContext;
                ctx.LoadProperty(order, o => o.Customer);
                ctx.LoadProperty(order, o => o.OrderDetails);
                foreach (var detail in order.OrderDetails)
                    ctx.LoadProperty(detail, od => od.Product);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, order);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = order.OrderId }));
                return response;
            }

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        // DELETE api/Order/5
        public HttpResponseMessage DeleteOrder(int id)
        {
            // Get order to be deleted
            Order order = _dbContext.Orders
                .Include(o => o.OrderDetails)
                .SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            // First remove order
            _dbContext.Orders.Attach(order);
            _dbContext.Orders.Remove(order);

            // Then remove order details
            foreach (var detail in order.OrderDetails)
            {
                _dbContext.OrderDetails.Attach(detail);
                _dbContext.OrderDetails.Remove(detail);
            }

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