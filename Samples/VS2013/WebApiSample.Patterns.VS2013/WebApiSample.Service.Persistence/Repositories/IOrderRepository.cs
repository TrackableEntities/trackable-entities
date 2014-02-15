using System.Collections.Generic;
using System.Threading.Tasks;
using TrackableEntities.Patterns;
using WebApiSample.Service.Entities.Models;

namespace WebApiSample.Service.Persistence.Repositories
{
    public interface IOrderRepository : IRepository<Order>, IRepositoryAsync<Order>
    {
        Task<IEnumerable<Order>> GetOrders();
        Task<IEnumerable<Order>> GetOrders(string customerId);
        Task<Order> GetOrder(int id);
        Task<bool> DeleteOrder(int id);
        void LoadRelatedEntities(Order order);
        void LoadProductsOnAddedDetails(Order order);
    }
}
