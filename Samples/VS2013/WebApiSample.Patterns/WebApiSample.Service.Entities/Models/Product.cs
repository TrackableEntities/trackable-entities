using System;
using System.Collections.Generic;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    public partial class Product : ITrackable, IMergeable
    {
        public Product()
        {
            this.OrderDetails = new List<OrderDetail>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public bool Discontinued { get; set; }
        public byte[] RowVersion { get; set; }
        public Category Category { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}
