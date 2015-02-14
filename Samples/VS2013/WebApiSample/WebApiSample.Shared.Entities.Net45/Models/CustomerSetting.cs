using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Client;

namespace WebApiSample.Shared.Entities.Models
{
    [Table("CustomerSetting")]
    public partial class CustomerSetting : EntityBase
    {
        [Key]
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
        [StringLength(50)]
		public string Setting
		{ 
			get { return _Setting; }
			set
			{
				if (Equals(value, _Setting)) return;
				_Setting = value;
				NotifyPropertyChanged();
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
				NotifyPropertyChanged();
			}
		}
		private Customer _Customer;
		private ChangeTrackingCollection<Customer> CustomerChangeTracker { get; set; }
    }
}
