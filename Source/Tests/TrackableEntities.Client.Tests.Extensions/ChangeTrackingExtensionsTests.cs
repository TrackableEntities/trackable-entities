using System;
using System.Collections;
using System.Collections.Generic;
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
        public void MergeChanges_Should_Set_Properties_For_Modified_Order_With_Updated_Customer()
        {
            // Arrange
            var database = new MockNorthwind();
            var origOrder = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(origOrder);
            origOrder.CustomerId = "ALFKI";
            var updatedOrder = UpdateOrders(database, origOrder)[0];

            // Act
            changeTracker.MergeChanges(updatedOrder);

            // Assert
            Assert.AreEqual(updatedOrder.CustomerId, origOrder.CustomerId);
            Assert.AreEqual(updatedOrder.Customer.CustomerId, origOrder.Customer.CustomerId);
            Assert.AreEqual(updatedOrder.OrderDate, origOrder.OrderDate);
        }

        [Test]
        public void MergeChanges_Should_Set_TrackingState_To_Unchanged_For_Modified_Order_With_Updated_Customer()
        {
            // Arrange
            var database = new MockNorthwind();
            var origOrder = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(origOrder);
            origOrder.CustomerId = "ALFKI";
            TrackingState origTrackingState = origOrder.TrackingState;
            var updatedOrder = UpdateOrders(database, origOrder)[0];

            // Act
            changeTracker.MergeChanges(updatedOrder);

            // Assert
            Assert.AreEqual(TrackingState.Modified, origTrackingState);
            Assert.AreEqual(TrackingState.Unchanged, origOrder.TrackingState);
        }

        [Test]
        public void MergeChanges_Should_Set_ModifiedProperties_To_Null_For_Modified_Order_With_Updated_Customer()
        {
            // Arrange
            var database = new MockNorthwind();
            var origOrder = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(origOrder);
            origOrder.CustomerId = "ALFKI";
            var origModifiedProps = origOrder.ModifiedProperties;
            var updatedOrder = UpdateOrders(database, origOrder)[0];

            // Act
            changeTracker.MergeChanges(updatedOrder);

            // Assert
            Assert.Contains("CustomerId", (ICollection)origModifiedProps);
            Assert.IsNull(origOrder.ModifiedProperties);
        }

        [Test]
        public void MergeChanges_Should_Set_ChangeTracker_For_Modified_Order_With_Updated_Customer()
        {
            // Arrange
            var database = new MockNorthwind();
            var origOrder = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(origOrder);
            origOrder.CustomerId = "ALFKI";
            var updatedOrder = UpdateOrders(database, origOrder)[0];

            // Act
            changeTracker.MergeChanges(updatedOrder);
            TrackingState origTrackingState = origOrder.Customer.TrackingState;
            origOrder.Customer.CustomerName = "xxx";

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, origTrackingState);
            Assert.AreEqual(TrackingState.Modified, origOrder.Customer.TrackingState);
            Assert.Contains("CustomerName", (ICollection)origOrder.Customer.ModifiedProperties);
        }

        [Test]
        public void MergeChanges_Should_Set_Properties_For_Added_Order_With_Null_Customer()
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
            var addedOrder = AddOrders(database, origOrder)[0];

            // Act
            changeTracker.MergeChanges(addedOrder);

            // Assert
            Assert.AreEqual(TrackingState.Added, origTrackingState);
            Assert.AreEqual(TrackingState.Unchanged, origOrder.TrackingState);
        }

        [Test]
        public void MergeChanges_Should_Set_TrackingState_To_Unchanged_For_Added_Order_With_Null_Customer()
        {
            // Arrange
            var database = new MockNorthwind();
            var origOrder = new Order
            {
                OrderDate = DateTime.Parse("1996-07-04"),
                CustomerId = "ALFKI"
            };
            var changeTracker = new ChangeTrackingCollection<Order>(true) { origOrder };
            var addedOrder = AddOrders(database, origOrder)[0];

            // Act
            changeTracker.MergeChanges(addedOrder);

            // Assert
            Assert.AreEqual(addedOrder.CustomerId, origOrder.CustomerId);
            Assert.AreEqual(addedOrder.Customer.CustomerId, origOrder.Customer.CustomerId);
            Assert.AreEqual(addedOrder.OrderDate, origOrder.OrderDate);
        }

        [Test]
        public void MergeChanges_Should_Set_ChangeTracker_For_Added_Order_With_Null_Customer()
        {
            // Arrange
            var database = new MockNorthwind();
            var origOrder = new Order
            {
                OrderDate = DateTime.Parse("1996-07-04"),
                CustomerId = "ALFKI"
            };
            var changeTracker = new ChangeTrackingCollection<Order>(true) { origOrder };
            var addedOrder = AddOrders(database, origOrder)[0];

            // Act
            changeTracker.MergeChanges(addedOrder);
            TrackingState origTrackingState = origOrder.Customer.TrackingState;
            origOrder.Customer.CustomerName = "xxx";

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, origTrackingState);
            Assert.AreEqual(TrackingState.Modified, origOrder.Customer.TrackingState);
            Assert.Contains("CustomerName", (ICollection)origOrder.Customer.ModifiedProperties);
        }

        #endregion

        #region MergeChanges: Multiple Entities

        [Test]
        public void MergeChanges_Should_Set_Properties_For_Multiple_Modified_Orders_With_Updated_Customer()
        {
            // Arrange
            var database = new MockNorthwind();
            var origOrder1 = database.Orders[0];
            var origOrder2 = database.Orders[1];
            var changeTracker = new ChangeTrackingCollection<Order>(origOrder1, origOrder2);
            origOrder1.CustomerId = "ALFKI";
            origOrder2.CustomerId = "ANATR";
            var updatedOrders = UpdateOrders(database, origOrder1, origOrder2);

            // Act
            changeTracker.MergeChanges(updatedOrders.ToArray());

            // Assert
            Assert.AreEqual(updatedOrders[0].CustomerId, origOrder1.CustomerId);
            Assert.AreEqual(updatedOrders[0].Customer.CustomerId, origOrder1.Customer.CustomerId);
            Assert.AreEqual(updatedOrders[0].OrderDate, origOrder1.OrderDate);
            Assert.AreEqual(updatedOrders[1].CustomerId, origOrder2.CustomerId);
            Assert.AreEqual(updatedOrders[1].Customer.CustomerId, origOrder2.Customer.CustomerId);
            Assert.AreEqual(updatedOrders[1].OrderDate, origOrder2.OrderDate);
        }

        #endregion

        #region MergeChanges: One-to-Many

        [Test]
        public void MergeChanges_Should_Set_Properties_For_Order_With_OrderDetails()
        {
            // Arrange
            var database = new MockNorthwind();
            var origOrder = database.Orders[0];
            var product = database.Products.Single(p => p.ProductId == 14);
            origOrder.OrderDetails.Add(new OrderDetail { ProductId = 14, OrderId = 10249, Quantity = 9, UnitPrice = 18.6000M, Product = product });
            var unchangedDetail = origOrder.OrderDetails[0];
            var changeTracker = new ChangeTrackingCollection<Order>(origOrder);
            origOrder.OrderDetails[1].Product.ProductName = "xxx";
            origOrder.OrderDetails[2].Quantity++;
            origOrder.OrderDetails[2].ProductId = 1;
            var newUnitPrice = origOrder.OrderDetails[2].UnitPrice + 1;
            origOrder.OrderDetails.RemoveAt(3);
            origOrder.OrderDetails.Add(new OrderDetail { ProductId = 51, OrderId = 10249, Quantity = 40, UnitPrice = 42.4000M });

            var changes = changeTracker.GetChanges();
            var updatedOrder = UpdateOrdersWithDetails(database, changes)[0];

            // Act
            changeTracker.MergeChanges(updatedOrder);

            // Assert
            Assert.Contains(unchangedDetail, origOrder.OrderDetails); // Unchanged present
            Assert.AreEqual("xxx", origOrder.OrderDetails[1].Product.ProductName); // Prod name updated
            Assert.AreEqual(updatedOrder.OrderDetails[1].ProductId, origOrder.OrderDetails[2].Product.ProductId); // Changed Product set
            Assert.AreEqual(newUnitPrice, origOrder.OrderDetails[2].UnitPrice); // Db-generated value set
            Assert.AreEqual(updatedOrder.OrderDetails[2].Product.ProductId, origOrder.OrderDetails[3].Product.ProductId); // Added detail Product set
            ICollection cachedDeletes = ((ITrackingCollection)origOrder.OrderDetails).GetChanges(true);
            Assert.IsEmpty(cachedDeletes); // Cached deletes have been removed
        }

        // TODO: Continue Testing
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

        private List<Order> AddOrders(MockNorthwind database, params Order[] origOrders)
        {
            var addedOrders = new List<Order>();
            for (int i = 0; i < origOrders.Length; i++)
            {
                // Simulate serialization
                var origOrder = origOrders[i];
                var addedOrder = origOrder.Clone<Order>();

                // Simulate load related entities
                string customerId = i == 0 ? "ALFKI" : "ANATR";
                addedOrder.Customer = database.Customers.Single(c => c.CustomerId == customerId);

                // Simulate db-generated values
                addedOrder.OrderId = new Random().Next(1000);
                addedOrder.OrderDate = origOrder.OrderDate.AddDays(1);
                addedOrder.AcceptChanges();
                addedOrders.Add(addedOrder);
            }
            return addedOrders;
        }

        private List<Order> UpdateOrders(MockNorthwind database, params Order[] origOrders)
        {
            var updatedOrders = new List<Order>();
            for (int i = 0; i < origOrders.Length; i++)
            {
                // Simulate serialization
                var origOrder = origOrders[i];
                var updatedOrder = origOrder.Clone<Order>();

                // Simulate load related entities
                string customerId = i == 0 ? "ALFKI" : "ANATR";
                updatedOrder.Customer = database.Customers.Single(c => c.CustomerId == customerId);

                // Simulate db-generated values
                updatedOrder.OrderDate = origOrder.OrderDate.AddDays(1);
                updatedOrder.AcceptChanges();
                updatedOrders.Add(updatedOrder);
            }
            return updatedOrders;
        }

        private List<Order> UpdateOrdersWithDetails(MockNorthwind database, IEnumerable<Order> changes)
        {
            var updatedOrders = new List<Order>();
            foreach (var origOrder in changes)
            {
                // Simulate serialization
                var updatedOrder = origOrder.Clone<Order>();

                // Simulate load related entities
                updatedOrder.OrderDetails[1].Product = database.Products.Single(p => p.ProductId == 1);
                updatedOrder.OrderDetails[2].Product = database.Products.Single(p => p.ProductId == 51);

                // Simulate db-generated values
                updatedOrder.OrderDetails[1].UnitPrice++;
                updatedOrder.AcceptChanges();
                updatedOrders.Add(updatedOrder);
            }
            return updatedOrders;
        }

        #endregion

        #endregion
    }
}
