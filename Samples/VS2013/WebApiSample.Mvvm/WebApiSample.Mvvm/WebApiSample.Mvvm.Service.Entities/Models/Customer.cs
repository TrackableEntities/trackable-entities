using System;
using System.Collections.Generic;
using TrackableEntities;

namespace WebApiSample.Mvvm.Service.Entities.Models
{
    public partial class Customer : ITrackable, IMergeable
    {
        public Customer()
        {
            this.Orders = new List<Order>();
        }

        public string CustomerId { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public CustomerSetting CustomerSetting { get; set; }
        public List<Order> Orders { get; set; }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}
