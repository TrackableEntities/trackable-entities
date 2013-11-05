using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrackableEntities.EF.Tests.FamilyModels
{
    public class GrandGrandChild : ITrackable
    {
        public GrandGrandChild() { }
        public GrandGrandChild(string name)
        {
            Name = name;
        }

        [Key]
        public string Name { get; set; }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
