using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackableEntities.EF.Tests.NorthwindModels
{
    public partial class Area : ITrackable
    {
        [Key]
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public List<Territory> Territories { get; set; }

        [NotMapped]
        public TrackingState TrackingState { get; set; }
        [NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
