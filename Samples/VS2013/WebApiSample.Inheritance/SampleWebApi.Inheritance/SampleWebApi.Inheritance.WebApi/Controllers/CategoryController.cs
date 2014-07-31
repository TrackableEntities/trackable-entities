using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SampleWebApi.Inheritance.Entities;
using TrackableEntities;
using TrackableEntities.Common;
using TrackableEntities.EF6;

namespace SampleWebApi.Inheritance.WebApi.Controllers
{
    public class CategoryController : ApiController
    {
        private readonly NorthwindSlimInheritance _dbContext = new NorthwindSlimInheritance();

        // GET api/Category
        [ResponseType(typeof(IEnumerable<Category>))]
        public async Task<IHttpActionResult> GetCategories()
        {
            IEnumerable<Category> entities = await _dbContext.Categories
                .ToListAsync();

            return Ok(entities);
        }

        // GET api/Category/5
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> GetCategory(int id)
        {
            Category entity = await _dbContext.Categories
                .SingleOrDefaultAsync(e => e.CategoryId == id);

            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        // POST api/Category
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> PostCategory(Category entity)
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
                if (_dbContext.Categories.Any(e => e.CategoryId == entity.CategoryId))
                {
                    return Conflict();
                }
                throw;
            }

            await _dbContext.LoadRelatedEntitiesAsync(entity);
            entity.AcceptChanges();
            return CreatedAtRoute("DefaultApi", new { id = entity.CategoryId }, entity);
        }

        // PUT api/Category
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> PutCategory(Category entity)
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
                if (!_dbContext.Categories.Any(e => e.CategoryId == entity.CategoryId))
                {
                    return Conflict();
                }
                throw;
            }

            await _dbContext.LoadRelatedEntitiesAsync(entity);
            entity.AcceptChanges();
            return Ok(entity);
        }

        // DELETE api/Category/5
        public async Task<IHttpActionResult> DeleteCategory(int id)
        {
            Category entity = await _dbContext.Categories
                .SingleOrDefaultAsync(e => e.CategoryId == id);
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
                if (!_dbContext.Categories.Any(e => e.CategoryId == entity.CategoryId))
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
