using System;
using TrackableEntities.Client;
using TrackableEntities.EF.Tests.NorthwindModels;

namespace TrackableEntities.Tests.Acceptance.Helpers
{
    internal static class EntityExtensions
    {
        public static ClientEntities.Category ToClientEntity(this Category category)
        {
            if (category == null) return null;
            return new ClientEntities.Category
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            };
        }

        public static ClientEntities.Product ToClientEntity(this Product product)
        {
            if (product == null) return null;
            return new ClientEntities.Product
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                UnitPrice = product.UnitPrice,
                CategoryId = product.CategoryId,
                Category = product.Category.ToClientEntity()
            };
        }

        public static ClientEntities.CustomerSetting ToClientEntity(this CustomerSetting setting)
        {
            if (setting == null) return null;
            return new ClientEntities.CustomerSetting
            {
                CustomerId = setting.CustomerId,
                Setting = setting.Setting
            };
        }

        public static ClientEntities.Customer ToClientEntity(this Customer customer)
        {
            if (customer == null) return null;
            return new ClientEntities.Customer
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.CustomerName,
                CustomerSetting = customer.CustomerSetting.ToClientEntity()
            };
        }

        public static ClientEntities.OrderDetail ToClientEntity(this OrderDetail detail, ClientEntities.Order order)
        {
            if (detail == null) return null;
            return new ClientEntities.OrderDetail
            {
                OrderDetailId = detail.OrderDetailId,
                OrderId = detail.OrderId,
                ProductId = detail.ProductId,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                Order = order
            };
        }

        public static ClientEntities.Order ToClientEntity(this Order order)
        {
            if (order == null) return null;
            var clientOrder = new ClientEntities.Order
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                Customer = order.Customer.ToClientEntity()
            };
            if (order.OrderDetails != null)
            {
                clientOrder.OrderDetails = new ChangeTrackingCollection<ClientEntities.OrderDetail>();
                foreach (var detail in order.OrderDetails)
                {
                    if (detail != null)
                        clientOrder.OrderDetails.Add(detail.ToClientEntity(clientOrder));
                }
            }
            return clientOrder;
        }

        public static ClientEntities.Order CreateNewOrder(string customerId, int[] productIds)
        {
            var clientOrder = new ClientEntities.Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.Today, 
                OrderDetails = new ChangeTrackingCollection<ClientEntities.OrderDetail>
                    {
                        new ClientEntities.OrderDetail { ProductId = productIds[0], Quantity = 5, UnitPrice = 10 },
                        new ClientEntities.OrderDetail { ProductId = productIds[1], Quantity = 10, UnitPrice = 20 },
                        new ClientEntities.OrderDetail { ProductId = productIds[2], Quantity = 40, UnitPrice = 40 }
                    }
            };
            return clientOrder;
        }
    }
}
