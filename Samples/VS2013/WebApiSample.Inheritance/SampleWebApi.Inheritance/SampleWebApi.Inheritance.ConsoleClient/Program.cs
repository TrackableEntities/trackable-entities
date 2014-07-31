using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SampleWebApi.Inheritance.Entities;
using TrackableEntities;
using TrackableEntities.Client;

namespace SampleWebApi.Inheritance.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Main method

            Console.WriteLine("Press Enter to start");
            Console.ReadLine();

            // Create http client
            const string serviceBaseAddress = "http://localhost:" + "49640" + "/";
            var client = new HttpClient { BaseAddress = new Uri(serviceBaseAddress) };

            List<Product> products = GetProducts(client).ToList();
            foreach (var p in products)
                PrintProduct(p);

            // Select individual product
            Console.WriteLine("\nProduct to update {ProductId}:");
            int productId = int.Parse(Console.ReadLine());
            var product = products.Single(p => p.ProductId == productId);

            // Track changes (does not currently support inheritance
            //var changeTracker = new ChangeTrackingCollection<Product>(product);

            // Update product
            product.UnitPrice++;

            // Get changes (does not currently support inheritance)
            //var changedProduct = changeTracker.GetChanges().SingleOrDefault();
            product.TrackingState = TrackingState.Modified;
            product.ModifiedProperties = new List<string> {"UnitPrice"};

            // Submit changes
            Product updatedProduct;
            if (product is DiscontinuedProduct)
                updatedProduct = UpdateEntity(client, (DiscontinuedProduct)product);
            else
                updatedProduct = UpdateEntity(client, product);

            // Merge changes (does not currently support inheritance)
            //changeTracker.MergeChanges(updatedProduct);
            product.UnitPrice = updatedProduct.UnitPrice;
            product.RowVersion = updatedProduct.RowVersion;

            Console.WriteLine("Updated product:");
            PrintProduct(product);

            // Keep console open
            Console.WriteLine("\nPress any key to exit");
            Console.ReadKey(true);
        }

        private static IEnumerable<Category> GetCategories(HttpClient client)
        {
            const string request = "api/Category";
            var response = client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<IEnumerable<Category>>().Result;
            return result;
        }

        private static IEnumerable<Product> GetProducts
            (HttpClient client)
        {
            string request = "api/Product";
            var response = client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var result = ReadContentAsAsync<IEnumerable<Product>>(response).Result;
            return result;
        }

        private static T UpdateEntity<T>(HttpClient client, T entity)
        {
            string request = "api/Product";
            var content = GetObjectContent(entity);
            var response = client.PutAsync(request, content).Result;
            response.EnsureSuccessStatusCode();

            var result = ReadContentAsAsync<T>(response).Result;
            return result;
        }

        private static async Task<T> ReadContentAsAsync<T>(HttpResponseMessage response)
        {
            var jsonFormatter = new JsonMediaTypeFormatter { SerializerSettings = { TypeNameHandling = TypeNameHandling.Auto } };
            var result = await response.Content.ReadAsAsync<T>(new MediaTypeFormatter[] { jsonFormatter });
            return result;
        }

        private static ObjectContent<T> GetObjectContent<T>(T entity)
        {
            var jsonFormatter = new JsonMediaTypeFormatter { SerializerSettings = { TypeNameHandling = TypeNameHandling.Auto } };
            var content = new ObjectContent<T>(entity, jsonFormatter);
            return content;
        }

        private static void PrintCategory(Category c)
        {
            Console.WriteLine("{0} {1}",
                c.CategoryId,
                c.CategoryName);
        }

        private static void PrintProduct(Product p)
        {
            // See if this is a discontinued product
            var discProduct = p as DiscontinuedProduct;

            Console.WriteLine("{0} {1} {2} {3}",
                p.ProductId,
                p.ProductName,
                p.UnitPrice.GetValueOrDefault().ToString("C"),
                discProduct != null
                    ? string.Format("Discontinued: {0}",
                    discProduct.DiscontinuedDate.GetValueOrDefault().ToShortDateString())
                    : string.Empty);
        }
    }
}
