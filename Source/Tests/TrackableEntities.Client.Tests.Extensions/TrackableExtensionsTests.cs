using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using TrackableEntities.Client.Tests.Entities.Mocks;
using TrackableEntities.Client.Tests.Entities.FamilyModels;
using TrackableEntities.Client.Tests.Entities.NorthwindModels;

namespace TrackableEntities.Client.Tests.Extensions
{
    [TestFixture]
    public class TrackableExtensionsTests
    {
        #region Setup

        // Mock database
        MockFamily _family;

        [SetUp]
        public void Init()
        {
            // Create new mock database for each test
            _family = new MockFamily();
        }

        #endregion

        #region Set Tracking Tests

        [Test]
        public void Parent_Set_Tracking_Should_Enable_Tracking_For_Children()
        {
            // Arrange
            var parent = _family.Parent;

            // Act
            parent.SetTracking(true);

            // Assert
            IEnumerable<bool> trackings = GetTrackings(parent);
            Assert.IsTrue(trackings.All(t => t));
        }

        [Test]
        public void Parent_Set_Tracking_Should_Disable_Tracking_For_Children()
        {
            // Arrange
            var parent = _family.Parent;
            parent.SetTracking(true);

            // Act
            parent.SetTracking(false);

            // Assert
            IEnumerable<bool> trackings = GetTrackings(parent);
            Assert.IsTrue(trackings.All(t => !t));
        }

        [Test]
        public void Collection_Set_Tracking_Should_Enable_Tracking_For_Children()
        {
            // Arrange
            var parent = _family.Parent;
            var changeTracker = new ChangeTrackingCollection<Parent>();
            changeTracker.Add(parent);

            // Act
            changeTracker.Tracking = true;

            // Assert
            Assert.IsTrue(changeTracker.Tracking);
            IEnumerable<bool> trackings = GetTrackings(parent);
            Assert.IsTrue(trackings.All(t => t));
        }

        [Test]
        public void Collection_Ctor_Should_Disable_Tracking_For_Children()
        {
            // Arrange
            var parent = _family.Parent;

            // Act
            var changeTracker = new ChangeTrackingCollection<Parent>(parent);

            // Assert
            Assert.IsTrue(changeTracker.Tracking);
            IEnumerable<bool> trackings = GetTrackings(parent);
            Assert.IsTrue(trackings.All(t => t));
        }

        #endregion

        #region Set State Tests

        [Test]
        public void Parent_Set_State_As_Added_Should_Mark_Children_As_Added()
        {
            // Arrange
            var parent = _family.Parent;

            // Act
            parent.SetState(TrackingState.Added, null, null);

            // Assert
            IEnumerable<TrackingState> trackings = GetStates(parent);
            Assert.IsTrue(trackings.All(t => t == TrackingState.Added));
        }

        [Test]
        public void Parent_Set_State_As_Deleted_Should_Mark_Children_As_Deleted()
        {
            // Arrange
            var parent = _family.Parent;

            // Act
            parent.SetState(TrackingState.Deleted, null, null);

            // Assert
            IEnumerable<TrackingState> trackings = GetStates(parent);
            Assert.IsTrue(trackings.All(t => t == TrackingState.Deleted));
        }

        [Test]
        public void Parent_Set_State_As_Deleted_Should_Mark_Added_Children_As_Unchanged()
        {
            // NOTE: Deleting an added child should not mark it as deleted, but mark
            // it as unchanged because it says it was not really added in the first place.

            // Arrange
            var parent = _family.Parent;
            parent.TrackingState = TrackingState.Added;
            parent.Children.ToList().ForEach(c1 =>
            {
                c1.TrackingState = TrackingState.Added;
                c1.Children.ToList().ForEach(c2 =>
                {
                    c2.TrackingState = TrackingState.Added;
                    c2.Children.ToList().ForEach(c3 =>
                    {
                        c3.TrackingState = TrackingState.Added;
                    });
                });
            });

            // Act
            parent.SetState(TrackingState.Deleted, null, null);

            // Assert
            IEnumerable<TrackingState> trackings = GetStates(parent);
            Assert.IsTrue(trackings.All(t => t == TrackingState.Unchanged));
        }

        [Test]
        public void Collection_Set_State_As_Deleted_Should_Mark_Children_As_Deleted()
        {
            // Arrange
            var parent = _family.Parent;
            var changeTracker = new ChangeTrackingCollection<Parent>(parent);

            // Act
            changeTracker.Remove(parent);

            // Assert
            Assert.AreEqual(TrackingState.Deleted, parent.TrackingState);
            IEnumerable<TrackingState> trackings = GetStates(parent);
            Assert.IsTrue(trackings.All(t => t == TrackingState.Deleted));
        }

        [Test]
        public void Collection_Added_Set_State_As_Deleted_Should_Mark_Children_As_Unchanged()
        {
            // Arrange
            var parent = _family.Parent;
            var changeTracker = new ChangeTrackingCollection<Parent>(true);
            changeTracker.Add(parent);

            // Act
            changeTracker.Remove(parent);

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, parent.TrackingState);
            IEnumerable<TrackingState> trackings = GetStates(parent);
            Assert.IsTrue(trackings.All(t => t == TrackingState.Unchanged));
        }

        [Test]
        public void Parent_Set_Modified_Props_Should_Set_Children()
        {
            // Arrange
            var parent = _family.Parent;

            // Act
            parent.SetModifiedProperties(new List<string> { "Children" });

            // Assert
            IEnumerable<ICollection<string>> modifieds = GetModifieds(parent);
            Assert.IsTrue(modifieds.All(t => t.Contains("Children")));
        }

        #endregion

        #region GetChanges Tests

        [Test]
        public void Collection_GetChanges_Should_Add_Only_Modified_Children()
        {
            // Arrange
            var parent = new Parent("Parent")
            {
                Children = new ChangeTrackingCollection<Child>
                    {
                        new Child("Child1"), 
                        new Child("Child2")
                    }
            };
            var changeTracker = new ChangeTrackingCollection<Parent>(parent);
            parent.Children[0].Name += "_Changed";

            // Act
            var changes = changeTracker.GetChanges();
            var changedParent = changes.First();

            // Assert
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(1, changedParent.Children.Count);
        }

        [Test]
        public void Collection_GetChanges_Should_Add_Only_Deleted_Children()
        {
            // Arrange
            var parent = new Parent("Parent")
            {
                Children = new ChangeTrackingCollection<Child>
                    {
                        new Child("Child1")
                    }
            };
            var changeTracker = new ChangeTrackingCollection<Parent>(parent);
            parent.Children.RemoveAt(0);

            // Act
            var changes = changeTracker.GetChanges();
            var changedParent = changes.First();

            // Assert
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(1, changedParent.Children.Count);
        }

        [Test]
        public void Collection_GetChanges_Should_Add_Only_Added_Modified_Deleted_Children()
        {
            // Arrange
            var parent = new Parent("Parent")
            {
                Children = new ChangeTrackingCollection<Child>
                    {
                        new Child("Child1"), 
                        new Child("Child2"),
                        new Child("Child3")
                    }
            };
            var changeTracker = new ChangeTrackingCollection<Parent>(parent);
            parent.Children.Add(new Child("Child4"));
            parent.Children[0].Name += "_Changed";
            parent.Children.RemoveAt(2);

            // Act
            var changes = changeTracker.GetChanges();
            var changedParent = changes.First();

            // Assert
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(3, changedParent.Children.Count);
        }

        #endregion

        #region Clone Tests

        [Test]
        public void Clone_Should_Deep_Copy_Object()
        {
            // Arrange
            var parentOrig = _family.Parent;

            // Act
            var parentCopy = parentOrig.Clone<Parent>();

            // Assert
            Assert.AreNotSame(parentOrig, parentCopy);
            Assert.AreNotSame(parentOrig.Children[0], parentCopy.Children[0]);
            Assert.AreNotSame(parentOrig.Children[1], parentCopy.Children[1]);
            Assert.AreNotSame(parentOrig.Children[2], parentCopy.Children[2]);
            Assert.AreNotSame(parentOrig.Children[0].Children[0], parentCopy.Children[0].Children[0]);
            Assert.AreNotSame(parentOrig.Children[1].Children[0], parentCopy.Children[1].Children[0]);
            Assert.AreNotSame(parentOrig.Children[2].Children[0], parentCopy.Children[2].Children[0]);
        }

        [Test]
        public void Collection_Clone_Should_Deep_Copy_Category()
        {
            // Arrange
            var database = new MockNorthwind();
            var categoryOrig = database.Categories[0];

            // Act
            var categoryCopy = categoryOrig.Clone<Category>();

            // Assert
            Assert.AreNotSame(categoryOrig, categoryCopy);
            Assert.AreNotSame(categoryOrig.Products[0], categoryCopy.Products[0]);
            Assert.AreNotSame(categoryOrig.Products[1], categoryCopy.Products[1]);
        }

        [Test]
        public void Collection_Clone_Should_Deep_Copy_Product()
        {
            // Arrange
            var database = new MockNorthwind();
            var productOrig = database.Products[0];

            // Act
            var productCopy = productOrig.Clone<Product>();

            // Assert
            Assert.AreNotSame(productOrig, productCopy);
            Assert.AreNotSame(productOrig.Category, productCopy.Category);
        }

        #endregion

        #region Restore and Remove Deletes Tests

        [Test]
        public void Collection_Restore_Deletes_Should_Add_Deleted_Items()
        {
            // Arrange
            var parent = new Parent("Parent")
            {
                Children = new ChangeTrackingCollection<Child>
                    {
                        new Child("Child1"), 
                        new Child("Child2"),
                        new Child("Child3")
                    }
            };
            parent.Children.Tracking = true;
            parent.Children.RemoveAt(2);

            // Act
            var trackableColl = (ITrackingCollection)parent.Children;
            trackableColl.RestoreDeletes();

            // Assert
            Assert.AreEqual(3, trackableColl.Count);
        }

        [Test]
        public void Collection_Remove_Deletes_Should_Remove_Added_Deleted_Items()
        {
            // Arrange
            var parent = new Parent("Parent")
            {
                Children = new ChangeTrackingCollection<Child>
                    {
                        new Child("Child1"), 
                        new Child("Child2"),
                        new Child("Child3")
                    }
            };
            parent.Children[0].TrackingState = TrackingState.Deleted;
            parent.Children.Tracking = true;
            parent.Children.RemoveAt(0);
            parent.Children.RestoreDeletes();

            // Act
            var trackableColl = (ITrackingCollection)parent.Children;
            trackableColl.RemoveRestoredDeletes();

            // Assert
            Assert.AreEqual(2, trackableColl.Count);
        }

        [Test]
        public void Item_Restore_Deletes_Should_Recursively_Add_Deleted_Items()
        {
            // Arrange
            var parent = new Parent("Parent")
            {
                Children = new ChangeTrackingCollection<Child>
                    {
                        new Child("Child1")
                        { 
                            Children = new ChangeTrackingCollection<Child>
                                {
                                    new Child("Grandchild1"),
                                    new Child("Grandchild2")
                                }
                        }
                    }
            };
            var count = parent.Children[0].Children.Count;
            parent.SetTracking(true);
            parent.Children[0].Children.RemoveAt(1);

            // Act
            parent.Children[0].Children.RestoreDeletes();

            // Assert
            Assert.AreEqual(count, parent.Children[0].Children.Count);
        }

        [Test]
        public void Item_Remove_Deletes_Should_Recursively_Remove_Added_Deleted_Items()
        {
            // Arrange
            var parent = new Parent("Parent")
            {
                Children = new ChangeTrackingCollection<Child>
                    {
                        new Child("Child1")
                        { 
                            Children = new ChangeTrackingCollection<Child>
                                {
                                    new Child("Grandchild1"),
                                    new Child("Grandchild2")
                                }
                        }
                    }
            };
            var deleted = parent.Children[0].Children[1];
            parent.Children[0].Children.Tracking = true;
            parent.Children[0].Children.RemoveAt(1);
            parent.Children[0].Children.RestoreDeletes();

            // Act
            var trackableColl = (ITrackingCollection)parent.Children[0].Children;
            trackableColl.RemoveRestoredDeletes();

            // Assert
            Assert.AreEqual(1, parent.Children[0].Children.Count);
            Assert.That(trackableColl, Has.No.Member(deleted));
        }

        [Test]
        public void Order_Remove_Deletes_Should_Remove_Deleted_OrderDetails()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var deleted1 = order.OrderDetails[0];
            var deleted2 = order.OrderDetails[2];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.OrderDetails.Remove(deleted1);
            order.OrderDetails.Remove(deleted2);
            changeTracker.RestoreDeletes();

            // Act
            changeTracker.RemoveRestoredDeletes();

            // Assert
            Assert.AreEqual(1, order.OrderDetails.Count);
            Assert.That(changeTracker, Has.No.Member(deleted1));
            Assert.That(changeTracker, Has.No.Member(deleted2));
        }

        #endregion

        #region IsEquatable Tests

        [Test]
        public void IsEquatable_Should_Return_False_If_SetEntityIdentifier_Not_Called()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var customer1 = northwind.Customers[0];
            var customer2 = northwind.Customers[1];

            // Act
            bool areEquatable = customer1.IsEquatable(customer2);

            // Assert
            Assert.IsFalse(areEquatable);
        }

        [Test]
        public void IsEquatable_Should_Return_True_If_SetEntityIdentifier_Called()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var customer1 = northwind.Customers[0];
            var customer2 = northwind.Customers[1];
            customer1.SetEntityIdentifier();
            Guid entityIdentifier = GetEntityIdentifier(customer1);
            customer2.SetEntityIdentifier(entityIdentifier);

            // Act
            bool areEquatable = customer1.IsEquatable(customer2);

            // Assert
            Assert.IsTrue(areEquatable);
        }

        [Test]
        public void IsEquatable_Should_Return_False_If_EntityIdentifier_Cleared()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var customer1 = northwind.Customers[0];
            var customer2 = northwind.Customers[1];
            customer1.SetEntityIdentifier();
            Guid entityIdentifier = GetEntityIdentifier(customer1);
            customer2.SetEntityIdentifier(entityIdentifier);

            customer1.SetEntityIdentifier(default(Guid)); // Cleared

            // Act
            bool areEquatable = customer1.IsEquatable(customer2);

            // Assert
            Assert.IsFalse(areEquatable);
        }

        [Test]
        public void IsEquatable_Should_Return_False_If_EntityIdentifier_Reset()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var customer1 = northwind.Customers[0];
            var customer2 = northwind.Customers[1];
            customer1.SetEntityIdentifier();
            Guid entityIdentifier = GetEntityIdentifier(customer1);
            customer2.SetEntityIdentifier(entityIdentifier);

            customer1.SetEntityIdentifier(default(Guid)); // Cleared
            customer1.SetEntityIdentifier(); // Reset

            // Act
            bool areEquatable = customer1.IsEquatable(customer2);

            // Assert
            Assert.IsTrue(areEquatable);
        }

        #endregion

        #region Helper Methods

        private IEnumerable<bool> GetTrackings(Parent parent)
        {
            var trackings = new List<bool>();
            trackings.Add(parent.Children.Tracking);
            trackings.AddRange(from c in parent.Children
                               let ch = c.Children
                               select ch.Tracking);
            trackings.AddRange(from c in parent.Children
                               from gc in c.Children
                               let ch = gc.Children
                               select ch.Tracking);
            return trackings;
        }

        private IEnumerable<TrackingState> GetStates(Parent parent)
        {
            var trackings = new List<TrackingState>();
            trackings.AddRange(from c in parent.Children
                               select c.TrackingState);
            trackings.AddRange(from c in parent.Children
                               from gc in c.Children
                               select gc.TrackingState);
            trackings.AddRange(from c in parent.Children
                               from gc in c.Children
                               from ggc in gc.Children
                               select ggc.TrackingState);
            return trackings;
        }

        private IEnumerable<ICollection<string>> GetModifieds(Parent parent)
        {
            var modifieds = new List<ICollection<string>>();
            modifieds.AddRange(from c in parent.Children
                               select c.ModifiedProperties);
            modifieds.AddRange(from c in parent.Children
                               from gc in c.Children
                               select gc.ModifiedProperties);
            modifieds.AddRange(from c in parent.Children
                               from gc in c.Children
                               from ggc in gc.Children
                               select ggc.ModifiedProperties);
            return modifieds;
        }

        private Guid GetEntityIdentifier(object item)
        {
            var property = typeof(Customer).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
                .SingleOrDefault(m => m.Name == Constants.EquatableMembers.EntityIdentifierProperty);
            var entityIdentifier = (Guid)property.GetValue(item);
            return entityIdentifier;
        }

        #endregion
    }
}
