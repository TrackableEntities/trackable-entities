using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TrackableEntities.Client.Tests.Entities.Mocks;
using TrackableEntities.Client.Tests.Entities.NorthwindModels;

namespace TrackableEntities.Client.Tests
{
    [TestFixture]
    public class ChangeTrackingCollectionTests
    {
        #region Ctor: Tracking Enablement Tests

        [Test]
        public void Tracking_Should_Be_Disabled_With_Default_Ctor()
        {
            // Arrange
            var database = new MockNorthwind();
            var product = database.Products[0];
            var changeTracker = new ChangeTrackingCollection<Product>();

            // Act
            changeTracker.Add(product);

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, changeTracker[0].TrackingState);
        }

        [Test]
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
            Assert.AreEqual(TrackingState.Added, changeTracker[1].TrackingState);
        }

        [Test]
        public void Tracking_Should_Be_Enabled_With_Array_Ctor()
        {
            // Arrange
            var database = new MockNorthwind();
            var product = database.Products[1];

            // Act
            var changeTracker = new ChangeTrackingCollection<Product>(database.Products[0]);
            changeTracker.Add(product);

            // Assert
            Assert.AreEqual(TrackingState.Added, changeTracker[1].TrackingState);
        }

        #endregion

        #region Ctor: Items Added State Tests

        [Test]
        public void Items_Should_Be_Added_As_Unchanged_With_Enumerable_Ctor()
        {
            // Arrange
            var database = new MockNorthwind();
            var product = database.Products[0];
            var products = new List<Product> { product };

            // Act
            var changeTracker = new ChangeTrackingCollection<Product>(products);

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, changeTracker[0].TrackingState);
        }

        [Test]
        public void Items_Should_Be_Added_As_Unchanged_With_Array_Ctor()
        {
            // Arrange
            var database = new MockNorthwind();
            var product = database.Products[0];

            // Act
            var changeTracker = new ChangeTrackingCollection<Product>(product);

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, changeTracker[0].TrackingState);
        }

        #endregion

        #region Added Items Tests

        [Test]
        public void Added_Items_After_Tracking_Enabled_Should_Be_Marked_As_Added()
        {
            // Arrange
            var database = new MockNorthwind();
            var product = database.Products[0];
            var changeTracker = new ChangeTrackingCollection<Product>(true);

            // Act
            changeTracker.Add(product);

            // Assert
            Assert.AreEqual(TrackingState.Added, product.TrackingState);
        }

        [Test]
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
            Assert.AreEqual(TrackingState.Added, product.TrackingState);
        }

        #endregion

        #region Added Items: Many-to-Many Tests

        [Test]
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
            Assert.AreEqual(TrackingState.Added, employee.TrackingState);
            Assert.IsTrue(employee.Territories.All(t => t.TrackingState == TrackingState.Added));
        }

        #endregion

        #region Modified Items Tests

        [Test]
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
            Assert.AreEqual(TrackingState.Added, product.TrackingState);
        }

        [Test]
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
            Assert.IsTrue(product.ModifiedProperties == null
                || product.ModifiedProperties.Count == 0);
        }

        [Test]
        public void Modified_Existing_Items_Should_Be_Marked_As_Modified()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>(database.Products[0]);
            var product = changeTracker[0];

            // Act
            product.UnitPrice++;

            // Assert
            Assert.AreEqual(TrackingState.Modified, product.TrackingState);
        }

        [Test]
        public void Modified_Existing_Items_Should_Add_ModifiedProperty()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>(database.Products[0]);
            var product = changeTracker[0];

            // Act
            product.UnitPrice++;

            // Assert
            Assert.Contains("UnitPrice", (ICollection)product.ModifiedProperties);
        }

        [Test]
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
            Assert.AreEqual(TrackingState.Unchanged, product.TrackingState);
        }

        [Test]
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
            Assert.IsTrue(product.ModifiedProperties == null
                || product.ModifiedProperties.Count == 0);
        }

        [Test]
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
            Assert.Contains("UnitPrice", (ICollection)product.ModifiedProperties);
            Assert.Contains("Discontinued", (ICollection)product.ModifiedProperties);
            Assert.Contains("ProductName", (ICollection)product.ModifiedProperties);
        }

        [Test]
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
            Assert.AreEqual(TrackingState.Modified, product.TrackingState);
            Assert.Contains("ProductName", (ICollection)product.ModifiedProperties);
            Assert.That(product.ModifiedProperties, Has.No.Member("UnitPrice"));
        }

        #endregion

        #region Removed Items Tests

        [Test]
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
            Assert.AreEqual(TrackingState.Unchanged, product.TrackingState);
        }

        [Test]
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
            Assert.IsTrue(product.ModifiedProperties == null
                || product.ModifiedProperties.Count == 0);
        }

        [Test]
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
            Assert.IsTrue(product.ModifiedProperties == null
                || product.ModifiedProperties.Count == 0);
        }

        [Test]
        public void Removed_Existing_Unchanged_Items_Should_Be_Marked_As_Deleted()
        {
            // Arrange
            var database = new MockNorthwind();
            var changeTracker = new ChangeTrackingCollection<Product>(database.Products[0]);
            var product = changeTracker[0];

            // Act
            changeTracker.Remove(product);

            // Assert
            Assert.AreEqual(TrackingState.Deleted, product.TrackingState);
        }

        [Test]
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
            Assert.AreEqual(TrackingState.Deleted, product.TrackingState);
        }

        [Test]
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
            Assert.IsTrue(product.ModifiedProperties == null
                || product.ModifiedProperties.Count == 0);
        }

        #endregion

        #region GetChanges Test

        [Test]
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
            Assert.AreEqual(TrackingState.Added, changes.First().TrackingState);
        }

        [Test]
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
            Assert.AreEqual(TrackingState.Modified, changes.First().TrackingState);
        }

        [Test]
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
            Assert.AreEqual(TrackingState.Deleted, changes.First().TrackingState);
        }

        [Test]
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
            Assert.AreEqual(TrackingState.Added, changes.ElementAt(1).TrackingState);
            Assert.AreEqual(TrackingState.Modified, changes.ElementAt(0).TrackingState);
            Assert.AreEqual(TrackingState.Deleted, changes.ElementAt(2).TrackingState);
        }

        [Test]
        public void GetChanges_Issue56_1()
        {
            // Arrange
            var database = new MockNorthwind();
            
            var customer = database.Customers[0];
            customer.CustomerSetting = new CustomerSetting() { Setting = "Setting1" };
            customer.CustomerSetting.Customer = customer;
            customer.CustomerSetting.CustomerId = customer.CustomerId; 
            var order = database.Orders[0];
            order.Customer = customer;
            order.CustomerId = customer.CustomerId;
            customer.Orders = new ChangeTrackingCollection<Order>() { order };

            var changeTracker = new ChangeTrackingCollection<Customer>(customer);
            customer.CustomerSetting.Setting = "Setting2";
            customer.Orders.Remove(customer.Orders[0]);

            // Act
            var changes = changeTracker.GetChanges();
            
            // Assert
            Assert.AreEqual(TrackingState.Unchanged, changes.ElementAt(0).TrackingState);
            Assert.AreEqual(TrackingState.Modified, changes.ElementAt(0).CustomerSetting.TrackingState);
            Assert.AreEqual(TrackingState.Deleted, changes.ElementAt(0).Orders[0].TrackingState);
        }

        [Test]
        public void GetChanges_Issue56_2()
        {
            // Arrange
            var database = new MockNorthwind();

            var customer = database.Customers[0];
            customer.CustomerSetting = new CustomerSetting() { Setting = "Setting1" };
            customer.CustomerSetting.Customer = customer;
            customer.CustomerSetting.CustomerId = customer.CustomerId;
            var order = database.Orders[0];
            order.Customer = customer;
            order.CustomerId = customer.CustomerId;
            customer.Orders = new ChangeTrackingCollection<Order>() { order };

            var changeTracker = new ChangeTrackingCollection<Customer>(customer);
            customer.Orders.Remove(customer.Orders[0]);
            customer.CustomerSetting.Setting = "Setting2";

            // Act
            var changes = changeTracker.GetChanges();

            // Assert
            Assert.AreEqual(TrackingState.Unchanged, changes.ElementAt(0).TrackingState);
            Assert.AreEqual(TrackingState.Modified, changes.ElementAt(0).CustomerSetting.TrackingState);
            Assert.AreEqual(TrackingState.Deleted, changes.ElementAt(0).Orders[0].TrackingState);
        }

        #endregion

        #region EntityChanged Event Tests

        [Test]
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
            Assert.AreEqual(3, changesCount);
        }

        #endregion
    }
}
