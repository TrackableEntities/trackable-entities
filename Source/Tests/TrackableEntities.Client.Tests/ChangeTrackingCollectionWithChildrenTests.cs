using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TrackableEntities.Client.Tests.Entities.Mocks;
using TrackableEntities.Client.Tests.Entities.NorthwindModels;

namespace TrackableEntities.Client.Tests
{
    [TestFixture]
    public class ChangeTrackingCollectionWithChildrenTests
    {
        #region Setup

        // Mock database
        MockNorthwind _database;

        [SetUp]
        public void Init()
        {
            // Create new mock database for each test
            _database = new MockNorthwind();
        }

        #endregion

        #region Set Status Tests

        [Test]
        public void Existing_Parent_With_Children_Should_Have_Children_Marked()
        {
            // Arrange
            var order = _database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var orderDetails = (IList<OrderDetail>)changeTracker[0].OrderDetails;
            var addedDetail = new OrderDetail
                {
                    ProductId = 1,
                    Product = _database.Products[0],
                    Quantity = 10,
                    UnitPrice = 20M
                };
            var modifiedDetail = orderDetails[0];
            var deletedDetail = orderDetails[1];

            // Act
            orderDetails.Add(addedDetail);
            modifiedDetail.UnitPrice++;
            orderDetails.Remove(deletedDetail);

            // Assert
            Assert.AreEqual(TrackingState.Added, addedDetail.TrackingState);
            Assert.AreEqual(TrackingState.Modified, modifiedDetail.TrackingState);
            Assert.AreEqual(TrackingState.Deleted, deletedDetail.TrackingState);
        }

        [Test]
        public void Added_Parent_With_Children_Should_Have_Children_Marked_As_Added()
        {
            // Arrange
            var changeTracker = new ChangeTrackingCollection<Order>(true);
            var order = _database.Orders[0];
            var orderDetails = (IList<OrderDetail>)order.OrderDetails;

            // Act
            changeTracker.Add(order);

            // Assert
            Assert.AreEqual(TrackingState.Added, order.TrackingState);
            Assert.AreEqual(TrackingState.Added, orderDetails[0].TrackingState);
        }

        [Test]
        public void Added_Parent_With_Modified_Children_Should_Have_Children_Marked_As_Added()
        {
            // Arrange
            var changeTracker = new ChangeTrackingCollection<Order>(true);
            var order = _database.Orders[0];
            var orderDetails = (IList<OrderDetail>)order.OrderDetails;

            // Act
            changeTracker.Add(order);
            orderDetails[0].Quantity++;

            // Assert
            Assert.AreEqual(TrackingState.Added, order.TrackingState);
            Assert.AreEqual(TrackingState.Added, orderDetails[0].TrackingState);
        }

        [Test]
        public void Added_Parent_Then_Removed_Should_Have_Children_Marked_As_Unchanged()
        {
            // Arrange
            var changeTracker = new ChangeTrackingCollection<Order>(true);
            var order = _database.Orders[0];
            var orderDetails = (IList<OrderDetail>)order.OrderDetails;
            var orderDetail = orderDetails[0];

            // Act
            changeTracker.Add(order);
            changeTracker.Remove(order);

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, order.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, orderDetail.TrackingState);
        }

        [Test]
        public void Added_Parent_With_Removed_Children_Should_Have_Children_Marked_As_Unchanged()
        {
            // Arrange
            var changeTracker = new ChangeTrackingCollection<Order>(true);
            var order = _database.Orders[0];
            var orderDetails = (IList<OrderDetail>)order.OrderDetails;
            var orderDetail = orderDetails[0];

            // Act
            changeTracker.Add(order);
            order.OrderDetails.Remove(orderDetail);

            // Assert
            Assert.AreEqual(TrackingState.Added, order.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, orderDetail.TrackingState);
        }

        [Test]
        public void Existing_Parent_Removed_With_Modified_Children_Should_Have_Children_Marked_As_Deleted()
        {
            // Arrange
            var order = _database.Orders[0];
            var orderDetails = (IList<OrderDetail>)order.OrderDetails;
            var orderDetail = orderDetails[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            orderDetail.Quantity++;

            // Act
            changeTracker.Remove(order);

            // Assert
            Assert.AreEqual(TrackingState.Deleted, order.TrackingState);
            Assert.AreEqual(TrackingState.Deleted, orderDetail.TrackingState);
        }

        #endregion

        #region Modified Properties Tests

        [Test]
        public void Existing_Parent_With_Modified_Children_Should_Add_ModifiedProperties()
        {
            // Arrange
            var order = _database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var orderDetails = (IList<OrderDetail>)changeTracker[0].OrderDetails;
            var modifiedDetail = orderDetails[0];

            // Act
            modifiedDetail.UnitPrice++;

            // Assert
            Assert.Contains("UnitPrice", (ICollection)modifiedDetail.ModifiedProperties);
        }

        [Test]
        public void Existing_Parent_With_Modified_Children_Should_Add_Multiple_ModifiedProperties()
        {
            // Arrange
            var order = _database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var orderDetails = (IList<OrderDetail>)changeTracker[0].OrderDetails;
            var modifiedDetail = orderDetails[0];

            // Act
            modifiedDetail.Quantity++;
            modifiedDetail.UnitPrice++;

            // Assert
            Assert.Contains("Quantity", (ICollection)modifiedDetail.ModifiedProperties);
            Assert.Contains("UnitPrice", (ICollection)modifiedDetail.ModifiedProperties);
        }

        [Test]
        public void Existing_Parent_With_Excluded_Children_Should_Not_Be_Marked_As_Modified()
        {
            // Arrange
            var order = _database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var orderDetails = changeTracker[0].OrderDetails;
            orderDetails.ExcludedProperties.Add("Quantity");
            var modifiedDetail = orderDetails[0];

            // Act
            modifiedDetail.Quantity++;

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, modifiedDetail.TrackingState);
        }

        [Test]
        public void Existing_Parent_With_Excluded_Children_Should_Not_Add_ModifiedProperty()
        {
            // Arrange
            var order = _database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var orderDetails = changeTracker[0].OrderDetails;
            orderDetails.ExcludedProperties.Add("Quantity");
            var modifiedDetail = orderDetails[0];

            // Act
            modifiedDetail.Quantity++;

            // Assert
            Assert.IsTrue(modifiedDetail.ModifiedProperties == null
                || modifiedDetail.ModifiedProperties.Count == 0);
        }

        [Test]
        public void Existing_Parent_With_Mixed_Children_Should_Not_Add_ModifiedProperty()
        {
            // Arrange
            var order = _database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var orderDetails = changeTracker[0].OrderDetails;
            orderDetails.ExcludedProperties.Add("Quantity");
            var modifiedDetail = orderDetails[0];

            // Act
            modifiedDetail.Quantity++;
            modifiedDetail.UnitPrice++;

            // Assert
            Assert.AreEqual(TrackingState.Modified, modifiedDetail.TrackingState);
            Assert.Contains("UnitPrice", (ICollection)modifiedDetail.ModifiedProperties);
            Assert.That(modifiedDetail.ModifiedProperties, Has.No.Member("Quantity"));
        }

        [Test]
        public void Existing_Parent_Removed_With_Modified_Children_Should_Have_Children_Modified_Properties_Cleared()
        {
            // Arrange
            var order = _database.Orders[0];
            var orderDetails = (IList<OrderDetail>)order.OrderDetails;
            var orderDetail = orderDetails[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            orderDetail.Quantity++;

            // Act
            changeTracker.Remove(order);

            // Assert
            Assert.IsTrue(orderDetail.ModifiedProperties == null
                || orderDetail.ModifiedProperties.Count == 0);
        }
        
        #endregion

        #region GetChanges Tests

        [Test]
        public void GetChanges_On_Existing_Parent_With_Children_Should_Return_Marked_Children()
        {
            // Arrange
            var order = _database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var orderDetails = (IList<OrderDetail>)changeTracker[0].OrderDetails;
            var unchangedDetail = orderDetails[0];
            var modifiedDetail = orderDetails[1];
            var deletedDetail = orderDetails[2];
            var addedDetail = new OrderDetail
            {
                ProductId = 1,
                Product = _database.Products[0],
                Quantity = 10,
                UnitPrice = 20M
            };
            orderDetails.Add(addedDetail);
            modifiedDetail.UnitPrice++;
            orderDetails.Remove(deletedDetail);

            // Act
            var changes = changeTracker.GetChanges();
            var changedOrder = changes.First();
            var changedModifiedDetail = changedOrder.OrderDetails.Single(d => d.ProductId == modifiedDetail.ProductId);
            var changedAddedDetail = changedOrder.OrderDetails.Single(d => d.ProductId == addedDetail.ProductId);
            var changedDeletedDetail = changedOrder.OrderDetails.Single(d => d.ProductId == deletedDetail.ProductId);

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, changedOrder.TrackingState);
            Assert.AreEqual(3, changedOrder.OrderDetails.Count);
            Assert.AreEqual(TrackingState.Modified, changedModifiedDetail.TrackingState);
            Assert.AreEqual(TrackingState.Added, changedAddedDetail.TrackingState);
            Assert.AreEqual(TrackingState.Deleted, changedDeletedDetail.TrackingState);
            Assert.That(changedOrder.OrderDetails, Has.No.Member(unchangedDetail));
        }

        #endregion

        #region MergeChangesTests

        [Test]
        public void Updated_Parent_With_Children_Should_Merge_Unchanged_Children()
        {
            // Arrange

            // Get order, fix up details, clone
            var origOrder = _database.Orders[0];
            foreach (var detail in origOrder.OrderDetails)
                detail.Order = origOrder;
            var updatedOrder = origOrder.Clone<Order>();

            // Add unchanged detail to orig order
            origOrder.OrderDetails.Add(new OrderDetail
            {
                ProductId = 1,
                Product = _database.Products[0],
                Quantity = 10,
                UnitPrice = 20M,
                OrderId = origOrder.OrderId,
                Order = origOrder
            });

            // Replicate changes in updated order
            updatedOrder.OrderDetails[0].UnitPrice++;
            updatedOrder.OrderDetails.RemoveAt(1);

            // Remove 3rd detail so it can be re-added
            var addedDetail = origOrder.OrderDetails[2];
            origOrder.OrderDetails.Remove(addedDetail);

            // Clone orig order and start tracking
            var changeTracker = new ChangeTrackingCollection<Order>(origOrder);

            // Set state on orig order details
            origOrder.OrderDetails[0].UnitPrice++;
            origOrder.OrderDetails.RemoveAt(1);
            origOrder.OrderDetails.Add(addedDetail);

            // Act

            // Merge updates into orig order
            changeTracker.MergeChanges(ref origOrder, updatedOrder);

            // Assert
            // Orig reference pointed to updated
            Assert.IsTrue(ReferenceEquals(origOrder, updatedOrder));

            // Tracking states set to unchanged
            Assert.AreEqual(TrackingState.Unchanged, origOrder.OrderDetails[0].TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, origOrder.OrderDetails[1].TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, origOrder.OrderDetails[2].TrackingState);

            // Modified properties set to null
            Assert.IsNull(origOrder.OrderDetails[0].ModifiedProperties);
            Assert.IsNull(origOrder.OrderDetails[1].ModifiedProperties);
            Assert.IsNull(origOrder.OrderDetails[2].ModifiedProperties);

            // Detail orders fixed up
            Assert.IsTrue(ReferenceEquals(origOrder, origOrder.OrderDetails[0].Order));
            Assert.IsTrue(ReferenceEquals(origOrder, origOrder.OrderDetails[1].Order));
            Assert.IsTrue(ReferenceEquals(origOrder, origOrder.OrderDetails[2].Order));
        }

        #endregion

        #region ManyToMany - Set Status Tests

        [Test]
        public void Existing_Employee_With_Territories_Should_Have_Territories_Marked()
        {
            // Arrange
            var employee = _database.Employees[0];
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            var unchangedTerritory = employee.Territories[0];
            var modifiedTerritory = employee.Territories[1];
            var deletedTerritory = employee.Territories[2];
            var addedTerritory = _database.Territories[3];

            // Act
            modifiedTerritory.TerritoryDescription = "xxx";
            employee.Territories.Add(addedTerritory);
            employee.Territories.Remove(deletedTerritory);

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, employee.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, unchangedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Modified, modifiedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Added, addedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Deleted, deletedTerritory.TrackingState);
        }

        [Test]
        public void Added_Employee_With_Territories_Should_Have_Territories_Marked_As_Added()
        {
            // NOTE: Removing child from added parent marks child as unchanged.
            // This is so that GetChanges will not include the removed child.

            // Arrange
            var changeTracker = new ChangeTrackingCollection<Employee>(true);
            var employee = _database.Employees[0];
            var unchangedTerritory = employee.Territories[0];
            var modifiedTerritory = employee.Territories[1];
            var deletedTerritory = employee.Territories[2];
            var addedTerritory = _database.Territories[3];
            changeTracker.Add(employee);

            // Act
            modifiedTerritory.TerritoryDescription = "xxx";
            employee.Territories.Add(addedTerritory);
            employee.Territories.Remove(deletedTerritory);

            // Assert
            Assert.AreEqual(TrackingState.Added, employee.TrackingState);
            Assert.AreEqual(TrackingState.Added, unchangedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Added, modifiedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Added, addedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, deletedTerritory.TrackingState);
        }

        [Test]
        public void Added_Employee_Then_Removed_With_Territories_Should_Have_Territories_Marked_As_Unchanged()
        {
            // NOTE: Removing an added parent should mark parent and children as unchanged.
            // This is so that GetChanges will not include parent or children.

            // Arrange
            var changeTracker = new ChangeTrackingCollection<Employee>(true);
            var employee = _database.Employees[0];
            var unchangedTerritory = employee.Territories[0];
            var modifiedTerritory = employee.Territories[1];
            var deletedTerritory = employee.Territories[2];
            var addedTerritory = _database.Territories[3];
            changeTracker.Add(employee);

            // Act
            modifiedTerritory.TerritoryDescription = "xxx";
            employee.Territories.Add(addedTerritory);
            employee.Territories.Remove(deletedTerritory);
            changeTracker.Remove(employee);

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, employee.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, unchangedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, modifiedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, addedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, deletedTerritory.TrackingState);
        }

        [Test]
        public void Removed_Employee_With_Territories_Should_Have_Territories_Marked_As_Deleted()
        {
            // NOTE: Removing a parent will mark both parent and children as deleted.
            // Deleted M-M childen are simply removed from the relation with parent.

            // Arrange
            var employee = _database.Employees[0];
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            var unchangedTerritory = employee.Territories[0];
            var modifiedTerritory = employee.Territories[1];
            var deletedTerritory = employee.Territories[2];
            var addedTerritory = _database.Territories[3];

            // Act
            modifiedTerritory.TerritoryDescription = "xxx";
            employee.Territories.Add(addedTerritory);
            employee.Territories.Remove(deletedTerritory);
            changeTracker.Remove(employee);

            // Assert
            Assert.AreEqual(TrackingState.Deleted, employee.TrackingState);
            Assert.AreEqual(TrackingState.Deleted, unchangedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Deleted, modifiedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Deleted, addedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Deleted, deletedTerritory.TrackingState);
        }

        #endregion

        #region ManyToMany - Modified Properties Tests

        [Test]
        public void Existing_Employee_With_Modified_Children_Should_Add_ModifiedProperties()
        {
            // Arrange
            var employee = _database.Employees[0];
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            var modifiedTerritory = employee.Territories[1];

            // Act
            modifiedTerritory.TerritoryDescription = "xxx";

            // Assert
            Assert.Contains("TerritoryDescription", (ICollection)modifiedTerritory.ModifiedProperties);
        }

        [Test]
        public void Existing_Employee_With_Modified_Children_Should_Add_Multiple_ModifiedProperties()
        {
            // Arrange
            var employee = _database.Employees[0];
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            var modifiedTerritory = employee.Territories[1];

            // Act
            modifiedTerritory.TerritoryDescription = "xxx";
            modifiedTerritory.Data = "xxx";

            // Assert
            Assert.Contains("TerritoryDescription", (ICollection)modifiedTerritory.ModifiedProperties);
            Assert.Contains("Data", (ICollection)modifiedTerritory.ModifiedProperties);
        }

        [Test]
        public void Existing_Employee_With_Excluded_Children_Should_Not_Be_Marked_As_Modified()
        {
            // Arrange
            var employee = _database.Employees[0];
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            employee.Territories.ExcludedProperties.Add("TerritoryDescription");
            var modifiedTerritory = employee.Territories[1];

            // Act
            modifiedTerritory.TerritoryDescription = "xxx";

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, modifiedTerritory.TrackingState);
            Assert.IsTrue(modifiedTerritory.ModifiedProperties == null
                || modifiedTerritory.ModifiedProperties.Count == 0);
        }

        [Test]
        public void Existing_Employee_With_Mixed_Children_Should_Not_Add_ModifiedProperties()
        {
            // Arrange
            var employee = _database.Employees[0];
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            employee.Territories.ExcludedProperties.Add("TerritoryDescription");
            var modifiedTerritory = employee.Territories[1];

            // Act
            modifiedTerritory.TerritoryDescription = "xxx";
            modifiedTerritory.Data = "xxx";

            // Assert
            Assert.AreEqual(TrackingState.Modified, modifiedTerritory.TrackingState);
            Assert.Contains("Data", (ICollection)modifiedTerritory.ModifiedProperties);
            Assert.That(modifiedTerritory.ModifiedProperties, Has.No.Member("TerritoryDescription"));
        }

        [Test]
        public void Existing_Employee_Removed_With_Modified_Children_Have_Children_ModifiedProperties_Cleared()
        {
            // Arrange
            var employee = _database.Employees[0];
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            var modifiedTerritory = employee.Territories[1];
            modifiedTerritory.TerritoryDescription = "xxx";

            // Act
            changeTracker.Remove(employee);

            // Assert
            Assert.IsTrue(modifiedTerritory.ModifiedProperties == null
                || modifiedTerritory.ModifiedProperties.Count == 0);
        }

        #endregion

        #region ManyToMany - GetChanges Tests

        [Test]
        public void GetChanges_On_Existing_Employee_With_Territories_Should_Return_Marked_Territories()
        {
            // Arrange
            var employee = _database.Employees[0];
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            var unchangedTerritory = employee.Territories[0];
            var modifiedTerritory = employee.Territories[1];
            var deletedTerritory = employee.Territories[2];
            var addedTerritory = _database.Territories[3];
            employee.Territories.Add(addedTerritory);
            modifiedTerritory.TerritoryDescription = "xxx";
            employee.Territories.Remove(deletedTerritory);

            // Act
            var changes = changeTracker.GetChanges();
            var changedEmployee = changes.First();
            var changedModifiedTerritory = changedEmployee.Territories.Single(t => t.TerritoryId == modifiedTerritory.TerritoryId);
            var changedAddedTerritory = changedEmployee.Territories.Single(t => t.TerritoryId == addedTerritory.TerritoryId);
            var changedDeletedTerritory = changedEmployee.Territories.Single(t => t.TerritoryId == deletedTerritory.TerritoryId);

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, changedEmployee.TrackingState);
            Assert.AreEqual(3, changedEmployee.Territories.Count);
            Assert.AreEqual(TrackingState.Modified, changedModifiedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Added, changedAddedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Deleted, changedDeletedTerritory.TrackingState);
            Assert.That(changedEmployee.Territories, Has.No.Member(unchangedTerritory));
        }

        #endregion
    }
}
