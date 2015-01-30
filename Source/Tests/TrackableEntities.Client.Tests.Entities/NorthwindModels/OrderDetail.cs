using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class OrderDetail : EntityBase
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

        private int _orderId;
        public int OrderId
        {
            get { return _orderId; }
            set
            {
                if (value == _orderId) return;
                _orderId = value;
                NotifyPropertyChanged(() => OrderId);
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
                NotifyPropertyChanged(() => Product);
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
                NotifyPropertyChanged(() => Order);
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
                NotifyPropertyChanged(() => UnitPrice);
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
                NotifyPropertyChanged(() => Quantity);
            }
        }
    }
}
