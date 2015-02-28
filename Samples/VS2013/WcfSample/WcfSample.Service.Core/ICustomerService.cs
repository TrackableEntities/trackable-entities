using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfSample.Shared.Entities.Net45.Models;
using WcfSerializationHelper;

namespace WcfSample.Service.Core
{
    [ServiceContract(Namespace = "urn:trackable-entities:samples")]
    [DataContractSerializerPreserveReferences] // Configure serializer to handle cycles
    public interface ICustomerService
    {
        [OperationContract]
        Task<IEnumerable<Customer>> GetCustomers();

        [OperationContract]
        Task<Customer> GetCustomer(string id);
        
        [OperationContract]
        Task<Customer> UpdateCustomer(Customer customer);
        
        [OperationContract]
        Task<Customer> CreateCustomer(Customer customer);
        
        [OperationContract]
        Task<bool> DeleteCustomer(string id);
    }
}
