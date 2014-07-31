using System.Data.Entity;
using SampleWebApi.Inheritance.Client.Entities.Models;
using SampleWebApi.Inheritance.Service.Entities.Models.Mapping;

namespace SampleWebApi.Inheritance.Service.Entities.Models
{
    public partial class NorthwindSlimInheritanceContext : DbContext
    {
        static NorthwindSlimInheritanceContext()
        {
            Database.SetInitializer<NorthwindSlimInheritanceContext>(null);
        }

        public NorthwindSlimInheritanceContext()
            : base("Name=NorthwindSlimInheritanceContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new ProductMap());
        }
    }
}
