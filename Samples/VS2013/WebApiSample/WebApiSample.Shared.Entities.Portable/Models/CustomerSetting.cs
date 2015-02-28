using System;
using System.Collections.Generic;
using TrackableEntities.Client;

namespace WebApiSample.Shared.Entities.Portable.Models
{
    public partial class CustomerSetting : EntityBase
    {
		public string CustomerId
		{ 
			get { return _CustomerId; }
			set
			{
				if (Equals(value, _CustomerId)) return;
				_CustomerId = value;
				NotifyPropertyChanged(() => CustomerId);
			}
		}
		private string _CustomerId;

		public string Setting
		{ 
			get { return _Setting; }
			set
			{
				if (Equals(value, _Setting)) return;
				_Setting = value;
				NotifyPropertyChanged(() => Setting);
			}
		}
		private string _Setting;

		public Customer Customer
		{
			get { return _Customer; }
			set
			{
				if (Equals(value, _Customer)) return;
				_Customer = value;
				CustomerChangeTracker = _Customer == null ? null
					: new ChangeTrackingCollection<Customer> { _Customer };
				NotifyPropertyChanged(() => Customer);
			}
		}
		private Customer _Customer;
		private ChangeTrackingCollection<Customer> CustomerChangeTracker { get; set; }

	}
}
