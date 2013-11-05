using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrackableEntities.EF.Tests.FamilyModels
{
    public class Parent : ITrackable
    {
        public Parent() { }
        public Parent(string name)
        {
            Name = name;
        }

        [Key]
        public string Name { get; set; }
        public List<Child> Children { get; set; }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
