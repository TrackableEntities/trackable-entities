using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TrackableEntities.Client;

namespace TrackableEntities.Tests.Acceptance.ClientEntities
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
                NotifyPropertyChanged(m => m.Customer);
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

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }

        bool IEquatable<Order>.Equals(Order other)
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
