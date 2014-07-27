using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    [Table("Employee")]
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Employee : ITrackable
    {
        public Employee()
        {
            Territories = new List<Territory>();
        }

		[DataMember]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(20)]
		[DataMember]
        public string LastName { get; set; }

        [Required]
        [StringLength(20)]
		[DataMember]
        public string FirstName { get; set; }

		[DataMember]
        public DateTime? BirthDate { get; set; }

		[DataMember]
        public DateTime? HireDate { get; set; }

        [StringLength(15)]
		[DataMember]
        public string City { get; set; }

        [StringLength(15)]
		[DataMember]
        public string Country { get; set; }

		[DataMember]
        public ICollection<Territory> Territories { get; set; }

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
