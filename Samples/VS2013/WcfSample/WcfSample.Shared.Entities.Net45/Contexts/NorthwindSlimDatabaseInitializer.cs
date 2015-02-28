using System;
using System.Data.Entity;
using TrackableEntities.Client;
using WcfSample.Shared.Entities.Net45.Models;

namespace WcfSample.Shared.Entities.Net45.Contexts
{
    public class NorthwindSlimDatabaseInitializer : DropCreateDatabaseIfModelChanges<NorthwindSlim>
    {
        protected override void Seed(NorthwindSlim context)
        {
            AddCategories(context);
            AddProducts(context);
            AddCustomers(context);
            AddCustomerSettings(context);
            AddOrders(context);
            AddEmployees(context);
        }

        private void AddOrders(NorthwindSlim context)
        {
            context.Orders.Add(new Order
            {
                CustomerId = "ALFKI",
                OrderId = 1,
                OrderDate = DateTime.Today,
                ShippedDate = DateTime.Today,
                Freight = 41.34M,
                OrderDetails = new ChangeTrackingCollection<OrderDetail>()
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
                OrderDetails = new ChangeTrackingCollection<OrderDetail>
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
                OrderDetails = new ChangeTrackingCollection<OrderDetail>
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
                OrderDetails = new ChangeTrackingCollection<OrderDetail>
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

        private void AddCustomers(NorthwindSlim context)
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

        private void AddCustomerSettings(NorthwindSlim context)
        {
            context.CustomerSettings.Add(new CustomerSetting
            {
                CustomerId = "ALFKI",
                Setting = "Setting1"
            });
            context.CustomerSettings.Add(new CustomerSetting
            {
                CustomerId = "ANATR",
                Setting = "Setting2"
            });
            context.CustomerSettings.Add(new CustomerSetting
            {
                CustomerId = "ANTON",
                Setting = "Setting3"
            });
        }

        private void AddProducts(NorthwindSlim context)
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

        private void AddCategories(NorthwindSlim context)
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

        private void AddEmployees(NorthwindSlim context)
        {
            var territory1 = new Territory
            {
                TerritoryId = "01581",
                TerritoryDescription = "Westboro"
            };
            var territory2 = new Territory
            {
                TerritoryId = "01730",
                TerritoryDescription = "Bedford"
            };
            var territory3 = new Territory
            {
                TerritoryId = "01833",
                TerritoryDescription = "Georgetown"
            };
            var territory4 = new Territory
            {
                TerritoryId = "02116",
                TerritoryDescription = "Boston"
            };
            context.Employees.Add(new Employee
            {
                EmployeeId = 1,
                LastName = "Davolio",
                FirstName = "Nancy",
                BirthDate = DateTime.Parse("1948-12-08"),
                HireDate = DateTime.Parse("1992-05-01"),
                City = "Seattle",
                Country = "USA",
                Territories = new ChangeTrackingCollection<Territory> { territory1, territory2, territory3 }
            });
            context.Employees.Add(new Employee
            {
                EmployeeId = 2,
                LastName = "Fuller",
                FirstName = "Andrew",
                BirthDate = DateTime.Parse("1952-02-19"),
                HireDate = DateTime.Parse("1992-08-14"),
                City = "Tacoma",
                Country = "USA",
                Territories = new ChangeTrackingCollection<Territory> { territory2, territory3 }
            });
            context.Employees.Add(new Employee
            {
                EmployeeId = 3,
                LastName = "Leverling",
                FirstName = "Janet",
                BirthDate = DateTime.Parse("1963-08-30"),
                HireDate = DateTime.Parse("1992-05-01"),
                City = "Kirkland",
                Country = "USA",
                Territories = new ChangeTrackingCollection<Territory> { territory3, territory4 }
            });
            context.Employees.Add(new Employee
            {
                EmployeeId = 4,
                LastName = "Peacock",
                FirstName = "Margaret",
                BirthDate = DateTime.Parse("1937-09-19"),
                HireDate = DateTime.Parse("1993-05-03"),
                City = "Redmond",
                Country = "USA",
                Territories = new ChangeTrackingCollection<Territory> { territory1, territory3 }
            });
        }
    }
}
