using System;
using System.Collections.Generic;
using TrackableEntities;

namespace WebApiSample.Mvvm.Service.Entities.Models
{
    public partial class CustomerSetting : ITrackable, IMergeable
    {
        public string CustomerId { get; set; }
        public string Setting { get; set; }
        public Customer Customer { get; set; }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}
