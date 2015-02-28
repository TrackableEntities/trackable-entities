using System;
using System.Collections.Generic;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    public partial class OrderDetail : ITrackable, IMergeable
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}
