using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using TrackableEntities;
using TrackableEntities.EF6;
using TrackableEntities.Common;
using $baseNamespace$.Entities.Models;

// NOTE: Primary key name and/or type may need to be set manually.

namespace $rootnamespace$
{
	public class $safeitemname$ : ApiController
	{
        private readonly $dbContextName$ _dbContext = new $dbContextName$();

        // GET api/$entityName$
        [ResponseType(typeof(IEnumerable<$entityName$>))]
        public async Task<IHttpActionResult> Get$entitySetName$()
        {
	        IEnumerable<$entityName$> entities = await _dbContext.$entitySetName$
			    // TODO: Add Includes for reference and/or collection properties
				.ToListAsync();
	
            return Ok(entities);
        }

        // GET api/$entityName$/5
        [ResponseType(typeof($entityName$))]
        public async Task<IHttpActionResult> Get$entityName$(int id)
        {
	        $entityName$ entity = await _dbContext.$entitySetName$
			    // TODO: Add Includes for reference and/or collection properties
				.SingleOrDefaultAsync(e => e.$entityName$Id == id);
	
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

            entity.TrackingState = TrackingState.Added;
            _dbContext.ApplyChanges(entity);


            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (_dbContext.$entitySetName$.Any(e => e.$entityName$Id == entity.$entityName$Id))
                {
                    return Conflict();
                }
                throw;
            }

            await _dbContext.LoadRelatedEntitiesAsync(entity);
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

            _dbContext.ApplyChanges(entity);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.$entitySetName$.Any(e => e.$entityName$Id == entity.$entityName$Id))
                {
                    return Conflict();
                }
                throw;
            }

			await _dbContext.LoadRelatedEntitiesAsync(entity);
			entity.AcceptChanges();
	        return Ok(entity);
        }

        // DELETE api/$entityName$/5
        public async Task<IHttpActionResult> Delete$entityName$(int id)
        {
			$entityName$ entity = await _dbContext.$entitySetName$
			    // TODO: Include child entities if any
				.SingleOrDefaultAsync(e => e.$entityName$Id == id);
			if (entity == null)
            {
                return Ok();
            }

			entity.TrackingState = TrackingState.Deleted;
			_dbContext.ApplyChanges(entity);

            try
            {
	            await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.$entitySetName$.Any(e => e.$entityName$Id == entity.$entityName$Id))
                {
                    return Conflict();
                }
                throw;
            }

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
            base.Dispose(disposing);
        }
    }
}
