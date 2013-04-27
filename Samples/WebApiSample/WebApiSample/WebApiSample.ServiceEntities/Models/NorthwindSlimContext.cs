using System.Data.Entity;
using WebApiSample.ServiceEntities.Models.Mapping;

namespace WebApiSample.ServiceEntities.Models
{
    public partial class NorthwindSlimContext : DbContext
    {
        static NorthwindSlimContext()
        {
            Database.SetInitializer<NorthwindSlimContext>(null);
        }

        public NorthwindSlimContext()
            : base("Name=NorthwindSlimContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new CustomerMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new OrderDetailMap());
            modelBuilder.Configurations.Add(new ProductMap());
        }
    }
}
