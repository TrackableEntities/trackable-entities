using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    [Table("Product")]
    public partial class Product : ITrackable, IMergeable
    {
        public Product()
        {
            OrderDetails = new List<OrderDetail>();
        }

        public int ProductId { get; set; }

        [Required]
        [StringLength(40)]
        public string ProductName { get; set; }

        public int? CategoryId { get; set; }

        [Column(TypeName = "money")]
        public decimal? UnitPrice { get; set; }

        public bool Discontinued { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        public Category Category { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }

		[NotMapped]
        public TrackingState TrackingState { get; set; }

		[NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }

		[NotMapped]
        public Guid EntityIdentifier { get; set; }
    }
}
