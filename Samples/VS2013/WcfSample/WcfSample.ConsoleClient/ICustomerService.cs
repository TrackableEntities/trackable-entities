using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfSample.Shared.Entities.Net45.Models;

namespace WcfSample.Client.ConsoleApp
{
    [ServiceContract(Namespace = "urn:trackable-entities:samples")]
    public interface ICustomerService
    {
        [OperationContract(Name = "GetCustomers")]
        Task<IEnumerable<Customer>> GetCustomersAsync();

        [OperationContract(Name = "GetCustomer")]
        Task<Customer> GetCustomerAsync(string id);

        [OperationContract(Name = "UpdateCustomer")]
        Task<Customer> UpdateCustomerAsync(Customer customer);

        [OperationContract(Name = "CreateCustomer")]
        Task<Customer> CreateCustomerAsync(Customer customer);

        [OperationContract(Name = "DeleteCustomer")]
        Task DeleteCustomerAsync(string id);
    }
}
