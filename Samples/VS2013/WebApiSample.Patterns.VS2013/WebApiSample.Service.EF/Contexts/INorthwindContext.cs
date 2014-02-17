using System;
using System.Data.Entity;

// TODO: Alter DbContext class to implement this interface and change
// each DbSet property to IDbSet
using WebApiSample.Service.Entities.Models;

namespace WebApiSample.Service.EF.Contexts
{
    public interface INorthwindContext
    {
        // TODO: Add IDbSet<Entity> properties for each entity set on the DbContext class
        IDbSet<Product> Products { get; set; }
    }
}
