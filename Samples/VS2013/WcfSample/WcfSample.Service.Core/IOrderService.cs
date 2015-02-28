using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfSample.Shared.Entities.Net45.Models;
using WcfSerializationHelper;

namespace WcfSample.Service.Core
{
    [ServiceContract(Namespace = "urn:trackable-entities:samples")]
    [DataContractSerializerPreserveReferences] // Configure serializer to handle cycles
    public interface IOrderService
    {
        [OperationContract]
        Task<IEnumerable<Order>> GetOrders();

        [OperationContract]
        Task<IEnumerable<Order>> GetCustomerOrders(string customerId);

        [OperationContract]
        Task<Order> GetOrder(int id);

        [OperationContract]
        Task<Order> UpdateOrder(Order order);

        [OperationContract]
        Task<Order> CreateOrder(Order order);

        [OperationContract]
        Task<bool> DeleteOrder(int id);
    }
}
