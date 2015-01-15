using System;
using Newtonsoft.Json;

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
                NotifyPropertyChanged(() => PromoCode);
            }
        }

        bool IEquatable<PromotionalProduct>.Equals(PromotionalProduct other)
        {
            // Delegate to base IEquatable<Product>
            return ((IEquatable<Product>)this).Equals(other);
        }
    }
}
