using System;
using System.Collections.Generic;
using TrackableEntities;

namespace WebApiSample.Mvvm.Service.Entities.Models
{
    public partial class Territory : ITrackable, IMergeable
    {
        public Territory()
        {
            this.Employees = new List<Employee>();
        }

        public string TerritoryId { get; set; }
        public string TerritoryDescription { get; set; }
        public List<Employee> Employees { get; set; }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}
