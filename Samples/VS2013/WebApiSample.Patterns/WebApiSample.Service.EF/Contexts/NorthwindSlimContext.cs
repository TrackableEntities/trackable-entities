using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WebApiSample.Service.EF.Mapping;
using WebApiSample.Service.Entities.Models;

namespace WebApiSample.Service.EF.Contexts
{
    public partial class NorthwindSlimContext : DbContext, INorthwindSlimContext
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

        public IDbSet<Category> Categories { get; set; }
        public IDbSet<Customer> Customers { get; set; }
        public IDbSet<CustomerSetting> CustomerSettings { get; set; }
        public IDbSet<Order> Orders { get; set; }
        public IDbSet<OrderDetail> OrderDetails { get; set; }
        public IDbSet<Product> Products { get; set; }
        public IDbSet<Employee> Employees { get; set; }
        public IDbSet<Territory> Territories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new CustomerMap());
            modelBuilder.Configurations.Add(new CustomerSettingMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new OrderDetailMap());
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new EmployeeMap());
            modelBuilder.Configurations.Add(new TerritoryMap());

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}
