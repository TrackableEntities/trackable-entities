using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TrackableEntities.Client;

namespace TrackableEntities.Tests.Acceptance.ClientEntities
{
    [JsonObject(IsReference = true)]
    public class Order : EntityBase
    {
        private int _orderId;
        public int OrderId
        {
            get { return _orderId; }
            set
            {
                if (value == _orderId) return;
                _orderId = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                CustomerChangeTracker = _customer == null ? null 
                    : new ChangeTrackingCollection<Customer> { _customer };
                NotifyPropertyChanged();
            }
        }
        private ChangeTrackingCollection<Customer> CustomerChangeTracker { get; set; }

        private ChangeTrackingCollection<OrderDetail> _orderDetails;
        public ChangeTrackingCollection<OrderDetail> OrderDetails
        {
            get { return _orderDetails; }
            set
            {
                if (Equals(value, _orderDetails)) return;
                _orderDetails = value;
                NotifyPropertyChanged();
            }
        }
    }
}
