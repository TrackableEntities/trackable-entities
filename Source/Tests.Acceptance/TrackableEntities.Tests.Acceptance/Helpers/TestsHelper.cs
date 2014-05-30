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
    }
}
