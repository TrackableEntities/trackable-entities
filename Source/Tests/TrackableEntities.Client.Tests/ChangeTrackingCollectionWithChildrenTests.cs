﻿using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;
using TrackableEntities.Client.Tests.Entities.Mocks;
using TrackableEntities.Client.Tests.Entities.NorthwindModels;

namespace TrackableEntities.Client.Tests
{
    [TestFixture]
    public class ChangeTrackingCollectionWithChildrenTests
    {
        #region Set Status Tests

        [Test]
        public void Existing_Parent_With_Children_Should_Have_Children_Marked()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var orderDetails = (IList<OrderDetail>)changeTracker[0].OrderDetails;
            var addedDetail = new OrderDetail
                {
                    ProductId = 1,
                    Product = database.Products[0],
                    Quantity = 10,
                    UnitPrice = 20M
                };
            var modifiedDetail = orderDetails[0];
            var deletedDetail = orderDetails[1];
            addedDetail.Order = order;
            modifiedDetail.Order = order;
            deletedDetail.Order = order;

            // Act
            modifiedDetail.UnitPrice++;

            // BUG: Tracking property is set to false on orderDetails
            // As a result, adding detail does not mark it as added
            orderDetails.Remove(deletedDetail);
            orderDetails.Add(addedDetail);

            // Assert
            Assert.AreEqual(TrackingState.Added, addedDetail.TrackingState);
            Assert.AreEqual(TrackingState.Modified, modifiedDetail.TrackingState);
            Assert.AreEqual(TrackingState.Deleted, deletedDetail.TrackingState);
        }

        [Test]
        public void Existing_Parent_With_Added_Children_Should_Have_Children_Marked_As_Added()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var orderDetails = (IList<OrderDetail>)changeTracker[0].OrderDetails;
            var addedDetail1 = new OrderDetail
            {
                ProductId = 1,
                Product = database.Products[0],
                Quantity = 10,
                UnitPrice = 20M
            };
            var addedDetail2 = new OrderDetail
            {
                ProductId = 2,
                Product = database.Products[1],
                Quantity = 20,
                UnitPrice = 30M
            };

            // Act
            orderDetails.Add(addedDetail1);
            orderDetails.Add(addedDetail2);

            // Assert
            Assert.AreEqual(TrackingState.Added, addedDetail1.TrackingState);
            Assert.AreEqual(TrackingState.Added, addedDetail2.TrackingState);
        }

        [Test]
        public void Added_Parent_With_Children_Should_Have_Children_Marked_As_Added()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Order>(true);
            var order = database.Orders[0];
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
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Order>(true);
            var order = database.Orders[0];
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
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Order>(true);
            var order = database.Orders[0];
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
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Order>(true);
            var order = database.Orders[0];
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
            var database = new MockNorthwind();
            var order = database.Orders[0];
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
            var database = new MockNorthwind();
            var order = database.Orders[0];
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
            var database = new MockNorthwind();
            var order = database.Orders[0];
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
            var database = new MockNorthwind();
            var order = database.Orders[0];
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
            var database = new MockNorthwind();
            var order = database.Orders[0];
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
            var database = new MockNorthwind();
            var order = database.Orders[0];
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
            var database = new MockNorthwind();
            var order = database.Orders[0];
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
        public void GetChanges_On_Unchanged_Order_With_Details_Should_Return_Marked_Children()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var order3 = database.Orders[3];
            var changeTracker = new ChangeTrackingCollection<Order>(order, order3);
            order3.OrderDate += new System.TimeSpan(1, 0, 0, 0); // + one day
            order3.Customer.CustomerName += " (test)";
            var orderDetails = (IList<OrderDetail>)changeTracker[0].OrderDetails;
            var unchangedDetail = orderDetails[0];
            var modifiedDetail = orderDetails[1];
            var deletedDetail = orderDetails[2];
            var addedDetail = new OrderDetail
            {
                ProductId = 1,
                Product = database.Products[0],
                Quantity = 10,
                UnitPrice = 20M
            };
            orderDetails.Add(addedDetail);
            modifiedDetail.UnitPrice++;
            orderDetails.Remove(deletedDetail);

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            var changedOrder = changes.First();
            var changedOrder3 = changes[1];
            var changedModifiedDetail = changedOrder.OrderDetails.Single(d => d.ProductId == modifiedDetail.ProductId);
            var changedAddedDetail = changedOrder.OrderDetails.Single(d => d.ProductId == addedDetail.ProductId);
            var changedDeletedDetail = changedOrder.OrderDetails.Single(d => d.ProductId == deletedDetail.ProductId);
            Assert.AreEqual(TrackingState.Unchanged, changedOrder.TrackingState);
            Assert.AreEqual(3, changedOrder.OrderDetails.Count);
            Assert.AreEqual(TrackingState.Modified, changedModifiedDetail.TrackingState);
            Assert.AreEqual(TrackingState.Added, changedAddedDetail.TrackingState);
            Assert.AreEqual(TrackingState.Deleted, changedDeletedDetail.TrackingState);
            Assert.That(changedOrder.OrderDetails, Has.No.Member(unchangedDetail));
            Assert.IsNotNull(order.Customer);
            Assert.IsNotNull(order3.Customer);
            Assert.IsNotNull(changedOrder.Customer);
            Assert.IsNotNull(changedOrder3.Customer);
            Assert.IsTrue(object.ReferenceEquals(order.Customer, order3.Customer));
            Assert.IsFalse(object.ReferenceEquals(order.Customer, changedOrder.Customer));
            Assert.IsTrue(object.ReferenceEquals(changedOrder.Customer, changedOrder3.Customer));
        }

        [Test]
        public void GetChanges_On_Added_Order_With_Details_Should_Return_Marked_Children_Added()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(true) {order};
            var orderDetails = (IList<OrderDetail>)changeTracker[0].OrderDetails;
            var unchangedDetail = orderDetails[0];
            var modifiedDetail = orderDetails[1];
            var deletedDetail = orderDetails[2];
            var addedDetail = new OrderDetail
            {
                ProductId = 1,
                Product = database.Products[0],
                Quantity = 10,
                UnitPrice = 20M
            };
            orderDetails.Add(addedDetail);
            modifiedDetail.UnitPrice++;
            orderDetails.Remove(deletedDetail);

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            var changedOrder = changes.First();
            var changedExistingDetail = changedOrder.OrderDetails.Single(d => d.ProductId == unchangedDetail.ProductId);
            var changedModifiedDetail = changedOrder.OrderDetails.Single(d => d.ProductId == modifiedDetail.ProductId);
            var changedAddedDetail = changedOrder.OrderDetails.Single(d => d.ProductId == addedDetail.ProductId);
            Assert.AreEqual(TrackingState.Added, changedOrder.TrackingState);
            Assert.AreEqual(3, changedOrder.OrderDetails.Count);
            Assert.AreEqual(TrackingState.Added, changedModifiedDetail.TrackingState);
            Assert.AreEqual(TrackingState.Added, changedAddedDetail.TrackingState);
            Assert.AreEqual(TrackingState.Added, changedExistingDetail.TrackingState);
            Assert.That(changedOrder.OrderDetails, Has.No.Member(deletedDetail));
        }

        [Test]
        public void GetChanges_On_Deleted_Order_With_Details_Should_Return_Marked_Children_Deleted()
        {
            // NOTE: Removed order with added detail should exclude added detail
            // Removed order with deleted detail should include deleted detail

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var orderDetails = (IList<OrderDetail>)changeTracker[0].OrderDetails;
            var unchangedDetail = orderDetails[0];
            var modifiedDetail = orderDetails[1];
            var deletedDetail = orderDetails[2];
            var addedDetail = new OrderDetail
            {
                ProductId = 1,
                Product = database.Products[0],
                Quantity = 10,
                UnitPrice = 20M
            };
            orderDetails.Add(addedDetail);
            modifiedDetail.UnitPrice++;
            orderDetails.Remove(deletedDetail);
            changeTracker.Remove(order);

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            var changedOrder = changes.First();
            var changedExistingDetail = changedOrder.OrderDetails.SingleOrDefault(d => d.ProductId == unchangedDetail.ProductId);
            var changedModifiedDetail = changedOrder.OrderDetails.SingleOrDefault(d => d.ProductId == modifiedDetail.ProductId);
            var changedAddedDetail = changedOrder.OrderDetails.SingleOrDefault(d => d.ProductId == addedDetail.ProductId);
            var changedDeletedDetail = changedOrder.OrderDetails.SingleOrDefault(d => d.ProductId == deletedDetail.ProductId);
            Assert.AreEqual(TrackingState.Deleted, changedOrder.TrackingState);
            Assert.AreEqual(3, changedOrder.OrderDetails.Count);
            Assert.AreEqual(TrackingState.Deleted, changedModifiedDetail.TrackingState);
            Assert.AreEqual(TrackingState.Deleted, changedExistingDetail.TrackingState);
            Assert.AreEqual(TrackingState.Deleted, changedDeletedDetail.TrackingState);
            Assert.IsNull(changedAddedDetail);
            Assert.AreEqual(TrackingState.Unchanged, addedDetail.TrackingState);
        }

        [Test]
        public void GetChanges_On_Modified_Order_Should_Return_Order()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.OrderDate = order.OrderDate.AddDays(1);

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.IsFalse(changes.Tracking);
            Assert.AreNotSame(changes[0], order);
            Assert.IsTrue(changes[0].IsEquatable(order));
        }

        [Test]
        public void GetChanges_On_Unchanged_Order_With_Modified_Detail_Should_Return_Order()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.OrderDetails[0].Quantity++;

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, changes[0].TrackingState);
            Assert.AreEqual(1, changes[0].OrderDetails.Count);
            Assert.AreEqual(TrackingState.Modified, changes[0].OrderDetails[0].TrackingState);
            Assert.IsTrue(changes[0].IsEquatable(order));
            Assert.IsTrue(changes[0].OrderDetails[0].IsEquatable(order.OrderDetails[0]));
        }

        [Test]
        public void GetChanges_On_Unchanged_OrderDetail_With_Modified_Product_Should_Return_OrderDetail()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.OrderDetails[0].Product.ProductName = "xxx";

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, changes[0].OrderDetails[0].TrackingState);
            Assert.AreEqual(1, changes[0].OrderDetails.Count);
            Assert.AreEqual(TrackingState.Modified, changes[0].OrderDetails[0].Product.TrackingState);
            Assert.IsTrue(changes[0].IsEquatable(order));
            Assert.IsTrue(changes[0].OrderDetails[0].IsEquatable(order.OrderDetails[0]));
            Assert.IsTrue(changes[0].OrderDetails[0].Product.IsEquatable(order.OrderDetails[0].Product));
        }

        #endregion

        #region MergeChangesTests

        [Test]
        public void Updated_Parent_With_Children_Should_Merge_Unchanged_Children()
        {
            // Arrange
            var database = new MockNorthwind();

            // Get order, fix up details, clone
            var origOrder = database.Orders[0];
            foreach (var detail in origOrder.OrderDetails)
                detail.Order = origOrder;
            var updatedOrder = origOrder.Clone<Order>();

            // Add unchanged detail to orig order
            origOrder.OrderDetails.Add(new OrderDetail
            {
                ProductId = 1,
                Product = database.Products[0],
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
            origOrder.OrderDetails.Add(addedDetail);
            origOrder.OrderDetails.RemoveAt(1);

            // Act

            // Merge updates into orig order
#pragma warning disable 618
            changeTracker.MergeChanges(ref origOrder, updatedOrder);
#pragma warning restore 618

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
            var database = new MockNorthwind();
            var employee = database.Employees[0];
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            var unchangedTerritory = employee.Territories[0];
            var modifiedTerritory = employee.Territories[1];
            var deletedTerritory = employee.Territories[2];
            var addedTerritory = database.Territories[3];

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
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Employee>(true);
            var employee = database.Employees[0];
            var unchangedTerritory = employee.Territories[0];
            var modifiedTerritory = employee.Territories[1];
            var deletedTerritory = employee.Territories[2];
            var addedTerritory = database.Territories[3];
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
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Employee>(true);
            var employee = database.Employees[0];
            var unchangedTerritory = employee.Territories[0];
            var modifiedTerritory = employee.Territories[1];
            var deletedTerritory = employee.Territories[2];
            var addedTerritory = database.Territories[3];
            changeTracker.Add(employee);

            // Act
            modifiedTerritory.TerritoryDescription = "xxx";
            employee.Territories.Add(addedTerritory); // stays added
            employee.Territories.Remove(deletedTerritory); // from added to unchanged
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
            // 

            // Arrange
            var database = new MockNorthwind();
            var employee = database.Employees[0];
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            var unchangedTerritory = employee.Territories[0];
            var modifiedTerritory = employee.Territories[1];
            var deletedTerritory = employee.Territories[2];
            var addedTerritory = database.Territories[3];

            // Act
            modifiedTerritory.TerritoryDescription = "xxx";
            employee.Territories.Add(addedTerritory);
            employee.Territories.Remove(deletedTerritory);
            changeTracker.Remove(employee);

            // Assert
            Assert.AreEqual(TrackingState.Deleted, employee.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, unchangedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Modified, modifiedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, addedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Deleted, deletedTerritory.TrackingState);
        }

        #endregion

        #region ManyToMany - Modified Properties Tests

        [Test]
        public void Existing_Employee_With_Modified_Children_Should_Add_ModifiedProperties()
        {
            // Arrange
            var database = new MockNorthwind();
            var employee = database.Employees[0];
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
            var database = new MockNorthwind();
            var employee = database.Employees[0];
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
            var database = new MockNorthwind();
            var employee = database.Employees[0];
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
            var database = new MockNorthwind();
            var employee = database.Employees[0];
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
            var database = new MockNorthwind();
            var employee = database.Employees[0];
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
            var database = new MockNorthwind();
            var employee = database.Employees[0];
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            var unchangedTerritory = employee.Territories[0];
            var modifiedTerritory = employee.Territories[1];
            var deletedTerritory = employee.Territories[2];
            var addedTerritory = database.Territories[3];
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

        [Test]
        public void GetChanges_On_Existing_Employee_With_Territory_And_Modified_Area_Should_Return_Marked_Territory()
        {
            // Ensure that changes are retrieved across M-M relationships.

            // Arrange
            var database = new MockNorthwind();
            var employee = database.Employees[0];
            var territory = employee.Territories[0];
            var area = new Area
            {
                AreaId = 1,
                AreaName = "Northern",
                TrackingState = TrackingState.Modified
            };
            territory.AreaId = 1;
            territory.Area = area;
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            area.AreaName = "xxx"; // Modify area

            // Act
            var changes = changeTracker.GetChanges();
            var changedEmployee = changes.First();
            var changedTerritory = changedEmployee.Territories.Single(t => t.TerritoryId == territory.TerritoryId);
            var changedArea = changedTerritory.Area;

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, changedEmployee.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, changedTerritory.TrackingState);
            Assert.AreEqual(TrackingState.Modified, changedArea.TrackingState);
            Assert.Contains("AreaName", (ICollection)area.ModifiedProperties);
        }

        [Test]
        public void GetChanges_On_Modified_Employee_With_Unchanged_Territories_Should_Return_No_Territories()
        {
            // Arrange
            var database = new MockNorthwind();
            var employee = database.Employees[0];
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            employee.LastName = "xxx";

            // Act
            var changes = changeTracker.GetChanges();
            var changedEmployee = changes.First();

            // Assert
            Assert.IsEmpty(changedEmployee.Territories);
        }

        #endregion

        #region ManyToOne - Set Status Tests

        [Test]
        public void Existing_Order_With_Unchanged_Customer_Should_Mark_Customer_As_Unchanged()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);

            // Act

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, order.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, order.Customer.TrackingState);
            Assert.IsFalse(order.HasChanges());
        }

        [Test]
        public void Existing_Order_With_Assigned_Customer_Should_Mark_Customer_As_Modified()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var customer = database.Customers[5];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.Customer = customer;

            // Act
            order.Customer.CustomerName = "xxx";

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, order.TrackingState);
            Assert.AreEqual(TrackingState.Modified, order.Customer.TrackingState);
            Assert.IsTrue(order.HasChanges());
        }

        [Test]
        public void Existing_Order_With_Modified_Customer_Should_Mark_Customer_As_Modified()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);

            // Act
            order.Customer.CustomerName = "xxx";

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, order.TrackingState);
            Assert.AreEqual(TrackingState.Modified, order.Customer.TrackingState);
            Assert.IsTrue(order.HasChanges());
        }

        [Test]
        public void Existing_Order_With_New_Customer_Should_Mark_Customer_As_Unchanged()
        {
            // NOTE: Setting Order.Customer to a new customer will not automatically
            // mark it as Added, because Order has no way to tell if customer is new
            // or already exists.  However, Customer can be marked as Added manually.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);

            // Act
            // Because not marked as added, refers to existing customer
            order.Customer = new Customer
            {
                CustomerId = "ABCD",
                CustomerName = "Test"
            };

            // NOTE: Reference properties are change-tracked but do not call 
            // NotifyPropertyChanged because it is called by foreign key's property setter.

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, order.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, order.Customer.TrackingState);
            Assert.IsFalse(order.HasChanges());
        }

        [Test]
        public void Existing_Order_With_Manually_Added_Customer_Should_Mark_Customer_As_Added()
        {
            // NOTE: Manually marking reference as added will create a new entity
            // when it is saved.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);

            // Act
            order.Customer = new Customer
            {
                CustomerId = "ABCD",
                CustomerName = "Test",
                TrackingState = TrackingState.Added // required to insert new customer
            };

            // NOTE: Reference properties are change-tracked but do not call 
            // NotifyPropertyChanged because it is called by foreign key's property setter.

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, order.TrackingState);
            Assert.AreEqual(TrackingState.Added, order.Customer.TrackingState);
            Assert.IsTrue(order.HasChanges());
        }

        [Test]
        public void Existing_Order_With_Removed_Customer_Should_Mark_Customer_As_Unchanged()
        {
            // NOTE: Setting Order.Customer to null will not automatically
            // mark it as Deleted. Deleting customer from order is not supported
            // because it could be related to other entities. However, it is possible
            // to delete a customer independently.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var customer = order.Customer;

            // Act
            order.Customer = null;

            // NOTE: Reference properties are change-tracked but do not call 
            // NotifyPropertyChanged because it is called by foreign key's property setter.

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, order.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, customer.TrackingState);
            Assert.IsFalse(order.HasChanges());
        }

        [Test]
        public void Added_Order_With_Unchanged_Customer_Should_Mark_Customer_As_Unchanged()
        {
            // NOTE: Referenced customer will remain unchanged because Order has no way to  
            // tell if customer is new or already exists. However, Customer can be marked 
            // as Added manually. (If it were 1-M or M-M, marking parent as added automatically
            // marks children as added.)

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(true);
            changeTracker.Add(order);

            // Act

            // Assert
            Assert.AreEqual(TrackingState.Added, order.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, order.Customer.TrackingState);
            Assert.IsTrue(order.HasChanges());
        }

        [Test]
        public void Added_Order_With_Modified_Customer_Should_Mark_Customer_As_Modified()
        {
            // NOTE: Modified reference entity will remain modified if parent is added.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(true);
            changeTracker.Add(order);

            // Act
            order.Customer.CustomerName = "xxx";

            // Assert
            Assert.AreEqual(TrackingState.Added, order.TrackingState);
            Assert.AreEqual(TrackingState.Modified, order.Customer.TrackingState);
            Assert.IsTrue(order.HasChanges());
        }

        [Test]
        public void Added_Order_With_New_Customer_Should_Mark_Customer_As_Unchanged()
        {
            // NOTE: Setting Order.Customer to a new customer will not automatically
            // mark it as Added, because Order has no way to tell if customer is new
            // or already exists.  However, Customer can be marked as Added manually.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(true);
            changeTracker.Add(order);

            // Act
            // Because not marked as added, refers to existing customer
            order.Customer = new Customer
            {
                CustomerId = "ABCD",
                CustomerName = "Test"
            };

            // Assert
            Assert.AreEqual(TrackingState.Added, order.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, order.Customer.TrackingState);
            Assert.IsTrue(order.HasChanges());
        }

        [Test]
        public void Added_Order_With_Manually_Added_Customer_Should_Mark_Customer_As_Added()
        {
            // NOTE: Manually marking reference as added will create a new entity
            // when it is saved.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(true);
            changeTracker.Add(order);

            // Act
            order.Customer = new Customer
            {
                CustomerId = "ABCD",
                CustomerName = "Test",
                TrackingState = TrackingState.Added // required to insert new customer
            };

            // Assert
            Assert.AreEqual(TrackingState.Added, order.TrackingState);
            Assert.AreEqual(TrackingState.Added, order.Customer.TrackingState);
            Assert.IsTrue(order.HasChanges());
        }

        [Test]
        public void Added_Order_With_Removed_Customer_Should_Mark_Customer_As_Unchanged()
        {
            // NOTE: Removed customer will not be marked as deleted. Instead to be deleted
            // it must be independently marked as deleted.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(true);
            changeTracker.Add(order);
            var customer = order.Customer;

            // Act
            order.Customer = null;

            // Assert
            Assert.AreEqual(TrackingState.Added, order.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, customer.TrackingState);
            Assert.IsTrue(order.HasChanges());
        }

        [Test]
        public void Removed_Order_With_Unchanged_Customer_Should_Mark_Customer_As_Unchanged()
        {
            // NOTE: Removing order will not automatically mark customer as deleted.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var customer = order.Customer;

            // Act
            changeTracker.Remove(order);

            // Assert
            Assert.AreEqual(TrackingState.Deleted, order.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, order.Customer.TrackingState);
            Assert.IsTrue(order.HasChanges());
        }

        [Test]
        public void Removed_Order_With_Modified_Customer_Should_Mark_Customer_As_Modified()
        {
            // NOTE: Modified reference entity will remain modified if parent is deleted.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.Customer.CustomerName = "xxx";

            // Act
            changeTracker.Remove(order);

            // Assert
            Assert.AreEqual(TrackingState.Deleted, order.TrackingState);
            Assert.AreEqual(TrackingState.Modified, order.Customer.TrackingState);
            Assert.IsTrue(order.HasChanges());
        }

        [Test]
        public void Removed_Order_With_New_Customer_Should_Mark_Customer_As_Unchanged()
        {
            // NOTE: Setting Order.Customer to null will not mark customer as deleted.
            // Because customer not marked as deleted, it refers to an existing customer.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.Customer = new Customer
            {
                CustomerId = "ABCD",
                CustomerName = "Test"
            };

            // Act
            changeTracker.Remove(order);

            // Assert
            Assert.AreEqual(TrackingState.Deleted, order.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, order.Customer.TrackingState);
            Assert.IsTrue(order.HasChanges());
        }

        [Test]
        public void Removed_Order_With_Manually_Added_Customer_Should_Mark_Customer_As_Added()
        {
            // NOTE: Manually marking reference as added will create a new entity
            // when it is saved.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.Customer = new Customer
            {
                CustomerId = "ABCD",
                CustomerName = "Test",
                TrackingState = TrackingState.Added
            };

            // Act
            changeTracker.Remove(order);

            // Assert
            Assert.AreEqual(TrackingState.Deleted, order.TrackingState);
            Assert.AreEqual(TrackingState.Added, order.Customer.TrackingState);
            Assert.IsTrue(order.HasChanges());
        }

        [Test]
        public void Removed_Order_With_Removed_Customer_Should_Mark_Customer_As_Unchanged()
        {
            // NOTE: Removed customer will not be marked as deleted. Instead to be deleted
            // it must be independently marked as deleted.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var customer = order.Customer;
            order.Customer = null;

            // Act
            changeTracker.Remove(order);

            // Assert
            Assert.AreEqual(TrackingState.Deleted, order.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, customer.TrackingState);
            Assert.IsTrue(order.HasChanges());
        }

        #endregion

        #region ManyToOne - Modified Properties Tests

        [Test]
        public void Existing_Order_With_Modified_Customer_Should_Add_ModifiedProperties()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);

            // Act
            order.Customer.CustomerName = "xxx";

            // Assert
            Assert.Contains("CustomerName", (ICollection)order.Customer.ModifiedProperties);
        }

        [Test]
        public void Existing_Order_With_Modified_Customer_Should_Add_Multiple_ModifiedProperties()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);

            // Act
            order.Customer.CustomerName = "xxx";
            order.Customer.Data = "xxx";

            // Assert
            Assert.Contains("CustomerName", (ICollection)order.Customer.ModifiedProperties);
            Assert.Contains("Data", (ICollection)order.Customer.ModifiedProperties);
        }

        [Test]
        public void Existing_Order_Removed_With_Modified_Customer_Has_Children_ModifiedProperties_Cleared()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var modifiedCustomer = order.Customer;
            modifiedCustomer.CustomerName = "xxx";

            // Act
            changeTracker.Remove(order);

            // Assert
            Assert.IsTrue(modifiedCustomer.ModifiedProperties == null
                || modifiedCustomer.ModifiedProperties.Count == 0);
        }

        #endregion

        #region ManyToOne - GetChanges Tests

        [Test]
        public void GetChanges_On_Existing_Order_With_Unchanged_Customer_Should_Return_Empty_Collection()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.IsEmpty(changes);
        }

        [Test]
        public void GetChanges_On_Existing_Order_With_Modified_Customer_Should_Return_Customer_Marked_As_Modified()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.Customer.CustomerName = "xxx";

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.IsNotEmpty(changes);
            Assert.AreEqual(TrackingState.Unchanged, changes.First().TrackingState);
            Assert.AreEqual(TrackingState.Modified, changes.First().Customer.TrackingState);
        }

        [Test]
        public void GetChanges_On_Existing_Order_With_New_Customer_Should_Return_Empty_Collection()
        {
            // NOTE: Reference properties not explicitly marked as Added will be considered
            // unchanged.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.Customer = new Customer()
            {
                CustomerId = "ABCD",
                CustomerName = "Test"
            };

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.IsEmpty(changes);
        }

        [Test]
        public void GetChanges_On_Existing_Order_With_Manually_Added_Customer_Should_Return_Customer_Marked_As_Added()
        {
            // NOTE: Reference properties must be explicitly marked as Added.
            
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.Customer = new Customer()
            {
                CustomerId = "ABCD",
                CustomerName = "Test",
                TrackingState = TrackingState.Added
            };

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.IsNotEmpty(changes);
            Assert.AreEqual(TrackingState.Unchanged, changes.First().TrackingState);
            Assert.AreEqual(TrackingState.Added, changes.First().Customer.TrackingState);
        }

        [Test]
        public void GetChanges_On_Existing_Order_With_Removed_Customer_Should_Return_Empty_Collection()
        {
            // NOTE: Reference properties are change-tracked but do not call 
            // NotifyPropertyChanged because it is called by foreign key's property setter.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var customer = order.Customer;
            order.Customer = null;

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.IsEmpty(changes);
            Assert.AreEqual(TrackingState.Unchanged, customer.TrackingState);
        }

        [Test]
        public void GetChanges_On_Existing_Order_With_Customer_Modified_Territory_Should_Return_Territory_Marked_As_Modified()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            order.Customer.Territory = new Territory
            {
                TerritoryId = "91360", TerritoryDescription = "Southern California",
                Customers = new ChangeTrackingCollection<Customer> { order.Customer }
            };
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.Customer.Territory.TerritoryDescription = "xxx";

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.IsNotEmpty(changes);
            Assert.AreEqual(TrackingState.Unchanged, changes.First().TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, changes.First().Customer.TrackingState);
            Assert.AreEqual(TrackingState.Modified, changes.First().Customer.Territory.TrackingState);
        }

        [Test]
        public void GetChanges_On_Modified_Order_With_UnModified_Customer_Should_Set_Customer_To_Null()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            order.Customer.Territory = new Territory
            {
                TerritoryId = "91360",
                TerritoryDescription = "Southern California",
                Customers = new ChangeTrackingCollection<Customer> { order.Customer }
            };
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.OrderDate = order.OrderDate.AddDays(1);

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.IsNotEmpty(changes);
            Assert.AreEqual(TrackingState.Modified, changes.First().TrackingState);
            Assert.IsNull(changes.First().Customer);
        }

        [Test]
        public void GetChanges_On_Existing_Order_With_Modified_Customer_With_Unmodified_Territory_Should_Set_Territory_To_Null()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            order.Customer.Territory = new Territory
            {
                TerritoryId = "91360",
                TerritoryDescription = "Southern California",
                Customers = new ChangeTrackingCollection<Customer> { order.Customer }
            };
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.Customer.CustomerName = "xxx";

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.IsNotEmpty(changes);
            Assert.AreEqual(TrackingState.Unchanged, changes.First().TrackingState);
            Assert.AreEqual(TrackingState.Modified, changes.First().Customer.TrackingState);
            Assert.IsNull(changes.First().Customer.Territory);
        }

        [Test]
        public void GetChanges_On_Existing_Order_With_Modified_Customer_With_Deleted_Territory_Should_Set_Territory_To_Null()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            order.Customer.Territory = new Territory
            {
                TerritoryId = "91360",
                TerritoryDescription = "Southern California",
                Customers = new ChangeTrackingCollection<Customer> { order.Customer }
            };
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.Customer.CustomerName = "xxx";
            order.Customer.Territory.TrackingState = TrackingState.Deleted;

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.IsNotEmpty(changes);
            Assert.AreEqual(TrackingState.Unchanged, changes.First().TrackingState);
            Assert.AreEqual(TrackingState.Modified, changes.First().Customer.TrackingState);
            Assert.IsNull(changes.First().Customer.Territory);
        }

        [Test]
        public void GetChanges_On_Existing_Order_With_Customer_Territory_Added_Modified_Removed_Employees_Should_Return_Marked_Employees()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            order.Customer.Territory = new Territory
            {
                TerritoryId = "91360",
                TerritoryDescription = "Southern California",
                Customers = new ChangeTrackingCollection<Customer> { order.Customer }
            };
            var employee1 = database.Employees[0];
            var employee2 = database.Employees[1];
            var employee3 = database.Employees[2];
            var employee4 = database.Employees[3];
            employee1.Territories.Add(order.Customer.Territory);
            employee2.Territories.Add(order.Customer.Territory);
            employee3.Territories.Add(order.Customer.Territory);
            employee4.Territories.Add(order.Customer.Territory);
            order.Customer.Territory.Employees = new ChangeTrackingCollection<Employee>
                { employee1, employee2, employee3 };
            var changeTracker = new ChangeTrackingCollection<Order>(order);

            employee2.FirstName = "xxx";
            order.Customer.Territory.Employees.Remove(employee3);
            order.Customer.Territory.Employees.Add(employee4);

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.IsNotEmpty(changes);
            var changedOrder = changes.First();
            Assert.AreEqual(TrackingState.Unchanged, changedOrder.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, changedOrder.Customer.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, changedOrder.Customer.Territory.TrackingState);
            Assert.AreEqual(3, changedOrder.Customer.Territory.Employees.Count);
            Assert.AreEqual(TrackingState.Modified, changedOrder.Customer.Territory.Employees[0].TrackingState);
            Assert.AreEqual(TrackingState.Added, changedOrder.Customer.Territory.Employees[1].TrackingState);
            Assert.AreEqual(TrackingState.Deleted, changedOrder.Customer.Territory.Employees[2].TrackingState);
        }

        #endregion

        #region OneToOne - Set Status Tests

        [Test]
        public void Existing_Customer_With_Unchanged_CustomerSetting_Should_Have_CustomerSetting_Marked_As_Unchanged()
        {
            // Arrange
            var database = new MockNorthwind();
            var customer = database.Customers[0];
            var customerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Setting = "Setting1",
                Customer = customer
            };
            customer.CustomerSetting = customerSetting;
            var changeTracker = new ChangeTrackingCollection<Customer>(customer);

            // Act

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, customer.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, customerSetting.TrackingState);
        }

        [Test]
        public void Existing_Customer_With_Modified_CustomerSetting_Should_Have_CustomerSetting_Marked_As_Modified()
        {
            // Arrange
            var database = new MockNorthwind();
            var customer = database.Customers[0];
            var customerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Setting = "Setting1",
                Customer = customer
            };
            customer.CustomerSetting = customerSetting;
            var changeTracker = new ChangeTrackingCollection<Customer>(customer);

            // Act
            customer.CustomerSetting.Setting = "xxx";

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, customer.TrackingState);
            Assert.AreEqual(TrackingState.Modified, customerSetting.TrackingState);
        }

        [Test]
        public void Existing_Customer_With_Assigned_CustomerSetting_Should_Have_CustomerSetting_Marked_As_Modified()
        {
            // Arrange
            var database = new MockNorthwind();
            var customer = database.Customers[0];
            var customerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Setting = "Setting1",
                Customer = customer
            };
            var changeTracker = new ChangeTrackingCollection<Customer>(customer);
            customer.CustomerSetting = customerSetting; // will be tracked

            // Act
            customer.CustomerSetting.Setting = "xxx";

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, customer.TrackingState);
            Assert.AreEqual(TrackingState.Modified, customerSetting.TrackingState);
        }

        [Test]
        public void Existing_Customer_With_New_CustomerSetting_Should_Have_CustomerSetting_Marked_As_Unchanged()
        {
            // Arrange
            var database = new MockNorthwind();
            var customer = database.Customers[0];
            var changeTracker = new ChangeTrackingCollection<Customer>(customer);

            // Act
            var customerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Setting = "Setting1",
                Customer = customer
            };
            customer.CustomerSetting = customerSetting;

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, customer.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, customerSetting.TrackingState);
        }

        [Test]
        public void Existing_Customer_With_Manually_Added_CustomerSetting_Should_Have_CustomerSetting_Marked_As_Added()
        {
            // Arrange
            var database = new MockNorthwind();
            var customer = database.Customers[0];
            var changeTracker = new ChangeTrackingCollection<Customer>(customer);

            // Act
            var customerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Setting = "Setting1",
                Customer = customer,
                TrackingState = TrackingState.Added
            };
            customer.CustomerSetting = customerSetting;

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, customer.TrackingState);
            Assert.AreEqual(TrackingState.Added, customerSetting.TrackingState);
        }

        [Test]
        public void Existing_Customer_With_Removed_CustomerSetting_Should_Have_CustomerSetting_Marked_As_Unchanged()
        {
            // Arrange
            var database = new MockNorthwind();
            var customer = database.Customers[0];
            var customerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Setting = "Setting1",
                Customer = customer
            };
            customer.CustomerSetting = customerSetting;
            var changeTracker = new ChangeTrackingCollection<Customer>(customer);

            // Act
            customer.CustomerSetting = null;

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, customer.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, customerSetting.TrackingState);
        }

        #endregion

        #region OneToOne - Modified Properties Tests

        [Test]
        public void Existing_Customer_With_Modified_CustomerSetting_Should_Add_ModifiedProperties()
        {
            // Arrange
            var database = new MockNorthwind();
            var customer = database.Customers[0];
            customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Setting = "Setting1",
                Customer = customer
            };
            var changeTracker = new ChangeTrackingCollection<Customer>(customer);

            // Act
            customer.CustomerSetting.Setting = "xxx";

            // Assert
            Assert.Contains("Setting", (ICollection)customer.CustomerSetting.ModifiedProperties);
        }

        [Test]
        public void Existing_Customer_Removed_With_Modified_CustomerSetting_Has_Children_ModifiedProperties_Cleared()
        {
            // Arrange
            var database = new MockNorthwind();
            var customer = database.Customers[0];
            customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Setting = "Setting1",
                Customer = customer
            };
            var changeTracker = new ChangeTrackingCollection<Customer>(customer);
            var modifiedSetting = customer.CustomerSetting;
            modifiedSetting.Setting = "xxx";

            // Act
            changeTracker.Remove(customer);

            // Assert
            Assert.IsTrue(modifiedSetting.ModifiedProperties == null
                || modifiedSetting.ModifiedProperties.Count == 0);
        }
        
        #endregion

        #region OneToOne - GetChangesTests

        [Test]
        public void GetChanges_On_Existing_Customer_With_Unchanged_CustomerSetting_Should_Return_Empty_Collection()
        {
            // Arrange
            var database = new MockNorthwind();
            var customer = database.Customers[0];
            customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Setting = "Setting1",
                Customer = customer
            };
            var changeTracker = new ChangeTrackingCollection<Customer>(customer);

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.IsEmpty(changes);
        }

        [Test]
        public void GetChanges_On_Existing_Customer_With_Modified_CustomerSetting_Return_CustomerSetting_Marked_As_Modified()
        {
            // Arrange
            var database = new MockNorthwind();
            var customer = database.Customers[0];
            customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Setting = "Setting1",
                Customer = customer
            };
            var changeTracker = new ChangeTrackingCollection<Customer>(customer);

            // Act
            customer.CustomerSetting.Setting = "xxx";
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.IsNotEmpty(changes);
            Assert.AreEqual(TrackingState.Unchanged, changes.First().TrackingState);
            Assert.AreEqual(TrackingState.Modified, changes.First().CustomerSetting.TrackingState);
        }

        [Test]
        public void GetChanges_On_Existing_Customer_With_New_CustomerSetting_Return_Empty_Collection()
        {
            // Arrange
            var database = new MockNorthwind();
            var customer = database.Customers[0];
            var changeTracker = new ChangeTrackingCollection<Customer>(customer);
            customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Setting = "Setting1",
                Customer = customer
            };

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.IsEmpty(changes);
        }

        [Test]
        public void GetChanges_On_Existing_Customer_With_Manually_Added_CustomerSetting_Return_CustomerSetting_Marked_As_Added()
        {
            // Arrange
            var database = new MockNorthwind();
            var customer = database.Customers[0];
            var changeTracker = new ChangeTrackingCollection<Customer>(customer);
            customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Setting = "Setting1",
                Customer = customer,
                TrackingState = TrackingState.Added
            };

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.IsNotEmpty(changes);
            Assert.AreEqual(TrackingState.Unchanged, changes.First().TrackingState);
            Assert.AreEqual(TrackingState.Added, changes.First().CustomerSetting.TrackingState);
        }

        [Test]
        public void GetChanges_On_Existing_Customer_With_Removed_CustomerSetting_Return_Empty_Collection()
        {
            // Arrange
            var database = new MockNorthwind();
            var customer = database.Customers[0];
            customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Setting = "Setting1",
                Customer = customer
            };
            var customerSetting = customer.CustomerSetting;
            var changeTracker = new ChangeTrackingCollection<Customer>(customer);
            customer.CustomerSetting = null;

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.IsEmpty(changes);
            Assert.AreEqual(TrackingState.Unchanged, customerSetting.TrackingState);
        }

        #endregion
    }
}
