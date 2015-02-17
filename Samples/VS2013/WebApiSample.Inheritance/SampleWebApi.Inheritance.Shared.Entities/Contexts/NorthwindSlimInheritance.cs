using System.Data.Entity;
using SampleWebApi.Inheritance.Shared.Entities.Models;

namespace SampleWebApi.Inheritance.Shared.Entities.Contexts
{
    public partial class NorthwindSlimInheritance : DbContext
    {
        static NorthwindSlimInheritance()
        {
            // To seed database, delete the database then uncomment:
            //Database.SetInitializer(new NorthwindSlimDatabaseInitializer());

            Database.SetInitializer(new NullDatabaseInitializer<NorthwindSlimInheritance>());
        }

        public NorthwindSlimInheritance()
            : base("name=NorthwindSlimInheritance")
        {
            Configuration.ProxyCreationEnabled = false;
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(e => e.UnitPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Product>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            ModelCreating(modelBuilder);
        }

        partial void ModelCreating(DbModelBuilder modelBuilder);
    }
}
