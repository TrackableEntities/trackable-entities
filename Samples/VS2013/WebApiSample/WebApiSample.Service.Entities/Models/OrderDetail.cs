using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    [Table("OrderDetail")]
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class OrderDetail : ITrackable
    {
		[DataMember]
        public int OrderDetailId { get; set; }

		[DataMember]
        public int OrderId { get; set; }

		[DataMember]
        public int ProductId { get; set; }

        [Column(TypeName = "money")]
		[DataMember]
        public decimal UnitPrice { get; set; }

		[DataMember]
        public short Quantity { get; set; }

		[DataMember]
        public float Discount { get; set; }

		[DataMember]
        public Order Order { get; set; }

		[DataMember]
        public Product Product { get; set; }

		[NotMapped]
        [DataMember]
        public TrackingState TrackingState { get; set; }

		[NotMapped]
        [DataMember]
        public ICollection<string> ModifiedProperties { get; set; }

		[NotMapped]
        [JsonProperty, DataMember]
        private Guid EntityIdentifier { get; set; }
    }
}
