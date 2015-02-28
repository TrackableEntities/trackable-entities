using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Client;

namespace WcfSample.Shared.Entities.Net45.Models
{
    [Table("Customer")]
    public partial class Customer : EntityBase
    {
        public Customer()
        {
            Orders = new ChangeTrackingCollection<Order>();
        }

        [StringLength(5)]
		public string CustomerId
		{ 
			get { return _CustomerId; }
			set
			{
				if (Equals(value, _CustomerId)) return;
				_CustomerId = value;
				NotifyPropertyChanged();
			}
		}
		private string _CustomerId;

        [Required]
        [StringLength(40)]
		public string CompanyName
		{ 
			get { return _CompanyName; }
			set
			{
				if (Equals(value, _CompanyName)) return;
				_CompanyName = value;
				NotifyPropertyChanged();
			}
		}
		private string _CompanyName;

        [StringLength(30)]
		public string ContactName
		{ 
			get { return _ContactName; }
			set
			{
				if (Equals(value, _ContactName)) return;
				_ContactName = value;
				NotifyPropertyChanged();
			}
		}
		private string _ContactName;

        [StringLength(15)]
		public string City
		{ 
			get { return _City; }
			set
			{
				if (Equals(value, _City)) return;
				_City = value;
				NotifyPropertyChanged();
			}
		}
		private string _City;

        [StringLength(15)]
		public string Country
		{ 
			get { return _Country; }
			set
			{
				if (Equals(value, _Country)) return;
				_Country = value;
				NotifyPropertyChanged();
			}
		}
		private string _Country;


		public CustomerSetting CustomerSetting
		{
			get { return _CustomerSetting; }
			set
			{
				if (Equals(value, _CustomerSetting)) return;
				_CustomerSetting = value;
				CustomerSettingChangeTracker = _CustomerSetting == null ? null
					: new ChangeTrackingCollection<CustomerSetting> { _CustomerSetting };
				NotifyPropertyChanged();
			}
		}
		private CustomerSetting _CustomerSetting;
		private ChangeTrackingCollection<CustomerSetting> CustomerSettingChangeTracker { get; set; }

		public ChangeTrackingCollection<Order> Orders
		{
			get { return _Orders; }
			set
			{
				if (Equals(value, _Orders)) return;
				_Orders = value;
				NotifyPropertyChanged();
			}
		}
		private ChangeTrackingCollection<Order> _Orders;
    }
}
