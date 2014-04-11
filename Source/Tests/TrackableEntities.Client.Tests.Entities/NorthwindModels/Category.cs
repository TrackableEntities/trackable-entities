using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class Category : ModelBase<Category>, ITrackable, IEquatable<Category>
    {
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

        private string _categoryName;
        public string CategoryName
        {
            get { return _categoryName; }
            set
            {
                if (value == _categoryName) return;
                _categoryName = value;
                NotifyPropertyChanged(m => m.CategoryName);
            }
        }

        private ChangeTrackingCollection<Product> _products;
        public ChangeTrackingCollection<Product> Products
        {
            get { return _products; }
            set
            {
                if (Equals(value, _products)) return;
                _products = value;
                NotifyPropertyChanged(m => m.Products);
            }
        }

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

        bool IEquatable<Category>.Equals(Category other)
        {
            if (EntityIdentifier != new Guid())
                return EntityIdentifier == other.EntityIdentifier;
            return CategoryId.Equals(other.CategoryId);
        }
        private Guid EntityIdentifier { get; set; }
    }
}
