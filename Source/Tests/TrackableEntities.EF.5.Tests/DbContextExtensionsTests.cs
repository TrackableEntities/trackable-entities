using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using NUnit.Framework;
#if EF_6
using TrackableEntities.EF6;
#else
using TrackableEntities.EF5;
#endif
using TrackableEntities.EF.Tests;
using TrackableEntities.EF.Tests.FamilyModels;
using TrackableEntities.EF.Tests.Mocks;
using TrackableEntities.EF.Tests.NorthwindModels;

#if EF_6
namespace TrackableEntities.EF6.Tests
#else
namespace TrackableEntities.EF5.Tests
#endif
{
    [TestFixture]
    public class DbContextExtensionsTests
    {
        const CreateDbOptions CreateFamilyDbOptions = CreateDbOptions.DropCreateDatabaseIfModelChanges;
        const CreateDbOptions CreateNorthwindDbOptions = CreateDbOptions.DropCreateDatabaseIfModelChanges;

        #region FamilyDb Tests

        [Test]
        public void Apply_Changes_Should_Mark_Added_Parent()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var parent = new Parent("Parent");
            parent.TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.AreEqual(EntityState.Added, context.Entry(parent).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Modified_Parent()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var parent = new Parent("Parent");
            parent.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.AreEqual(EntityState.Modified, context.Entry(parent).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Modified_Parent_Properties()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var parent = new Parent("Parent");
            parent.Name += "_Changed";
            parent.TrackingState = TrackingState.Modified;
            parent.ModifiedProperties = new List<string> {"Name"};

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.AreEqual(EntityState.Modified, context.Entry(parent).State);
            Assert.IsTrue(context.Entry(parent).Property("Name").IsModified);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Deleted_Parent()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var parent = new Parent("Parent");
            parent.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.AreEqual(EntityState.Deleted, context.Entry(parent).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Added_Parent_With_Children()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var parent = new Parent("Parent")
            {
                Children = new List<Child>
                    {
                        new Child("Child1"), 
                        new Child("Child2"),
                        new Child("Child3")
                    }
            };
            parent.TrackingState = TrackingState.Added;
            parent.Children[0].TrackingState = TrackingState.Added;
            parent.Children[1].TrackingState = TrackingState.Added;
            parent.Children[2].TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.AreEqual(EntityState.Added, context.Entry(parent).State);
            Assert.AreEqual(EntityState.Added, context.Entry(parent.Children[0]).State);
            Assert.AreEqual(EntityState.Added, context.Entry(parent.Children[1]).State);
            Assert.AreEqual(EntityState.Added, context.Entry(parent.Children[2]).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Family_Unchanged()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var parent = new MockFamily().Parent;

            // Act
            context.ApplyChanges(parent);

            // Assert
            IEnumerable<EntityState> states = context.GetEntityStates(parent, EntityState.Unchanged);
            Assert.AreEqual(40, states.Count());
        }

        [Test]
        public void Apply_Changes_Should_Mark_Family_Added()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var parent = new MockFamily().Parent;
            parent.TrackingState = TrackingState.Added;
            parent.SetTrackingState(TrackingState.Added);

            // Act
            context.ApplyChanges(parent);

            // Assert
            IEnumerable<EntityState> states = context.GetEntityStates(parent, EntityState.Added);
            Assert.AreEqual(40, states.Count());
        }

        [Test]
        public void Apply_Changes_Should_Mark_Family_Modified()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var parent = new MockFamily().Parent;
            parent.TrackingState = TrackingState.Modified;
            parent.SetTrackingState(TrackingState.Modified);

            // Act
            context.ApplyChanges(parent);

            // Assert
            IEnumerable<EntityState> states = context.GetEntityStates(parent, EntityState.Modified);
            Assert.AreEqual(40, states.Count());
        }

        [Test]
        public void Apply_Changes_Should_Mark_Parent_With_Added_Modified_Deleted_Children()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var child1 = new Child("Child1");
            var child2 = new Child("Child2");
            var child3 = new Child("Child3");
            var parent = new Parent("Parent")
            {
                Children = new List<Child> { child1, child2, child3 }
            };
            child1.TrackingState = TrackingState.Added;
            child2.TrackingState = TrackingState.Modified;
            child3.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.AreEqual(EntityState.Unchanged, context.Entry(parent).State);
            Assert.AreEqual(EntityState.Added, context.Entry(child1).State);
            Assert.AreEqual(EntityState.Modified, context.Entry(child2).State);
            Assert.AreEqual(EntityState.Deleted, context.Entry(child3).State);
        }

        #endregion

        #region NorthwindDbTests

        [Test]
        public void Apply_Changes_Should_Mark_Product_Unchanged()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var parent = new Product();
            parent.TrackingState = TrackingState.Unchanged;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.AreEqual(EntityState.Unchanged, context.Entry(parent).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Product_Added()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var parent = new Product();
            parent.TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.AreEqual(EntityState.Added, context.Entry(parent).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Product_Modified()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var parent = new Product();
            parent.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.AreEqual(EntityState.Modified, context.Entry(parent).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Product_Deleted()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var parent = new Product();
            parent.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.AreEqual(EntityState.Deleted, context.Entry(parent).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Order_Unchanged()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            var detail2 = order.OrderDetails[1];
            var detail3 = order.OrderDetails[2];
            var detail4 = order.OrderDetails[3];

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.AreEqual(EntityState.Unchanged, context.Entry(order).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(detail1).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(detail2).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(detail3).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(detail4).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Order_Added()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            var detail2 = order.OrderDetails[1];
            var detail3 = order.OrderDetails[2];
            var detail4 = order.OrderDetails[3];
            order.TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.AreEqual(EntityState.Added, context.Entry(order).State);
            Assert.AreEqual(EntityState.Added, context.Entry(detail1).State);
            Assert.AreEqual(EntityState.Added, context.Entry(detail2).State);
            Assert.AreEqual(EntityState.Added, context.Entry(detail3).State);
            Assert.AreEqual(EntityState.Added, context.Entry(detail4).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Order_Deleted()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            order.CustomerId = null;
            order.Customer = null;
            var detail1 = order.OrderDetails[0];
            var detail2 = order.OrderDetails[1];
            var detail3 = order.OrderDetails[2];
            var detail4 = order.OrderDetails[3];
            order.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.AreEqual(EntityState.Deleted, context.Entry(order).State);
            Assert.AreEqual(EntityState.Deleted, context.Entry(detail1).State);
            Assert.AreEqual(EntityState.Deleted, context.Entry(detail2).State);
            Assert.AreEqual(EntityState.Deleted, context.Entry(detail3).State);
            Assert.AreEqual(EntityState.Deleted, context.Entry(detail4).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Order_Only_Modified()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            var detail2 = order.OrderDetails[1];
            var detail3 = order.OrderDetails[2];
            var detail4 = order.OrderDetails[3];
            order.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.AreEqual(EntityState.Modified, context.Entry(order).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(detail1).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(detail2).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(detail3).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(detail4).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Order_Details_Only_Modified()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            var detail2 = order.OrderDetails[1];
            var detail3 = order.OrderDetails[2];
            var detail4 = order.OrderDetails[3];
            detail1.TrackingState = TrackingState.Modified;
            detail2.TrackingState = TrackingState.Modified;
            detail3.TrackingState = TrackingState.Modified;
            detail4.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.AreEqual(EntityState.Unchanged, context.Entry(order).State);
            Assert.AreEqual(EntityState.Modified, context.Entry(detail1).State);
            Assert.AreEqual(EntityState.Modified, context.Entry(detail2).State);
            Assert.AreEqual(EntityState.Modified, context.Entry(detail3).State);
            Assert.AreEqual(EntityState.Modified, context.Entry(detail4).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Order_With_Details_Modified()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            var detail2 = order.OrderDetails[1];
            var detail3 = order.OrderDetails[2];
            var detail4 = order.OrderDetails[3];
            order.TrackingState = TrackingState.Modified;
            detail1.TrackingState = TrackingState.Modified;
            detail2.TrackingState = TrackingState.Modified;
            detail3.TrackingState = TrackingState.Modified;
            detail4.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.AreEqual(EntityState.Modified, context.Entry(order).State);
            Assert.AreEqual(EntityState.Modified, context.Entry(detail1).State);
            Assert.AreEqual(EntityState.Modified, context.Entry(detail2).State);
            Assert.AreEqual(EntityState.Modified, context.Entry(detail3).State);
            Assert.AreEqual(EntityState.Modified, context.Entry(detail4).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_OrderDetails_Added_Modified_Deleted_Unchanged()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            var detail2 = order.OrderDetails[1];
            var detail3 = order.OrderDetails[2];
            var detail4 = order.OrderDetails[3];
            detail1.TrackingState = TrackingState.Added;
            detail2.TrackingState = TrackingState.Modified;
            detail3.TrackingState = TrackingState.Deleted;
            detail4.TrackingState = TrackingState.Unchanged;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.AreEqual(EntityState.Unchanged, context.Entry(order).State);
            Assert.AreEqual(EntityState.Added, context.Entry(detail1).State);
            Assert.AreEqual(EntityState.Modified, context.Entry(detail2).State);
            Assert.AreEqual(EntityState.Deleted, context.Entry(detail3).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(detail4).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Unchanged_Employee_As_Unchanged_And_Unchanged_Territories_As_Unchanged()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            var territory1 = employee.Territories[0];
            var territory2 = employee.Territories[1];
            var territory3 = employee.Territories[2];

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.AreEqual(EntityState.Unchanged, context.Entry(employee).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Unchanged_Employee_As_Unchanged_And_Modified_Territories_As_Modified()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            var territory1 = employee.Territories[0];
            var territory2 = employee.Territories[1];
            var territory3 = employee.Territories[2];
            territory3.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.AreEqual(EntityState.Unchanged, context.Entry(employee).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.AreEqual(EntityState.Modified, context.Entry(territory3).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Unchanged_Employee_As_Unchanged_And_Added_Territories_As_Unchanged()
        {
            // NOTE: With M-M properties there is no way to tell if the related entity is new or should 
            // or simply be added to the relationship, because it is an independent association.
            // Therefore, added children are added to the relationship and marked unchanged.

            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            var territory1 = employee.Territories[0];
            var territory2 = employee.Territories[1];
            var territory3 = employee.Territories[2];
            var territory4 = nw.Territories[3];
            territory4.TrackingState = TrackingState.Added;
            employee.Territories.Add(territory4);

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.AreEqual(EntityState.Unchanged, context.Entry(employee).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory4).State);
            Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory4));
        }

        [Test]
        public void Apply_Changes_Should_Mark_Unchanged_Employee_As_Unchanged_And_Deleted_Territories_As_Unchanged()
        {
            // NOTE: With M-M properties there is no way to tell if the related entity should be deleted
            // or simply removed from the relationship, because it is an independent association.
            // Therefore, deleted children are removed from the relationship and marked unchanged.

            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            var territory1 = employee.Territories[0];
            var territory2 = employee.Territories[1];
            var territory3 = employee.Territories[2];
            territory3.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.AreEqual(EntityState.Unchanged, context.Entry(employee).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
            Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory3));
            Assert.AreEqual(2, employee.Territories.Count);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Added_Employee_As_Added_And_Unchanged_Territories_As_Unchanged()
        {
            // NOTE: Because parent is added, unchanged children will be added to M-M relation,
            // even though the entities themselves are unchanged.

            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            employee.TrackingState = TrackingState.Added;
            var territory1 = employee.Territories[0];
            var territory2 = employee.Territories[1];
            var territory3 = employee.Territories[2];

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.AreEqual(EntityState.Added, context.Entry(employee).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
            Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory1));
            Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory2));
            Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory3));
        }

        [Test]
        public void Apply_Changes_Should_Mark_Added_Employee_As_Added_And_Modified_Territories_As_Modified()
        {
            // NOTE: Modified children of an added parent will remain modified,
            // but they will be added to the M-M relation.

            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            employee.TrackingState = TrackingState.Added;
            var territory1 = employee.Territories[0];
            var territory2 = employee.Territories[1];
            var territory3 = employee.Territories[2];
            territory3.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.AreEqual(EntityState.Added, context.Entry(employee).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.AreEqual(EntityState.Modified, context.Entry(territory3).State);
            Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory1));
            Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory2));
            Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory3));
        }

        [Test]
        public void Apply_Changes_Should_Mark_Added_Employee_As_Added_And_Added_Territories_As_Unchanged()
        {
            // NOTE: Because parent is added, added children will be marked as unchanged
            // but added to the M-M relation

            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            employee.TrackingState = TrackingState.Added;
            var territory1 = employee.Territories[0];
            var territory2 = employee.Territories[1];
            var territory3 = employee.Territories[2];
            var territory4 = nw.Territories[3];
            territory4.TrackingState = TrackingState.Added;
            employee.Territories.Add(territory4);

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.AreEqual(EntityState.Added, context.Entry(employee).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory4).State);
            Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory4));
        }

        [Test]
        public void Apply_Changes_Should_Mark_Added_Employee_As_Added_And_Deleted_Territories_As_Deleted()
        {
            // NOTE: If a deleted child is assocated with an added parent, 
            // we will just ignore the delete and add the item, since this is unsupported.

            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            employee.TrackingState = TrackingState.Added;
            var territory1 = employee.Territories[0];
            var territory2 = employee.Territories[1];
            var territory3 = employee.Territories[2];
            territory3.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.AreEqual(EntityState.Added, context.Entry(employee).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
            Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory3));
        }

        [Test]
        public void Apply_Changes_Should_Mark_Deleted_Employee_As_Deleted_And_Unchanged_Territories_As_Unchanged()
        {
            // NOTE: Because parent is deleted, unchanged children will be deleted from M-M relation,
            // even though the entities themselves are unchanged.

            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            employee.TrackingState = TrackingState.Deleted;
            var territory1 = employee.Territories[0];
            var territory2 = employee.Territories[1];
            var territory3 = employee.Territories[2];

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.AreEqual(EntityState.Deleted, context.Entry(employee).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
            Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory1));
            Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory2));
            Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory3));
            Assert.AreEqual(0, employee.Territories.Count);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Deleted_Employee_As_Deleted_And_Modified_Territories_As_Modified()
        {
            // NOTE: Modified children of a deleted parent will remain modified,
            // but they will be removed from the M-M relation.

            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            employee.TrackingState = TrackingState.Deleted;
            var territory1 = employee.Territories[0];
            var territory2 = employee.Territories[1];
            var territory3 = employee.Territories[2];
            territory3.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.AreEqual(EntityState.Deleted, context.Entry(employee).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.AreEqual(EntityState.Modified, context.Entry(territory3).State);
            Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory1));
            Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory2));
            Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory3));
            Assert.AreEqual(0, employee.Territories.Count);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Deleted_Employee_As_Deleted_And_Added_Territories_As_Unchanged()
        {
            // NOTE: Because parent is deleted, added children will be marked as unchanged
            // but removed from the M-M relation

            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            employee.TrackingState = TrackingState.Deleted;
            var territory1 = employee.Territories[0];
            var territory2 = employee.Territories[1];
            var territory3 = employee.Territories[2];
            var territory4 = nw.Territories[3];
            territory4.TrackingState = TrackingState.Added;
            employee.Territories.Add(territory4);

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.AreEqual(EntityState.Deleted, context.Entry(employee).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory4).State);
            Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory1));
            Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory2));
            Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory3));
            Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory4));
            Assert.AreEqual(0, employee.Territories.Count);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Deleted_Employee_As_Deleted_And_Deleted_Territories_As_Unchanged()
        {
            // NOTE: If a deleted child is assocated with a deleted parent, 
            // it should be set to unchanged and removed from the M-M relation.

            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            employee.TrackingState = TrackingState.Deleted;
            var territory1 = employee.Territories[0];
            var territory2 = employee.Territories[1];
            var territory3 = employee.Territories[2];
            territory3.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.AreEqual(EntityState.Deleted, context.Entry(employee).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
            Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory1));
            Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory2));
            Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory3));
            Assert.AreEqual(0, employee.Territories.Count);
        }

        #endregion
    }
}
