using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    [Table("CustomerSetting")]
    public partial class CustomerSetting : ITrackable, IMergeable
    {
        [Key]
        [StringLength(5)]
        public string CustomerId { get; set; }

        [Required]
        [StringLength(50)]
        public string Setting { get; set; }

        public Customer Customer { get; set; }

		[NotMapped]
        public TrackingState TrackingState { get; set; }

		[NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }

		[NotMapped]
        public Guid EntityIdentifier { get; set; }
    }
}
