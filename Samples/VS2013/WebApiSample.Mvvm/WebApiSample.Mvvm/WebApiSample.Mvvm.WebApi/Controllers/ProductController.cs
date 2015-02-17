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
using WebApiSample.Mvvm.Service.Entities.Contexts;
using WebApiSample.Mvvm.Service.Entities.Models;

namespace WebApiSample.Mvvm.WebApi.Controllers
{
    public class ProductController : ApiController
    {
        private readonly NorthwindSlimContext _dbContext = new NorthwindSlimContext();

        // GET api/Product
        [ResponseType(typeof(IEnumerable<Product>))]
        public async Task<IHttpActionResult> GetProducts()
        {
            IEnumerable<Product> entities = await _dbContext.Products
                .Include(e => e.Category)
                .OrderBy(e => e.ProductName)
                .ToListAsync();

            return Ok(entities);
        }

        // GET api/Product/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProduct(int id)
        {
            Product entity = await _dbContext.Products
                .Include(e => e.Category)
                .SingleOrDefaultAsync(e => e.ProductId == id);

            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        // POST api/Product
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> PostProduct(Product entity)
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
                if (_dbContext.Products.Any(e => e.ProductId == entity.ProductId))
                {
                    return Conflict();
                }
                throw;
            }

            await _dbContext.LoadRelatedEntitiesAsync(entity);
            entity.AcceptChanges();
            return CreatedAtRoute("DefaultApi", new { id = entity.ProductId }, entity);
        }

        // PUT api/Product
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> PutProduct(Product entity)
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
                if (!_dbContext.Products.Any(e => e.ProductId == entity.ProductId))
                {
                    return Conflict();
                }
                throw;
            }

            await _dbContext.LoadRelatedEntitiesAsync(entity);
            entity.AcceptChanges();
            return Ok(entity);
        }

        // DELETE api/Product/5
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            Product entity = await _dbContext.Products
                .SingleOrDefaultAsync(e => e.ProductId == id);
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
                if (!_dbContext.Products.Any(e => e.ProductId == entity.ProductId))
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
