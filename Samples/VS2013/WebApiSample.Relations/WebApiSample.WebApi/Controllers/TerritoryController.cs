using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TrackableEntities;
using TrackableEntities.EF6;
using TrackableEntities.Common;
using WebApiSample.Service.Entities.Models;
using WebApiSample.Service.Entities.Contexts;

namespace WebApiSample.Service.WebApi.Controllers
{
    public class TerritoryController : ApiController
    {
        private readonly NorthwindSlimContext _dbContext = new NorthwindSlimContext();

        // GET api/Territory
        [ResponseType(typeof(IEnumerable<Territory>))]
        public async Task<IHttpActionResult> GetTerritories()
        {
	        IEnumerable<Territory> territories = await _dbContext.Territories.ToListAsync();
	
            return Ok(territories);
        }

        // GET api/Territory/5
        [ResponseType(typeof(Territory))]
        public async Task<IHttpActionResult> GetTerritory(string id)
        {
	        Territory territory = await _dbContext.Territories.SingleOrDefaultAsync(t => t.TerritoryId == id);
	
            if (territory == null)
            {
                return NotFound();
            }

            return Ok(territory);
        }

        // POST api/Territory
        [ResponseType(typeof(Territory))]
        public async Task<IHttpActionResult> PostTerritory(Territory territory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            territory.TrackingState = TrackingState.Added;
            _dbContext.ApplyChanges(territory);


            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (_dbContext.Territories.Any(t => t.TerritoryId == territory.TerritoryId))
                {
                    return Conflict();
                }
                throw;
            }

            await _dbContext.LoadRelatedEntitiesAsync(territory);
            territory.AcceptChanges();
            return CreatedAtRoute("DefaultApi", new { id = territory.TerritoryId }, territory);
        }

        // PUT api/Territory
        [ResponseType(typeof(Territory))]
        public async Task<IHttpActionResult> PutTerritory(Territory territory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.ApplyChanges(territory);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Territories.Any(t => t.TerritoryId == territory.TerritoryId))
                {
                    return Conflict();
                }
                throw;
            }

			await _dbContext.LoadRelatedEntitiesAsync(territory);
			territory.AcceptChanges();
	        return Ok(territory);
        }

        // DELETE api/Territory/5
        public async Task<IHttpActionResult> DeleteTerritory(string id)
        {
			Territory territory = await _dbContext.Territories
				.SingleOrDefaultAsync(t => t.TerritoryId == id);
			if (territory == null)
            {
                return Ok();
            }

			territory.TrackingState = TrackingState.Deleted;
			_dbContext.ApplyChanges(territory);

            try
            {
	            await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Territories.Any(t => t.TerritoryId == territory.TerritoryId))
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
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}