
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace WcfSample.Client.Entities.Models.Mapping
{
    public class EmployeeMap : EntityTypeConfiguration<Employee>
    {
        public EmployeeMap()
        {
            // Primary Key
            this.HasKey(t => t.EmployeeId);

            // Properties
            this.Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.City)
                .HasMaxLength(15);

            this.Property(t => t.Country)
                .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("Employee");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.BirthDate).HasColumnName("BirthDate");
            this.Property(t => t.HireDate).HasColumnName("HireDate");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.Country).HasColumnName("Country");

            // Tracking Properties
			this.Ignore(t => t.TrackingState);
			this.Ignore(t => t.ModifiedProperties);
			this.Ignore(t => t.EntityIdentifier);

            // Relationships
            this.HasMany(t => t.Territories)
                .WithMany(t => t.Employees)
                .Map(m =>
                    {
                        m.ToTable("EmployeeTerritories");
                        m.MapLeftKey("EmployeeId");
                        m.MapRightKey("TerritoryId");
                    });


        }
    }
}
