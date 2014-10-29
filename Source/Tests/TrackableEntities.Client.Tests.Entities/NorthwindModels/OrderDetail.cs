﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class OrderDetail : ModelBase<OrderDetail>, ITrackable, IEquatable<OrderDetail>
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

        private int _orderId;
        public int OrderId
        {
            get { return _orderId; }
            set
            {
                if (value == _orderId) return;
                _orderId = value;
                NotifyPropertyChanged(m => m.OrderId);
            }
        }

        private Product _product;
        public Product Product
        {
            get { return _product; }
            set
            {
                if (value == _product) return;
                _product = value;
                ProductChangeTracker = _product == null ? null
                    : new ChangeTrackingCollection<Product> { _product };
                NotifyPropertyChanged(m => m.Product);
            }
        }
        private ChangeTrackingCollection<Product> ProductChangeTracker { get; set; }

        public Order Order
        {
            get { return _order; }
            set
            {
                if (Equals(value, _order)) return;
                _order = value;
                OrderChangeTracker = _order == null ? null
                    : new ChangeTrackingCollection<Order> { _order };
                NotifyPropertyChanged(m => m.Order);
            }
        }
        private Order _order;
        private ChangeTrackingCollection<Order> OrderChangeTracker { get; set; }

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

        private double _quanity;
        public double Quantity
        {
            get { return _quanity; }
            set
            {
                if (Math.Abs(value - _quanity) < double.Epsilon) return;
                _quanity = value;
                NotifyPropertyChanged(m => m.Quantity);
            }
        }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }

        bool IEquatable<OrderDetail>.Equals(OrderDetail other)
        {
            if (EntityIdentifier != default(Guid))
                return EntityIdentifier == other.EntityIdentifier;
            return false;
        }

#pragma warning disable 414
        [JsonProperty]
        private Guid EntityIdentifier { get; set; }
        [JsonProperty]
        private Guid _entityIdentity = default(Guid);
#pragma warning restore 414
    }
}
