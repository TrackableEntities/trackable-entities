using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackableEntities.EF.Tests.NorthwindModels
{
    public class Customer : ITrackable
    {
        [Key]
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }

        public string TerritoryId { get; set; }
        [ForeignKey("TerritoryId")]
        public Territory Territory { get; set; }

        public List<Order> Orders { get; set; }
        public CustomerSetting CustomerSetting { get; set; }

        [NotMapped]
        public TrackingState TrackingState { get; set; }
        [NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
