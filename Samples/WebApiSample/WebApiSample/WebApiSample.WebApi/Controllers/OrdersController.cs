using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TrackableEntities;
using TrackableEntities.EF;
using WebApiSample.ServiceEntities.Models;

namespace WebApiSample.WebApi.Controllers
{
    public class OrdersController : ApiController
    {
        private NorthwindSlimContext db = new NorthwindSlimContext();

        // GET api/Orders
        public IEnumerable<Order> GetOrders()
        {
            IEnumerable<Order> orders = db.Orders.Include(o => o.Customer)
                .Include("OrderDetails.Product")
                .AsEnumerable();
            return orders;
        }

        // GET api/Orders?customerId=ABCD
        public IEnumerable<Order> GetOrders(string customerId)
        {
            IEnumerable<Order> orders = db.Orders.Include(o => o.Customer)
                .Include("OrderDetails.Product")
                .Where(o => o.CustomerId == customerId)
                .AsEnumerable();
            return orders;
        }

        // GET api/Orders/5
        public Order GetOrder(int id)
        {
            Order order = db.Orders.Include(o => o.Customer)
                .Include("OrderDetails.Product")
                .SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return order;
        }

        // PUT api/Orders
        public HttpResponseMessage PutOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
				// Update object graph entity state
                db.ApplyChanges(order);
                db.SaveChanges();

                // Load Products into added order details
                var ctx = ((IObjectContextAdapter)db).ObjectContext;
                var added = order.OrderDetails
                    .Where(od => od.TrackingState == TrackingState.Added);
                foreach (var detail in added)
                    ctx.LoadProperty(detail, od => od.Product);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, order);
                return response;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
        }

        // POST api/Orders
        public HttpResponseMessage PostOrder(Order order)
        {
            if (ModelState.IsValid)
            {
				db.Orders.Add(order);
                db.SaveChanges();

                var ctx = ((IObjectContextAdapter) db).ObjectContext;
                ctx.LoadProperty(order, o => o.Customer);

                // Load order details
                ctx.LoadProperty(order, o => o.OrderDetails);
                foreach (var detail in order.OrderDetails)
                    ctx.LoadProperty(detail, od => od.Product);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, order);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = order.OrderId }));
                return response;
            }

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        // DELETE api/Orders/5
		// TODO: Accept entity concurrency property (rowversion)
        public HttpResponseMessage DeleteOrder(int id)
        {
            // Include order details
            Order order = db.Orders
                .Include(o => o.OrderDetails)
                .SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            db.Orders.Attach(order);
            db.Orders.Remove(order);

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