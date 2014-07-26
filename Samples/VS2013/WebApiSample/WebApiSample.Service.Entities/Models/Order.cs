using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Order : ITrackable
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

		[DataMember]
        public int OrderId { get; set; }

		[DataMember]
        public string CustomerId { get; set; }

		[DataMember]
        public DateTime? OrderDate { get; set; }

		[DataMember]
        public DateTime? ShippedDate { get; set; }

		[DataMember]
        public int? ShipVia { get; set; }

		[DataMember]
        public decimal? Freight { get; set; }

		[DataMember]
        public Customer Customer { get; set; }

		[DataMember]
        public ICollection<OrderDetail> OrderDetails { get; set; }

        [DataMember]
        public TrackingState TrackingState { get; set; }

        [DataMember]
        public ICollection<string> ModifiedProperties { get; set; }

        [JsonProperty, DataMember]
        private Guid EntityIdentifier { get; set; }
    }
}
