using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class Product : EntityBase
    {
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

        private bool _discontinued;
        public bool Discontinued
        {
            get { return _discontinued; }
            set
            {
                if (value == _discontinued) return;
                _discontinued = value;
                NotifyPropertyChanged(() => Discontinued);
            }
        }

        private int _categoryId;
        public int CategoryId
        {
            get { return _categoryId; }
            set
            {
                if (value == _categoryId) return;
                _categoryId = value;
                NotifyPropertyChanged(() => CategoryId);
            }
        }

        private Category _category;
        public Category Category
        {
            get { return _category; }
            set
            {
                if (value == _category) return;
                _category = value;
                CategoryChangeTracker = _category == null ? null
                    : new ChangeTrackingCollection<Category> { _category };
                NotifyPropertyChanged(() => Category);
            }
        }
        private ChangeTrackingCollection<Category> CategoryChangeTracker { get; set; }
    }
}
