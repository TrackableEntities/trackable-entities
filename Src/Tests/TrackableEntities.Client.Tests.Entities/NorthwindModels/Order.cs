using System;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class Order : ModelBase<Order>
    {
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

        private DateTime _orderDate;
        public DateTime OrderDate
        {
            get { return _orderDate; }
            set
            {
                if (value == _orderDate) return;
                _orderDate = value;
                NotifyPropertyChanged(m => m.OrderDate);
            }
        }
        private string _customerId;
        public string CustomerId
        {
            get { return _customerId; }
            set
            {
                if (value == _customerId) return;
                _customerId = value;
                NotifyPropertyChanged(m => m.CustomerId);
            }
        }

        private Customer _customer;
        public Customer Customer
        {
            get { return _customer; }
            set
            {
                if (value == _customer) return;
                _customer = value;
                NotifyPropertyChanged(m => m.Customer);
            }
        }

        private ChangeTrackingCollection<OrderDetail> _orderDetails;
        public ChangeTrackingCollection<OrderDetail> OrderDetails
        {
            get { return _orderDetails; }
            set
            {
                if (Equals(value, _orderDetails)) return;
                _orderDetails = value;
                NotifyPropertyChanged(m => m.OrderDetails);
            }
        }
    }
}
