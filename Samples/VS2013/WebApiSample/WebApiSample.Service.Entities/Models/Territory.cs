using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    [Table("Territory")]
    public partial class Territory : ITrackable, IMergeable
    {
        public Territory()
        {
            Employees = new List<Employee>();
        }

        [StringLength(20)]
        public string TerritoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string TerritoryDescription { get; set; }

        public ICollection<Employee> Employees { get; set; }

		[NotMapped]
        public TrackingState TrackingState { get; set; }

		[NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }

		[NotMapped]
        public Guid EntityIdentifier { get; set; }
    }
}
