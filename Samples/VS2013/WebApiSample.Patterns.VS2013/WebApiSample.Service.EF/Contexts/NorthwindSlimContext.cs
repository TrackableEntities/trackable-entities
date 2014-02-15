using System.Data.Entity;
using WebApiSample.Service.Entities.Models;
using WebApiSample.Service.Entities.Models.Mapping;

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
        public IDbSet<Order> Orders { get; set; }
        public IDbSet<OrderDetail> OrderDetails { get; set; }
        public IDbSet<Product> Products { get; set; }

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
