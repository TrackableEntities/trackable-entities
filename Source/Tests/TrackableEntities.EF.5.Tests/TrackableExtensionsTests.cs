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
        #region OneToMany AcceptChanges Tests

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
        public void Accept_Changes_Should_Remove_ModifiedProperties_From_Family()
        {
            // Arrange
            var parent = new MockFamily().Parent;
            parent.ModifiedProperties = new List<string> { "Name" };
            parent.Children[0].ModifiedProperties = new List<string> { "Name" };
            parent.Children[0].Children[0].ModifiedProperties = new List<string> { "Name" };
            parent.Children[0].Children[0].Children[1].ModifiedProperties = new List<string> { "Name" };

            // Act
            parent.AcceptChanges();

            // Assert
            IEnumerable<IEnumerable<string>> modifiedProps = parent.GetModifiedProperties();
            Assert.IsFalse(modifiedProps.Any(p => p != null));
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
        public void Accept_Changes_Should_Mark_Multiple_Orders_As_Unchanged()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var order1 = northwind.Orders[0];
            order1.TrackingState = TrackingState.Modified;
            order1.Customer.TrackingState = TrackingState.Modified;
            order1.OrderDetails[1].TrackingState = TrackingState.Modified;
            order1.OrderDetails[2].TrackingState = TrackingState.Added;
            order1.OrderDetails[3].TrackingState = TrackingState.Deleted;

            var order2 = northwind.Orders[2];
            order2.Customer.TrackingState = TrackingState.Modified;
            order2.OrderDetails[0].TrackingState = TrackingState.Modified;
            order2.OrderDetails[1].TrackingState = TrackingState.Added;
            order2.OrderDetails[2].TrackingState = TrackingState.Deleted;

            // Act
            var orders = new List<Order> {order1, order2};
            orders.AcceptChanges();

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, order1.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, order1.Customer.TrackingState);
            Assert.IsFalse(order1.OrderDetails.Any(d => d.TrackingState != TrackingState.Unchanged));
            Assert.AreEqual(TrackingState.Unchanged, order2.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, order2.Customer.TrackingState);
            Assert.IsFalse(order2.OrderDetails.Any(d => d.TrackingState != TrackingState.Unchanged));
        }

        #endregion

        #region ManyToMany AcceptChanges Tests

        [Test]
        public void Accept_Changes_Should_Mark_Employee_With_Territories_Unchanged()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var employee = northwind.Employees[0];
            employee.TrackingState = TrackingState.Modified;
            employee.Territories[0].TrackingState = TrackingState.Modified;
            employee.Territories[1].TrackingState = TrackingState.Deleted;
            var territory = new Territory
            {
                TerritoryId = "75070",
                TerritoryDescription = "North Dallas",
                TrackingState = TrackingState.Added,
                Employees = new List<Employee> { employee }
            };
            employee.Territories.Add(territory);

            // Act
            employee.AcceptChanges();

            // Assert
            IEnumerable<TrackingState> allStates = employee.GetTrackingStates();
            IEnumerable<TrackingState> unchangedStates = employee.GetTrackingStates(TrackingState.Unchanged);
            Assert.AreEqual(allStates.Count(), unchangedStates.Count());
        }

        [Test]
        public void Accept_Changes_Should_Remove_ModifiedProperties_From_Employee_With_Territories()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var employee = northwind.Employees[0];
            employee.TrackingState = TrackingState.Modified;
            employee.ModifiedProperties = new List<string> { "LastName" };
            employee.Territories[0].ModifiedProperties = new List<string> { "TerritoryDescription" };

            // Act
            employee.AcceptChanges();

            // Assert
            IEnumerable<IEnumerable<string>> modifiedProps = employee.GetModifiedProperties();
            Assert.IsFalse(modifiedProps.Any(p => p != null));
        }

        [Test]
        public void Accept_Changes_Should_Remove_Deleted_Territories_From_Employee()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var employee = northwind.Employees[0];
            employee.TrackingState = TrackingState.Modified;
            employee.Territories[0].TrackingState = TrackingState.Modified;
            employee.Territories[1].TrackingState = TrackingState.Deleted;
            var territory = new Territory
            {
                TerritoryId = "75070",
                TerritoryDescription = "North Dallas",
                TrackingState = TrackingState.Added,
                Employees = new List<Employee> { employee }
            };
            employee.Territories.Add(territory);

            // Act
            employee.AcceptChanges();

            // Assert
            Assert.AreEqual(3, employee.Territories.Count);
        }

        #endregion

        #region ManyToOne AcceptChanges Tests

        [Test]
        public void Accept_Changes_Should_Mark_Order_With_Modified_Customer_Unchanged()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var order = northwind.Orders[0];
            order.TrackingState = TrackingState.Modified;
            order.Customer.TrackingState = TrackingState.Modified;

            // Act
            order.AcceptChanges();

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, order.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, order.Customer.TrackingState);
        }

        [Test]
        public void Accept_Changes_Should_Mark_Order_With_Added_Customer_Unchanged()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var order = northwind.Orders[0];
            order.TrackingState = TrackingState.Modified;
            order.Customer.TrackingState = TrackingState.Added;

            // Act
            order.AcceptChanges();

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, order.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, order.Customer.TrackingState);
        }

        [Test]
        public void Accept_Changes_Should_Mark_Order_With_Deleted_Customer_Unchanged()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var order = northwind.Orders[0];
            order.TrackingState = TrackingState.Modified;
            order.Customer.TrackingState = TrackingState.Deleted;

            // Act
            order.AcceptChanges();

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, order.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, order.Customer.TrackingState);
        }

        [Test]
        public void Accept_Changes_Should_Remove_ModifiedProperties_From_Order_With_Customer()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var order = northwind.Orders[0];
            order.TrackingState = TrackingState.Modified;
            order.ModifiedProperties = new List<string> { "OrderDate" };
            order.Customer.TrackingState = TrackingState.Modified;
            order.Customer.ModifiedProperties = new List<string> { "CustomerName" };

            // Act
            order.AcceptChanges();

            // Assert
            Assert.IsFalse(order.GetModifiedProperties().Any(p => p != null));
            Assert.IsFalse(order.Customer.GetModifiedProperties().Any(p => p != null));
        }

        [Test]
        public void Accept_Changes_Should_Not_Remove_Deleted_Customer_From_Order()
        {
            // NOTE: Reference entities cannot be deleted from a related entity, because
            // of possible referential constraints, but they can be deleted independently.

            // Arrange
            var northwind = new MockNorthwind();
            var order = northwind.Orders[0];
            order.TrackingState = TrackingState.Modified;
            order.Customer.TrackingState = TrackingState.Deleted;

            // Act
            order.AcceptChanges();

            // Assert
            Assert.IsNotNull(order.Customer);
            Assert.AreEqual(TrackingState.Unchanged, order.Customer.TrackingState);
        }

        #endregion

        #region OneToOne AcceptChanges Tests

        [Test]
        public void Accept_Changes_Should_Mark_Customer_With_Modified_CustomerSetting_Unchanged()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var customer = northwind.Customers[0];
            customer.TrackingState = TrackingState.Modified;
            customer.CustomerSetting = new CustomerSetting
                {TrackingState = TrackingState.Modified};

            // Act
            customer.AcceptChanges();

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, customer.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, customer.CustomerSetting.TrackingState);
        }

        [Test]
        public void Accept_Changes_Should_Mark_Customer_With_Added_CustomerSetting_Unchanged()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var customer = northwind.Customers[0];
            customer.TrackingState = TrackingState.Modified;
            customer.CustomerSetting = new CustomerSetting
                { TrackingState = TrackingState.Added };

            // Act
            customer.AcceptChanges();

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, customer.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, customer.CustomerSetting.TrackingState);
        }

        [Test]
        public void Accept_Changes_Should_Mark_Customer_With_Deleted_CustomerSetting_Unchanged()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var customer = northwind.Customers[0];
            customer.TrackingState = TrackingState.Modified;
            customer.CustomerSetting = new CustomerSetting
            { TrackingState = TrackingState.Deleted };

            // Act
            customer.AcceptChanges();

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, customer.TrackingState);
            Assert.AreEqual(TrackingState.Unchanged, customer.CustomerSetting.TrackingState);
        }

        [Test]
        public void Accept_Changes_Should_Remove_ModifiedProperties_From_Customer_With_CustomerSetting()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var customer = northwind.Customers[0];
            customer.TrackingState = TrackingState.Modified;
            customer.ModifiedProperties = new List<string> { "CustomerName" };
            customer.CustomerSetting = new CustomerSetting
                { TrackingState = TrackingState.Modified };
            customer.CustomerSetting.ModifiedProperties = new List<string> { "Setting" };

            // Act
            customer.AcceptChanges();

            // Assert
            Assert.IsFalse(customer.GetModifiedProperties().Any(p => p != null));
            Assert.IsFalse(customer.CustomerSetting.GetModifiedProperties().Any(p => p != null));
        }

        [Test]
        public void Accept_Changes_Should_Not_Remove_Deleted_CustomerSetting_From_Customer()
        {
            // NOTE: Reference entities cannot be deleted from a related entity, because
            // of possible referential constraints, but they can be deleted independently.

            // Arrange
            var northwind = new MockNorthwind();
            var customer = northwind.Customers[0];
            customer.TrackingState = TrackingState.Modified;
            customer.CustomerSetting = new CustomerSetting { TrackingState = TrackingState.Deleted };

            // Act
            customer.AcceptChanges();

            // Assert
            Assert.IsNotNull(customer.CustomerSetting);
            Assert.AreEqual(TrackingState.Unchanged, customer.CustomerSetting.TrackingState);
        }

        #endregion
    }
}
