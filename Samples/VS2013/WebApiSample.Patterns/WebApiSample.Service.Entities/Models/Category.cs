using System;
using System.Collections.Generic;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    public partial class Category : ITrackable, IMergeable
    {
        public Category()
        {
            this.Products = new List<Product>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<Product> Products { get; set; }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}
