#if EF_6
using System.Data.Entity;
#else
using System.Data;
#endif
using System.Collections.Generic;
using System.Linq;
using TrackableEntities.EF.Tests;
using TrackableEntities.EF.Tests.Mocks;
using TrackableEntities.EF.Tests.NorthwindModels;
using Xunit;

// ReSharper disable CheckNamespace
#if EF_6
namespace TrackableEntities.EF6.Tests
#else
namespace TrackableEntities.EF5.Tests
#endif
{
    public class StateInterceptorTests
    {
        // ReSharper disable once InconsistentNaming
        private const CreateDbOptions CreateNorthwindDbOptions = CreateDbOptions.DropCreateDatabaseIfModelChanges;

        [Fact]
        public void Interceptor_Should_Change_Entities_States()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            int[] ids = {1, 2, 3};
            var products = nw.Products.Where(e => ids.Contains(e.ProductId)).ToList();
            products.ForEach(e => e.TrackingState = TrackingState.Modified);
            var cache = new Dictionary<int, EntityState>
            {
                { 1, EntityState.Added },
                { 2, EntityState.Deleted },
                { 3, EntityState.Unchanged },
            };

            // Act
            context
                .WithInterceptor(new ProductCacheStateInterceptor(cache))
                .ApplyChanges(products);

            // Assert
            Assert.Equal(EntityState.Added, context.Entry(products[0]).State);
            Assert.Equal(EntityState.Deleted, context.Entry(products[1]).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(products[2]).State);
        }

        [Fact]
        public void Interceptor_Should_Not_Change_Entities_States()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            int[] ids = { 4, 5, 6 };
            var products = nw.Products.Where(e => ids.Contains(e.ProductId)).ToList();
            products.ForEach(e => e.TrackingState = TrackingState.Modified);
            var cache = new Dictionary<int, EntityState>
            {
                { 1, EntityState.Added },
                { 2, EntityState.Deleted },
                { 3, EntityState.Unchanged },
            };

            // Act
            context
                .WithInterceptor(new ProductCacheStateInterceptor(cache))
                .ApplyChanges(products);

            // Assert
            Assert.Equal(EntityState.Modified, context.Entry(products[0]).State);
            Assert.Equal(EntityState.Modified, context.Entry(products[1]).State);
            Assert.Equal(EntityState.Modified, context.Entry(products[2]).State);
        }

        #region IStateInterceptor Implementations

        // Look up entity states in a products cache
        private class ProductCacheStateInterceptor : IStateInterceptor
        {
            private readonly Dictionary<int, EntityState> _cache;

            public ProductCacheStateInterceptor(Dictionary<int, EntityState> cache)
            {
                _cache = cache;
            }
             
            public EntityState? GetEntityState(ITrackable item, RelationshipType? relationshipType)
            {
                var product = item as Product;
                if (product == null) return null;

                EntityState state;
                if (_cache.TryGetValue(product.ProductId, out state))
                    return state;
                return null;
            }
        }

        #endregion
    }
}
