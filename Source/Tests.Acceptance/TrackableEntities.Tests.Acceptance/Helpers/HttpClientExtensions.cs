using System;
using System.Collections.Generic;
using System.Net.Http;

namespace TrackableEntities.Tests.Acceptance.Helpers
{
    internal static class HttpClientExtensions
    {
        public static TEntity GetEntity<TEntity, TKey>(this HttpClient client, TKey id)
        {
            string request = string.Format("api/{0}/{1}", typeof(TEntity).Name, id);
            var response = client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<TEntity>().Result;
            return result;
        }

        public static IEnumerable<TEntity> GetEntities<TEntity>(this HttpClient client)
        {
            string request = string.Format("api/{0}", typeof(TEntity).Name);
            var response = client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<IEnumerable<TEntity>>().Result;
            return result;
        }

        public static TEntity CreateEntity<TEntity>(this HttpClient client, TEntity entity)
        {
            string request = string.Format("api/{0}", typeof(TEntity).Name);
            var response = client.PostAsJsonAsync(request, entity).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<TEntity>().Result;
            return result;
        }

        public static TEntity UpdateTEntity<TEntity>(this HttpClient client, TEntity entity)
        {
            string request = string.Format("api/{0}", typeof(TEntity).Name);
            var response = client.PutAsJsonAsync(request, entity).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<TEntity>().Result;
            return result;
        }

        public static void DeleteTEntity<TEntity, TKey>(this HttpClient client, TKey id)
        {
            string request = string.Format("api/{0}/{1}", typeof(TEntity).Name, id);
            var response = client.DeleteAsync(request);
            response.Result.EnsureSuccessStatusCode();
        }

        public static bool VerifyEntityDeleted<TEntity, TKey>(this HttpClient client, TKey id)
        {
            string request = string.Format("api/{0}/{1}", typeof(TEntity).Name, id);
            var response = client.GetAsync(request).Result;
            if (response.IsSuccessStatusCode) return false;
            return true;
        }
    }
}
