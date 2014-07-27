using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    [Table("Territory")]
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Territory : ITrackable
    {
        public Territory()
        {
            Employees = new List<Employee>();
        }

        [StringLength(20)]
		[DataMember]
        public string TerritoryId { get; set; }

        [Required]
        [StringLength(50)]
		[DataMember]
        public string TerritoryDescription { get; set; }

		[DataMember]
        public ICollection<Employee> Employees { get; set; }

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
