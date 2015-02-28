
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace WcfSample.Client.Entities.Models.Mapping
{
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            // Primary Key
            this.HasKey(t => t.OrderId);

            // Properties
            this.Property(t => t.CustomerId)
                .IsFixedLength()
                .HasMaxLength(5);

            // Table & Column Mappings
            this.ToTable("Order");
            this.Property(t => t.OrderId).HasColumnName("OrderId");
            this.Property(t => t.CustomerId).HasColumnName("CustomerId");
            this.Property(t => t.OrderDate).HasColumnName("OrderDate");
            this.Property(t => t.ShippedDate).HasColumnName("ShippedDate");
            this.Property(t => t.ShipVia).HasColumnName("ShipVia");
            this.Property(t => t.Freight).HasColumnName("Freight");

            // Tracking Properties
			this.Ignore(t => t.TrackingState);
			this.Ignore(t => t.ModifiedProperties);
			this.Ignore(t => t.EntityIdentifier);

            // Relationships
            this.HasOptional(t => t.Customer)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.CustomerId);

        }
    }
}
