using System;
using System.Collections.Generic;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    public partial class Order : ITrackable, IMergeable
    {
        public Order()
        {
            this.OrderDetails = new List<OrderDetail>();
        }

        public int OrderId { get; set; }
        public string CustomerId { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<System.DateTime> ShippedDate { get; set; }
        public Nullable<int> ShipVia { get; set; }
        public Nullable<decimal> Freight { get; set; }
        public Customer Customer { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}
