using System;
using System.Data.Entity;
using TrackableEntities.EF.Tests.Contexts;
using TrackableEntities.EF.Tests.Mocks;

namespace TrackableEntities.EF.Tests
{
    public class NorthwindDbInitializer : DropCreateDatabaseAlways<NorthwindDbContext>
    {
        protected override void Seed(NorthwindDbContext context)
        {
            var model = new MockNorthwind();
            foreach (var category in model.Categories)
            {
                context.Categories.Add(category);
            }
            foreach (var product in model.Products)
            {
                context.Products.Add(product);
            }
            foreach (var customer in model.Customers)
            {
                context.Customers.Add(customer);
            }
            foreach (var order in model.Orders)
            {
                context.Orders.Add(order);
            }
            foreach (var territory in model.Territories)
            {
                context.Territories.Add(territory);
            }
            foreach (var employee in model.Employees)
            {
                context.Employees.Add(employee);
            }
            context.SaveChanges();
        }
    }
}
