using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class Product : ModelBase<Product>, ITrackable, IEquatable<Product>
    {
        private int _productId;
        public int ProductId
        {
            get { return _productId; }
            set
            {
                if (value == _productId) return;
                _productId = value;
                NotifyPropertyChanged(m => m.ProductId);
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
                NotifyPropertyChanged(m => m.ProductName);
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
                NotifyPropertyChanged(m => m.UnitPrice);
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
                NotifyPropertyChanged(m => m.Discontinued);
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
                NotifyPropertyChanged(m => m.CategoryId);
            }
        }

        // NOTE: Reference properties are change-tracked but do not call 
        // NotifyPropertyChanged because it is called by foreign key's property setter.

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
            }
        }
        private ChangeTrackingCollection<Category> CategoryChangeTracker { get; set; }

        public ICollection<string> ModifiedProperties { get; set; }

        private TrackingState _trackingState;
        public TrackingState TrackingState
        {
            get { return _trackingState; }
            set
            {
                EntityIdentifier = value == TrackingState.Added
                    ? Guid.NewGuid()
                    : new Guid();
                _trackingState = value;
            }
        }

        bool IEquatable<Product>.Equals(Product other)
        {
            if (EntityIdentifier != new Guid())
                return EntityIdentifier == other.EntityIdentifier;
            return ProductId.Equals(other.ProductId);
        }
        private Guid EntityIdentifier { get; set; }
    }
}
