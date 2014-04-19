
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
    public partial class CustomerSetting : ModelBase<CustomerSetting>, IEquatable<CustomerSetting>, ITrackable
    {
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
		public string Setting
		{ 
			get { return _Setting; }
			set
			{
				if (Equals(value, _Setting)) return;
				_Setting = value;
				NotifyPropertyChanged(m => m.Setting);
			}
		}
		private string _Setting;

		[DataMember]
		public Customer Customer
		{
			get { return _Customer; }
			set
			{
				if (Equals(value, _Customer)) return;
				_Customer = value;
				CustomerChangeTracker = _Customer == null ? null
					: new ChangeTrackingCollection<Customer> { _Customer };
			}
		}
		private Customer _Customer;
		private ChangeTrackingCollection<Customer> CustomerChangeTracker { get; set; }

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

		bool IEquatable<CustomerSetting>.Equals(CustomerSetting other)
		{
			if (EntityIdentifier != default(Guid))
				return EntityIdentifier == other.EntityIdentifier;
			return false;
		}
        #endregion
	}
}
