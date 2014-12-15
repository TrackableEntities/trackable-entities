using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackableEntities.Client;
using TrackableEntities.EF.Tests.Edmx;

namespace TrackableEntities.EF.Tests.Mocks
{
    public class MockProducsDbFirst
    {
        public List<CultureCodes> CultureCodes { get; private set; }
        public List<Product> Products { get; private set; }
        public List<Localization> Localizations { get; private set; }
        public List<ResourceKey> ResourceKeys { get; private set; }

        public MockProducsDbFirst()
        {
            CultureCodes = new List<CultureCodes>
            {
                new CultureCodes{culture_codes_id = 1, culture_codes_name = "en-US",TrackingState = TrackingState.Unchanged},
                new CultureCodes{culture_codes_id = 2, culture_codes_name = "en-EN",TrackingState = TrackingState.Unchanged},
                new CultureCodes{culture_codes_id = 3, culture_codes_name = "de-DE",TrackingState = TrackingState.Unchanged},

            };

            Localizations = new List<Localization>
            {
                new Localization{ localization_id = 1, fk_resource_key = "titleResource1",fk_culture_codes_id = 1,  resource_value = "first Title Resource", TrackingState = TrackingState.Unchanged},
                new Localization{ localization_id = 2, fk_resource_key = "descriptionResource1",fk_culture_codes_id = 1, resource_value = "second Title Resource", TrackingState = TrackingState.Unchanged}
            };

            ResourceKeys = new List<ResourceKey>
            {
                new ResourceKey{ resource_key_id = 1, resource_key_name = "titleResource1" ,  localization = new ChangeTrackingCollection<Localization>(new List<Localization>{Localizations[0]}), TrackingState = TrackingState.Unchanged},
                new ResourceKey{ resource_key_id = 2, resource_key_name = "descriptionResource1", localization = new ChangeTrackingCollection<Localization>(new List<Localization>{Localizations[1]}), TrackingState = TrackingState.Unchanged}
            };

            Products= new List<Product>
            {
                new Product{ product_id = 1, product_title = "titleResource1", product_description = "descriptionResource1",
                    title_resource = ResourceKeys[0], description_resource = ResourceKeys[1], TrackingState = TrackingState.Unchanged}
            };
            
        }
    }

}
