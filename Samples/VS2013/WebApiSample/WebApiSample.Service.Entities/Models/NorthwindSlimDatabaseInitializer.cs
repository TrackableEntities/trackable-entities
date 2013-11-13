using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace WebApiSample.Service.Entities.Models
{
    public class NorthwindSlimDatabaseInitializer : CreateDatabaseIfNotExists<NorthwindSlimContext>
    {
        protected override void Seed(NorthwindSlimContext context)
        {
            AddCategories(context);
            AddProducts(context);
            AddCustomers(context);
            AddOrders(context);
        }

        private void AddOrders(NorthwindSlimContext context)
        {
            context.Orders.Add(new Order
            {
                CustomerId = "ALFKI",
                OrderId = 1,
                OrderDate = DateTime.Today,
                ShippedDate = DateTime.Today,
                Freight = 41.34M,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        ProductId = 1,
                        Quantity = 1,
                        UnitPrice = 10M
                    },
                    new OrderDetail
                    {
                        ProductId = 2,
                        Quantity = 2,
                        UnitPrice = 20M
                    },
                }
            });
            context.Orders.Add(new Order
            {
                CustomerId = "ALFKI",
                OrderId = 2,
                OrderDate = DateTime.Today,
                ShippedDate = DateTime.Today,
                Freight = 41.34M,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        ProductId = 3,
                        Quantity = 3,
                        UnitPrice = 30M
                    },
                }
            });
            context.Orders.Add(new Order
            {
                CustomerId = "ANATR",
                OrderId = 3,
                OrderDate = DateTime.Today,
                ShippedDate = DateTime.Today,
                Freight = 41.34M,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        ProductId = 2,
                        Quantity = 4,
                        UnitPrice = 20M
                    },
                }
            });
            context.Orders.Add(new Order
            {
                CustomerId = "ANTON",
                OrderId = 4,
                OrderDate = DateTime.Today,
                ShippedDate = DateTime.Today,
                Freight = 41.34M,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        ProductId = 3,
                        Quantity = 5,
                        UnitPrice = 30M
                    },
                }
            });
        }

        private void AddCustomers(NorthwindSlimContext context)
        {
            context.Customers.Add(new Customer
            {
                CustomerId = "ALFKI",
                CompanyName = "Alfreds Futterkiste",
                ContactName = "Maria Anders",
                City = "Berlin",
                Country = "Germany"
            });
            context.Customers.Add(new Customer
            {
                CustomerId = "ANATR",
                CompanyName = "Ana Trujillo Emparedados y helados",
                ContactName = "Ana Trujillo",
                City = "México D.F.",
                Country = "Mexico"
            });
            context.Customers.Add(new Customer
            {
                CustomerId = "ANTON",
                CompanyName = "Antonio Moreno Taquería",
                ContactName = "Antonio Moreno",
                City = "México D.F.",
                Country = "Mexico"
            });
        }

        private void AddProducts(NorthwindSlimContext context)
        {
            context.Products.Add(new Product
            {
                ProductId = 1,
                CategoryId = 1,
                ProductName = "Chai",
                UnitPrice = 23M
            });
            context.Products.Add(new Product
            {
                ProductId = 2,
                CategoryId = 1,
                ProductName = "Chang",
                UnitPrice = 23M
            });
            context.Products.Add(new Product
            {
                ProductId = 3,
                CategoryId = 2,
                ProductName = "Aniseed Syrup",
                UnitPrice = 23M
            });
            context.Products.Add(new Product
            {
                ProductId = 4,
                CategoryId = 2,
                ProductName = "Chef Anton's Cajun Seasoning",
                UnitPrice = 23M
            });
        }

        private void AddCategories(NorthwindSlimContext context)
        {
            context.Categories.Add(new Category
            {
                CategoryId = 1,
                CategoryName = "Beverages"
            });
            context.Categories.Add(new Category
            {
                CategoryId = 2,
                CategoryName = "Condiments"
            });
            context.Categories.Add(new Category
            {
                CategoryId = 3,
                CategoryName = "Confections"
            });
            context.Categories.Add(new Category
            {
                CategoryId = 4,
                CategoryName = "Dairy Products"
            });
            context.Categories.Add(new Category
            {
                CategoryId = 5,
                CategoryName = "Grains/Cereals"
            });
            context.Categories.Add(new Category
            {
                CategoryId = 6,
                CategoryName = "Meat/Poultry"
            });
            context.Categories.Add(new Category
            {
                CategoryId = 7,
                CategoryName = "Produce"
            });
            context.Categories.Add(new Category
            {
                CategoryId = 7,
                CategoryName = "Seafood"
            });
        }
    }
}
