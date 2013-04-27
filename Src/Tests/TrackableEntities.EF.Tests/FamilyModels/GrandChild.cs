using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrackableEntities.EF.Tests.FamilyModels
{
    public class GrandChild : ITrackable
    {
        public GrandChild() { }
        public GrandChild(string name)
        {
            Name = name;
        }

        [Key]
        public string Name { get; set; }
        public List<GrandGrandChild> Children { get; set; }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
