using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiSample.Mvvm.Client.Entities.Models;

namespace WebApiSample.Mvvm.WpfClient
{
    public class ProductServiceAgent : IProductServiceAgent
    {
        public async Task<IEnumerable<Product>> GetProducts()
        {
            const string request = "api/Product";
            var response = await ServiceProxy.Instance.GetAsync(request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<IEnumerable<Product>>(new[] { ServiceProxy.Formatter });
            return result;
        }
    }
}
