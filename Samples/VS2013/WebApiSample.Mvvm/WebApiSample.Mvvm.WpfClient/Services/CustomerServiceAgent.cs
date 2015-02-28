using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiSample.Mvvm.Client.Entities.Models;

namespace WebApiSample.Mvvm.WpfClient
{
    public class CustomerServiceAgent : ICustomerServiceAgent
    {
        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            const string request = "api/Customer";
            var response = await ServiceProxy.Instance.GetAsync(request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<IEnumerable<Customer>>(new[] { ServiceProxy.Formatter });
            return result;
        }
    }
}
