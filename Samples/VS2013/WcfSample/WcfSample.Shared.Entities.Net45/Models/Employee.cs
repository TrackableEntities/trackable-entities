using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Client;

namespace WcfSample.Shared.Entities.Net45.Models
{
    [Table("Employee")]
    public partial class Employee : EntityBase
    {
        public Employee()
        {
            Territories = new ChangeTrackingCollection<Territory>();
        }

		public int EmployeeId
		{ 
			get { return _EmployeeId; }
			set
			{
				if (Equals(value, _EmployeeId)) return;
				_EmployeeId = value;
				NotifyPropertyChanged();
			}
		}
		private int _EmployeeId;

        [Required]
        [StringLength(20)]
		public string LastName
		{ 
			get { return _LastName; }
			set
			{
				if (Equals(value, _LastName)) return;
				_LastName = value;
				NotifyPropertyChanged();
			}
		}
		private string _LastName;

        [Required]
        [StringLength(20)]
		public string FirstName
		{ 
			get { return _FirstName; }
			set
			{
				if (Equals(value, _FirstName)) return;
				_FirstName = value;
				NotifyPropertyChanged();
			}
		}
		private string _FirstName;

		public DateTime? BirthDate
		{ 
			get { return _BirthDate; }
			set
			{
				if (Equals(value, _BirthDate)) return;
				_BirthDate = value;
				NotifyPropertyChanged();
			}
		}
		private DateTime? _BirthDate;

		public DateTime? HireDate
		{ 
			get { return _HireDate; }
			set
			{
				if (Equals(value, _HireDate)) return;
				_HireDate = value;
				NotifyPropertyChanged();
			}
		}
		private DateTime? _HireDate;

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

		public ChangeTrackingCollection<Territory> Territories
		{
			get { return _Territories; }
			set
			{
				if (Equals(value, _Territories)) return;
				_Territories = value;
				NotifyPropertyChanged();
			}
		}
		private ChangeTrackingCollection<Territory> _Territories;
    }
}
