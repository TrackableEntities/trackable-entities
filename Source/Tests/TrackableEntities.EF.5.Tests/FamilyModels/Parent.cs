using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [NotMapped]
        public string Nickname1 { get; set; }
        public string Nickname2 { get; set; }

        public List<Child> Children { get; set; }

        public Address Address { get; set; } = new Address();

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
