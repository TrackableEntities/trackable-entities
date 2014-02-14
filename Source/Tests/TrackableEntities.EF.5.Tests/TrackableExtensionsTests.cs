using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TrackableEntities.Common;
using TrackableEntities.EF.Tests.Mocks;

namespace TrackableEntities.EF.Tests
{
    [TestFixture]
    public class TrackableExtensionsTests
    {
        [Test]
        public void Accept_Changes_Should_Mark_Family_Unchanged()
        {
            // Arrange
            var parent = new MockFamily().Parent;
            parent.TrackingState = TrackingState.Modified;
            parent.Children[0].TrackingState = TrackingState.Modified;
            parent.Children[0].Children[0].TrackingState = TrackingState.Modified;
            parent.Children[0].Children[0].Children[0].TrackingState = TrackingState.Added;
            parent.Children[0].Children[0].Children[1].TrackingState = TrackingState.Modified;
            parent.Children[0].Children[0].Children[2].TrackingState = TrackingState.Deleted;
            parent.Children[1].TrackingState = TrackingState.Added;
            parent.Children[1].Children[0].TrackingState = TrackingState.Added;
            parent.Children[1].Children[0].Children[0].TrackingState = TrackingState.Added;
            parent.Children[1].Children[0].Children[1].TrackingState = TrackingState.Added;
            parent.Children[1].Children[0].Children[2].TrackingState = TrackingState.Added;
            parent.Children[2].TrackingState = TrackingState.Deleted;
            parent.Children[2].Children[1].TrackingState = TrackingState.Deleted;
            parent.Children[2].Children[1].Children[0].TrackingState = TrackingState.Deleted;
            parent.Children[2].Children[1].Children[1].TrackingState = TrackingState.Deleted;
            parent.Children[2].Children[1].Children[2].TrackingState = TrackingState.Deleted;

            // Act
            parent.AcceptChanges();

            // Assert
            IEnumerable<TrackingState> states = parent.GetTrackingStates(TrackingState.Unchanged);
            Assert.AreEqual(40, states.Count());
            IEnumerable<IEnumerable<string>> modifiedProps = parent.GetModifiedProperties();
            Assert.IsFalse(modifiedProps.Any(p => p != null));
        }
    }
}
