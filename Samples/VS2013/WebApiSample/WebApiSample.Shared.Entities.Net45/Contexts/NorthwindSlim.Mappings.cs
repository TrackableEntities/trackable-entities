using System.Data.Entity;
using WebApiSample.Shared.Entities.Models;

namespace WebApiSample.Shared.Entities.Contexts
{
    public partial class NorthwindSlim
    {
        partial void ModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerSetting>()
                .HasKey(e => e.CustomerId);
        }
    }
}
