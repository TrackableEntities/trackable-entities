using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    [Table("Order")]
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Order : ITrackable
    {
        public Order()
        {
            OrderDetails = new List<OrderDetail>();
        }

		[DataMember]
        public int OrderId { get; set; }

        [StringLength(5)]
		[DataMember]
        public string CustomerId { get; set; }

		[DataMember]
        public DateTime? OrderDate { get; set; }

		[DataMember]
        public DateTime? ShippedDate { get; set; }

		[DataMember]
        public int? ShipVia { get; set; }

        [Column(TypeName = "money")]
		[DataMember]
        public decimal? Freight { get; set; }

		[DataMember]
        public Customer Customer { get; set; }

		[DataMember]
        public ICollection<OrderDetail> OrderDetails { get; set; }

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
