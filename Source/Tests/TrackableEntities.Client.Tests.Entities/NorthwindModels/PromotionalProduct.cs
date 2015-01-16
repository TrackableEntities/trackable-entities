using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class PromotionalProduct : Product, IEquatable<PromotionalProduct>
    {
        private string _giftCode;
        public string PromoCode
        {
            get { return _giftCode; }
            set
            {
                if (value == _giftCode) return;
                _giftCode = value;
                NotifyPropertyChanged(this, m => m.PromoCode);
            }
        }

        bool IEquatable<PromotionalProduct>.Equals(PromotionalProduct other)
        {
            // Delegate to base IEquatable<Product>
            return ((IEquatable<Product>)this).Equals(other);
        }
    }
}
