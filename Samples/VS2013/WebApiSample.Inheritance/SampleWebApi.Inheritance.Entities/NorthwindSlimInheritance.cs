namespace SampleWebApi.Inheritance.Entities
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class NorthwindSlimInheritance : DbContext
    {
        static NorthwindSlimInheritance()
        {
            // To re-create database, delete the database then uncomment:
            //Database.SetInitializer(new NullDatabaseInitializer<NorthwindSlimInheritance>());
            Database.SetInitializer(new NorthwindSlimDatabaseInitializer());
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
