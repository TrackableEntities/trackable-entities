using System.Data.Entity;
using TrackableEntities.EF.Tests.NorthwindModels;

namespace TrackableEntities.EF.Tests.Contexts
{
    public class NorthwindDbContext : DbContext
    {
        public NorthwindDbContext(CreateDbOptions createDbOptions = CreateDbOptions.CreateDatabaseIfNotExists)
            : base("NorthwindTestDb")
        {
            switch (createDbOptions)
            {
                case CreateDbOptions.DropCreateDatabaseAlways:
                    Database.SetInitializer(new DropCreateDatabaseAlways<NorthwindDbContext>());
                    break;
                case CreateDbOptions.DropCreateDatabaseSeed:
                    Database.SetInitializer(new NorthwindDbInitializer());
                    break;
                case CreateDbOptions.DropCreateDatabaseIfModelChanges:
                    Database.SetInitializer(new DropCreateDatabaseIfModelChanges<FamilyDbContext>());
                    break;
                default:
                    Database.SetInitializer(new DropCreateDatabaseAlways<NorthwindDbContext>());
                    break;
            }
            //Categories = Set<Category>();
            //Products = Set<Product>();
            //Customers = Set<Customer>();
            //Orders = Set<Order>();
            //OrderDetails = Set<OrderDetail>();
        }
        
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
