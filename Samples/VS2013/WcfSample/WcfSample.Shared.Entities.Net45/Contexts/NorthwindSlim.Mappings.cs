using System.Data.Entity;
using WcfSample.Shared.Entities.Net45.Models;

namespace WcfSample.Shared.Entities.Net45.Contexts
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
