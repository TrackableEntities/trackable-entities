using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    [Table("Product")]
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Product : ITrackable
    {
        public Product()
        {
            OrderDetails = new List<OrderDetail>();
        }

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

		[DataMember]
        public bool Discontinued { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
		[DataMember]
        public byte[] RowVersion { get; set; }

		[DataMember]
        public Category Category { get; set; }

		[DataMember]
        public ICollection<OrderDetail> OrderDetails { get; set; }

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
