using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackableEntities.EF.Tests.NorthwindModels
{
    public partial class Category : ITrackable
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<Product> Products { get; set; }

        [NotMapped]
        public TrackingState TrackingState { get; set; }
        [NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
