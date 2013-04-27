using System.Collections.Generic;
using TrackableEntities.Client.Tests.Entities.FamilyModels;

namespace TrackableEntities.Client.Tests.Entities.Mocks
{
    public class MockFamily
    {
        public Parent Parent { get; private set; }

        public MockFamily()
        {
            var child1 = new Child("Child1") { Children = new ChangeTrackingCollection<GrandChild>(CreateGrandChildren("Child1"), true) };
            var child2 = new Child("Child2") { Children = new ChangeTrackingCollection<GrandChild>(CreateGrandChildren("Child2"), true) };
            var child3 = new Child("Child3") { Children = new ChangeTrackingCollection<GrandChild>(CreateGrandChildren("Child3"), true) };
            Parent = new Parent("Parent")
                {
                    Children = new ChangeTrackingCollection<Child> { child1, child2, child3 }
                };
        }

        IEnumerable<GrandChild> CreateGrandChildren(string childName)
        {
            var grandChild1Name = childName + "-" + "GrandChild1";
            var grandChild1 = new GrandChild(grandChild1Name)
            {
                Children = new ChangeTrackingCollection<GrandGrandChild>
                        {
                            new GrandGrandChild(grandChild1Name + "-" + "GrandGrandChild1"),
                            new GrandGrandChild(grandChild1Name + "-" + "GrandGrandChild2"),
                            new GrandGrandChild(grandChild1Name + "-" + "GrandGrandChild3")
                        }
            };
            var grandChild2Name = childName + "-" + "GrandChild2";
            var grandChild2 = new GrandChild(grandChild2Name)
            {
                Children = new ChangeTrackingCollection<GrandGrandChild>
                        {
                            new GrandGrandChild(grandChild2Name + "-" + "GrandGrandChild1"),
                            new GrandGrandChild(grandChild2Name + "-" + "GrandGrandChild2"),
                            new GrandGrandChild(grandChild2Name + "-" + "GrandGrandChild3")
                        }
            };
            var grandChild3Name = childName + "-" + "GrandChild3";
            var grandChild3 = new GrandChild(grandChild3Name)
            {
                Children = new ChangeTrackingCollection<GrandGrandChild>
                        {
                            new GrandGrandChild(grandChild3Name + "-" + "GrandGrandChild1"),
                            new GrandGrandChild(grandChild3Name + "-" + "GrandGrandChild2"),
                            new GrandGrandChild(grandChild3Name + "-" + "GrandGrandChild3")
                        }
            };
            var result = new List<GrandChild> {grandChild1, grandChild2, grandChild3};
            return result;
        }
    }
}
