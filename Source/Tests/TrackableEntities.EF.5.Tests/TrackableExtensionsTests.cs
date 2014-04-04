using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using TrackableEntities.Common;
using TrackableEntities.EF.Tests;
using TrackableEntities.EF.Tests.Mocks;
using TrackableEntities.EF.Tests.NorthwindModels;

#if EF_6
namespace TrackableEntities.EF6.Tests
#else
namespace TrackableEntities.EF5.Tests
#endif
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
            var states = parent.GetTrackingStates(TrackingState.Unchanged).ToList();
            Assert.AreEqual(26, states.Count());
        }

        [Test]
        public void Accept_Changes_Should_Remove_Family_Deleted()
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
            Assert.AreEqual(2, parent.Children.Count);
            Assert.AreEqual(2, parent.Children[0].Children[0].Children.Count);
        }

        [Test]
        public void Accept_Changes_Should_Remove_Family_ModifiedProperties()
        {
            // Arrange
            var parent = new MockFamily().Parent;
            parent.ModifiedProperties = new List<string> {"Name"};
            parent.Children[0].ModifiedProperties = new List<string> {"Name"};
            parent.Children[0].Children[0].ModifiedProperties = new List<string> {"Name"};
            parent.Children[0].Children[0].Children[1].ModifiedProperties = new List<string> {"Name"};

            // Act
            parent.AcceptChanges();

            // Assert
            IEnumerable<IEnumerable<string>> modifiedProps = parent.GetModifiedProperties();
            Assert.IsFalse(modifiedProps.Any(p => p != null));
        }

        [Test]
        public void Accept_Changes_Should_Mark_Employee_Unchanged()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var employee = northwind.Employees[0];
            employee.TrackingState = TrackingState.Modified;
            employee.Territories[0].TrackingState = TrackingState.Modified;
            employee.Territories[1].TrackingState = TrackingState.Deleted;
            employee.Territories.Add(northwind.Territories[2]); // unchanged
            var territory = new Territory
            {
                TerritoryId = "75070",
                TerritoryDescription = "North Dallas",
                TrackingState = TrackingState.Added,
                Employees = new List<Employee> {employee}
            };
            employee.Territories.Add(territory);

            // Act
            employee.AcceptChanges();

            // Assert
            IEnumerable<TrackingState> states = employee.GetTrackingStates(TrackingState.Unchanged);
            Assert.AreEqual(5, states.Count());
        }

        [Test]
        public void Accept_Changes_Should_Remove_Employee_ModifiedProperties()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var employee = northwind.Employees[0];
            employee.TrackingState = TrackingState.Modified;
            employee.ModifiedProperties = new List<string> {"LastName"};
            employee.Territories[0].ModifiedProperties = new List<string> {"TerritoryDescription"};

            // Act
            employee.AcceptChanges();

            // Assert
            IEnumerable<IEnumerable<string>> modifiedProps = employee.GetModifiedProperties();
            Assert.IsFalse(modifiedProps.Any(p => p != null));
        }
    }
}
