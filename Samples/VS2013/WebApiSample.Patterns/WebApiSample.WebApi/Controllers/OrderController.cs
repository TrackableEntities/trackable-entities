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
	public class OrderController : ApiController
	{
		private readonly INorthwindUnitOfWork _unitOfWork;

		public OrderController(INorthwindUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET api/Order
		[ResponseType(typeof(IEnumerable<Order>))]
		public async Task<IHttpActionResult> GetOrders()
		{
			IEnumerable<Order> orders = await _unitOfWork.OrderRepository.GetOrders();	
			return Ok(orders);
		}

		// GET api/Order?customerId=ABCD
		[ResponseType(typeof(IEnumerable<Order>))]
		public async Task<IHttpActionResult> GetOrders(string customerId)
		{
			IEnumerable<Order> orders = await _unitOfWork.OrderRepository.GetOrders(customerId);
			return Ok(orders);
		}

		// GET api/Order/5
		[ResponseType(typeof(Order))]
		public async Task<IHttpActionResult> GetOrder(int id)
		{
			Order order = await _unitOfWork.OrderRepository.GetOrder(id);
			if (order == null)
			{
				return NotFound();
			}
			return Ok(order);
		}


        // POST api/Order
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Insert order
            _unitOfWork.OrderRepository.Insert(order);

            try
            {
                // Save and accept changes
                await _unitOfWork.SaveChangesAsync();
            }
            catch (UpdateException)
            {
                if (_unitOfWork.OrderRepository.Find(order.OrderId) == null)
                {
                    return Conflict();
                }
                throw;
            }

            // Load related entities and accept changes
            await _unitOfWork.OrderRepository.LoadRelatedEntitiesAsync(order);
            order.AcceptChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.OrderId }, order);
        }

        // PUT api/Order
		[ResponseType(typeof(Order))]
		public async Task<IHttpActionResult> PutOrder(Order order)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

            // Update order
            _unitOfWork.OrderRepository.Update(order);
            
            try
			{
				await _unitOfWork.SaveChangesAsync();
			}
			catch (UpdateConcurrencyException)
			{
				if (_unitOfWork.OrderRepository.Find(order.OrderId) == null)
				{
					return Conflict();
				}
				throw;
			}

            // Load related entities and accept changes
            await _unitOfWork.OrderRepository.LoadRelatedEntitiesAsync(order);
            order.AcceptChanges();
            return Ok(order);
        }

		// DELETE api/Order/5
		public async Task<IHttpActionResult> DeleteOrder(int id)
		{
            // Delete order
            bool result = await _unitOfWork.OrderRepository.DeleteOrder(id);
            if (!result) return Ok();
            
            try
			{
				await _unitOfWork.SaveChangesAsync();
			}
			catch (UpdateConcurrencyException)
			{
				if (_unitOfWork.OrderRepository.Find(id) == null)
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