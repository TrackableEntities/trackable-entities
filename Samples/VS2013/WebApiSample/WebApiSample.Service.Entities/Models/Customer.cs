using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    [Table("Customer")]
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Customer : ITrackable
    {
        public Customer()
        {
            Orders = new List<Order>();
        }

        [StringLength(5)]
		[DataMember]
        public string CustomerId { get; set; }

        [Required]
        [StringLength(40)]
		[DataMember]
        public string CompanyName { get; set; }

        [StringLength(30)]
		[DataMember]
        public string ContactName { get; set; }

        [StringLength(15)]
		[DataMember]
        public string City { get; set; }

        [StringLength(15)]
		[DataMember]
        public string Country { get; set; }

		[DataMember]
        public CustomerSetting CustomerSetting { get; set; }

		[DataMember]
        public ICollection<Order> Orders { get; set; }

		[NotMapped]
        [DataMember]
        public TrackingState TrackingState { get; set; }

		[NotMapped]
        [DataMember]
        public ICollection<string> ModifiedProperties { get; set; }

		[NotMapped]
        [JsonProperty, DataMember]
        private Guid EntityIdentifier { get; set; }
    }
}
