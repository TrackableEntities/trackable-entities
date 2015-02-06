using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TrackableEntities.Client;

namespace TrackableEntities.Tests.Acceptance.ClientEntities
{
    [JsonObject(IsReference = true)]
    public class Customer : EntityBase
    {
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

        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set
            {
                if (value == _customerName) return;
                _customerName = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }

        private string _territoryId;
        public string TerritoryId
        {
            get { return _territoryId; }
            set
            {
                _territoryId = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
    }
}
