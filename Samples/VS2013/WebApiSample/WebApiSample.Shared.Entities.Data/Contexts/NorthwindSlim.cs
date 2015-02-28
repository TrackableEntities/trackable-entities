using System.Data.Entity;
using WebApiSample.Shared.Entities.Data.Models.Mapping;
using WebApiSample.Shared.Entities.Portable.Models;

namespace WebApiSample.Shared.Entities.Data.Contexts
{
    public partial class NorthwindSlim : DbContext
    {
        static NorthwindSlim()
        {
            Database.SetInitializer<NorthwindSlim>(null);
        }

        public NorthwindSlim()
            : base("Name=NorthwindSlimContext")
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerSetting> CustomerSettings { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Territory> Territories { get; set; }

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
        }
    }
}
