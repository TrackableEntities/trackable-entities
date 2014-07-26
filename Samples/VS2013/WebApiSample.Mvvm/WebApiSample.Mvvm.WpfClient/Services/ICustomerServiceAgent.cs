using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiSample.Mvvm.Client.Entities.Models;

namespace WebApiSample.Mvvm.WpfClient
{
    public interface ICustomerServiceAgent
    {
        Task<IEnumerable<Customer>> GetCustomers();
    }
}
