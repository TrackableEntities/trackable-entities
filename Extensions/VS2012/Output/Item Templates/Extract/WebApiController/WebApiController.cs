using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using TrackableEntities;
using TrackableEntities.EF5;
using TrackableEntities.Common;
using $entitiesNamespace$;

// NOTE: Primary key name and/or type may need to be set manually.

namespace $rootnamespace$
{
	public class $safeitemname$ : ApiController
	{
        private readonly $dbContextName$ _dbContext = new $dbContextName$();

		// GET api/$entityName$
        public IEnumerable<$entityName$> Get$entitySetName$()
        {
            IEnumerable<$entityName$> entities = _dbContext.$entitySetName$
			    // TODO: Add Includes for reference and/or collection properties
				.ToList();

            return entities;
        }

        // GET api/$entityName$/5
        public $entityName$ Get$entityName$(int id)
        {
            $entityName$ entity = _dbContext.$entitySetName$
			    // TODO: Add Includes for reference and/or collection properties
                .SingleOrDefault(e => e.$entityName$Id == id);

            if (entity == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return entity;
        }

        // POST api/$entityName$
        public HttpResponseMessage Post$entityName$($entityName$ entity)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            entity.TrackingState = TrackingState.Added;
            _dbContext.ApplyChanges(entity);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (_dbContext.$entitySetName$.Any(e => e.$entityName$Id == entity.$entityName$Id))
                {
	                return Request.CreateErrorResponse(HttpStatusCode.Conflict, ex);
                }
                throw;
            }

            _dbContext.LoadRelatedEntities(entity);
            entity.AcceptChanges();

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, entity);
            response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = entity.$entityName$Id }));
            return response;
        }

        // PUT api/$entityName$
        public HttpResponseMessage Put$entityName$($entityName$ entity)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            _dbContext.ApplyChanges(entity);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_dbContext.$entitySetName$.Any(e => e.$entityName$Id == entity.$entityName$Id))
                {
	                return Request.CreateErrorResponse(HttpStatusCode.Conflict, ex);
                }
                throw;
            }

			_dbContext.LoadRelatedEntities(entity);
			entity.AcceptChanges();

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, entity);
            return response;
        }

        // DELETE api/$entityName$/5
        public HttpResponseMessage Delete$entityName$(int id)
        {
            $entityName$ entity = _dbContext.$entitySetName$
			    // TODO: Include child entities if any
                .SingleOrDefault(e => e.$entityName$Id == id);
            if (entity == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

			entity.TrackingState = TrackingState.Deleted;
			_dbContext.ApplyChanges(entity);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_dbContext.$entitySetName$.Any(e => e.$entityName$Id == entity.$entityName$Id))
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
