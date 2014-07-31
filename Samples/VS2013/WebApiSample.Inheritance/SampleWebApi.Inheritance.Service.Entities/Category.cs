namespace SampleWebApi.Inheritance.Service.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
	using System.Runtime.Serialization;
	using Newtonsoft.Json;
	using TrackableEntities;

    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Category : ITrackable
    {
        public Category()
        {
            Products = new List<Product>();
        }

		[DataMember]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(15)]
		[DataMember]
        public string CategoryName { get; set; }

		[DataMember]
        public ICollection<Product> Products { get; set; }

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
