using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    [Table("CustomerSetting")]
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class CustomerSetting : ITrackable
    {
        [Key]
        [StringLength(5)]
		[DataMember]
        public string CustomerId { get; set; }

        [Required]
        [StringLength(50)]
		[DataMember]
        public string Setting { get; set; }

		[DataMember]
        public Customer Customer { get; set; }

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
