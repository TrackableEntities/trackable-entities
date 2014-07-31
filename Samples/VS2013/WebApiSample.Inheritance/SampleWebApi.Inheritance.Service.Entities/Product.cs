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
    public partial class Product : ITrackable
    {
		[DataMember]
        public int ProductId { get; set; }

        [Required]
        [StringLength(40)]
		[DataMember]
        public string ProductName { get; set; }

		[DataMember]
        public int? CategoryId { get; set; }

        [Column(TypeName = "money")]
		[DataMember]
        public decimal? UnitPrice { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        [DataMember]
        public byte[] RowVersion { get; set; }

		[DataMember]
        public DateTime? DiscontinuedDate { get; set; }

        [Required]
        [StringLength(128)]
		[DataMember]
        public string Discriminator { get; set; }

		[DataMember]
        public Category Category { get; set; }

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
