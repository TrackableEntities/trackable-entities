using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackableEntities.EF.Tests.FamilyModels
{
    public class ContactData : ITrackable
    {
        [Key, ForeignKey(nameof(Contact))]
        public int ContactId { get; set; }

        public string Data { get; set; }

        public Contact Contact { get; set; }

        [NotMapped]
        public TrackingState TrackingState { get; set; }

        [NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }
    }
}