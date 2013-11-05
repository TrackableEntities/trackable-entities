using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Customer : ITrackable
    {
        public Customer()
        {
            this.Orders = new List<Order>();
        }

        [DataMember]
        public string CustomerId { get; set; }
        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public string ContactName { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public ICollection<Order> Orders { get; set; }

        [DataMember]
        public TrackingState TrackingState { get; set; }
        [DataMember]
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
