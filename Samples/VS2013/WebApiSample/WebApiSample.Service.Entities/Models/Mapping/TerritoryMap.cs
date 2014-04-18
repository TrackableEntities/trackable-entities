using System.Data.Entity.ModelConfiguration;

namespace WebApiSample.Service.Entities.Models.Mapping
{
    public class TerritoryMap : EntityTypeConfiguration<Territory>
    {
        public TerritoryMap()
        {
            // Primary Key
            this.HasKey(t => t.TerritoryId);

            // Properties
            this.Property(t => t.TerritoryId)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.TerritoryDescription)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            // Table & Column Mappings
            this.ToTable("Territory");
            this.Property(t => t.TerritoryId).HasColumnName("TerritoryId");
            this.Property(t => t.TerritoryDescription).HasColumnName("TerritoryDescription");

            // Tracking Properties
			this.Ignore(t => t.TrackingState);
			this.Ignore(t => t.ModifiedProperties);
        }
    }
}
