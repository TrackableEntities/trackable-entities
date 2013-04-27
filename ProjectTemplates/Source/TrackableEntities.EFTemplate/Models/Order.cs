using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;

namespace TrackableEntities.EFTemplate.Models
{
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Order : ITrackable
    {
        public Order()
        {
            this.OrderDetails = new List<OrderDetail>();
        }

        [DataMember]
        public int OrderId { get; set; }
        [DataMember]
        public string CustomerId { get; set; }
        [DataMember]
        public Nullable<System.DateTime> OrderDate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ShippedDate { get; set; }
        [DataMember]
        public Nullable<int> ShipVia { get; set; }
        [DataMember]
        public Nullable<decimal> Freight { get; set; }
        [DataMember]
        public Customer Customer { get; set; }
        [DataMember]
        public ICollection<OrderDetail> OrderDetails { get; set; }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
