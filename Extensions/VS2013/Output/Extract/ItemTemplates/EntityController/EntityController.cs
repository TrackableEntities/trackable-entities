using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TrackableEntities.Common;
using $baseNamespace$.Entities.Models;
using $baseNamespace$.Persistence.Exceptions;
using $baseNamespace$.Persistence.UnitsOfWork;

namespace $rootnamespace$
{
	public class $safeitemname$ : ApiController
	{
        // TODO: Rename IExampleUnitOfWork to match Unit of Work Interface added to Persistence project
		private readonly IExampleUnitOfWork _unitOfWork;

        // TODO: Rename IExampleUnitOfWork parameter
		public $safeitemname$(IExampleUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET api/$entityName$
		[ResponseType(typeof(IEnumerable<$entityName$>))]
		public async Task<IHttpActionResult> Get$entitySetName$()
		{
			IEnumerable<$entityName$> entities = await _unitOfWork.$entityName$Repository.Get$entitySetName$();	
			return Ok(entities);
		}

		// GET api/$entityName$/5
		[ResponseType(typeof($entityName$))]
		public async Task<IHttpActionResult> Get$entityName$(int id)
		{
			$entityName$ entity = await _unitOfWork.$entityName$Repository.Get$entityName$(id);	
			if (entity == null)
			{
				return NotFound();
			}
			return Ok(entity);
		}

		// POST api/$entityName$
		[ResponseType(typeof($entityName$))]
		public async Task<IHttpActionResult> Post$entityName$($entityName$ entity)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			_unitOfWork.$entityName$Repository.Insert(entity);

			try
			{
				await _unitOfWork.SaveChangesAsync();
			}
			catch (UpdateException)
			{
				if (_unitOfWork.$entityName$Repository.Find(entity.$entityName$Id) == null)
				{
					return Conflict();
				}
				throw;
			}

            await _unitOfWork.OrderRepository.LoadRelatedEntitiesAsync(entity);
			entity.AcceptChanges();

			return CreatedAtRoute("DefaultApi", new { id = entity.$entityName$Id }, entity);
		}

		// PUT api/$entityName$
		[ResponseType(typeof($entityName$))]
		public async Task<IHttpActionResult> Put$entityName$($entityName$ entity)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			_unitOfWork.$entityName$Repository.Update(entity);

			try
			{
				await _unitOfWork.SaveChangesAsync();
			}
			catch (UpdateConcurrencyException)
			{
				if (_unitOfWork.$entityName$Repository.Find(entity.$entityName$Id) == null)
				{
					return Conflict();
				}
				throw;
			}

            await _unitOfWork.OrderRepository.LoadRelatedEntitiesAsync(entity);
			entity.AcceptChanges();
			return Ok(entity);
		}

		// DELETE api/$entityName$/5
		public async Task<IHttpActionResult> Delete$entityName$(int id)
		{
			bool result = await _unitOfWork.$entityName$Repository.Delete$entityName$(id);	
            if (!result) return Ok();

			try
			{
				await _unitOfWork.SaveChangesAsync();
			}
			catch (UpdateConcurrencyException)
			{
				if (_unitOfWork.$entityName$Repository.Find(id) == null)
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
