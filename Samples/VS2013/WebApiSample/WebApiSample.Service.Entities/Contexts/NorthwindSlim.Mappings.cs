using System;
using System.Data.Entity;
using WebApiSample.Service.Entities.Models;

namespace WebApiSample.Service.Entities.Contexts
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
