using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    [Table("Order")]
    public partial class Order : ITrackable, IMergeable
    {
        public Order()
        {
            OrderDetails = new List<OrderDetail>();
        }

        public int OrderId { get; set; }

        [StringLength(5)]
        public string CustomerId { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public int? ShipVia { get; set; }

        [Column(TypeName = "money")]
        public decimal? Freight { get; set; }

        public Customer Customer { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }

		[NotMapped]
        public TrackingState TrackingState { get; set; }

		[NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }

		[NotMapped]
        public Guid EntityIdentifier { get; set; }
    }
}
