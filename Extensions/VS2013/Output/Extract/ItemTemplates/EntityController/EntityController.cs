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

		// PUT api/$entityName$
		[ResponseType(typeof($entityName$))]
		public async Task<IHttpActionResult> Put$entityName$($entityName$ entity)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				_unitOfWork.$entityName$Repository.Update(entity);
				await _unitOfWork.SaveChangesAsync();
				entity.AcceptChanges();
				return Ok(entity);
			}
			catch (UpdateConcurrencyException)
			{
				if (_unitOfWork.$entityName$Repository.Find(entity.$entityName$Id) == null)
				{
					return NotFound();
				}
				throw;
			}
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
				entity.AcceptChanges();
			}
			catch (UpdateException)
			{
				if (_unitOfWork.$entityName$Repository.Find(entity.$entityName$Id) == null)
				{
					return Conflict();
				}
				throw;
			}

			return CreatedAtRoute("DefaultApi", new { id = entity.$entityName$Id }, entity);
		}

		// DELETE api/$entityName$/5
		public async Task<IHttpActionResult> Delete$entityName$(int id)
		{
			bool exists = await _unitOfWork.$entityName$Repository.DeleteAsync(id);
			if (!exists) return Ok();

			try
			{
				await _unitOfWork.SaveChangesAsync();
				return Ok();
			}
			catch (UpdateConcurrencyException)
			{
				if (_unitOfWork.$entityName$Repository.Find(id) == null)
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
