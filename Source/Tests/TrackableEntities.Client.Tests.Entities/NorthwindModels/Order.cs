using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class Order : ModelBase<Order>, ITrackable, IEquatable<Order>
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

        // NOTE: Reference properties are change-tracked but do not call 
        // NotifyPropertyChanged because it is called by foreign key's property setter.

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
                NotifyPropertyChanged(m => m.OrderDetails);
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

        bool IEquatable<Order>.Equals(Order other)
        {
            if (EntityIdentifier != new Guid())
                return EntityIdentifier == other.EntityIdentifier;
            return OrderId.Equals(other.OrderId);
        }
        private Guid EntityIdentifier { get; set; }
    }
}
