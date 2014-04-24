
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiSample.Service.Entities.Models.Mapping
{
    public class CustomerSettingMap : EntityTypeConfiguration<CustomerSetting>
    {
        public CustomerSettingMap()
        {
            // Primary Key
            this.HasKey(t => t.CustomerId);

            // Properties
            this.Property(t => t.CustomerId)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(5);

            this.Property(t => t.Setting)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CustomerSetting");
            this.Property(t => t.CustomerId).HasColumnName("CustomerId");
            this.Property(t => t.Setting).HasColumnName("Setting");

            // Relationships
            this.HasRequired(t => t.Customer)
                .WithOptional(t => t.CustomerSetting);

            // Tracking Properties
			this.Ignore(t => t.TrackingState);
			this.Ignore(t => t.ModifiedProperties);
        }
    }
}
