using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class SpecialProduct : AppEntityBase
    {
        public SpecialProduct()
        {
            DeletedEntities = new Dictionary<string, ITrackable>
            {
                { "item1", new Product { ProductId = 1, ProductName = "Chai", UnitPrice = 10 } }
            };
        }

        private int _productId;
        public int ProductId
        {
            get { return _productId; }
            set
            {
                if (value == _productId) return;
                _productId = value;
                NotifyPropertyChanged(() => ProductId);
            }
        }

        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set
            {
                if (value == _productName) return;
                _productName = value;
                NotifyPropertyChanged(() => ProductName);
            }
        }

        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get { return _unitPrice; }
            set
            {
                if (value == _unitPrice) return;
                _unitPrice = value;
                NotifyPropertyChanged(() => UnitPrice);
            }
        }
    }
}
