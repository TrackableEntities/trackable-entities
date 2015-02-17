using System;
using System.Data.Entity;
using SampleWebApi.Inheritance.Shared.Entities.Models;

namespace SampleWebApi.Inheritance.Shared.Entities.Contexts
{
    public class NorthwindSlimDatabaseInitializer : DropCreateDatabaseIfModelChanges<NorthwindSlimInheritance>
    {
        protected override void Seed(NorthwindSlimInheritance context)
        {
            AddCategories(context);
            AddProducts(context);
        }

        private void AddProducts(NorthwindSlimInheritance context)
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
            context.Products.Add(new DiscontinuedProduct
            {
                ProductId = 3,
                CategoryId = 2,
                ProductName = "Aniseed Syrup",
                UnitPrice = 23M,
                DiscontinuedDate = DateTime.Today.Subtract(TimeSpan.FromDays(1))
            });
            context.Products.Add(new DiscontinuedProduct
            {
                ProductId = 4,
                CategoryId = 2,
                ProductName = "Chef Anton's Cajun Seasoning",
                UnitPrice = 23M,
                DiscontinuedDate = DateTime.Today.Subtract(TimeSpan.FromDays(2))
            });
        }

        private void AddCategories(NorthwindSlimInheritance context)
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
