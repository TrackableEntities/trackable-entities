using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class CustomerSetting : EntityBase
    {
        private string _customerId;
        public string CustomerId
        {
            get { return _customerId; }
            set
            {
                if (value == _customerId) return;
                _customerId = value;
                NotifyPropertyChanged(() => CustomerId);
            }
        }

        private string _setting;
        public string Setting
        {
            get { return _setting; }
            set
            {
                if (value == _setting) return;
                _setting = value;
                NotifyPropertyChanged(() => Setting);
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
                NotifyPropertyChanged(() => Customer);
            }
        }
        private ChangeTrackingCollection<Customer> CustomerChangeTracker { get; set; }
    }
}
