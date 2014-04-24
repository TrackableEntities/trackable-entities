using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using TrackableEntities;
using TrackableEntities.Client;
using TrackableEntities.Common;
using WebApiSample.Client.Entities.Models;

namespace WebApiSample.Client.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Press Enter to start");
            Console.ReadLine();

            // TODO: Address for Web API service (replace port number)
            const string serviceBaseAddress = "http://localhost:" + "58540" + "/";
            var client = new HttpClient { BaseAddress = new Uri(serviceBaseAddress) };

            // Perform updates on a 1-1 property
            Console.WriteLine("\nPART A: One-to-One Relation: Customer.CustomerSetting");
            Console.WriteLine("Update 1-1 relation property? {Y/N}");
            if (Console.ReadLine().ToUpper() == "Y")
                OneToOneRelation(client);

            // Perform updates on a M-1 property
            Console.WriteLine("\nPART B: Many-to-One Relation: Order.Customer");
            Console.WriteLine("Update M-1 relation property? {Y/N}");
            if (Console.ReadLine().ToUpper() == "Y")
                ManyToOneRelation(client);

            // Keep console open
            Console.WriteLine("Press any key to exit");
            Console.ReadKey(true);
        }

        private static void OneToOneRelation(HttpClient client)
        {
            Console.WriteLine("\nPress Enter to create a new customer");
            Console.ReadLine();

            // Create a new customer
            var customer = new Customer
            {
                CustomerId = "ABCDE",
                CompanyName = "Acme Company",
                ContactName = "John Doe",
                City = "Dallas",
                Country = "USA"
            };

            // Add to change tracker to mark as Added
            var customerChangeTracker = new ChangeTrackingCollection<Customer>(true) { customer };

            // Insert customer and merge
            Customer updatedCustomer = CreateEntity(client, customer);
            customerChangeTracker.MergeChanges(updatedCustomer);
            PrintCustomer(customer);

            Console.WriteLine("\nPress Enter to add a new setting");
            Console.ReadLine();

            // Add a new customer setting
            // NOTE: To add a 1-1 property, you must manually mark it as Added. 
            customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Setting = " Test Setting",
                TrackingState = TrackingState.Added // Mark as added
            };

            // Update customer, then accept changes
            updatedCustomer = UpdateTEntity(client, customer);
            Console.WriteLine("\tCustomer setting added: {0}",
                updatedCustomer.CustomerSetting != null);
            customer.AcceptChanges();
            PrintCustomer(customer);

            // Add new customer setting
            Console.WriteLine("\nPress Enter to modify the setting");
            Console.ReadLine();

            // Modify customer setting
            var newSetting = customer.CustomerSetting.Setting += " - Changed";

            // Update customer, then accept changes
            updatedCustomer = UpdateTEntity(client, customer);
            Console.WriteLine("\tCustomer setting modified: {0}", 
                updatedCustomer.CustomerSetting.Setting == newSetting);
            customer.AcceptChanges();
            PrintCustomer(customer);

            // Remove existing customer setting
            Console.WriteLine("\nPress Enter to remove the existing setting");
            Console.ReadLine();

            // Delete existing customer setting
            // NOTE: To remove a 1-1 property, you must manually mark it as Deleted. 
            customer.CustomerSetting.TrackingState = TrackingState.Deleted;

            // Update customer, then set 1-1 property to null
            updatedCustomer = UpdateTEntity(client, customer);
            Console.WriteLine("\tCustomer setting removed: {0}",
                updatedCustomer.CustomerSetting == null);
            customer.CustomerSetting = null;
            PrintCustomer(customer);

            // Delete the customer
            Console.WriteLine("\nPress Enter to delete the customer");
            Console.ReadLine();
            DeleteTEntity<Customer, string>(client, customer.CustomerId);

            // Verify order was deleted
            var deleted = VerifyEntityDeleted<Customer, string>(client, customer.CustomerId);
            Console.WriteLine(deleted ?
                "Customer was successfully deleted" :
                "Customer was NOT deleted");
        }

        private static void ManyToOneRelation(HttpClient client)
        {
            Console.WriteLine("\nPress Enter to create a new order for an existing customer");
            Console.ReadLine();

            // Create a new order for an existing customer
            var order = new Order {CustomerId = "ALFKI", OrderDate = DateTime.Today, ShippedDate = DateTime.Today.AddDays(1)};

            // Add to change tracker to mark as Added
            var orderChangeTracker = new ChangeTrackingCollection<Order>(true) {order};

            // Insert order and merge
            Order updatedOrder = CreateEntity(client, order);
            orderChangeTracker.MergeChanges(updatedOrder);
            PrintOrder(order);

            Console.WriteLine("\nPress Enter to add a new customer to the order");
            Console.ReadLine();

            // Create a new customer
            // NOTE: To add a M-1 property, you must manually mark it as Added.
            const string customerId = "WXYZ";
            var customer = new Customer
            {
                CustomerId = customerId,
                CompanyName = "Widget Company",
                ContactName = "Jane Doe",
                City = "New York",
                Country = "USA",
                TrackingState = TrackingState.Added // Mark as added
            };
            order.Customer = customer; // new customer will be created
            order.CustomerId = customerId; // cust will be assigned to order

            // Update order, then accept changes
            updatedOrder = UpdateTEntity(client, order);
            var updatedCustomer = GetEntity<Customer, string>(client, customerId);
            Console.WriteLine("\tOrder's customer added: {0}", updatedCustomer != null);
            order.AcceptChanges();
            PrintOrder(order);
            PrintCustomer(order.Customer);

            Console.WriteLine("\nPress Enter to modify order's customer");
            Console.ReadLine();

            // Modify order customer
            var newCompanyName = order.Customer.CompanyName += " - Changed";

            // Update order, then accept changes
            UpdateTEntity(client, order);
            updatedCustomer = GetEntity<Customer, string>(client, customerId);
            Console.WriteLine("\tOrder customer's name modified: {0}",
                updatedCustomer.CompanyName == newCompanyName);
            customer.AcceptChanges();
            PrintCustomer(customer);

            // Delete the order and customer
            Console.WriteLine("\nPress Enter to delete the order and customer");
            Console.ReadLine();

            // Delete order and verify
            DeleteTEntity<Order, int>(client, order.OrderId);
            var orderDeleted = VerifyEntityDeleted<Order, int>(client, order.OrderId);
            Console.WriteLine(orderDeleted ?
                "Order was successfully deleted" :
                "Order was NOT deleted");

            // Delete order and verify
            DeleteTEntity<Customer, string>(client, customer.CustomerId);
            var customerDeleted = VerifyEntityDeleted<Customer, string>(client, customer.CustomerId);
            Console.WriteLine(customerDeleted ?
                "Customer was successfully deleted" :
                "Customer was NOT deleted");
        }

        #region Helper Methods

        private static IEnumerable<TEntity> GetEntities<TEntity>(HttpClient client)
        {
            string request = string.Format("api/{0}", typeof(TEntity).Name);
            var response = client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<IEnumerable<TEntity>>().Result;
            return result;
        }

        private static TEntity GetEntity<TEntity, TKey>(HttpClient client, TKey id)
        {
            string request = string.Format("api/{0}/{1}", typeof(TEntity).Name, id);
            var response = client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<TEntity>().Result;
            return result;
        }

        private static TEntity CreateEntity<TEntity>(HttpClient client, TEntity entity)
        {
            string request = string.Format("api/{0}", typeof(TEntity).Name);
            var response = client.PostAsJsonAsync(request, entity).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<TEntity>().Result;
            return result;
        }

        private static TEntity UpdateTEntity<TEntity>(HttpClient client, TEntity entity)
        {
            string request = string.Format("api/{0}", typeof(TEntity).Name);
            var response = client.PutAsJsonAsync(request, entity).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<TEntity>().Result;
            return result;
        }

        private static void DeleteTEntity<TEntity, TKey>(HttpClient client, TKey id)
        {
            string request = string.Format("api/{0}/{1}", typeof(TEntity).Name, id);
            var response = client.DeleteAsync(request);
            response.Result.EnsureSuccessStatusCode();
        }

        private static bool VerifyEntityDeleted<TEntity, TKey>(HttpClient client, TKey id)
        {
            string request = string.Format("api/{0}/{1}", typeof(TEntity).Name, id);
            var response = client.GetAsync(request).Result;
            if (response.IsSuccessStatusCode) return false;
            return true;
        }

        private static void PrintCustomer(Customer c)
        {
            Console.WriteLine("\t{0} {1} {2} {3} {4}",
                c.CustomerId,
                c.CompanyName,
                c.ContactName,
                c.City,
                c.CustomerSetting != null ? c.CustomerSetting.Setting : string.Empty);
        }

        private static void PrintOrder(Order o)
        {
            Console.WriteLine("\t{0} {1}",
                o.OrderId,
                o.OrderDate.GetValueOrDefault().ToShortDateString());
        }

        #endregion
    }
}
