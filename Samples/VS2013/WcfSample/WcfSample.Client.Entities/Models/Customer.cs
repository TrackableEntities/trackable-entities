using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;
using TrackableEntities.Client;

namespace WcfSample.Client.Entities.Models
{
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Customer : ModelBase<Customer>, IEquatable<Customer>, ITrackable
    {
		public Customer()
		{
			this.Orders = new ChangeTrackingCollection<Order>();
		}

		[DataMember]
		public string CustomerId
		{ 
			get { return _CustomerId; }
			set
			{
				if (Equals(value, _CustomerId)) return;
				_CustomerId = value;
				NotifyPropertyChanged(m => m.CustomerId);
			}
		}
		private string _CustomerId;

		[DataMember]
		public string CompanyName
		{ 
			get { return _CompanyName; }
			set
			{
				if (Equals(value, _CompanyName)) return;
				_CompanyName = value;
				NotifyPropertyChanged(m => m.CompanyName);
			}
		}
		private string _CompanyName;

		[DataMember]
		public string ContactName
		{ 
			get { return _ContactName; }
			set
			{
				if (Equals(value, _ContactName)) return;
				_ContactName = value;
				NotifyPropertyChanged(m => m.ContactName);
			}
		}
		private string _ContactName;

		[DataMember]
		public string City
		{ 
			get { return _City; }
			set
			{
				if (Equals(value, _City)) return;
				_City = value;
				NotifyPropertyChanged(m => m.City);
			}
		}
		private string _City;

		[DataMember]
		public string Country
		{ 
			get { return _Country; }
			set
			{
				if (Equals(value, _Country)) return;
				_Country = value;
				NotifyPropertyChanged(m => m.Country);
			}
		}
		private string _Country;

		[DataMember]
		public CustomerSetting CustomerSetting
		{
			get { return _CustomerSetting; }
			set
			{
				if (Equals(value, _CustomerSetting)) return;
				_CustomerSetting = value;
				CustomerSettingChangeTracker = _CustomerSetting == null ? null
					: new ChangeTrackingCollection<CustomerSetting> { _CustomerSetting };
				NotifyPropertyChanged(m => m.CustomerSetting);
			}
		}
		private CustomerSetting _CustomerSetting;
		private ChangeTrackingCollection<CustomerSetting> CustomerSettingChangeTracker { get; set; }

		[DataMember]
		public ChangeTrackingCollection<Order> Orders
		{
			get { return _Orders; }
			set
			{
				if (Equals(value, _Orders)) return;
				_Orders = value;
				NotifyPropertyChanged(m => m.Orders);
			}
		}
		private ChangeTrackingCollection<Order> _Orders;

        #region Change Tracking

		[DataMember]
		public TrackingState TrackingState { get; set; }

		[DataMember]
		public ICollection<string> ModifiedProperties { get; set; }

		[JsonProperty, DataMember]
		private Guid EntityIdentifier { get; set; }

		#pragma warning disable 414

		[JsonProperty, DataMember]
		private Guid _entityIdentity = default(Guid);

		#pragma warning restore 414

		bool IEquatable<Customer>.Equals(Customer other)
		{
			if (EntityIdentifier != default(Guid))
				return EntityIdentifier == other.EntityIdentifier;
			return false;
		}
        #endregion
	}
}
