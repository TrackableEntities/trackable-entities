using System.Collections;
using System.Collections.Generic;
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

        #region EntityChanged Event Tests

        [Ignore] // TODO: Drop
        public void EntityChanged_Event_Should_Fire_When_Child_Added_Modified_Deleted()
        {
            //// Arrange
            //int changesCount = 0;
            //var order = _database.Orders[0];
            //var changeTracker = new ChangeTrackingCollection<Order>(order);
            //var orderDetails = changeTracker[0].OrderDetails;
            //var addedDetail = new OrderDetail
            //{
            //    ProductId = 1,
            //    Product = _database.Products[0],
            //    Quantity = 10,
            //    UnitPrice = 20M
            //};
            //var modifiedDetail = orderDetails[0];
            //var deletedDetail = orderDetails[1];
            //changeTracker.EntityChanged += (s, e) => changesCount++;

            //// Act
            //orderDetails.Add(addedDetail);
            //modifiedDetail.UnitPrice++;
            //orderDetails.Remove(deletedDetail);

            //// Assert
            //Assert.AreEqual(3, changesCount);
        }

        #endregion

        #region GetChanges Tests

        // TODO: Get Changes

        /* [Test]
        public void GetChanges_Should_Return_Added_Items()
        {
            // Arrange
            var changeTracker = new ChangeTrackingCollection<Product>(true);
            var product = _database.Products[0];
            changeTracker.Add(product);

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.AreSame(product, changes[0]);
            Assert.AreEqual(TrackingState.Added, changes[0].TrackingState);
        }

        [Test]
        public void GetChanges_Should_Return_Modified_Items()
        {
            // Arrange
            var product = _database.Products[0];
            var changeTracker = new ChangeTrackingCollection<Product>(product);
            product.UnitPrice++;

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.AreSame(product, changes[0]);
            Assert.AreEqual(TrackingState.Modified, changes[0].TrackingState);
        }

        [Test]
        public void GetChanges_Should_Return_Deleted_Items()
        {
            // Arrange
            var product = _database.Products[0];
            var changeTracker = new ChangeTrackingCollection<Product>(product);
            changeTracker.Remove(product);

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.AreSame(product, changes[0]);
            Assert.AreEqual(TrackingState.Deleted, changes[0].TrackingState);
        }

        [Test]
        public void GetChanges_Should_Return_Added_Modified_Deleted_Items()
        {
            // Arrange
            var addedProduct = _database.Products[0];
            var updatedProduct = _database.Products[1];
            var deletedProduct = _database.Products[2];

            var changeTracker = new ChangeTrackingCollection<Product>(updatedProduct, deletedProduct);

            changeTracker.Add(addedProduct);
            updatedProduct.UnitPrice++;
            changeTracker.Remove(deletedProduct);

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.AreSame(addedProduct, changes[1]);
            Assert.AreEqual(TrackingState.Added, changes[1].TrackingState);
            Assert.AreSame(updatedProduct, changes[0]);
            Assert.AreEqual(TrackingState.Modified, changes[0].TrackingState);
            Assert.AreSame(deletedProduct, changes[2]);
            Assert.AreEqual(TrackingState.Deleted, changes[2].TrackingState);
        }
        */

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
    }
}
