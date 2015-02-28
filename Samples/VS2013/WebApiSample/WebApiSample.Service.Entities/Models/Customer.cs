using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    [Table("Customer")]
    public partial class Customer : ITrackable, IMergeable
    {
        public Customer()
        {
            Orders = new List<Order>();
        }

        [StringLength(5)]
        public string CustomerId { get; set; }

        [Required]
        [StringLength(40)]
        public string CompanyName { get; set; }

        [StringLength(30)]
        public string ContactName { get; set; }

        [StringLength(15)]
        public string City { get; set; }

        [StringLength(15)]
        public string Country { get; set; }

        public CustomerSetting CustomerSetting { get; set; }

        public ICollection<Order> Orders { get; set; }

		[NotMapped]
        public TrackingState TrackingState { get; set; }

		[NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }

		[NotMapped]
        public Guid EntityIdentifier { get; set; }
    }
}
