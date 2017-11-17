using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackableEntities.EF.Tests.FamilyModels
{
    public class Contact : ITrackable
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(ContactCategory))]
        public int ContactCategoryId { get; set; }

        public ContactCategory ContactCategory { get; set; }

        public ContactData ContactData { get; set; }

        [NotMapped]
        public TrackingState TrackingState { get; set; }

        [NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }
    }
}