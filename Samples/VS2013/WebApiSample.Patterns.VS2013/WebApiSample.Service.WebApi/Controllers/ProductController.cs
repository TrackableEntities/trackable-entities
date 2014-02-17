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
    public class ProductController : ApiController
    {
        // TODO: Rename IExampleUnitOfWork to match Unit of Work Interface added to Persistence project
        private readonly INorthwindSlimUnitOfWork _unitOfWork;

        // TODO: Rename IExampleUnitOfWork parameter
        public ProductController(INorthwindSlimUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET api/Product
        [ResponseType(typeof(IEnumerable<Product>))]
        public async Task<IHttpActionResult> GetProducts()
        {
            IEnumerable<Product> entities = await _unitOfWork.ProductRepository.GetProducts();
            return Ok(entities);
        }

        // GET api/Product/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProduct(int id)
        {
            Product entity = await _unitOfWork.ProductRepository.GetProduct(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        // PUT api/Product
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> PutProduct(Product entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _unitOfWork.ProductRepository.Update(entity);
                await _unitOfWork.SaveChangesAsync();
                entity.AcceptChanges();
                return Ok(entity);
            }
            catch (UpdateConcurrencyException)
            {
                if (_unitOfWork.ProductRepository.Find(entity.ProductId) == null)
                {
                    return NotFound();
                }
                throw;
            }
        }

        // POST api/Product
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> PostProduct(Product entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.ProductRepository.Insert(entity);

            try
            {
                await _unitOfWork.SaveChangesAsync();
                entity.AcceptChanges();
            }
            catch (UpdateException)
            {
                if (_unitOfWork.ProductRepository.Find(entity.ProductId) == null)
                {
                    return Conflict();
                }
                throw;
            }

            return CreatedAtRoute("DefaultApi", new { id = entity.ProductId }, entity);
        }

        // DELETE api/Product/5
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            bool exists = await _unitOfWork.ProductRepository.DeleteAsync(id);
            if (!exists) return Ok();

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (UpdateConcurrencyException)
            {
                if (_unitOfWork.ProductRepository.Find(id) == null)
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
