using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiSample.Mvvm.Client.Entities.Models;

namespace WebApiSample.Mvvm.WpfClient
{
    public interface IOrderServiceAgent
    {
        Task<IEnumerable<Order>> GetCustomerOrders(string customerId);

        Task<Order> GetOrder(int orderId);

        Task<Order> CreateOrder(Order order);

        Task<Order> UpdateOrder(Order order);

        Task DeleteOrder(int orderId);

        Task<bool> VerifyOrderDeleted(int orderId);
    }
}
