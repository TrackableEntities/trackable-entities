using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiSample.Mvvm.Client.Entities.Models;

namespace WebApiSample.Mvvm.WpfClient
{
    public class OrderServiceAgent : IOrderServiceAgent
    {
        public async Task<IEnumerable<Order>> GetCustomerOrders(string customerId)
        {
            string request = "api/Order?customerId=" + customerId;
            var response = await ServiceProxy.Instance.GetAsync(request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<IEnumerable<Order>>();
            return result;
        }

        public async Task<Order> GetOrder(int orderId)
        {
            string request = "api/Order/" + orderId;
            var response = await ServiceProxy.Instance.GetAsync(request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<Order>();
            return result;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            string request = "api/Order";
            var response = await ServiceProxy.Instance.PostAsJsonAsync(request, order);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<Order>();
            return result;
        }

        public async Task<Order> UpdateOrder(Order order)
        {
            string request = "api/Order";
            var response = await ServiceProxy.Instance.PutAsJsonAsync(request, order);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<Order>();
            return result;
        }

        public async Task DeleteOrder(int orderId)
        {
            string request = "api/Order/" + orderId;
            var response =  await ServiceProxy.Instance.DeleteAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> VerifyOrderDeleted(int orderId)
        {
            string request = "api/Order/" + orderId;
            var response = await ServiceProxy.Instance.GetAsync(request);
            if (response.IsSuccessStatusCode) return false;
            return true;
        }
    }
}
