﻿using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using NUnit.Framework;
#if EF_6
using TrackableEntities.EF6;
#else
using TrackableEntities.EF5;
#endif
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
	public class NorthwindDbContextTests
	{
		const CreateDbOptions CreateNorthwindDbOptions = CreateDbOptions.DropCreateDatabaseIfModelChanges;

		#region Product: Single Entity

		[Test]
		public void Apply_Changes_Should_Mark_Product_Unchanged()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var parent = new Product();
			parent.TrackingState = TrackingState.Unchanged;

			// Act
			context.ApplyChanges(parent);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(parent).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Product_Added()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var parent = new Product();
			parent.TrackingState = TrackingState.Added;

			// Act
			context.ApplyChanges(parent);

			// Assert
			Assert.AreEqual(EntityState.Added, context.Entry(parent).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Product_Modified()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var parent = new Product();
			parent.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(parent);

			// Assert
			Assert.AreEqual(EntityState.Modified, context.Entry(parent).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Product_Deleted()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var parent = new Product();
			parent.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(parent);

			// Assert
			Assert.AreEqual(EntityState.Deleted, context.Entry(parent).State);
		}

		#endregion

		#region Product: Multiple Entities

		[Test]
		public void Apply_Changes_Should_Mark_Products_Unchanged()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var products = new List<Product>
			{
				new Product {ProductId = 1}, 
				new Product { ProductId = 2}
			};
			products.ForEach(p => p.TrackingState = TrackingState.Unchanged);

			// Act
			context.ApplyChanges(products);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(products[0]).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(products[1]).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Products_Added()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var products = new List<Product>
			{
				new Product {ProductId = 1}, 
				new Product { ProductId = 2}
			};
			products.ForEach(p => p.TrackingState = TrackingState.Added);

			// Act
			context.ApplyChanges(products);

			// Assert
			Assert.AreEqual(EntityState.Added, context.Entry(products[0]).State);
			Assert.AreEqual(EntityState.Added, context.Entry(products[1]).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Products_Modified()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var products = new List<Product>
			{
				new Product {ProductId = 1}, 
				new Product { ProductId = 2}
			};
			products.ForEach(p => p.TrackingState = TrackingState.Modified);

			// Act
			context.ApplyChanges(products);

			// Assert
			Assert.AreEqual(EntityState.Modified, context.Entry(products[0]).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(products[1]).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Products_Deleted()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var products = new List<Product>
			{
				new Product {ProductId = 1}, 
				new Product { ProductId = 2}
			};
			products.ForEach(p => p.TrackingState = TrackingState.Deleted);

			// Act
			context.ApplyChanges(products);

			// Assert
			Assert.AreEqual(EntityState.Deleted, context.Entry(products[0]).State);
			Assert.AreEqual(EntityState.Deleted, context.Entry(products[1]).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Products()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var products = new List<Product>
			{
				new Product { ProductId = 1}, 
				new Product { ProductId = 2},
				new Product { ProductId = 3},
				new Product { ProductId = 4},
			};
			products[1].TrackingState = TrackingState.Modified;
			products[2].TrackingState = TrackingState.Added;
			products[3].TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(products);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(products[0]).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(products[1]).State);
			Assert.AreEqual(EntityState.Added, context.Entry(products[2]).State);
			Assert.AreEqual(EntityState.Deleted, context.Entry(products[3]).State);
		}

		#endregion

		#region Order: One to Many

		[Test]
		public void Apply_Changes_Should_Mark_Order_Unchanged()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var order = new MockNorthwind().Orders[0];
			var detail1 = order.OrderDetails[0];
			var detail2 = order.OrderDetails[1];
			var detail3 = order.OrderDetails[2];
			var detail4 = order.OrderDetails[3];

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(order).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(detail1).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(detail2).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(detail3).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(detail4).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Order_Added()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var order = new MockNorthwind().Orders[0];
			var detail1 = order.OrderDetails[0];
			var detail2 = order.OrderDetails[1];
			var detail3 = order.OrderDetails[2];
			var detail4 = order.OrderDetails[3];
			order.TrackingState = TrackingState.Added;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Added, context.Entry(order).State);
			Assert.AreEqual(EntityState.Added, context.Entry(detail1).State);
			Assert.AreEqual(EntityState.Added, context.Entry(detail2).State);
			Assert.AreEqual(EntityState.Added, context.Entry(detail3).State);
			Assert.AreEqual(EntityState.Added, context.Entry(detail4).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Order_Deleted()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var order = new MockNorthwind().Orders[0];
			order.CustomerId = null;
			order.Customer = null;
			var detail1 = order.OrderDetails[0];
			var detail2 = order.OrderDetails[1];
			var detail3 = order.OrderDetails[2];
			var detail4 = order.OrderDetails[3];
			order.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Deleted, context.Entry(order).State);
			Assert.AreEqual(EntityState.Deleted, context.Entry(detail1).State);
			Assert.AreEqual(EntityState.Deleted, context.Entry(detail2).State);
			Assert.AreEqual(EntityState.Deleted, context.Entry(detail3).State);
			Assert.AreEqual(EntityState.Deleted, context.Entry(detail4).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Order_Only_Modified()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var order = new MockNorthwind().Orders[0];
			var detail1 = order.OrderDetails[0];
			var detail2 = order.OrderDetails[1];
			var detail3 = order.OrderDetails[2];
			var detail4 = order.OrderDetails[3];
			order.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Modified, context.Entry(order).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(detail1).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(detail2).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(detail3).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(detail4).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Order_Details_Only_Modified()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var order = new MockNorthwind().Orders[0];
			var detail1 = order.OrderDetails[0];
			var detail2 = order.OrderDetails[1];
			var detail3 = order.OrderDetails[2];
			var detail4 = order.OrderDetails[3];
			detail1.TrackingState = TrackingState.Modified;
			detail2.TrackingState = TrackingState.Modified;
			detail3.TrackingState = TrackingState.Modified;
			detail4.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(order).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(detail1).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(detail2).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(detail3).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(detail4).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Order_With_Details_Modified()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var order = new MockNorthwind().Orders[0];
			var detail1 = order.OrderDetails[0];
			var detail2 = order.OrderDetails[1];
			var detail3 = order.OrderDetails[2];
			var detail4 = order.OrderDetails[3];
			order.TrackingState = TrackingState.Modified;
			detail1.TrackingState = TrackingState.Modified;
			detail2.TrackingState = TrackingState.Modified;
			detail3.TrackingState = TrackingState.Modified;
			detail4.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Modified, context.Entry(order).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(detail1).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(detail2).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(detail3).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(detail4).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Order_Unchanged_With_OrderDetails_Added_Modified_Deleted_Unchanged()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var order = new MockNorthwind().Orders[0];
			var detail1 = order.OrderDetails[0];
		    detail1.OrderDetailId = 0;
			var detail2 = order.OrderDetails[1];
			var detail3 = order.OrderDetails[2];
			var detail4 = order.OrderDetails[3];
			detail1.TrackingState = TrackingState.Added;
			detail2.TrackingState = TrackingState.Modified;
			detail3.TrackingState = TrackingState.Deleted;
			detail4.TrackingState = TrackingState.Unchanged;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(order).State);
			Assert.AreEqual(EntityState.Added, context.Entry(detail1).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(detail2).State);
			Assert.AreEqual(EntityState.Deleted, context.Entry(detail3).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(detail4).State);
		}

        [Test]
        public void Apply_Changes_Should_Mark_Order_Modified_With_OrderDetails_Added_Modified_Deleted_Unchanged()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            detail1.OrderDetailId = 0;
            var detail2 = order.OrderDetails[1];
            var detail3 = order.OrderDetails[2];
            var detail4 = order.OrderDetails[3];
            order.TrackingState = TrackingState.Modified;
            detail1.TrackingState = TrackingState.Added;
            detail2.TrackingState = TrackingState.Modified;
            detail3.TrackingState = TrackingState.Deleted;
            detail4.TrackingState = TrackingState.Unchanged;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.AreEqual(EntityState.Modified, context.Entry(order).State);
            Assert.AreEqual(EntityState.Added, context.Entry(detail1).State);
            Assert.AreEqual(EntityState.Modified, context.Entry(detail2).State);
            Assert.AreEqual(EntityState.Deleted, context.Entry(detail3).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(detail4).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Unchanged_Order_With_Multiple_OrderDetails_Added()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            var detail2 = order.OrderDetails[1];
            detail1.OrderDetailId = 0;
            detail2.OrderDetailId = 0;
            detail1.TrackingState = TrackingState.Added;
            detail2.TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.AreEqual(EntityState.Unchanged, context.Entry(order).State);
            Assert.AreEqual(EntityState.Added, context.Entry(detail1).State);
            Assert.AreEqual(EntityState.Added, context.Entry(detail2).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Modified_Order_With_Multiple_OrderDetails_Added()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            var detail2 = order.OrderDetails[1];
            detail1.OrderDetailId = 0;
            detail2.OrderDetailId = 0;
            detail1.TrackingState = TrackingState.Added;
            detail2.TrackingState = TrackingState.Added;
            order.TrackingState = TrackingState.Modified;
            order.ModifiedProperties = new List<string> {"OrderDate"};

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.AreEqual(EntityState.Modified, context.Entry(order).State);
            Assert.AreEqual(EntityState.Added, context.Entry(detail1).State);
            Assert.AreEqual(EntityState.Added, context.Entry(detail2).State);
        }

        #endregion

        #region Order: Many to One to Many

        [Test]
        public void Apply_Changes_Should_Mark_Unchanged_Order_Unchanged_Customer_With_Multiple_Addresses_Added()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            order.OrderDetails = null;
            var address1 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street1",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address2 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street2",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            order.Customer.CustomerAddresses = new List<CustomerAddress>
                { address1, address2 };
            address1.TrackingState = TrackingState.Added;
            address2.TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.AreEqual(EntityState.Unchanged, context.Entry(order).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(order.Customer).State);
            Assert.AreEqual(EntityState.Added, context.Entry(address1).State);
            Assert.AreEqual(EntityState.Added, context.Entry(address2).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Unchanged_Order_Unchanged_Customer_With_Multiple_Addresses_Added_And_Modified()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            order.OrderDetails = null;
            var address1 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street1",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address2 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street2",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address3 = new CustomerAddress
            {
                CustomerAddressId = 1,
                Street = "Street3",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address4 = new CustomerAddress
            {
                CustomerAddressId = 2,
                Street = "Street4",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            order.Customer.CustomerAddresses = new List<CustomerAddress> { address1, address2, address3, address4 };
            address1.TrackingState = TrackingState.Added;
            address2.TrackingState = TrackingState.Added;
            address3.TrackingState = TrackingState.Modified;
            address4.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.AreEqual(EntityState.Unchanged, context.Entry(order).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(order.Customer).State);
            Assert.AreEqual(EntityState.Added, context.Entry(address1).State);
            Assert.AreEqual(EntityState.Added, context.Entry(address2).State);
        }

        #endregion

        #region Employee-Territory: Many to Many

        [Test]
		public void Apply_Changes_Should_Mark_Unchanged_Employee_As_Unchanged_And_Unchanged_Territories_As_Unchanged()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var employee = nw.Employees[0];
			var territory1 = employee.Territories[0];
			var territory2 = employee.Territories[1];
			var territory3 = employee.Territories[2];

			// Act
			context.ApplyChanges(employee);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(employee).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Unchanged_Employee_As_Unchanged_And_Modified_Territories_As_Modified()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var employee = nw.Employees[0];
			var territory1 = employee.Territories[0];
			var territory2 = employee.Territories[1];
			var territory3 = employee.Territories[2];
			territory3.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(employee);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(employee).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(territory3).State);
		}

        [Test]
        public void Apply_Changes_Should_Mark_Unchanged_Employee_As_Unchanged_And_Unchanged_Territories_With_Modified_Area_As_Modified()
        {
            // Ensure that changes are applied across M-M relationships.

            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            var territory1 = employee.Territories[0];
            var territory2 = employee.Territories[1];
            var territory3 = employee.Territories[2];
            var area = new Area
            {
                AreaId = 1,
                AreaName = "Northern",
                TrackingState = TrackingState.Modified
            };
            territory3.AreaId = 1;
            territory3.Area = area;

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.AreEqual(EntityState.Unchanged, context.Entry(employee).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
            Assert.AreEqual(EntityState.Modified, context.Entry(area).State);
        }

        [Test]
		public void Apply_Changes_Should_Mark_Unchanged_Employee_As_Unchanged_And_Added_Territories_As_Unchanged()
		{
			// NOTE: With M-M properties there is no way to tell if the related entity is new or should 
			// or simply be added to the relationship, because it is an independent association.
			// Therefore, added children are added to the relationship and marked unchanged.

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var employee = nw.Employees[0];
			var territory1 = employee.Territories[0];
			var territory2 = employee.Territories[1];
			var territory3 = employee.Territories[2];
			var territory4 = nw.Territories[3];
			territory4.TrackingState = TrackingState.Added;
			employee.Territories.Add(territory4);

			// Act
			context.ApplyChanges(employee);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(employee).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory4).State);
			Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory4));
		}

		[Test]
		public void Apply_Changes_Should_Mark_Unchanged_Employee_As_Unchanged_And_Deleted_Territories_As_Unchanged()
		{
			// NOTE: With M-M properties there is no way to tell if the related entity should be deleted
			// or simply removed from the relationship, because it is an independent association.
			// Therefore, deleted children are removed from the relationship and marked unchanged.

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var employee = nw.Employees[0];
			var territory1 = employee.Territories[0];
			var territory2 = employee.Territories[1];
			var territory3 = employee.Territories[2];
			territory3.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(employee);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(employee).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
			Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory3));
			Assert.AreEqual(2, employee.Territories.Count);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Added_Employee_As_Added_And_Unchanged_Territories_As_Unchanged()
		{
			// NOTE: Because parent is added, unchanged children will be added to M-M relation,
			// even though the entities themselves are unchanged.

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var employee = nw.Employees[0];
			employee.TrackingState = TrackingState.Added;
			var territory1 = employee.Territories[0];
			var territory2 = employee.Territories[1];
			var territory3 = employee.Territories[2];

			// Act
			context.ApplyChanges(employee);

			// Assert
			Assert.AreEqual(EntityState.Added, context.Entry(employee).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
			Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory1));
			Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory2));
			Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory3));
		}

		[Test]
		public void Apply_Changes_Should_Mark_Added_Employee_As_Added_And_Modified_Territories_As_Modified()
		{
			// NOTE: Modified children of an added parent will remain modified,
			// but they will be added to the M-M relation.

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var employee = nw.Employees[0];
			employee.TrackingState = TrackingState.Added;
			var territory1 = employee.Territories[0];
			var territory2 = employee.Territories[1];
			var territory3 = employee.Territories[2];
			territory3.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(employee);

			// Assert
			Assert.AreEqual(EntityState.Added, context.Entry(employee).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(territory3).State);
			Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory1));
			Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory2));
			Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory3));
		}

		[Test]
		public void Apply_Changes_Should_Mark_Added_Employee_As_Added_And_Added_Territories_As_Unchanged()
		{
			// NOTE: Because parent is added, added children will be marked as unchanged
			// but added to the M-M relation

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var employee = nw.Employees[0];
			employee.TrackingState = TrackingState.Added;
			var territory1 = employee.Territories[0];
			var territory2 = employee.Territories[1];
			var territory3 = employee.Territories[2];
			var territory4 = nw.Territories[3];
			territory4.TrackingState = TrackingState.Added;
			employee.Territories.Add(territory4);

			// Act
			context.ApplyChanges(employee);

			// Assert
			Assert.AreEqual(EntityState.Added, context.Entry(employee).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory4).State);
			Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory4));
		}

		[Test]
		public void Apply_Changes_Should_Mark_Added_Employee_As_Added_And_Deleted_Territories_As_Deleted()
		{
			// NOTE: If a deleted child is assocated with an added parent, 
			// we will just ignore the delete and add the item, since this is unsupported.

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var employee = nw.Employees[0];
			employee.TrackingState = TrackingState.Added;
			var territory1 = employee.Territories[0];
			var territory2 = employee.Territories[1];
			var territory3 = employee.Territories[2];
			territory3.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(employee);

			// Assert
			Assert.AreEqual(EntityState.Added, context.Entry(employee).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
			Assert.IsTrue(context.RelatedItemHasBeenAdded(employee, territory3));
		}

		[Test]
		public void Apply_Changes_Should_Mark_Deleted_Employee_As_Deleted_And_Unchanged_Territories_As_Unchanged()
		{
			// NOTE: Because parent is deleted, unchanged children will be deleted from M-M relation,
			// even though the entities themselves are unchanged.

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var employee = nw.Employees[0];
			employee.TrackingState = TrackingState.Deleted;
			var territory1 = employee.Territories[0];
			var territory2 = employee.Territories[1];
			var territory3 = employee.Territories[2];

			// Act
			context.ApplyChanges(employee);

			// Assert
			Assert.AreEqual(EntityState.Deleted, context.Entry(employee).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
			Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory1));
			Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory2));
			Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory3));
			Assert.AreEqual(0, employee.Territories.Count);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Deleted_Employee_As_Deleted_And_Modified_Territories_As_Modified()
		{
			// NOTE: Modified children of a deleted parent will remain modified,
			// but they will be removed from the M-M relation.

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var employee = nw.Employees[0];
			employee.TrackingState = TrackingState.Deleted;
			var territory1 = employee.Territories[0];
			var territory2 = employee.Territories[1];
			var territory3 = employee.Territories[2];
			territory3.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(employee);

			// Assert
			Assert.AreEqual(EntityState.Deleted, context.Entry(employee).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(territory3).State);
			Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory1));
			Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory2));
			Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory3));
			Assert.AreEqual(0, employee.Territories.Count);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Deleted_Employee_As_Deleted_And_Added_Territories_As_Unchanged()
		{
			// NOTE: Because parent is deleted, added children will be marked as unchanged
			// but removed from the M-M relation

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var employee = nw.Employees[0];
			employee.TrackingState = TrackingState.Deleted;
			var territory1 = employee.Territories[0];
			var territory2 = employee.Territories[1];
			var territory3 = employee.Territories[2];
			var territory4 = nw.Territories[3];
			territory4.TrackingState = TrackingState.Added;
			employee.Territories.Add(territory4);

			// Act
			context.ApplyChanges(employee);

			// Assert
			Assert.AreEqual(EntityState.Deleted, context.Entry(employee).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory4).State);
			Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory1));
			Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory2));
			Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory3));
			Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory4));
			Assert.AreEqual(0, employee.Territories.Count);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Deleted_Employee_As_Deleted_And_Deleted_Territories_As_Unchanged()
		{
			// NOTE: If a deleted child is assocated with a deleted parent, 
			// it should be set to unchanged and removed from the M-M relation.

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var employee = nw.Employees[0];
			employee.TrackingState = TrackingState.Deleted;
			var territory1 = employee.Territories[0];
			var territory2 = employee.Territories[1];
			var territory3 = employee.Territories[2];
			territory3.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(employee);

			// Assert
			Assert.AreEqual(EntityState.Deleted, context.Entry(employee).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory1).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory2).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(territory3).State);
			Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory1));
			Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory2));
			Assert.IsTrue(context.RelatedItemHasBeenRemoved(employee, territory3));
			Assert.AreEqual(0, employee.Territories.Count);
		}

		#endregion

		#region Customer-CustomerSetting: One to One

		[Test]
		public void Apply_Changes_Should_Mark_Unchanged_Customer_As_Unchanged_And_Unchanged_Setting_As_Unchanged()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			var setting = customer.CustomerSetting = new CustomerSetting 
				{ CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(customer).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(setting).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Unchanged_Customer_As_Unchanged_And_Modified_Setting_As_Modified()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			var setting = customer.CustomerSetting = new CustomerSetting 
				{ CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
			setting.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(customer).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(setting).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Unchanged_Customer_As_Unchanged_And_Added_Setting_As_Added()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			var setting = customer.CustomerSetting = new CustomerSetting 
				{ CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
			setting.TrackingState = TrackingState.Added;

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(customer).State);
			Assert.AreEqual(EntityState.Added, context.Entry(setting).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Unchanged_Customer_As_Unchanged_And_Deleted_Setting_As_Deleted()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			var setting = customer.CustomerSetting = new CustomerSetting 
				{ CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
			setting.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(customer).State);
			Assert.AreEqual(EntityState.Deleted, context.Entry(setting).State);
			Assert.IsNull(customer.CustomerSetting);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Added_Customer_As_Added_And_Unchanged_Setting_As_Added()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			customer.TrackingState = TrackingState.Added;
			var setting = customer.CustomerSetting = new CustomerSetting
				{ CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.AreEqual(EntityState.Added, context.Entry(customer).State);
			Assert.AreEqual(EntityState.Added, context.Entry(setting).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Added_Customer_As_Added_And_Modified_Setting_As_Added()
		{
			// NOTE: Because customer is added, modified setting will be added.

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			customer.TrackingState = TrackingState.Added;
			var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
			setting.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.AreEqual(EntityState.Added, context.Entry(customer).State);
			Assert.AreEqual(EntityState.Added, context.Entry(setting).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Added_Customer_As_Added_And_Added_Setting_As_Added()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			customer.TrackingState = TrackingState.Added;
			var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
			setting.TrackingState = TrackingState.Added;

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.AreEqual(EntityState.Added, context.Entry(customer).State);
			Assert.AreEqual(EntityState.Added, context.Entry(setting).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Added_Customer_As_Modified_And_Deleted_Setting_As_Deleted()
		{
			// NOTE: Because customer is added, removing setting is ignored

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			customer.TrackingState = TrackingState.Added;
			var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
			setting.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.AreEqual(EntityState.Added, context.Entry(customer).State);
			Assert.AreEqual(EntityState.Added, context.Entry(setting).State);
			Assert.IsNotNull(customer.CustomerSetting);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Deleted_Customer_As_Deleted_And_Unchanged_Setting_As_Deleted()
		{
			// NOTE: CustomerSetting will be set to null because customer is deleted.

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			customer.TrackingState = TrackingState.Deleted;
			var setting = customer.CustomerSetting = new CustomerSetting
				{ CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.AreEqual(EntityState.Deleted, context.Entry(customer).State);
			Assert.AreEqual(EntityState.Deleted, context.Entry(setting).State);
			Assert.IsNull(customer.CustomerSetting);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Deleted_Customer_As_Deleted_And_Added_Setting_As_Deleted()
		{
			// NOTE: CustomerSetting will be set to null because customer is deleted.

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			customer.TrackingState = TrackingState.Deleted;
			var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
			setting.TrackingState = TrackingState.Added;

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.AreEqual(EntityState.Deleted, context.Entry(customer).State);
			Assert.AreEqual(EntityState.Deleted, context.Entry(setting).State);
			Assert.IsNull(customer.CustomerSetting);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Deleted_Customer_As_Deleted_And_Deleted_Setting_As_Deleted()
		{
			// NOTE: CustomerSetting will be set to null because customer is deleted.

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			customer.TrackingState = TrackingState.Deleted;
			var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
			setting.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.AreEqual(EntityState.Deleted, context.Entry(customer).State);
			Assert.AreEqual(EntityState.Deleted, context.Entry(setting).State);
			Assert.IsNull(customer.CustomerSetting);
		}

		#endregion

		#region Order-Customer: Many-to-One

		[Test]
		public void Apply_Changes_Should_Mark_Unchanged_Order_As_Unchanged_And_Unchanged_Customer_As_Unchanged()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			var customer = order.Customer;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(order).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(customer).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Unchanged_Order_As_Unchanged_And_Modified_Customer_As_Modified()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(order).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(customer).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Unchanged_Order_As_Unchanged_And_Added_Customer_As_Added()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Added;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(order).State);
			Assert.AreEqual(EntityState.Added, context.Entry(customer).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Unchanged_Order_As_Unchanged_And_Deleted_Customer_As_Unchanged()
		{
			// NOTE: We ignore deletes of related M-1 entities to because it may be related
			// to other entities.

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Unchanged, context.Entry(order).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(customer).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Added_Order_As_Added_And_Unchanged_Customer_As_Unchanged()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			order.TrackingState = TrackingState.Added;
			var customer = order.Customer;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Added, context.Entry(order).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(customer).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Added_Order_As_Added_And_Modified_Customer_As_Modified()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			order.TrackingState = TrackingState.Added;
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Added, context.Entry(order).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(customer).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Added_Order_As_Added_And_Added_Customer_As_Added()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			order.TrackingState = TrackingState.Added;
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Added;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Added, context.Entry(order).State);
			Assert.AreEqual(EntityState.Added, context.Entry(customer).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Added_Order_As_Added_And_Deleted_Customer_As_Unchanged()
		{
			// NOTE: We ignore deletes of related M-1 entities to because it may be related
			// to other entities.

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			order.TrackingState = TrackingState.Added;
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Added, context.Entry(order).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(customer).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Deleted_Order_As_Deleted_And_Unchanged_Customer_As_Unchanged()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			order.TrackingState = TrackingState.Deleted;
			var customer = order.Customer;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Deleted, context.Entry(order).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(customer).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Deleted_Order_As_Deleted_And_Modified_Customer_As_Modified()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			order.TrackingState = TrackingState.Deleted;
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Deleted, context.Entry(order).State);
			Assert.AreEqual(EntityState.Modified, context.Entry(customer).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Deleted_Order_As_Deleted_And_Added_Customer_As_Added()
		{
			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			order.TrackingState = TrackingState.Deleted;
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Added;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Deleted, context.Entry(order).State);
			Assert.AreEqual(EntityState.Added, context.Entry(customer).State);
		}

		[Test]
		public void Apply_Changes_Should_Mark_Deleted_Order_As_Deleted_And_Deleted_Customer_As_Unchanged()
		{
			// NOTE: We ignore deletes of related M-1 entities to because it may be related
			// to other entities.

			// Arrange
			var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			order.TrackingState = TrackingState.Deleted;
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.AreEqual(EntityState.Deleted, context.Entry(order).State);
			Assert.AreEqual(EntityState.Unchanged, context.Entry(customer).State);
			Assert.IsNull(order.Customer);
			Assert.IsNull(order.CustomerId);
		}

		#endregion
	}
}
