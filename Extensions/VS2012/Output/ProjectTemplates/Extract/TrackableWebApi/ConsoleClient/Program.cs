using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using TrackableEntities.Client;
//using $saferootprojectname$.Client.Entities.Models;

// This is an example which retrieves customers and orders from the Northwind
// sample database. You should alter the code based on your own database schema.

namespace $safeprojectname$
{
    class Program
    {
        static void Main(string[] args)
        {
            /* // Main method
            
            Console.WriteLine("Press Enter to start");
            Console.ReadLine();

            // Create http client
            // TODO: Replace with port from web project
            const string serviceBaseAddress = "http://localhost:" + "12345" + "/";
            var client = new HttpClient { BaseAddress = new Uri(serviceBaseAddress) };

            // Get customers
            Console.WriteLine("Customers:");
            IEnumerable<Customer> customers = GetCustomers(client);
            if (customers == null) return;
            foreach (var c in customers)
                PrintCustomer(c);

            // Get orders for a customer
            Console.WriteLine("\nGet customer orders {CustomerId}:");
            string customerId = Console.ReadLine();
            if (!customers.Any(c => string.Equals(c.CustomerId, customerId, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Invalid customer id: {0}", customerId.ToUpper());
                return;
            }
            IEnumerable<Order> orders = GetCustomerOrders(client, customerId);
            foreach (var o in orders)
                PrintOrder(o);

            // Get an order
            Console.WriteLine("\nGet an order {OrderId}:");
            int orderId = int.Parse(Console.ReadLine());
            if (!orders.Any(o => o.OrderId == orderId))
            {
                Console.WriteLine("Invalid order id: {0}", orderId);
                return;
            }
            Order order = GetOrder(client, orderId);
            PrintOrderWithDetails(order);

            // Create a new order
            Console.WriteLine("\nPress Enter to create a new order for {0}",
                customerId.ToUpper());
            Console.ReadLine();

            var newOrder = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.Today,
                ShippedDate = DateTime.Today.AddDays(1),
                OrderDetails = new ChangeTrackingCollection<OrderDetail>
                    {
                        new OrderDetail { ProductId = 1, Quantity = 5, UnitPrice = 10 },
                        new OrderDetail { ProductId = 2, Quantity = 10, UnitPrice = 20 },
                        new OrderDetail { ProductId = 4, Quantity = 40, UnitPrice = 40 }
                    }
            };
            var createdOrder = CreateOrder(client, newOrder);
            PrintOrderWithDetails(createdOrder);

            // Update the order
            Console.WriteLine("\nPress Enter to update order details");
            Console.ReadLine();

            // Start change-tracking the order
            var changeTracker = new ChangeTrackingCollection<Order>(createdOrder);

            // Modify order details
            createdOrder.OrderDetails[0].UnitPrice++;
            createdOrder.OrderDetails.RemoveAt(1);
            createdOrder.OrderDetails.Add(new OrderDetail
            {
                OrderId = createdOrder.OrderId,
                ProductId = 3,
                Quantity = 15,
                UnitPrice = 30
            });

            // Submit changes
            var changedOrder = changeTracker.GetChanges().SingleOrDefault();
            var updatedOrder = UpdateOrder(client, changedOrder);

            // Merge changes
            changeTracker.MergeChanges(updatedOrder);
            Console.WriteLine("Updated order:");
            PrintOrderWithDetails(createdOrder);

            // Delete the order
            Console.WriteLine("\nPress Enter to delete the order");
            Console.ReadLine();
            DeleteOrder(client, createdOrder);

            // Verify order was deleted
            var deleted = VerifyOrderDeleted(client, createdOrder.OrderId);
            Console.WriteLine(deleted ?
                "Order was successfully deleted" :
                "Order was not deleted");

            // Keep console open
            Console.WriteLine("Press any key to exit");
            Console.ReadKey(true);
            */
        }

        /* // Service methods
         
        private static IEnumerable<Customer> GetCustomers(HttpClient client)
        {
            const string request = "api/Customer";
            var response = client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<IEnumerable<Customer>>().Result;
            return result;
        }

        private static IEnumerable<Order> GetCustomerOrders
            (HttpClient client, string customerId)
        {
            string request = "api/Order?customerId=" + customerId;
            var response = client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<IEnumerable<Order>>().Result;
            return result;
        }

        private static Order GetOrder(HttpClient client, int orderId)
        {
            string request = "api/Order/" + orderId;
            var response = client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<Order>().Result;
            return result;
        }

        private static Order CreateOrder(HttpClient client, Order order)
        {
            string request = "api/Order";
            var response = client.PostAsJsonAsync(request, order).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<Order>().Result;
            return result;
        }

        private static Order UpdateOrder(HttpClient client, Order order)
        {
            string request = "api/Order";
            var response = client.PutAsJsonAsync(request, order).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<Order>().Result;
            return result;
        }

        private static void DeleteOrder(HttpClient client, Order order)
        {
            string request = "api/Order/" + order.OrderId;
            var response = client.DeleteAsync(request);
            response.Result.EnsureSuccessStatusCode();
        }

        private static bool VerifyOrderDeleted(HttpClient client, int orderId)
        {
            string request = "api/Order/" + orderId;
            var response = client.GetAsync(request).Result;
            if (response.IsSuccessStatusCode) return false;
            return true;
        }
        */

        /* // Helper methods
        
        private static void PrintCustomer(Customer c)
        {
            Console.WriteLine("{0} {1} {2} {3}",
                c.CustomerId,
                c.CompanyName,
                c.ContactName,
                c.City);
        }

        private static void PrintOrder(Order o)
        {
            Console.WriteLine("{0} {1}",
                o.OrderId,
                o.OrderDate.GetValueOrDefault().ToShortDateString());
        }

        private static void PrintOrderWithDetails(Order o)
        {
            Console.WriteLine("{0} {1}",
                o.OrderId,
                o.OrderDate.GetValueOrDefault().ToShortDateString());
            foreach (var od in o.OrderDetails)
            {
                Console.WriteLine("\t{0} {1} {2} {3}",
                    od.OrderDetailId,
                    od.Product.ProductName,
                    od.Quantity,
                    od.UnitPrice.ToString("c"));
            }
        }
        */
    }
}
