using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrackableEntities.Client.Tests.Entities.FamilyModels;
using TrackableEntities.Client.Tests.Entities.Mocks;
using TrackableEntities.Client.Tests.Entities.NorthwindModels;
using TrackableEntities.Common;
using Xunit;

namespace TrackableEntities.Client.Tests.Extensions
{
    public class ChangeTrackingExtensionsTests
    {
        #region MergeChanges Tests

        #region MergeChanges: Single Entity
        
        [Fact]
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
            Assert.Equal(updatedOrder.CustomerId, origOrder.CustomerId);
            Assert.Equal(updatedOrder.Customer.CustomerId, origOrder.Customer.CustomerId);
            Assert.Equal(updatedOrder.OrderDate, origOrder.OrderDate);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Modified, origTrackingState);
            Assert.Equal(TrackingState.Unchanged, origOrder.TrackingState);
        }

        [Fact]
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
            Assert.Contains("CustomerId", origModifiedProps);
            Assert.Null(origOrder.ModifiedProperties);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, origTrackingState);
            Assert.Equal(TrackingState.Modified, origOrder.Customer.TrackingState);
            Assert.Contains("CustomerName", origOrder.Customer.ModifiedProperties);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Added, origTrackingState);
            Assert.Equal(TrackingState.Unchanged, origOrder.TrackingState);
        }

        [Fact]
        public void MergeChanges_Should_Set_TrackingState_To_Unchanged_For_Added_Order_With_Null_Customer()
        {
            // Arrange
            var database = new MockNorthwind();
            var origOrder = new PriorityOrder
            {
                PriorityPlan = "Silver",
                OrderDate = DateTime.Parse("1996-07-04"),
                CustomerId = "ALFKI"
            };
            var changeTracker = new ChangeTrackingCollection<Order>(true) { origOrder };
            var addedOrder = AddOrders(database, origOrder)[0];

            // Act
            changeTracker.MergeChanges(addedOrder);

            // Assert
            Assert.Equal(addedOrder.CustomerId, origOrder.CustomerId);
            Assert.Equal(addedOrder.Customer.CustomerId, origOrder.Customer.CustomerId);
            Assert.Equal(addedOrder.OrderDate, origOrder.OrderDate);
        }

        [Fact]
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
            Assert.Equal(TrackingState.Unchanged, origTrackingState);
            Assert.Equal(TrackingState.Modified, origOrder.Customer.TrackingState);
            Assert.Contains("CustomerName", origOrder.Customer.ModifiedProperties);
        }

        #endregion

        #region MergeChanges: Multiple Entities

        [Fact]
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
            Assert.Equal(updatedOrders[0].CustomerId, origOrder1.CustomerId);
            Assert.Equal(updatedOrders[0].Customer.CustomerId, origOrder1.Customer.CustomerId);
            Assert.Equal(updatedOrders[0].OrderDate, origOrder1.OrderDate);
            Assert.Equal(updatedOrders[1].CustomerId, origOrder2.CustomerId);
            Assert.Equal(updatedOrders[1].Customer.CustomerId, origOrder2.Customer.CustomerId);
            Assert.Equal(updatedOrders[1].OrderDate, origOrder2.OrderDate);
        }

        #endregion

        #region MergeChanges: One-to-Many

        [Fact]
        public void MergeChanges_Should_Merge_Updates_For_Unchanged_Order_With_Changed_OrderDetails()
        {
            // Arrange
            var database = new MockNorthwind();
            var origOrder = database.Orders[0];
            var product = database.Products.Single(p => p.ProductId == 14);
            origOrder.OrderDetails.Add(new OrderDetail { ProductId = 14, OrderId = 10249, Quantity = 9, UnitPrice = 18.6000M, Product = product });
            var unchangedDetail = origOrder.OrderDetails[0];
            var modifiedDetail = origOrder.OrderDetails[1];
            var deletedDetail = origOrder.OrderDetails[3];
            var changeTracker = new ChangeTrackingCollection<Order>(origOrder);
            origOrder.OrderDetails[1].Product.ProductName = "xxx";
            origOrder.OrderDetails[2].Quantity++;
            origOrder.OrderDetails[2].ProductId = 1;
            var newUnitPrice = origOrder.OrderDetails[2].UnitPrice + 1;
            origOrder.OrderDetails.RemoveAt(3);
            var addedDetail = new OrderDetail {ProductId = 51, OrderId = 10249, Quantity = 40, UnitPrice = 42.4000M};
            origOrder.OrderDetails.Add(addedDetail);

            var changes = changeTracker.GetChanges();
            var updatedOrder = UpdateOrdersWithDetails(database, changes)[0];

            // Act
            changeTracker.MergeChanges(updatedOrder);

            // Assert
            Assert.Contains(unchangedDetail, origOrder.OrderDetails); // Unchanged present
            Assert.Equal("xxx", origOrder.OrderDetails[1].Product.ProductName); // Prod name updated
            Assert.Equal(updatedOrder.OrderDetails[1].ProductId, origOrder.OrderDetails[2].Product.ProductId); // Changed Product set
            Assert.Equal(newUnitPrice, origOrder.OrderDetails[2].UnitPrice); // Db-generated value set
            Assert.Equal(updatedOrder.OrderDetails[2].Product.ProductId, origOrder.OrderDetails[3].Product.ProductId); // Added detail Product set
            Assert.True(origOrder.OrderDetails.All(d => d.TrackingState == TrackingState.Unchanged)); // Details unchanged
            Assert.Same(addedDetail, origOrder.OrderDetails.Single(d => d.ProductId == 51)); // Ref equality
            Assert.Same(modifiedDetail, origOrder.OrderDetails.Single(d => d.ProductId == 42)); // Ref equality
            Assert.DoesNotContain(deletedDetail, origOrder.OrderDetails); // Detail deleted
            ICollection cachedDeletes = ((ITrackingCollection)origOrder.OrderDetails).GetChanges(true);
            Assert.Empty(cachedDeletes); // Cached deletes have been removed
        }

        #endregion

        #region MergeChanges: Many-to-One

        [Fact]
        public void MergeChanges_Should_Set_Modified_Order_With_Unchanged_Customer()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var date = order.OrderDate = order.OrderDate.AddDays(1);
            var origState = order.TrackingState;
            var changes = changeTracker.GetChanges();
            var changedOrder = changes[0];
            var updatedOrder = changes[0].Clone<Order>();
            updatedOrder.AcceptChanges();

            // Act
            changeTracker.MergeChanges(updatedOrder);

            // Assert
            Assert.Equal(date, order.OrderDate); // Changed order date is preserved
            Assert.Null(changedOrder.Customer); // GetChanges sets unchanges ref props to null
            Assert.NotNull(order.Customer); // Unchanged order.Customer is not overwritten
            Assert.Equal(TrackingState.Modified, origState); // Changed state is modified
            Assert.Equal(TrackingState.Unchanged, order.TrackingState); // State set to unchanged
        }

        [Fact]
        public void MergeChanges_Should_Set_Order_With_Modified_Customer_With_Db_Generated_Values()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var customer = order.Customer;
            var name = customer.CustomerName = "xxx";
            var changes = changeTracker.GetChanges();
            var updatedOrder = changes[0].Clone<Order>();
            var data = updatedOrder.Customer.Data = "yyy"; // simulate db-generated values
            updatedOrder.AcceptChanges();

            // Act
            changeTracker.MergeChanges(updatedOrder);

            // Assert
            Assert.Equal(name, order.Customer.CustomerName);
            Assert.Equal(data, order.Customer.Data);
        }

        #endregion

        #region MergeChanges: One-to-One

        [Fact]
        public void MergeChanges_Should_Set_Modified_Customer_With_Unchanged_Setting()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var customer = order.Customer;
            customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Customer = customer,
                Setting = "Setting1"
            };
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var name = customer.CustomerName = "xxx";
            var origState = customer.TrackingState;
            var changes = changeTracker.GetChanges();
            var changedCustomer = changes[0].Customer;
            var updatedOrder = changes[0].Clone<Order>();
            updatedOrder.AcceptChanges();

            // Act
            changeTracker.MergeChanges(updatedOrder);

            // Assert
            Assert.Equal(name, customer.CustomerName); // Changed customer name is preserved
            Assert.Null(changedCustomer.CustomerSetting); // GetChanges sets unchanges ref props to null
            Assert.NotNull(order.Customer.CustomerSetting); // Unchanged ref props not overwritten
            Assert.Equal(TrackingState.Modified, origState); // Changed state is modified
            Assert.Equal(TrackingState.Unchanged, customer.TrackingState); // State set to unchanged
        }

        [Fact]
        public void MergeChanges_Should_Set_Unchanged_Customer_With_Modified_Setting()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var customer = order.Customer;
            customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Customer = customer,
                Setting = "Setting1"
            };
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var setting = customer.CustomerSetting.Setting = "xxx";
            var origState = customer.CustomerSetting.TrackingState;
            var changes = changeTracker.GetChanges();
            var changedCustomer = changes[0].Customer;
            var updatedOrder = changes[0].Clone<Order>();
            updatedOrder.AcceptChanges();

            // Act
            changeTracker.MergeChanges(updatedOrder);

            // Assert
            Assert.Equal(setting, customer.CustomerSetting.Setting); // Changed customer setting is preserved
            Assert.Equal(TrackingState.Modified, origState); // Changed state is modified
            Assert.Equal(TrackingState.Unchanged, customer.TrackingState); // State set to unchanged
        }
        
        #endregion

        #region MergeChanges: Many-to-Many

        [Fact]
        public void MergeChanges_Should_Set_Modified_Employee_With_Unchanged_Territories()
        {
            // Arrange
            var database = new MockNorthwind();
            var employee = database.Employees[0];
            var unchangedterritory = employee.Territories[0];
            var modifiedterritory = employee.Territories[1];
            var deletedterritory = employee.Territories[2];
            var addedExistingTerritory = database.Territories[3];
            var addedNewTerritory = new Territory
            {
                TerritoryId = "91360",
                TerritoryDescription = "SouthernCalifornia",
                Employees = new ChangeTrackingCollection<Employee> {employee}
            };
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            var name = employee.LastName = "xxx";
            var origState = employee.TrackingState;
            var changes = changeTracker.GetChanges();
            var changedTerritories = changes[0].Territories;
            var updatedEmployee = changes[0].Clone<Employee>();
            updatedEmployee.AcceptChanges();

            // Act
            changeTracker.MergeChanges(updatedEmployee);

            // Assert
            Assert.Equal(name, employee.LastName); // Changed prop is preserved
            Assert.Empty(changedTerritories); // GetChanges sets unchanges ref props to null
            Assert.Equal(3, employee.Territories.Count); // Unchanged items preserved
        }

        [Fact]
        public void MergeChanges_Should_Merge_Updates_For_Unchanged_Employee_With_Changed_Territories()
        {
            // Arrange
            var database = new MockNorthwind();
            var employee = database.Employees[0];
            var unchangedTerritory = employee.Territories[0];
            var modifiedTerritory = employee.Territories[1];
            var deletedTerritory = employee.Territories[2];
            var addedExistingTerritory = database.Territories[3];
            var area1 = new Area { AreaId = 1, AreaName = "Northern", Territories = new ChangeTrackingCollection<Territory> { unchangedTerritory } };
            var area2 = new Area { AreaId = 2, AreaName = "Southern", Territories = new ChangeTrackingCollection<Territory> { modifiedTerritory } };
            var area3 = new Area { AreaId = 3, AreaName = "Eastern", Territories = new ChangeTrackingCollection<Territory> { deletedTerritory } };
            var area4 = new Area { AreaId = 4, AreaName = "Western", Territories = new ChangeTrackingCollection<Territory> { addedExistingTerritory } };
            var addedNewTerritory = new Territory
            {
                TerritoryId = "91360",
                TerritoryDescription = "SouthernCalifornia",
                Employees = new ChangeTrackingCollection<Employee> { employee },
                AreaId = 5,
                TrackingState = TrackingState.Added // Must explicitly mark as added
            };
            unchangedTerritory.Area = area1;
            unchangedTerritory.AreaId = area1.AreaId;
            modifiedTerritory.Area = area2;
            modifiedTerritory.AreaId = area2.AreaId;
            deletedTerritory.Area = area3;
            deletedTerritory.AreaId = area3.AreaId;
            addedExistingTerritory.Area = area4;
            addedExistingTerritory.AreaId = area4.AreaId;

            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            modifiedTerritory.TerritoryDescription = "xxx"; // mod prop
            modifiedTerritory.AreaId = 6; // mod FK prop
            employee.Territories.Remove(deletedTerritory); // should mark area deleted
            employee.Territories.Add(addedExistingTerritory); // should keep area unchanged
            employee.Territories.Add(addedNewTerritory); // should keep territory added
            addedExistingTerritory.Area.AreaName = "zzz"; // should mark area modified

            var changes = changeTracker.GetChanges(); // should exclude unchanged territory
            var updatedEmployee = UpdateEmployeesWithTerritories(changes)[0]; // rel entities, db-gen vals

            // Act
            changeTracker.MergeChanges(updatedEmployee);

            // Assert
            Assert.Contains(unchangedTerritory, employee.Territories); // Unchanged present
            Assert.Equal("zzz", addedExistingTerritory.Area.AreaName); // Area name updated
            Assert.Equal("yyy", addedNewTerritory.Data); // Db-generated value set
            Assert.Equal(addedNewTerritory.AreaId, addedNewTerritory.Area.AreaId); // Added territory Area set
            Assert.Equal(modifiedTerritory.AreaId, modifiedTerritory.Area.AreaId); // Modified territory Area set
            Assert.Same(addedExistingTerritory, employee.Territories.Single(t => t.TerritoryId == "02116")); // Ref equality
            Assert.Same(addedNewTerritory, employee.Territories.Single(t => t.TerritoryId == "91360")); // Ref equality
            Assert.Same(modifiedTerritory, employee.Territories.Single(t => t.TerritoryId == "01730")); // Ref equality
            Assert.DoesNotContain(deletedTerritory, employee.Territories); // Detail deleted
            ICollection cachedDeletes = ((ITrackingCollection)employee.Territories).GetChanges(true);
            Assert.Empty(cachedDeletes); // Cached deletes have been removed
        }

        #endregion

        #region MergeChanges Exceptions

        [Fact]
        public void MergeChanges_On_Equatable_Customer_Should_Not_Throw_ArgumentException()
        {
            // Arrange
            var customer = new Customer { CustomerId = "ALFKI" };
            var changeTracker = new ChangeTrackingCollection<Customer>(customer);

            // Act
            changeTracker.MergeChanges();

            // Assert
            Assert.True(true);
        }

        [Fact]
        public void MergeChanges_On_Equatable_PromotionalProduct_Should_Not_Throw_ArgumentException()
        {
            // Arrange
            var product = new MockNorthwind().Products.OfType<PromotionalProduct>().First();
            var changeTracker = new ChangeTrackingCollection<PromotionalProduct>(product);
            var clonedProduct = product.Clone();

            // Act
            changeTracker.MergeChanges(clonedProduct);

            // Assert
            Assert.True(true);
        }

        #endregion

        #endregion

        #region HasChanges Tests

        #region HasChanges: One-to-Many

        [Fact]
        public void HasChanges_Unchanged_Parent_Should_Return_False()
        {
            // Arrange
            var parent = new Parent("Parent");
            var changeTracker = new ChangeTrackingCollection<Parent>(parent);

            // Act
            bool hasChanges = parent.HasChanges();

            // Assert
            Assert.False(hasChanges);
        }

        [Fact]
        public void HasChanges_Modified_Parent_Should_Return_True()
        {
            // Arrange
            var parent = new Parent("Parent");
            var changeTracker = new ChangeTrackingCollection<Parent>(parent);
            parent.Name += "_Changed";

            // Act
            bool hasChanges = parent.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Added_Parent_Should_Return_True()
        {
            // Arrange
            var parent = new Parent("Parent");
            var changeTracker = new ChangeTrackingCollection<Parent>(true);
            changeTracker.Add(parent);

            // Act
            bool hasChanges = parent.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Deleted_Parent_Should_Return_True()
        {
            // Arrange
            var parent = new Parent("Parent");
            var changeTracker = new ChangeTrackingCollection<Parent>(parent);
            changeTracker.Remove(parent);

            // Act
            bool hasChanges = parent.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Parent_With_Unchanged_Children_Should_Return_False()
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

            // Act
            bool hasChanges = parent.HasChanges();

            // Assert
            Assert.False(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Parent_With_Modified_Child_Should_Return_True()
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
            bool hasChanges = parent.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Parent_With_Added_Child_Should_Return_True()
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
            parent.Children.Add(new Child("Child3"));

            // Act
            bool hasChanges = parent.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Parent_With_Removed_Child_Should_Return_True()
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
            parent.Children.RemoveAt(0);

            // Act
            bool hasChanges = parent.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Parent_With_Unchanged_Grandchildren_Should_Return_False()
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
                            }, 
                        new Child("Child2")
                    }
            };
            var changeTracker = new ChangeTrackingCollection<Parent>(parent);

            // Act
            bool hasChanges = parent.HasChanges();

            // Assert
            Assert.False(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Parent_With_Modified_Grandchild_Should_Return_True()
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
                            }, 
                        new Child("Child2")
                    }
            };
            var changeTracker = new ChangeTrackingCollection<Parent>(parent);
            parent.Children[0].Children[0].Name += "_Changed";

            // Act
            bool hasChanges = parent.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Parent_With_Added_Grandchild_Should_Return_True()
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
                            }, 
                        new Child("Child2")
                    }
            };
            var changeTracker = new ChangeTrackingCollection<Parent>(parent);
            parent.Children[0].Children.Add(new Child("Grandchild3"));

            // Act
            bool hasChanges = parent.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Parent_With_Removed_Grandchild_Should_Return_True()
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
                            }, 
                        new Child("Child2")
                    }
            };
            var changeTracker = new ChangeTrackingCollection<Parent>(parent);
            parent.Children[0].Children.RemoveAt(0);

            // Act
            bool hasChanges = parent.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Order_OrderDetail_With_Modified_Product_Should_Return_True()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.OrderDetails[0].Product.ProductName = "xxx";

            // Act
            bool hasChanges = order.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Order_OrderDetail_With_Product_Set_To_Null_Should_Return_False()
        {
            // NOTE: Setting OrderDetail.Product to null is not considered a change,
            // because it will not result in any updated entities.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.OrderDetails[0].Product = null;

            // Act
            bool hasChanges = order.HasChanges();

            // Assert
            Assert.False(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Order_OrderDetail_With_Product_Set_To_Unchanged_Product_Should_Return_False()
        {
            // NOTE: Setting null OrderDetail.Product to a product is not considered a change,
            // because it will not result in any updated entities.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var product = database.Products[0];
            order.OrderDetails[0].Product = product;

            // Act
            bool hasChanges = order.HasChanges();

            // Assert
            Assert.False(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Order_OrderDetail_Set_Product_To_Added_Product_Should_Return_True()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var product = database.Products[0];
            product.TrackingState = TrackingState.Added;
            order.OrderDetails[0].Product = product;

            // Act
            bool hasChanges = order.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Order_OrderDetail_Set_Product_To_Deleted_Product_Should_Return_True()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            var product = database.Products[0];
            product.TrackingState = TrackingState.Deleted;
            order.OrderDetails[0].Product = product;

            // Act
            bool hasChanges = order.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        #endregion

        #region HasChanges: Many-to-One

        [Fact]
        public void HasChanges_Unchanged_Order_Customer_With_Unchanged_Territory_Should_Return_False()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var territory = database.Territories[0];
            var employee = database.Employees[0];
            territory.Employees = new ChangeTrackingCollection<Employee> { employee };
            order.Customer.Territory = territory;
            var changeTracker = new ChangeTrackingCollection<Order>(order);

            // Act
            bool hasChanges = order.HasChanges();

            // Assert
            Assert.False(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Order_Customer_With_Territory_Set_To_Null_Should_Return_False()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var territory = database.Territories[0];
            var employee = database.Employees[0];
            territory.Employees = new ChangeTrackingCollection<Employee> { employee };
            order.Customer.Territory = territory;
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.Customer.Territory = null;

            // Act
            bool hasChanges = order.HasChanges();

            // Assert
            Assert.False(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Order_Customer_With_Territory_Set_To_Unchanged_Territory_Should_Return_False()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var territory = database.Territories[0];
            var employee = database.Employees[0];
            territory.Employees = new ChangeTrackingCollection<Employee> { employee };
            order.Customer.Territory = territory;
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.Customer.Territory = database.Territories[1];

            // Act
            bool hasChanges = order.HasChanges();

            // Assert
            Assert.False(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Order_Customer_With_Modified_Territory_Should_Return_True()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var territory = database.Territories[0];
            var employee = database.Employees[0];
            territory.Employees = new ChangeTrackingCollection<Employee> { employee };
            order.Customer.Territory = territory;
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            territory.Data = "xxx";

            // Act
            bool hasChanges = order.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Order_Customer_Territory_With_Modified_Employee_Should_Return_True()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var territory = database.Territories[0];
            var employee = database.Employees[0];
            territory.Employees = new ChangeTrackingCollection<Employee> { employee };
            order.Customer.Territory = territory;
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            territory.Employees[0].LastName = "xxx";

            // Act
            bool hasChanges = order.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Order_Customer_Territory_With_Added_Employee_Should_Return_True()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var territory = database.Territories[0];
            var employee = database.Employees[0];
            territory.Employees = new ChangeTrackingCollection<Employee> { employee };
            order.Customer.Territory = territory;
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            territory.Employees.Add(database.Employees[1]);

            // Act
            bool hasChanges = order.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Order_Customer_Territory_With_Removed_Employee_Should_Return_True()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var territory = database.Territories[0];
            var employee = database.Employees[0];
            territory.Employees = new ChangeTrackingCollection<Employee> { employee };
            order.Customer.Territory = territory;
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            territory.Employees.Remove(employee);

            // Act
            bool hasChanges = order.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        #endregion

        #region HasChanges: One-to-One

        [Fact]
        public void HasChanges_Unchanged_Order_Customer_With_Unchanged_Setting_Should_Return_False()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            order.Customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = order.CustomerId,
                Customer = order.Customer,
                Setting = "Setting1"
            };
            var changeTracker = new ChangeTrackingCollection<Order>(order);

            // Act
            bool hasChanges = order.HasChanges();

            // Assert
            Assert.False(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Order_Customer_With_Modified_Setting_Should_Return_True()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            order.Customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = order.CustomerId,
                Customer = order.Customer,
                Setting = "Setting1"
            };
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.Customer.CustomerSetting.Setting = "xxx";

            // Act
            bool hasChanges = order.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Order_Customer_With_Setting_Set_To_Null_Should_Return_False()
        {
            // NOTE: Setting ref prop to false will not result in updated entities.

            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            order.Customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = order.CustomerId,
                Customer = order.Customer,
                Setting = "Setting1"
            };
            var changeTracker = new ChangeTrackingCollection<Order>(order);
            order.Customer.CustomerSetting = null;

            // Act
            bool hasChanges = order.HasChanges();

            // Assert
            Assert.False(hasChanges);
        }

        #endregion

        #region HasChanges: Many-to-Many

        [Fact]
        public void HasChanges_Unchanged_Employee_With_Unchanged_Territories_Should_Return_False()
        {
            // Arrange
            var database = new MockNorthwind();
            var employee = database.Employees[0];
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);

            // Act
            bool hasChanges = employee.HasChanges();

            // Assert
            Assert.False(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Employee_With_Modified_Territory_Should_Return_True()
        {
            // Arrange
            var database = new MockNorthwind();
            var employee = database.Employees[0];
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            employee.Territories[0].Data = "xxx";

            // Act
            bool hasChanges = employee.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Employee_With_Added_Territory_Should_Return_True()
        {
            // Arrange
            var database = new MockNorthwind();
            var employee = database.Employees[0];
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            employee.Territories.Add(database.Territories[4]);

            // Act
            bool hasChanges = employee.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        [Fact]
        public void HasChanges_Unchanged_Employee_With_Removed_Territory_Should_Return_True()
        {
            // Arrange
            var database = new MockNorthwind();
            var employee = database.Employees[0];
            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            employee.Territories.RemoveAt(0);

            // Act
            bool hasChanges = employee.HasChanges();

            // Assert
            Assert.True(hasChanges);
        }

        #endregion

        #endregion

        #region Clone Tests

        [Fact]
        public void Clone_Of_ChangeTrackingCollection_Should_Create_Deep_Copy()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var order3 = database.Orders[3];
            var customer = order.Customer;
            var details = order.OrderDetails;
            var changeTracker = new ChangeTrackingCollection<Order>(order, order3);

            // Act
            var clonedChangeTracker = changeTracker.Clone();

            // Assert
            Assert.NotSame(order, clonedChangeTracker[0]);
            Assert.NotSame(customer, clonedChangeTracker[0].Customer);
            Assert.NotSame(details, clonedChangeTracker[0].OrderDetails);
            Assert.True(order.IsEquatable(clonedChangeTracker[0]));
            Assert.True(customer.IsEquatable(clonedChangeTracker[0].Customer));
            Assert.True(object.ReferenceEquals(order.Customer, order3.Customer));
            Assert.False(object.ReferenceEquals(order.Customer, clonedChangeTracker[0].Customer));
            Assert.True(object.ReferenceEquals(clonedChangeTracker[0].Customer, clonedChangeTracker[1].Customer));
        }

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

        private List<Employee> UpdateEmployeesWithTerritories(IEnumerable<Employee> changes)
        {
            var updatedEmployees = new List<Employee>();
            foreach (var origEmployee in changes)
            {
                // Simulate serialization
                var updatedEmployee = origEmployee.Clone<Employee>();

                // Simulate load related entities
                var addedNewTerritory = updatedEmployee.Territories.Single(t => t.TerritoryId == "91360");
                var modifiedTerritory = updatedEmployee.Territories.Single(t => t.TerritoryId == "01730");
                var area5 = new Area { AreaId = 5, AreaName = "Southwestern", 
                    Territories = new ChangeTrackingCollection<Territory> { addedNewTerritory }};
                var area6 = new Area { AreaId = 6, AreaName = "Central", 
                    Territories = new ChangeTrackingCollection<Territory> { modifiedTerritory }};
                addedNewTerritory.Area = area5;
                modifiedTerritory.Area = area6;

                // Simulate db-generated values
                addedNewTerritory.Data = "yyy";

                updatedEmployee.AcceptChanges();
                updatedEmployees.Add(updatedEmployee);
            }
            return updatedEmployees;
        }

        #endregion
    }
}
