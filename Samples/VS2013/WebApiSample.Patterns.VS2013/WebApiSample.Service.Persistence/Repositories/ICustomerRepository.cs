using System.Collections.Generic;
using System.Threading.Tasks;
using TrackableEntities.Patterns;
using WebApiSample.Service.Entities.Models;

namespace WebApiSample.Service.Persistence.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer> GetCustomer(string id);
        Task<IEnumerable<Customer>> GetCustomers();
    }
}
