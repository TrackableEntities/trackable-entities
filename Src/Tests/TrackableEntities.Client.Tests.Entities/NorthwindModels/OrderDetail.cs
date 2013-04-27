using System;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class OrderDetail : ModelBase<OrderDetail>
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
                NotifyPropertyChanged(m => m.Product);
            }
        }

        private Order _order;
        public Order Order
        {
            get { return _order; }
            set
            {
                if (value == _order) return;
                _order = value;
                NotifyPropertyChanged(m => m.Order);
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
    }
}
