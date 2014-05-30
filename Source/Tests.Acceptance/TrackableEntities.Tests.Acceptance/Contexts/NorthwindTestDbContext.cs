using System;
using System.Data.Entity;
using TrackableEntities.EF.Tests;
using TrackableEntities.EF.Tests.NorthwindModels;

namespace TrackableEntities.Tests.Acceptance.Contexts
{
    public class NorthwindTestDbContext : DbContext
    {
        private const string TestDbName = "NorthwindAcceptTestDb";

        public NorthwindTestDbContext(CreateDbOptions createDbOptions = CreateDbOptions.CreateDatabaseIfNotExists)
            : base(TestDbName)
        {
            switch (createDbOptions)
            {
                case CreateDbOptions.DropCreateDatabaseAlways:
                    Database.SetInitializer(new DropCreateDatabaseAlways<NorthwindTestDbContext>());
                    break;
                case CreateDbOptions.DropCreateDatabaseSeed:
                    Database.SetInitializer(new NorthwindDbInitializer());
                    break;
                case CreateDbOptions.DropCreateDatabaseIfModelChanges:
                    Database.SetInitializer(new DropCreateDatabaseIfModelChanges<NorthwindTestDbContext>());
                    break;
                default:
                    Database.SetInitializer(new DropCreateDatabaseAlways<NorthwindTestDbContext>());
                    break;
            }
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerSetting> CustomerSettings { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Territory> Territories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerSetting>()
                .HasRequired(x => x.Customer)
                .WithOptional(x => x.CustomerSetting);
        }
    }
}
