using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackableEntities.EF.Tests.NorthwindModels
{
    public class Territory : ITrackable
    {
        public Territory()
        {
            Employees = new List<Employee>();
        }

        [Key]
        public string TerritoryId { get; set; }
        public string TerritoryDescription { get; set; }
        public List<Employee> Employees { get; set; }

        [NotMapped]
        public TrackingState TrackingState { get; set; }
        [NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
