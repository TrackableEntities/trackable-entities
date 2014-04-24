using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class Customer : ModelBase<Customer>, ITrackable, IEquatable<Customer>
    {
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

        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set
            {
                if (value == _customerName) return;
                _customerName = value;
                NotifyPropertyChanged(m => m.CustomerName);
            }
        }

        private string _data;
        public string Data
        {
            get { return _data; }
            set
            {
                if (value == _data) return;
                _data = value;
                NotifyPropertyChanged(m => m.Data);
            }
        }

        private string _territoryId;
        public string TerritoryId
        {
            get { return _territoryId; }
            set
            {
                _territoryId = value;
                NotifyPropertyChanged(m => m.TerritoryId);
            }
        }

        private CustomerSetting _customerSetting;
        public CustomerSetting CustomerSetting
        {
            get { return _customerSetting; }
            set
            {
                if (value == _customerSetting) return;
                _customerSetting = value;
                CustomerSettingChangeTracker = _customerSetting == null ? null
                    : new ChangeTrackingCollection<CustomerSetting> { _customerSetting };
                NotifyPropertyChanged(m => m.CustomerSetting);
            }
        }
        private ChangeTrackingCollection<CustomerSetting> CustomerSettingChangeTracker { get; set; }

        private Territory _territory;
        public Territory Territory
        {
            get { return _territory; }
            set
            {
                if (value == _territory) return;
                _territory = value;
                TerritoryChangeTracker = _territory == null ? null
                    : new ChangeTrackingCollection<Territory> { _territory };
            }
        }
        private ChangeTrackingCollection<Territory> TerritoryChangeTracker { get; set; }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }

        bool IEquatable<Customer>.Equals(Customer other)
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
