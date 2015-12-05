using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrackableEntities.Client.Tests.Entities.Mocks;
using TrackableEntities.Client.Tests.Entities.NorthwindModels;
using Xunit;

namespace TrackableEntities.Client.Tests
{
    public class ChangeTrackingCollectionTests
    {
        #region Ctor: Tracking Enablement Tests

        [Fact]
        public void Tracking_Should_Be_Disabled_With_Default_Ctor()
        {
            // Arrange
            var database = new MockNorthwind();
            var product = database.Products[0];
            var changeTracker = new ChangeTrackingCollection<Product>();

            // Act
            changeTracker.Add(product);

            // Assert
            Assert.Equal(TrackingState.Unchanged, changeTracker[0].TrackingState);
        }

        [Fact]
        public void Tracking_Should_Be_Enabled_With_Enumerable_Ctor()
        {
            // Arrange
            var database = new MockNorthwind();
            var products = new List<Product> { database.Products[0] };
            var product = database.Products[1];

            // Act
            var changeTracker = new ChangeTrackingCollection<Product>(products);
            changeTracker.Add(product);

            // Assert
            Assert.Equal(TrackingState.Added, changeTracker[1].TrackingState);
        }

        [Fact]
        public void Tracking_Should_Be_Enabled_With_Array_Ctor()
        {
            // Arrange
            var database = new MockNorthwind();
            var product = database.Products[1];

            // Act
            var changeTracker = new ChangeTrackingCollection<Product>(database.Products[0]);
            changeTracker.Add(product);

            // Assert
            Assert.Equal(TrackingState.Added, changeTracker[1].TrackingState);
        }

        #endregion

        #region Ctor: Items Added State Tests

        [Fact]
        public void Items_Should_Be_Added_As_Unchanged_With_Enumerable_Ctor()
        {
            // Arrange
            var database = new MockNorthwind();
            var product = database.Products[0];
            var products = new List<Product> { product };

            // Act
            var changeTracker = new ChangeTrackingCollection<Product>(products);

            // Assert
            Assert.Equal(TrackingState.Unchanged, changeTracker[0].TrackingState);
        }

        [Fact]
        public void Items_Should_Be_Added_As_Unchanged_With_Array_Ctor()
        {
            // Arrange
            var database = new MockNorthwind();
            var product = database.Products[0];

            // Act
            var changeTracker = new ChangeTrackingCollection<Product>(product);

            // Assert
            Assert.Equal(TrackingState.Unchanged, changeTracker[0].TrackingState);
        }

        #endregion

        #region Added Items Tests

        [Fact]
        public void Added_Items_After_Tracking_Enabled_Should_Be_Marked_As_Added()
        {
            // Arrange
            var database = new MockNorthwind();
            var product = database.Products[0];
            var changeTracker = new ChangeTrackingCollection<Product>(true);

            // Act
            changeTracker.Add(product);

            // Assert
            Assert.Equal(TrackingState.Added, product.TrackingState);
        }

        [Fact]
        public void Added_Items_With_Enumerable_Ctor_Should_Be_Marked_As_Added()
        {
            // Arrange
            var database = new MockNorthwind();
            var products = new List<Product> { database.Products[0] };
            var changeTracker = new ChangeTrackingCollection<Product>(products);
            var product = new Product
                {
                    ProductId = 100,
                    ProductName = "Test Beverage",
                    CategoryId = 1,
                    Category = database.Categories[0],
                    UnitPrice = 10M
                };

            // Act
            changeTracker.Add(product);

            // Assert
            Assert.Equal(TrackingState.Added, product.TrackingState);
        }

        #endregion

        #region Added Items: Many-to-Many Tests

        [Fact]
        public void Added_Employee_After_Tracking_Enabled_Should_Not_Mark_Manually_Added_Territories_As_Unchanged()
        {
            // Arrange
            var database = new MockNorthwind();
            var employee = database.Employees[0];
            employee.Territories.ToList().ForEach(t => t.TrackingState = TrackingState.Added);
            var changeTracker = new ChangeTrackingCollection<Employee>(true);

            // Act
            changeTracker.Add(employee);

            // Assert
            Assert.Equal(TrackingState.Added, employee.TrackingState);
            Assert.True(employee.Territories.All(t => t.TrackingState == TrackingState.Added));
        }

        #endregion

        #region Modified Items Tests

        [Fact]
        public void Modified_Added_Items_Should_Be_Marked_As_Added()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>();
            changeTracker.Tracking = true;
            var product = new Product
            {
                ProductId = 100,
                ProductName = "Test Beverage",
                CategoryId = 1,
                Category = database.Categories[0],
                UnitPrice = 10M
            };

            // Act
            changeTracker.Add(product);
            product.UnitPrice++;

            // Assert
            Assert.Equal(TrackingState.Added, product.TrackingState);
        }

        [Fact]
        public void Modified_Added_Items_Should_Not_Add_ModifiedProperties()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>();
            changeTracker.Tracking = true;
            var product = new Product
            {
                ProductId = 100,
                ProductName = "Test Beverage",
                CategoryId = 1,
                Category = database.Categories[0],
                UnitPrice = 10M
            };

            // Act
            changeTracker.Add(product);
            product.UnitPrice++;

            // Assert
            Assert.True(product.ModifiedProperties == null
                || product.ModifiedProperties.Count == 0);
        }

        [Fact]
        public void Modified_Existing_Items_Should_Be_Marked_As_Modified()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>(database.Products[0]);
            var product = changeTracker[0];

            // Act
            product.UnitPrice++;

            // Assert
            Assert.Equal(TrackingState.Modified, product.TrackingState);
        }

        [Fact]
        public void Modified_Existing_Items_Should_Add_ModifiedProperty()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>(database.Products[0]);
            var product = changeTracker[0];

            // Act
            product.UnitPrice++;

            // Assert
            Assert.Contains("UnitPrice", product.ModifiedProperties);
        }

        [Fact]
        public void Modified_Existing_Excluded_Items_Should_Not_Be_Marked_As_Modified()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>(database.Products[0]);
            changeTracker.ExcludedProperties.Add("UnitPrice");
            var product = changeTracker[0];

            // Act
            product.UnitPrice++;

            // Assert
            Assert.Equal(TrackingState.Unchanged, product.TrackingState);
        }

        [Fact]
        public void Modified_Existing_Excluded_Items_Should_Not_Add_ModifiedProperty()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>(database.Products[0]);
            changeTracker.ExcludedProperties.Add("UnitPrice");
            var product = changeTracker[0];

            // Act
            product.UnitPrice++;

            // Assert
            Assert.True(product.ModifiedProperties == null
                || product.ModifiedProperties.Count == 0);
        }

        [Fact]
        public void Modified_Existing_Items_Should_Add_Multiples_ModifiedProperties()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>(database.Products[0]);
            var product = changeTracker[0];

            // Act
            product.UnitPrice++;
            product.Discontinued = true;
            product.ProductName = "xxxxxxxx";

            // Assert
            Assert.Contains("UnitPrice", product.ModifiedProperties);
            Assert.Contains("Discontinued", product.ModifiedProperties);
            Assert.Contains("ProductName", product.ModifiedProperties);
        }

        [Fact]
        public void Modified_Existing_Mixed_Items_Should_Not_Be_Marked_As_Modified()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>(database.Products[0]);
            changeTracker.ExcludedProperties.Add("UnitPrice");
            var product = changeTracker[0];

            // Act
            product.UnitPrice++;
            product.ProductName = "xxxxxxxx";

            // Assert
            Assert.Equal(TrackingState.Modified, product.TrackingState);
            Assert.Contains("ProductName", product.ModifiedProperties);
            Assert.DoesNotContain("UnitPrice", product.ModifiedProperties);
        }

        #endregion

        #region Removed Items Tests

        [Fact]
        public void Removed_Added_Items_Should_Be_Marked_As_Unchanged()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>();
            changeTracker.Tracking = true;
            var product = new Product
            {
                ProductId = 100,
                ProductName = "Test Beverage",
                CategoryId = 1,
                Category = database.Categories[0],
                UnitPrice = 10M
            };

            // Act
            changeTracker.Add(product);
            product.UnitPrice++;
            changeTracker.Remove(product);

            // Assert
            Assert.Equal(TrackingState.Unchanged, product.TrackingState);
        }

        [Fact]
        public void Removed_Added_Items_Should_Not_Have_ModifiedProperties()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>();
            changeTracker.Tracking = true;
            var product = new Product
            {
                ProductId = 100,
                ProductName = "Test Beverage",
                CategoryId = 1,
                Category = database.Categories[0],
                UnitPrice = 10M
            };

            // Act
            changeTracker.Add(product);
            changeTracker.Remove(product);

            // Assert
            Assert.True(product.ModifiedProperties == null
                || product.ModifiedProperties.Count == 0);
        }

        [Fact]
        public void Removed_Added_Modified_Items_Should_Not_Have_ModifiedProperties()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>(true);
            var product = new Product
            {
                ProductId = 100,
                ProductName = "Test Beverage",
                CategoryId = 1,
                Category = database.Categories[0],
                UnitPrice = 10M
            };

            // Act
            changeTracker.Add(product);
            product.UnitPrice++;
            changeTracker.Remove(product);

            // Assert
            Assert.True(product.ModifiedProperties == null
                || product.ModifiedProperties.Count == 0);
        }

        [Fact]
        public void Removed_Existing_Unchanged_Items_Should_Be_Marked_As_Deleted()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>(database.Products[0]);
            var product = changeTracker[0];

            // Act
            changeTracker.Remove(product);

            // Assert
            Assert.Equal(TrackingState.Deleted, product.TrackingState);
        }

        [Fact]
        public void Removed_Existing_Modified_Items_Should_Be_Marked_As_Deleted()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>(database.Products[0]);
            var product = changeTracker[0];
            product.UnitPrice++;

            // Act
            changeTracker.Remove(product);

            // Assert
            Assert.Equal(TrackingState.Deleted, product.TrackingState);
        }

        [Fact]
        public void Removed_Existing_Modified_Items_Should_Not_Have_ModifiedProperties()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>(database.Products[0]);
            var product = changeTracker[0];
            product.UnitPrice++;

            // Act
            changeTracker.Remove(product);

            // Assert
            Assert.True(product.ModifiedProperties == null
                || product.ModifiedProperties.Count == 0);
        }

        [Fact]
        public void Removed_Items_Should_Disable_Change_Tracking_On_Entity()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];

            var changeTracker = new ChangeTrackingCollection<Order>(order);
            changeTracker.Remove(order);
            order.TrackingState = TrackingState.Unchanged;

            // Act
            order.OrderDate = order.OrderDate.AddDays(1);

            // Assert
            Assert.Equal(TrackingState.Unchanged, order.TrackingState);
        }

        [Fact]
        public void Removed_Items_Should_Disable_Change_Tracking_On_Related_Entities_OneToMany()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var detail = order.OrderDetails[0];

            var changeTracker = new ChangeTrackingCollection<Order>(order);
            changeTracker.Remove(order);
            detail.TrackingState = TrackingState.Unchanged;

            // Act
            detail.Quantity++;

            // Assert
            Assert.Equal(TrackingState.Unchanged, detail.TrackingState);
        }

        [Fact]
        public void Removed_Items_Should_NOT_Disable_Change_Tracking_On_Related_Entities_ManyToOne()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var customer = order.Customer;

            var changeTracker = new ChangeTrackingCollection<Order>(order);
            changeTracker.Remove(order);

            // Act
            customer.CustomerName = "XXX";

            // Assert
            Assert.Equal(TrackingState.Modified, customer.TrackingState);
        }

        [Fact]
        public void Removed_Items_Should_NOT_Disable_Change_Tracking_On_Related_Entities_OneToOne()
        {
            // Arrange
            var database = new MockNorthwind();
            var customer = database.Orders[0].Customer;
            var setting = customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Customer = customer,
                Setting = "Setting1"
            };

            var changeTracker = new ChangeTrackingCollection<Customer>(customer);
            changeTracker.Remove(customer);

            // Act
            setting.Setting = "XXX";

            // Assert
            Assert.Equal(TrackingState.Modified, setting.TrackingState);
        }

        [Fact]
        public void Removed_Items_Should_NOT_Disable_Change_Tracking_On_Related_Entities_ManyToMany()
        {
            // Arrange
            var database = new MockNorthwind();
            var employee = database.Employees[0];
            var territory = employee.Territories[0];

            var changeTracker = new ChangeTrackingCollection<Employee>(employee);
            changeTracker.Remove(employee);

            // Act
            territory.TerritoryDescription = "XXX";

            // Assert
            Assert.Equal(TrackingState.Modified, territory.TrackingState);
        }

        [Fact]
        public void Removed_Items_Should_NOT_Disable_Change_Tracking_On_Related_Entities_OneToMany_ToOne()
        {
            // Arrange
            var database = new MockNorthwind();
            var order = database.Orders[0];
            var product = order.OrderDetails[0].Product;

            var changeTracker = new ChangeTrackingCollection<Order>(order);
            changeTracker.Remove(order);

            // Act
            product.ProductName = "XXX";

            // Assert
            Assert.Equal(TrackingState.Modified, product.TrackingState);
        }

        #endregion

        #region GetChanges Test

        [Fact]
        public void GetChanges_Should_Return_Added_Items()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>(true);
            var product = database.Products[0];
            changeTracker.Add(product);

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.Equal(TrackingState.Added, changes.First().TrackingState);
        }

        [Fact]
        public void GetChanges_Should_Return_Modified_Items()
        {
            // Arrange
            var database = new MockNorthwind();
            var product = database.Products[0];
            var changeTracker = new ChangeTrackingCollection<Product>(product);
            product.UnitPrice++;

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.Equal(TrackingState.Modified, changes.First().TrackingState);
        }

        [Fact]
        public void GetChanges_Should_Return_Deleted_Items()
        {
            // Arrange
            var database = new MockNorthwind();
            var product = database.Products[0];
            var changeTracker = new ChangeTrackingCollection<Product>(product);
            changeTracker.Remove(product);

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.Equal(TrackingState.Deleted, changes.First().TrackingState);
        }

        [Fact]
        public void GetChanges_Should_Return_Added_Modified_Deleted_Items()
        {
            // Arrange
            var database = new MockNorthwind();
            var addedProduct = database.Products[0];
            var updatedProduct = database.Products[1];
            var deletedProduct = database.Products[2];

            var changeTracker = new ChangeTrackingCollection<Product>(updatedProduct, deletedProduct);
            
            changeTracker.Add(addedProduct);
            updatedProduct.UnitPrice++;
            changeTracker.Remove(deletedProduct);

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.Equal(TrackingState.Added, changes.ElementAt(1).TrackingState);
            Assert.Equal(TrackingState.Modified, changes.ElementAt(0).TrackingState);
            Assert.Equal(TrackingState.Deleted, changes.ElementAt(2).TrackingState);
        }

        #endregion

        #region EntityChanged Event Tests

        [Fact]
        public void EntityChanged_Event_Should_Fire_When_Items_Added_Modified_Deleted()
        {
            // Arrange
            int changesCount = 0;

            var database = new MockNorthwind();
            var addedProduct = database.Products[0];
            var updatedProduct = database.Products[1];
            var deletedProduct = database.Products[2];
            var changeTracker = new ChangeTrackingCollection<Product>(updatedProduct, deletedProduct);
            changeTracker.EntityChanged += (s, e) => changesCount++;

            // Act
            changeTracker.Add(addedProduct);
            updatedProduct.UnitPrice++;
            changeTracker.Remove(deletedProduct);

            // Assert
            Assert.Equal(3, changesCount);
        }

        #endregion
    }
}
