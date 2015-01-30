using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class Customer : EntityBase, IRefPropertyChangeTrackerResolver
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

        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set
            {
                if (value == _customerName) return;
                _customerName = value;
                NotifyPropertyChanged(() => CustomerName);
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
                NotifyPropertyChanged(() => Data);
            }
        }

        private string _territoryId;
        public string TerritoryId
        {
            get { return _territoryId; }
            set
            {
                _territoryId = value;
                NotifyPropertyChanged(() => TerritoryId);
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
                Xxx.CustomerSettingChangeTracker = _customerSetting == null ? null
                    : new ChangeTrackingCollection<CustomerSetting> { _customerSetting };
                NotifyPropertyChanged(() => CustomerSetting);
            }
        }
        private struct NON_STANDARD_LOCATION
        {
            internal ChangeTrackingCollection<CustomerSetting> CustomerSettingChangeTracker { get; set; }
        }
        private NON_STANDARD_LOCATION Xxx;

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

        ITrackingCollection IRefPropertyChangeTrackerResolver.GetRefPropertyChangeTracker(string propertyName)
        {
            if (propertyName == "CustomerSetting")
                return Xxx.CustomerSettingChangeTracker;
            if (propertyName == "Territory")
                return TerritoryChangeTracker;
            return null;
        }
    }
}
