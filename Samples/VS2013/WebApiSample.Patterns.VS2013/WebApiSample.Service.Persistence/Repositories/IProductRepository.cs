using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackableEntities.Patterns;
using WebApiSample.Service.Entities.Models;

namespace WebApiSample.Service.Persistence.Repositories
{
    public interface IProductRepository : IRepository<Product>, IRepositoryAsync<Product>
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> GetProduct(int id);
        Task<bool> DeleteProduct(int id);
    }
}
