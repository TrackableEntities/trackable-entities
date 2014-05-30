using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackableEntities.EF.Tests.NorthwindModels
{
    public partial class CustomerSetting : ITrackable
    {
        [Key]
        public string CustomerId { get; set; }
        public string Setting { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [NotMapped]
        public TrackingState TrackingState { get; set; }
        [NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
