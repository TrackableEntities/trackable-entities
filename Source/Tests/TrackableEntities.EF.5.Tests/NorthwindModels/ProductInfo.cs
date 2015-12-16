using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackableEntities.EF.Tests.NorthwindModels
{
    public partial class ProductInfo : ITrackable
    {
        [Key, Column(Order = 1)]
        public int ProductInfoKey1 { get; set; }
        [Key, Column(Order = 2)]
        public int ProductInfoKey2 { get; set; }

        public string Info { get; set; }

        [NotMapped]
        public TrackingState TrackingState { get; set; }
        [NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
