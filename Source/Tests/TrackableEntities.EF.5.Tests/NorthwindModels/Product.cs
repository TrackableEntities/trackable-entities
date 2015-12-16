using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackableEntities.EF.Tests.NorthwindModels
{
    public partial class Product : ITrackable
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public bool Discontinued { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public int? PromoId { get; set; }
        [ForeignKey("PromoId")]
        public HolidayPromo HolidayPromo { get; set; }

        [ForeignKey("ProductInfo"), Column(Order = 1)]
        public int? ProductInfoKey1 { get; set; }
        [ForeignKey("ProductInfo"), Column(Order = 2)]
        public int? ProductInfoKey2 { get; set; }
        public ProductInfo ProductInfo { get; set; }

        [NotMapped]
        public TrackingState TrackingState { get; set; }
        [NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
