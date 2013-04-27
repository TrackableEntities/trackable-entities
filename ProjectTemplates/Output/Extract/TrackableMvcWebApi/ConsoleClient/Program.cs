using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using TrackableEntities.Client;
using $saferootprojectname$.ClientEntities.Models;

// TODO: Address for Web API service (replace port number)
// TODO: Replace 'Entities', 'Entity', 'EntityId, 'entity' with class name (for ex, Entity)

namespace $safeprojectname$
{
    class Program
    {
        static void Main()
        {
            const string serviceBaseAddress = "http://localhost:58238/";
            var client = new HttpClient 
                { BaseAddress = new Uri(serviceBaseAddress) };

            // Get entity
            Console.WriteLine("\nGet an entity {EntityId}:");
            int entityId = int.Parse(Console.ReadLine());
            Entity entity = GetEntity(client, entityId);

            // Create new entity
            Console.WriteLine("\nPress Enter to create a new entity");
            Console.ReadLine();
            var newEntity = new Entity
                {
                    EntityDetails = new ChangeTrackingCollection<EntityDetail>
                        {
                            new EntityDetail { Quantity = 5, UnitPrice = 10 },
                            new EntityDetail { Quantity = 10, UnitPrice = 20 }
                        }
                };
            var createdEntity = CreateEntity(client, newEntity);

            // Update entity
            Console.WriteLine("\nPress Enter to update entity");
            Console.ReadLine();

            // Start change-tracking the entity
            var changeTracker = new ChangeTrackingCollection<Entity>(createdEntity);

            // Modify entity details
            createdEntity.EntityDetails[0].UnitPrice++;
            createdEntity.EntityDetails.RemoveAt(1);
            createdEntity.EntityDetails.Add(new EntityDetail
                {
                    EntityId = createdEntity.EntityId,
                    Quantity = 15,
                    UnitPrice = 30
                });

            // Submit changes
            var changedEntity = changeTracker.GetChanges().SingleOrDefault();
            var updatedEntity = UpdateEntity(client, changedEntity);

            // Delete the entity
            Console.WriteLine("\nPress Enter to delete the entity");
            Console.ReadLine();
            DeleteEntity(client, updatedEntity);

            // Verify entity was deleted
            var deleted = VerifyEntityDeleted(client, updatedEntity.EntityId);
            Console.WriteLine(deleted ? 
                "Entity was successfully deleted" : 
                "Entity was not deleted");

            // Keep console open
            Console.WriteLine("Press any key to exit");
            Console.ReadKey(true);
        }

        private static Entity GetEntity(HttpClient client, int entityId)
        {
            string request = "api/Entities/" + entityId;
            var response = client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<Entity>().Result;
            return result;
        }

        private static Entity CreateEntity(HttpClient client, Entity entity)
        {
            string request = "api/Entities";
            var response = client.PostAsJsonAsync(request, entity).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<Entity>().Result;
            return result;
        }

        private static Entity UpdateEntity(HttpClient client, Entity entity)
        {
            string request = "api/Entities";
            var response = client.PutAsJsonAsync(request, entity).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<Entity>().Result;
            return result;
        }

        private static void DeleteEntity(HttpClient client, Entity entity)
        {
            string request = "api/Entities/" + entity.EntityId;
            var response = client.DeleteAsync(request);
            response.Result.EnsureSuccessStatusCode();
        }

        private static bool VerifyEntityDeleted(HttpClient client, int id)
        {
            string request = "api/Entities/" + id;
            var response = client.GetAsync(request).Result;
            if (response.IsSuccessStatusCode) return false;
            return true;
        }
    }
}
