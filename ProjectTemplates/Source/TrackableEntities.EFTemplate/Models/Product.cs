using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;

namespace TrackableEntities.EFTemplate.Models
{
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Product : ITrackable
    {
        public Product()
        {
            this.OrderDetails = new List<OrderDetail>();
        }

        [DataMember]
        public int ProductId { get; set; }
        [DataMember]
        public string ProductName { get; set; }
        [DataMember]
        public Nullable<int> CategoryId { get; set; }
        [DataMember]
        public Nullable<decimal> UnitPrice { get; set; }
        [DataMember]
        public bool Discontinued { get; set; }
        [DataMember]
        public byte[] RowVersion { get; set; }
        [DataMember]
        public Category Category { get; set; }
        [DataMember]
        public ICollection<OrderDetail> OrderDetails { get; set; }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
