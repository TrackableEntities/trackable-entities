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

		// PUT api/Order
		[ResponseType(typeof(Order))]
		public async Task<IHttpActionResult> PutOrder(Order order)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				// Update order
				_unitOfWork.OrderRepository.Update(order);
				await _unitOfWork.SaveChangesAsync();
				order.AcceptChanges();

				// Load Products into added order details
				_unitOfWork.OrderRepository.LoadProductsOnAddedDetails(order);
				return Ok(order);
			}
			catch (UpdateConcurrencyException)
			{
				if (_unitOfWork.OrderRepository.Find(order.OrderId) == null)
				{
					return NotFound();
				}
				throw;
			}
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
			await _unitOfWork.SaveChangesAsync();
			order.AcceptChanges();

			// Load related entities
			_unitOfWork.OrderRepository.LoadRelatedEntities(order);

			return CreatedAtRoute("DefaultApi", new { id = order.OrderId }, order);
		}

		// DELETE api/Order/5
		public async Task<IHttpActionResult> DeleteOrder(int id)
		{
			bool exists = await _unitOfWork.OrderRepository.DeleteOrder(id);
			if (!exists) return Ok();

			try
			{
				await _unitOfWork.SaveChangesAsync();
				return Ok();
			}
			catch (UpdateConcurrencyException)
			{
				if (_unitOfWork.OrderRepository.Find(id) == null)
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
				var disposable = _unitOfWork as IDisposable;
				if (disposable != null)
					disposable.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}