using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TrackableEntities.EF.Tests;
using TrackableEntities.EF.Tests.Contexts;
using TrackableEntities.EF.Tests.NorthwindModels;

namespace TrackableEntities.Tests.Acceptance.Helpers
{
    internal static class TestsHelper
    {
        private const CreateDbOptions CreateNorthwindDbOptions = CreateDbOptions.DropCreateDatabaseIfModelChanges;

        public static NorthwindDbContext CreateNorthwindDbContext(CreateDbOptions createDbOptions)
        {
            // Create new context for all tests
            var context = new NorthwindDbContext(createDbOptions);
            Assert.GreaterOrEqual(context.Products.Count(), 0);
            return context;
        }

        public static void EnsureTestCustomer(string customerId, string customerName)
        {
            using (var context = CreateNorthwindDbContext(CreateNorthwindDbOptions))
            {
                var customer = context.Customers
                    .SingleOrDefault(c => c.CustomerId == customerId);
                if (customer == null)
                {
                    customer = new Customer
                    {
                        CustomerId = customerId,
                        CustomerName = customerName
                    };
                    context.Customers.Add(customer);
                }
                context.SaveChanges();
            }
        }

        public static Order CreateTestOrder(string customerId)
        {
            using (var context = CreateNorthwindDbContext(CreateNorthwindDbOptions))
            {
                var category1 = new Category
                {
                    CategoryName = "Test Category 1"
                };
                var category2 = new Category
                {
                    CategoryName = "Test Category 2"
                };
                var product1 = new Product
                {
                    ProductName = "Test Product 1",
                    UnitPrice = 10M,
                    Category = category1
                };
                var product2 = new Product
                {
                    ProductName = "Test Product 2",
                    UnitPrice = 20M,
                    Category = category2
                };
                var customer1 = context.Customers
                    .Single(c => c.CustomerId == customerId);
                var detail1 = new OrderDetail { Product = product1, Quantity = 11, UnitPrice = 11M };
                var detail2 = new OrderDetail { Product = product2, Quantity = 12, UnitPrice = 12M };
                var order = new Order
                {
                    OrderDate = DateTime.Today,
                    Customer = customer1,
                    OrderDetails = new List<OrderDetail>
                    {
                        detail1,
                        detail2,
                    }
                };

                context.Orders.Add(order);
                context.SaveChanges();
                return order;
            }
        }
    }
}
