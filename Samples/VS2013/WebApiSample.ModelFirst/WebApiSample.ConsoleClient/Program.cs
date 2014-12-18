using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using TrackableEntities;
using TrackableEntities.Client;
using WebApiSample.Client.Entities.Temp;

namespace WebApiSample.Client.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Press Enter to start");
            Console.ReadLine();

            // TODO: Address for Web API service (replace port number)
            const string serviceBaseAddress = "http://localhost:" + "58527" + "/";
            var client = new HttpClient { BaseAddress = new Uri(serviceBaseAddress) };

            // Get customers
            Console.WriteLine("Customers:");
            IEnumerable<Customer> customers = GetCustomers(client);
            if (customers == null) return;
            foreach (var c in customers)
                PrintCustomer(c);

            // Get customer
            Console.WriteLine("\nGet Customer {CustomerId}:");
            string customerId = Console.ReadLine();
            Customer customer = GetCustomer(client, customerId);
            if (customer == null) return;
            PrintCustomer(customer);

            Console.WriteLine("\nUpdate Customer City:");
            string city = Console.ReadLine();
            customer.Location.City = city;

            // Manually set tracking state and modified properties
            customer.TrackingState = TrackingState.Modified;
            customer.ModifiedProperties = new Collection<string> { "Location" };

            // Update customer city
            customer = UpdateCustomer(client, customer);
            if (customer == null) return;
            PrintCustomer(customer);

            // Get orders for a customer
            Console.WriteLine("\nGet customer orders:");
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
        }

        private static IEnumerable<Customer> GetCustomers(HttpClient client)
        {
            const string request = "api/Customer";
            var response = client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<IEnumerable<Customer>>().Result;
            return result;
        }

        private static Customer GetCustomer(HttpClient client, string customerId)
        {
            string request = "api/Customer/" + customerId;
            var response = client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<Customer>().Result;
            return result;
        }

        private static Customer UpdateCustomer(HttpClient client, Customer customer)
        {
            const string request = "api/Customer";
            var response = client.PutAsJsonAsync(request, customer).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<Customer>().Result;
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

        private static void PrintCustomer(Customer c)
        {
            Console.WriteLine("{0} {1} {2} {3}",
                c.CustomerId,
                c.CompanyName,
                c.ContactName,
                c.Location.City);
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
    }
}
