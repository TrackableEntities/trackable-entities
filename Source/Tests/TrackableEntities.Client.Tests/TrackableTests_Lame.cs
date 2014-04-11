using System;
using System.Collections.Generic;
using NUnit.Framework;
using TrackableEntities.Client;
using TrackableEntities.Client.Tests.Entities.Mocks;
using TrackableEntities.Client.Tests.Entities.NorthwindModels;

namespace TrackableEntities.Client_Lame.Tests
{
    [TestFixture]
    public class TrackableTests
    {
        [Test, Ignore]
        public void MergeChanges_Should_Return_Reference_to_Updated_Order()
        {
            // Arrange
            // Get order, fix up details
            var database = new MockNorthwind();
            var origOrder = database.Orders[0];
            foreach (var detail in origOrder.OrderDetails)
                detail.Order = origOrder;

            // Simulate update service operation with changed order date
            var updatedOrder = origOrder.Clone<Order>();
            var modifiedDate = updatedOrder.OrderDate = updatedOrder.OrderDate.AddDays(1);

            // Act
            // Merge changes from updatedOrder back to origOrder
            var changeTracker = new ChangeTrackingCollection<Order>(origOrder);
            var isTracking = changeTracker.Tracking;
            updatedOrder.MergeChanges(ref origOrder, changeTracker);

            // Assert
            Assert.AreSame(updatedOrder, origOrder);
            Assert.AreEqual(modifiedDate, origOrder.OrderDate);
            Assert.Contains(origOrder, changeTracker);
            Assert.AreEqual(isTracking, changeTracker.Tracking);
        }

        [Test, Ignore]
        public void MergeChanges_Should_Return_Reference_to_Updated_Order_With_Updated_Customer_Reference_Properties()
        {
            // Arrange
            // Get order, fix up details
            var database = new MockNorthwind();
            var origOrder = database.Orders[0];
            foreach (var detail in origOrder.OrderDetails)
                detail.Order = origOrder;
            var territory = new Territory
            {
                TerritoryId = "75070",
                TerritoryDescription = "Dallas",
                AreaId = 1
            };

            // Set foreign key values
            origOrder.Customer.TerritoryId = database.Territories[0].TerritoryId;

            // Simulate update service operation with updated customer
            var updatedOrder = origOrder.Clone<Order>();

            // Set reference properties
            updatedOrder.Customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = origOrder.Customer.CustomerId,
                Setting = "Setting1",
                Customer = origOrder.Customer
            };
            var modifiedTerritory = territory.Clone<Territory>();
            modifiedTerritory.Area = new Area
            {
                AreaId = 1,
                AreaName = "Northern"
            };
            updatedOrder.Customer.Territory = modifiedTerritory;

            // Act
            // Merge changes from updatedOrder back to origOrder
            var changeTracker = new ChangeTrackingCollection<Order>(origOrder);
            var isTracking = changeTracker.Tracking;
            updatedOrder.MergeChanges(ref origOrder, changeTracker);

            // Assert
            Assert.AreSame(updatedOrder, origOrder);
            Assert.IsNotNull(origOrder.Customer.CustomerSetting);
            Assert.IsNotNull(origOrder.Customer.Territory);
            Assert.IsNotNull(origOrder.Customer.Territory.Area);
            Assert.Contains(origOrder, changeTracker);
            Assert.AreEqual(isTracking, changeTracker.Tracking);
        }

        [Test, Ignore]
        public void MergeChanges_Should_Return_Reference_to_Updated_Order_With_Unchanged_OrderDetail()
        {
            // Arrange
            // Get order, fix up details
            var database = new MockNorthwind();
            var origOrder = database.Orders[0];
            foreach (var detail in origOrder.OrderDetails)
                detail.Order = origOrder;
            var unchangedDetail = origOrder.OrderDetails[0];

            // Change-track orig order
            var changeTracker = new ChangeTrackingCollection<Order>(origOrder);

            // Modify order details
            var modifiedDetail = origOrder.OrderDetails[1];
            var modifiedQuantity = modifiedDetail.Quantity++; // modify
            var removedDetail = origOrder.OrderDetails[2];
            origOrder.OrderDetails.RemoveAt(2); // remove
            var addedDetail = new OrderDetail();
            origOrder.OrderDetails.Add(addedDetail); // add

            // Simulate update service operation with updated customer
            var updatedOrder = origOrder.Clone<Order>();
            updatedOrder.OrderDetails.RemoveAt(0); // remove unchanged
            foreach (var detail in updatedOrder.OrderDetails)
                detail.TrackingState = TrackingState.Unchanged;

            // Act
            // Merge changes from updatedOrder back to origOrder
            var isTracking = changeTracker.Tracking;
            updatedOrder.MergeChanges(ref origOrder, changeTracker);

            // Assert
            Assert.AreSame(updatedOrder, origOrder);
            Assert.Contains(unchangedDetail, origOrder.OrderDetails);
            Assert.AreEqual(modifiedQuantity, origOrder.OrderDetails[1].Quantity);
            Assert.Contains(addedDetail, origOrder.OrderDetails);
            Assert.That(origOrder.OrderDetails, Has.No.Member(removedDetail));
            Assert.Contains(origOrder, changeTracker);
            Assert.AreEqual(isTracking, changeTracker.Tracking);
        }

        [Test, Ignore]
        public void MergeChanges_Should_Return_Reference_to_Updated_Order_With_Unchanged_Territory_Employee()
        {
            // Arrange
            // Get order, fix up details
            var database = new MockNorthwind();
            var origOrder = database.Orders[0];
            foreach (var detail in origOrder.OrderDetails)
                detail.Order = origOrder;
            var territory = new Territory
            {
                TerritoryId = "75070",
                TerritoryDescription = "Dallas",
                AreaId = 1,
                Employees = new ChangeTrackingCollection<Employee>
                {
                    new Employee { EmployeeId = 1 }, // unchanged
                    new Employee { EmployeeId = 2, FirstName = "Tony" }, // modified
                    new Employee { EmployeeId = 3 }, // removed
                }
            };
            var unchangedEmployee = territory.Employees[0];

            // Set foreign key values
            origOrder.Customer.TerritoryId = database.Territories[0].TerritoryId;

            // Simulate update service operation with updated customer
            var updatedOrder = origOrder.Clone<Order>();

            // Modifiy, add, remove territory employees
            // First employee is unchanged
            var modifiedTerritory = territory.Clone<Territory>();
            var modifiedName = modifiedTerritory.Employees[1].FirstName = "xxx";
            var addedEmployee = new Employee {EmployeeId = 4};
            modifiedTerritory.Employees.Add(addedEmployee);
            var removedEmployee = modifiedTerritory.Employees[2];
            modifiedTerritory.Employees.RemoveAt(2);
            updatedOrder.Customer.Territory = modifiedTerritory;

            // Act
            // Merge changes from updatedOrder back to origOrder
            var changeTracker = new ChangeTrackingCollection<Order>(origOrder);
            var isTracking = changeTracker.Tracking;
            updatedOrder.MergeChanges(ref origOrder, changeTracker);

            // Assert
            Assert.AreSame(updatedOrder, origOrder);
            Assert.IsNotNull(origOrder.Customer.Territory);
            Assert.Contains(unchangedEmployee, origOrder.Customer.Territory.Employees);
            Assert.AreEqual(modifiedName, origOrder.Customer.Territory.Employees[1].FirstName);
            Assert.Contains(addedEmployee, origOrder.OrderDetails);
            Assert.That(origOrder.OrderDetails, Has.No.Member(removedEmployee));
            Assert.Contains(origOrder, changeTracker);
            Assert.AreEqual(isTracking, changeTracker.Tracking);
        }
    }
}
