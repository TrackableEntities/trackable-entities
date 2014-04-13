using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using TrackableEntities.Client.Tests.Entities.Mocks;
using TrackableEntities.Client.Tests.Entities.NorthwindModels;
using TrackableEntities.Common;

namespace TrackableEntities.Client.Tests.Extensions
{
    [TestFixture]
    public class ChangeTrackingExtensionsTests
    {
        #region MergeChanges Tests

        #region MergeChanges: Single Entity
        
        [Test]
        public void MergeChanges_Should_Set_Modified_Order_Properties()
        {
            // Arrange
            var database = new MockNorthwind();
            var origOrder = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(origOrder);
            origOrder.CustomerId = "ALFKI";
            var updatedOrder = UpdateOrder(origOrder, database);

            // Act
            changeTracker.MergeChanges(updatedOrder);

            // Assert
            Assert.AreEqual(updatedOrder.CustomerId, origOrder.CustomerId);
            Assert.AreEqual(updatedOrder.Customer.CustomerId, origOrder.Customer.CustomerId);
            Assert.AreEqual(updatedOrder.OrderDate, origOrder.OrderDate);
        }

        [Test]
        public void MergeChanges_Should_Set_Modified_Order_TrackingState_To_Unchanged()
        {
            // Arrange
            var database = new MockNorthwind();
            var origOrder = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(origOrder);
            origOrder.CustomerId = "ALFKI";
            TrackingState origTrackingState = origOrder.TrackingState;
            var updatedOrder = UpdateOrder(origOrder, database);

            // Act
            changeTracker.MergeChanges(updatedOrder);

            // Assert
            Assert.AreEqual(TrackingState.Modified, origTrackingState);
            Assert.AreEqual(TrackingState.Unchanged, origOrder.TrackingState);
        }

        [Test]
        public void MergeChanges_Should_Set_Modified_Order_ModifiedProperties_To_Null()
        {
            // Arrange
            var database = new MockNorthwind();
            var origOrder = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(origOrder);
            origOrder.CustomerId = "ALFKI";
            var origModifiedProps = origOrder.ModifiedProperties;
            var updatedOrder = UpdateOrder(origOrder, database);

            // Act
            changeTracker.MergeChanges(updatedOrder);

            // Assert
            Assert.Contains("CustomerId", (ICollection)origModifiedProps);
            Assert.IsNull(origOrder.ModifiedProperties);
        }

        [Test]
        public void MergeChanges_Should_Set_Added_Order_Properties()
        {
            // Arrange
            var database = new MockNorthwind();
            var origOrder = new Order
            {
                OrderDate = DateTime.Parse("1996-07-04"),
                CustomerId = "ALFKI"
            };
            var changeTracker = new ChangeTrackingCollection<Order>(true) {origOrder};
            TrackingState origTrackingState = origOrder.TrackingState;
            var addedOrder = AddOrder(origOrder, database);

            // Act
            changeTracker.MergeChanges(addedOrder);

            // Assert
            Assert.AreEqual(TrackingState.Added, origTrackingState);
            Assert.AreEqual(TrackingState.Unchanged, origOrder.TrackingState);
        }

        [Test]
        public void MergeChanges_Should_Set_Added_Order_TrackingState_To_Unchanged()
        {
            // Arrange
            var database = new MockNorthwind();
            var origOrder = new Order
            {
                OrderDate = DateTime.Parse("1996-07-04"),
                CustomerId = "ALFKI"
            };
            var changeTracker = new ChangeTrackingCollection<Order>(true) { origOrder };
            var addedOrder = AddOrder(origOrder, database);

            // Act
            changeTracker.MergeChanges(addedOrder);

            // Assert
            Assert.AreEqual(addedOrder.CustomerId, origOrder.CustomerId);
            Assert.AreEqual(addedOrder.Customer.CustomerId, origOrder.Customer.CustomerId);
            Assert.AreEqual(addedOrder.OrderDate, origOrder.OrderDate);
        }

        #endregion

        #region MergeChanges: Multiple Entities
        
        #endregion

        #region MergeChanges: One-to-Many

        [Test]
        public void MergeChanges_Should_Set_Order_OrderDetails_NonTrackable_Properties()
        {

        }

        [Test]
        public void MergeChanges_Should_Set_Order_OrderDetails_TrackingState_To_Unchanged()
        {

        }

        [Test]
        public void MergeChanges_Should_Set_Order_OrderDetails_Modified_Properties_To_Null()
        {

        }

        [Test]
        public void MergeChanges_Should_Remove_Order_OrderDetails_Cached_Deletes()
        {

        }

        [Test]
        public void MergeChanges_Should_Set_Order_OrderDetails_Product_NonTrackable_Properties()
        {

        }

        [Test]
        public void MergeChanges_Should_Set_Order_OrderDetails_Product_TrackingState_To_Unchanged()
        {

        }

        [Test]
        public void MergeChanges_Should_Set_Order_OrderDetails_Product_Properties_To_Null()
        {

        }

        [Test]
        public void MergeChanges_Should_Set_Family_Children_NonTrackable_Properties()
        {

        }

        [Test]
        public void MergeChanges_Should_Set_Family_Children_TrackingState_To_Unchanged()
        {

        }

        [Test]
        public void MergeChanges_Should_Set_Family_Children_Properties_To_Null()
        {

        }

        #endregion

        #region MergeChanges: Many-to-One

        [Test]
        public void MergeChanges_Should_Set_Order_Customer()
        {

        }

        [Test]
        public void MergeChanges_Should_Set_Order_Customer_Non_Trackable_Properties()
        {

        }

        [Test]
        public void MergeChanges_Should_Set_Order_Customer_TrackingState_To_Unchanged()
        {

        }

        [Test]
        public void MergeChanges_Should_Set_Order_Customer_Modified_Properties_To_Null()
        {

        }

        #endregion

        #region MergeChanges: One-to-One

        #endregion

        #region MergeChanges: Many-to-Many

        #endregion

        #region Helpers

        private Order AddOrder(Order origOrder, MockNorthwind database)
        {
            // Simulate serialization
            var addedOrder = origOrder.Clone<Order>();

            // Simulate load related entities
            addedOrder.Customer = database.Customers.Single(c => c.CustomerId == "ALFKI");

            // Simulate db-generated values
            addedOrder.OrderId = new Random().Next(1000);
            addedOrder.OrderDate = origOrder.OrderDate.AddDays(1);
            addedOrder.AcceptChanges();
            return addedOrder;
        }

        private Order UpdateOrder(Order origOrder, MockNorthwind database)
        {
            // Simulate serialization
            var updatedOrder = origOrder.Clone<Order>();

            // Simulate load related entities
            updatedOrder.Customer = database.Customers.Single(c => c.CustomerId == "ALFKI");

            // Simulate db-generated values
            updatedOrder.OrderDate = origOrder.OrderDate.AddDays(1);
            updatedOrder.AcceptChanges();
            return updatedOrder;
        }

        #endregion

        #endregion
    }
}
