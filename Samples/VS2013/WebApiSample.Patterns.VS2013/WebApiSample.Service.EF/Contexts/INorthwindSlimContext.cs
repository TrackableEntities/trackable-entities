using System;
using System.Data.Entity;
using WebApiSample.Service.Entities.Models;

namespace WebApiSample.Service.EF.Contexts
{
    public interface INorthwindSlimContext
    {
        IDbSet<Category> Categories { get; set; }
        IDbSet<Customer> Customers { get; set; }
        IDbSet<CustomerSetting> CustomerSettings { get; set; }
        IDbSet<Order> Orders { get; set; }
        IDbSet<OrderDetail> OrderDetails { get; set; }
        IDbSet<Product> Products { get; set; }
        IDbSet<Employee> Employees { get; set; }
        IDbSet<Territory> Territories { get; set; }
    }
}
