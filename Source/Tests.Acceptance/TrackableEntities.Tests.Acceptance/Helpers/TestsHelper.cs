using System;
using System.Collections.Generic;
using System.Linq;
using TrackableEntities.EF.Tests;
using TrackableEntities.EF.Tests.NorthwindModels;
using TrackableEntities.Tests.Acceptance.Contexts;
using Xunit;

namespace TrackableEntities.Tests.Acceptance.Helpers
{
    internal static class TestsHelper
    {
        private const CreateDbOptions CreateNorthwindDbOptions = CreateDbOptions.DropCreateDatabaseIfModelChanges;

        public static NorthwindTestDbContext CreateNorthwindDbContext(CreateDbOptions createDbOptions)
        {
            // Create new context for all tests
            var context = new NorthwindTestDbContext(createDbOptions);
            Assert.True(context.Products.Count() >= 0);
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
                    context.SaveChanges();
                }
            }
        }

        public static Order EnsureTestOrder(string customerId)
        {
            using (var context = CreateNorthwindDbContext(CreateNorthwindDbOptions))
            {
                var order = context.Orders
                    .FirstOrDefault(o => o.CustomerId == customerId);
                if (order == null)
                {
                    int[] productIds = TestsHelper.CreateTestProducts();
                    order = CreateTestOrder(customerId, productIds);
                }
                return order;
            }
        }

        public static int[] CreateTestProducts()
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
                var product3 = new Product
                {
                    ProductName = "Test Product 3",
                    UnitPrice = 20M,
                    Category = category1
                };
                var product4 = new Product
                {
                    ProductName = "Test Product 4",
                    UnitPrice = 20M,
                    Category = category2
                };
                var product5 = new Product
                {
                    ProductName = "Test Product 5",
                    UnitPrice = 20M,
                    Category = category2
                };


                context.Products.Add(product1);
                context.Products.Add(product2);
                context.Products.Add(product3);
                context.Products.Add(product4);
                context.Products.Add(product5);
                context.SaveChanges();
                int[] ids =
                {
                    product1.ProductId,
                    product2.ProductId,
                    product3.ProductId,
                    product4.ProductId,
                    product5.ProductId,
                };
                return ids;
            }
        }

        public static Order CreateTestOrder(string customerId, int[] productIds)
        {
            using (var context = CreateNorthwindDbContext(CreateNorthwindDbOptions))
            {
                var detail1 = new OrderDetail { ProductId = productIds[0], Quantity = 11, UnitPrice = 11M };
                var detail2 = new OrderDetail { ProductId = productIds[1], Quantity = 12, UnitPrice = 12M };
                var detail3 = new OrderDetail { ProductId = productIds[2], Quantity = 13, UnitPrice = 13M };
                var order = new Order
                {
                    OrderDate = DateTime.Today,
                    CustomerId = customerId,
                    OrderDetails = new List<OrderDetail>
                    {
                        detail1,
                        detail2,
                        detail3
                    }
                };

                context.Orders.Add(order);
                context.SaveChanges();
                return order;
            }
        }
    }
}
