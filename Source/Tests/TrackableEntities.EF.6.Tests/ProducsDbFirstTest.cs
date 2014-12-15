using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TrackableEntities.Client;
using TrackableEntities.EF.Tests;
using TrackableEntities.EF.Tests.Edmx;
using TrackableEntities.EF.Tests.Mocks;
using TrackableEntities.EF6;

#if EF_6
namespace TrackableEntities.EF6.Tests
#else
namespace TrackableEntities.EF5.Tests
#endif
{
    [TestFixture]
    public class ProducsDbFirstTest
    {
        [Test]
        public void Apply_Changes_Should_Attach_Grandchilds_to_DbContext()
        {
            // Arrange
            var context = new ProductsModelContext();

            var mockProducts = new MockProducsDbFirst();
            

            var product = mockProducts.Products[0];

            var titleResKey = product.product_title;
            var descResKey = product.product_description;

            var tracker = new ChangeTrackingCollection<Product>(product);

            product.title_resource.localization.Add(new Localization { fk_culture_codes_id = 2, fk_resource_key = titleResKey, resource_value = "second localized title resource"});
            product.description_resource.localization.Add(new Localization { fk_culture_codes_id = 2, fk_resource_key = descResKey, resource_value = "second localized description resource"});

            var changes = tracker.GetChanges().SingleOrDefault();


            // Act
            context.ApplyChanges(changes);

            // Assert
            Assert.AreEqual(EntityState.Unchanged, context.Entry(product).State);
            
        }

    }
}
