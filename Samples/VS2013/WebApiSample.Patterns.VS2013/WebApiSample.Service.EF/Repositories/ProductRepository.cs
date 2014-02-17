using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using TrackableEntities.Patterns.EF6;
using WebApiSample.Service.EF.Contexts;
using WebApiSample.Service.Entities.Models;
using WebApiSample.Service.Persistence.Repositories;

namespace WebApiSample.Service.EF.Repositories
{
    // NOTE: First add Product Repository Interface in Service.Persistence project

    public class ProductRepository : Repository<Product>, IProductRepository
    {
        // TODO: Match Database Context Interface type
        private readonly INorthwindContext _context;

        // TODO: Match Database Context Interface type
        public ProductRepository(INorthwindContext context) :
            base(context as DbContext)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            // TODO: Add Includes for related entities if needed
            IEnumerable<Product> entities = await _context.Products
                .ToListAsync();
            return entities;
        }

        public async Task<Product> GetProduct(int id)
        {
            // TODO: Add Includes for related entities if needed
            Product entity = await _context.Products
                 .SingleOrDefaultAsync(t => t.ProductId == id);
            return entity;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            Product entity = await GetProduct(id);
            if (entity == null) return false;
            Set.Attach(entity);
            Set.Remove(entity);

            // TODO: Remove child entities
            return true;
        }

        // TODO: Add methods to load related entities if needed
    }
}
