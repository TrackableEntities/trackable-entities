using System.Data.Entity;
using WebApiSample.Service.Entities.Models;
using WebApiSample.Service.Entities.Models.Mapping;

namespace WebApiSample.Service.Entities.Contexts
{
    public partial class NorthwindSlimContext : DbContext
    {
        static NorthwindSlimContext()
        {
            Database.SetInitializer(new NorthwindSlimDatabaseInitializer());
        }

        public NorthwindSlimContext()
            : base("Name=NorthwindSlimContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerSetting> CustomerSettings { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Territory> Territories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new CustomerMap());
            modelBuilder.Configurations.Add(new CustomerSettingMap());
            modelBuilder.Configurations.Add(new EmployeeMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new OrderDetailMap());
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new TerritoryMap());

            ModelCreating(modelBuilder);
        }

        partial void ModelCreating(DbModelBuilder modelBuilder);
    }
}
