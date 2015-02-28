using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfSample.Shared.Entities.Net45.Models;

namespace WcfSample.Client.ConsoleApp
{
    [ServiceContract(Namespace = "urn:trackable-entities:samples")]
    public interface IOrderService
    {
        [OperationContract(Name = "GetOrders")]
        Task<IEnumerable<Order>> GetOrdersAsync();

        [OperationContract(Name = "GetCustomerOrders")]
        Task<IEnumerable<Order>> GetCustomerOrdersAsync(string customerId);

        [OperationContract(Name = "GetOrder")]
        Task<Order> GetOrderAsync(int id);

        [OperationContract(Name = "UpdateOrder")]
        Task<Order> UpdateOrderAsync(Order order);

        [OperationContract(Name = "CreateOrder")]
        Task<Order> CreateOrderAsync(Order order);

        [OperationContract(Name = "DeleteOrder")]
        Task<bool> DeleteOrderAsync(int id);
    }
}
