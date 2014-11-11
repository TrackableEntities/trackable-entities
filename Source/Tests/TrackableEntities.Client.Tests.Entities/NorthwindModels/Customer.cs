using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class Customer : ModelBase<Customer>, ITrackable, IEquatable<Customer>,
        IRefPropertyChangeTrackerResolver
    {
        /// <summary>
        /// Testing read-only calculated property
        /// </summary>
        public string CustomerIdAndName
        {
            get
            {
                return string.Format(
                    "{0} - {1}",
                    CustomerId.ToString(),
                    CustomerName == null ? string.Empty : CustomerName.ToString());
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
                Xxx.CustomerSettingChangeTracker = _customerSetting == null ? null
                    : new ChangeTrackingCollection<CustomerSetting> { _customerSetting };
                NotifyPropertyChanged(m => m.CustomerSetting);
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

        ITrackingCollection IRefPropertyChangeTrackerResolver.GetRefPropertyChangeTracker(string propertyName)
        {
            if (propertyName == "CustomerSetting")
                return Xxx.CustomerSettingChangeTracker;
            if (propertyName == "Territory")
                return TerritoryChangeTracker;
            return null;
        }

        #region Testing Non-Trackable properties for performance reflection

        public int NoneTrackableProperty01 { get; set; }
        public int NoneTrackableProperty02 { get; set; }
        public int NoneTrackableProperty03 { get; set; }
        public int NoneTrackableProperty04 { get; set; }
        public int NoneTrackableProperty05 { get; set; }
        public int NoneTrackableProperty06 { get; set; }
        public int NoneTrackableProperty07 { get; set; }
        public int NoneTrackableProperty08 { get; set; }
        public int NoneTrackableProperty09 { get; set; }

        public int NoneTrackableProperty11 { get; set; }
        public int NoneTrackableProperty12 { get; set; }
        public int NoneTrackableProperty13 { get; set; }
        public int NoneTrackableProperty14 { get; set; }
        public int NoneTrackableProperty15 { get; set; }
        public int NoneTrackableProperty16 { get; set; }
        public int NoneTrackableProperty17 { get; set; }
        public int NoneTrackableProperty18 { get; set; }
        public int NoneTrackableProperty19 { get; set; }

        public int NoneTrackableProperty21 { get; set; }
        public int NoneTrackableProperty22 { get; set; }
        public int NoneTrackableProperty23 { get; set; }
        public int NoneTrackableProperty24 { get; set; }
        public int NoneTrackableProperty25 { get; set; }
        public int NoneTrackableProperty26 { get; set; }
        public int NoneTrackableProperty27 { get; set; }
        public int NoneTrackableProperty28 { get; set; }
        public int NoneTrackableProperty29 { get; set; }

        public int NoneTrackableProperty31 { get; set; }
        public int NoneTrackableProperty32 { get; set; }
        public int NoneTrackableProperty33 { get; set; }
        public int NoneTrackableProperty34 { get; set; }
        public int NoneTrackableProperty35 { get; set; }
        public int NoneTrackableProperty36 { get; set; }
        public int NoneTrackableProperty37 { get; set; }
        public int NoneTrackableProperty38 { get; set; }
        public int NoneTrackableProperty39 { get; set; }

        public int NoneTrackableProperty41 { get; set; }
        public int NoneTrackableProperty42 { get; set; }
        public int NoneTrackableProperty43 { get; set; }
        public int NoneTrackableProperty44 { get; set; }
        public int NoneTrackableProperty45 { get; set; }
        public int NoneTrackableProperty46 { get; set; }
        public int NoneTrackableProperty47 { get; set; }
        public int NoneTrackableProperty48 { get; set; }
        public int NoneTrackableProperty49 { get; set; }

        #endregion 
    }
}
