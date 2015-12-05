using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TrackableEntities.Client.Tests.Entities.FamilyModels;
using TrackableEntities.Client.Tests.Entities.Mocks;
using TrackableEntities.Client.Tests.Entities.NorthwindModels;
using Xunit;

namespace TrackableEntities.Client.Tests
{
    public class ChangeTrackingCollectionWithChildrenTests
    {
        #region Set Status Tests

        [Fact]
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
            orderDetails.Remove(deletedDetail);
            orderDetails.Add(addedDetail);

            // Assert
            Assert.Equal(TrackingState.Added, addedDetail.TrackingState);
            Assert.Equal(TrackingState.Modified, modifiedDetail.TrackingState);
            Assert.Equal(TrackingState.Deleted, deletedDetail.TrackingState);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Added, addedDetail1.TrackingState);
            Assert.Equal(TrackingState.Added, addedDetail2.TrackingState);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Added, order.TrackingState);
            Assert.Equal(TrackingState.Added, orderDetails[0].TrackingState);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Added, order.TrackingState);
            Assert.Equal(TrackingState.Added, orderDetails[0].TrackingState);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, order.TrackingState);
            Assert.Equal(TrackingState.Unchanged, orderDetail.TrackingState);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Added, order.TrackingState);
            Assert.Equal(TrackingState.Unchanged, orderDetail.TrackingState);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Deleted, order.TrackingState);
            Assert.Equal(TrackingState.Deleted, orderDetail.TrackingState);
        }

        #endregion

        #region Modified Properties Tests

        [Fact]
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
            Assert.Contains("UnitPrice", modifiedDetail.ModifiedProperties);
        }

        [Fact]
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
            Assert.Contains("Quantity", modifiedDetail.ModifiedProperties);
            Assert.Contains("UnitPrice", modifiedDetail.ModifiedProperties);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, modifiedDetail.TrackingState);
        }

        [Fact]
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
            Assert.True(modifiedDetail.ModifiedProperties == null
                || modifiedDetail.ModifiedProperties.Count == 0);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Modified, modifiedDetail.TrackingState);
            Assert.Contains("UnitPrice", modifiedDetail.ModifiedProperties);
            Assert.DoesNotContain("Quantity", modifiedDetail.ModifiedProperties);
        }

        [Fact]
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
            Assert.True(orderDetail.ModifiedProperties == null
                || orderDetail.ModifiedProperties.Count == 0);
        }
        
        #endregion

        #region GetChanges Tests

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, changedOrder.TrackingState);
            Assert.Equal(3, changedOrder.OrderDetails.Count);
            Assert.Equal(TrackingState.Modified, changedModifiedDetail.TrackingState);
            Assert.Equal(TrackingState.Added, changedAddedDetail.TrackingState);
            Assert.Equal(TrackingState.Deleted, changedDeletedDetail.TrackingState);
            Assert.DoesNotContain(unchangedDetail, changedOrder.OrderDetails);
            Assert.NotNull(order.Customer);
            Assert.NotNull(order3.Customer);
            Assert.NotNull(changedOrder.Customer);
            Assert.NotNull(changedOrder3.Customer);
            Assert.True(object.ReferenceEquals(order.Customer, order3.Customer));
            Assert.False(object.ReferenceEquals(order.Customer, changedOrder.Customer));
            Assert.True(object.ReferenceEquals(changedOrder.Customer, changedOrder3.Customer));
        }

        [Fact]
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
            Assert.Equal(TrackingState.Added, changedOrder.TrackingState);
            Assert.Equal(3, changedOrder.OrderDetails.Count);
            Assert.Equal(TrackingState.Added, changedModifiedDetail.TrackingState);
            Assert.Equal(TrackingState.Added, changedAddedDetail.TrackingState);
            Assert.Equal(TrackingState.Added, changedExistingDetail.TrackingState);
            Assert.DoesNotContain(deletedDetail, changedOrder.OrderDetails);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Deleted, changedOrder.TrackingState);
            Assert.Equal(3, changedOrder.OrderDetails.Count);
            Assert.Equal(TrackingState.Deleted, changedModifiedDetail.TrackingState);
            Assert.Equal(TrackingState.Deleted, changedExistingDetail.TrackingState);
            Assert.Equal(TrackingState.Deleted, changedDeletedDetail.TrackingState);
            Assert.Null(changedAddedDetail);
            Assert.Equal(TrackingState.Unchanged, addedDetail.TrackingState);
        }

        [Fact]
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
            Assert.False(changes.Tracking);
            Assert.NotSame(changes[0], order);
            Assert.True(changes[0].IsEquatable(order));
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, changes[0].TrackingState);
            Assert.Equal(1, changes[0].OrderDetails.Count);
            Assert.Equal(TrackingState.Modified, changes[0].OrderDetails[0].TrackingState);
            Assert.True(changes[0].IsEquatable(order));
            Assert.True(changes[0].OrderDetails[0].IsEquatable(order.OrderDetails[0]));
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, changes[0].OrderDetails[0].TrackingState);
            Assert.Equal(1, changes[0].OrderDetails.Count);
            Assert.Equal(TrackingState.Modified, changes[0].OrderDetails[0].Product.TrackingState);
            Assert.True(changes[0].IsEquatable(order));
            Assert.True(changes[0].OrderDetails[0].IsEquatable(order.OrderDetails[0]));
            Assert.True(changes[0].OrderDetails[0].Product.IsEquatable(order.OrderDetails[0].Product));
        }

        [Fact]
        public void GetChanges_With_More_Than_One_ITrackable_Members_Should_Return_NonEmpty_Changeset_if_Modified()
        {
            // Arrange
            var family = new Family
            {
                Father = new Parent
                {
                    Name = "Alan"
                },
                Mother = new Parent
                {
                    Name = "Judith"
                },
                Child = new Child
                {
                    Name = "Jake"
                }
            };

            var changeTracker = new ChangeTrackingCollection<Family>(family);

            // Act
            family.Father.Name = "Herb";
            var changedFamily = changeTracker.GetChanges().SingleOrDefault();

            //Assert
            Assert.NotNull(changedFamily);
            Assert.Equal(TrackingState.Unchanged, changedFamily.TrackingState);
            Assert.NotNull(changedFamily.Father);
            Assert.Equal(TrackingState.Modified, changedFamily.Father.TrackingState);
            Assert.Null(changedFamily.Mother);
            Assert.Null(changedFamily.Child);
        }

        #endregion

        #region MergeChangesTests

        [Fact]
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
            Assert.True(ReferenceEquals(origOrder, updatedOrder));

            // Tracking states set to unchanged
            Assert.Equal(TrackingState.Unchanged, origOrder.OrderDetails[0].TrackingState);
            Assert.Equal(TrackingState.Unchanged, origOrder.OrderDetails[1].TrackingState);
            Assert.Equal(TrackingState.Unchanged, origOrder.OrderDetails[2].TrackingState);

            // Modified properties set to null
            Assert.Null(origOrder.OrderDetails[0].ModifiedProperties);
            Assert.Null(origOrder.OrderDetails[1].ModifiedProperties);
            Assert.Null(origOrder.OrderDetails[2].ModifiedProperties);

            // Detail orders fixed up
            Assert.True(ReferenceEquals(origOrder, origOrder.OrderDetails[0].Order));
            Assert.True(ReferenceEquals(origOrder, origOrder.OrderDetails[1].Order));
            Assert.True(ReferenceEquals(origOrder, origOrder.OrderDetails[2].Order));
        }

        #endregion

        #region ManyToMany - Set Status Tests

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, employee.TrackingState);
            Assert.Equal(TrackingState.Unchanged, unchangedTerritory.TrackingState);
            Assert.Equal(TrackingState.Modified, modifiedTerritory.TrackingState);
            Assert.Equal(TrackingState.Added, addedTerritory.TrackingState);
            Assert.Equal(TrackingState.Deleted, deletedTerritory.TrackingState);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Added, employee.TrackingState);
            Assert.Equal(TrackingState.Added, unchangedTerritory.TrackingState);
            Assert.Equal(TrackingState.Added, modifiedTerritory.TrackingState);
            Assert.Equal(TrackingState.Added, addedTerritory.TrackingState);
            Assert.Equal(TrackingState.Unchanged, deletedTerritory.TrackingState);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, employee.TrackingState);
            Assert.Equal(TrackingState.Unchanged, unchangedTerritory.TrackingState);
            Assert.Equal(TrackingState.Unchanged, modifiedTerritory.TrackingState);
            Assert.Equal(TrackingState.Unchanged, addedTerritory.TrackingState);
            Assert.Equal(TrackingState.Unchanged, deletedTerritory.TrackingState);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Deleted, employee.TrackingState);
            Assert.Equal(TrackingState.Unchanged, unchangedTerritory.TrackingState);
            Assert.Equal(TrackingState.Modified, modifiedTerritory.TrackingState);
            Assert.Equal(TrackingState.Unchanged, addedTerritory.TrackingState);
            Assert.Equal(TrackingState.Deleted, deletedTerritory.TrackingState);
        }

        #endregion

        #region ManyToMany - Modified Properties Tests

        [Fact]
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
            Assert.Contains("TerritoryDescription", modifiedTerritory.ModifiedProperties);
        }

        [Fact]
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
            Assert.Contains("TerritoryDescription", modifiedTerritory.ModifiedProperties);
            Assert.Contains("Data", modifiedTerritory.ModifiedProperties);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, modifiedTerritory.TrackingState);
            Assert.True(modifiedTerritory.ModifiedProperties == null
                || modifiedTerritory.ModifiedProperties.Count == 0);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Modified, modifiedTerritory.TrackingState);
            Assert.Contains("Data", modifiedTerritory.ModifiedProperties);
            Assert.DoesNotContain("TerritoryDescription", modifiedTerritory.ModifiedProperties);
        }

        [Fact]
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
            Assert.True(modifiedTerritory.ModifiedProperties == null
                || modifiedTerritory.ModifiedProperties.Count == 0);
        }

        #endregion

        #region ManyToMany - GetChanges Tests

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, changedEmployee.TrackingState);
            Assert.Equal(3, changedEmployee.Territories.Count);
            Assert.Equal(TrackingState.Modified, changedModifiedTerritory.TrackingState);
            Assert.Equal(TrackingState.Added, changedAddedTerritory.TrackingState);
            Assert.Equal(TrackingState.Deleted, changedDeletedTerritory.TrackingState);
            Assert.DoesNotContain(unchangedTerritory, changedEmployee.Territories);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, changedEmployee.TrackingState);
            Assert.Equal(TrackingState.Unchanged, changedTerritory.TrackingState);
            Assert.Equal(TrackingState.Modified, changedArea.TrackingState);
            Assert.Contains("AreaName", area.ModifiedProperties);
        }

        [Fact]
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
            Assert.Empty(changedEmployee.Territories);
        }

        #endregion

        #region ManyToOne - Set Status Tests

        [Fact]
        public void Existing_Order_With_Unchanged_Customer_Should_Mark_Customer_As_Unchanged()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);

            // Act

            // Assert
            Assert.Equal(TrackingState.Unchanged, order.TrackingState);
            Assert.Equal(TrackingState.Unchanged, order.Customer.TrackingState);
            Assert.False(order.HasChanges());
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, order.TrackingState);
            Assert.Equal(TrackingState.Modified, order.Customer.TrackingState);
            Assert.True(order.HasChanges());
        }

        [Fact]
        public void Existing_Order_With_Modified_Customer_Should_Mark_Customer_As_Modified()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);

            // Act
            order.Customer.CustomerName = "xxx";

            // Assert
            Assert.Equal(TrackingState.Unchanged, order.TrackingState);
            Assert.Equal(TrackingState.Modified, order.Customer.TrackingState);
            Assert.True(order.HasChanges());
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, order.TrackingState);
            Assert.Equal(TrackingState.Unchanged, order.Customer.TrackingState);
            Assert.False(order.HasChanges());
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, order.TrackingState);
            Assert.Equal(TrackingState.Added, order.Customer.TrackingState);
            Assert.True(order.HasChanges());
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, order.TrackingState);
            Assert.Equal(TrackingState.Unchanged, customer.TrackingState);
            Assert.False(order.HasChanges());
        }

        [Fact]
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
            Assert.Equal(TrackingState.Added, order.TrackingState);
            Assert.Equal(TrackingState.Unchanged, order.Customer.TrackingState);
            Assert.True(order.HasChanges());
        }

        [Fact]
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
            Assert.Equal(TrackingState.Added, order.TrackingState);
            Assert.Equal(TrackingState.Modified, order.Customer.TrackingState);
            Assert.True(order.HasChanges());
        }

        [Fact]
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
            Assert.Equal(TrackingState.Added, order.TrackingState);
            Assert.Equal(TrackingState.Unchanged, order.Customer.TrackingState);
            Assert.True(order.HasChanges());
        }

        [Fact]
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
            Assert.Equal(TrackingState.Added, order.TrackingState);
            Assert.Equal(TrackingState.Added, order.Customer.TrackingState);
            Assert.True(order.HasChanges());
        }

        [Fact]
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
            Assert.Equal(TrackingState.Added, order.TrackingState);
            Assert.Equal(TrackingState.Unchanged, customer.TrackingState);
            Assert.True(order.HasChanges());
        }

        [Fact]
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
            Assert.Equal(TrackingState.Deleted, order.TrackingState);
            Assert.Equal(TrackingState.Unchanged, order.Customer.TrackingState);
            Assert.True(order.HasChanges());
        }

        [Fact]
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
            Assert.Equal(TrackingState.Deleted, order.TrackingState);
            Assert.Equal(TrackingState.Modified, order.Customer.TrackingState);
            Assert.True(order.HasChanges());
        }

        [Fact]
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
            Assert.Equal(TrackingState.Deleted, order.TrackingState);
            Assert.Equal(TrackingState.Unchanged, order.Customer.TrackingState);
            Assert.True(order.HasChanges());
        }

        [Fact]
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
            Assert.Equal(TrackingState.Deleted, order.TrackingState);
            Assert.Equal(TrackingState.Added, order.Customer.TrackingState);
            Assert.True(order.HasChanges());
        }

        [Fact]
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
            Assert.Equal(TrackingState.Deleted, order.TrackingState);
            Assert.Equal(TrackingState.Unchanged, customer.TrackingState);
            Assert.True(order.HasChanges());
        }

        #endregion

        #region ManyToOne - Modified Properties Tests

        [Fact]
        public void Existing_Order_With_Modified_Customer_Should_Add_ModifiedProperties()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);

            // Act
            order.Customer.CustomerName = "xxx";

            // Assert
            Assert.Contains("CustomerName", order.Customer.ModifiedProperties);
        }

        [Fact]
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
            Assert.Contains("CustomerName", order.Customer.ModifiedProperties);
            Assert.Contains("Data", order.Customer.ModifiedProperties);
        }

        [Fact]
        public void Existing_Order_Removed_With_Modified_Customer_Has_Children_ModifiedProperties_Cleared()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var modifiedCustomer = order.Customer;
            modifiedCustomer.CustomerName = "xxx";
            order.OrderDetails[0].ModifiedProperties = new List<string> { "UnitPrice "};

            // Act
            changeTracker.Remove(order);

            // Assert
            Assert.True(modifiedCustomer.ModifiedProperties.Count == 1);
            Assert.True(order.OrderDetails[0].ModifiedProperties == null);
        }

        #endregion

        #region ManyToOne - GetChanges Tests

        [Fact]
        public void GetChanges_On_Existing_Order_With_Unchanged_Customer_Should_Return_Empty_Collection()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.Empty(changes);
        }

        [Fact]
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
            Assert.NotEmpty(changes);
            Assert.Equal(TrackingState.Unchanged, changes.First().TrackingState);
            Assert.Equal(TrackingState.Modified, changes.First().Customer.TrackingState);
        }

        [Fact]
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
            Assert.Empty(changes);
        }

        [Fact]
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
            Assert.NotEmpty(changes);
            Assert.Equal(TrackingState.Unchanged, changes.First().TrackingState);
            Assert.Equal(TrackingState.Added, changes.First().Customer.TrackingState);
        }

        [Fact]
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
            Assert.Empty(changes);
            Assert.Equal(TrackingState.Unchanged, customer.TrackingState);
        }

        [Fact]
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
            Assert.NotEmpty(changes);
            Assert.Equal(TrackingState.Unchanged, changes.First().TrackingState);
            Assert.Equal(TrackingState.Unchanged, changes.First().Customer.TrackingState);
            Assert.Equal(TrackingState.Modified, changes.First().Customer.Territory.TrackingState);
        }

        [Fact]
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
            Assert.NotEmpty(changes);
            Assert.Equal(TrackingState.Modified, changes.First().TrackingState);
            Assert.Null(changes.First().Customer);
        }

        [Fact]
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
            Assert.NotEmpty(changes);
            Assert.Equal(TrackingState.Unchanged, changes.First().TrackingState);
            Assert.Equal(TrackingState.Modified, changes.First().Customer.TrackingState);
            Assert.Null(changes.First().Customer.Territory);
        }

        [Fact]
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
            Assert.NotEmpty(changes);
            Assert.Equal(TrackingState.Unchanged, changes.First().TrackingState);
            Assert.Equal(TrackingState.Modified, changes.First().Customer.TrackingState);
            Assert.Null(changes.First().Customer.Territory);
        }

        [Fact]
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
            Assert.NotEmpty(changes);
            var changedOrder = changes.First();
            Assert.Equal(TrackingState.Unchanged, changedOrder.TrackingState);
            Assert.Equal(TrackingState.Unchanged, changedOrder.Customer.TrackingState);
            Assert.Equal(TrackingState.Unchanged, changedOrder.Customer.Territory.TrackingState);
            Assert.Equal(3, changedOrder.Customer.Territory.Employees.Count);
            Assert.Equal(TrackingState.Modified, changedOrder.Customer.Territory.Employees[0].TrackingState);
            Assert.Equal(TrackingState.Added, changedOrder.Customer.Territory.Employees[1].TrackingState);
            Assert.Equal(TrackingState.Deleted, changedOrder.Customer.Territory.Employees[2].TrackingState);
        }

        #endregion

        #region OneToOne - Set Status Tests

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, customer.TrackingState);
            Assert.Equal(TrackingState.Unchanged, customerSetting.TrackingState);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, customer.TrackingState);
            Assert.Equal(TrackingState.Modified, customerSetting.TrackingState);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, customer.TrackingState);
            Assert.Equal(TrackingState.Modified, customerSetting.TrackingState);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, customer.TrackingState);
            Assert.Equal(TrackingState.Unchanged, customerSetting.TrackingState);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, customer.TrackingState);
            Assert.Equal(TrackingState.Added, customerSetting.TrackingState);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, customer.TrackingState);
            Assert.Equal(TrackingState.Unchanged, customerSetting.TrackingState);
        }

        #endregion

        #region OneToOne - Modified Properties Tests

        [Fact]
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
            Assert.Contains("Setting", customer.CustomerSetting.ModifiedProperties);
        }

        #endregion

        #region OneToOne - GetChangesTests

        [Fact]
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
            Assert.Empty(changes);
        }

        [Fact]
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
            Assert.NotEmpty(changes);
            Assert.Equal(TrackingState.Unchanged, changes.First().TrackingState);
            Assert.Equal(TrackingState.Modified, changes.First().CustomerSetting.TrackingState);
        }

        [Fact]
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
            Assert.Empty(changes);
        }

        [Fact]
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
            Assert.NotEmpty(changes);
            Assert.Equal(TrackingState.Unchanged, changes.First().TrackingState);
            Assert.Equal(TrackingState.Added, changes.First().CustomerSetting.TrackingState);
        }

        [Fact]
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
            Assert.Empty(changes);
            Assert.Equal(TrackingState.Unchanged, customerSetting.TrackingState);
        }

        #endregion
    }
}
