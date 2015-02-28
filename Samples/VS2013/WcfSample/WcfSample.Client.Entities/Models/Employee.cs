using System;
using TrackableEntities.Client;

namespace WcfSample.Client.Entities.Models
{
    public partial class Employee : EntityBase
    {
		public Employee()
		{
			this.Territories = new ChangeTrackingCollection<Territory>();
		}

		public int EmployeeId
		{ 
			get { return _EmployeeId; }
			set
			{
				if (Equals(value, _EmployeeId)) return;
				_EmployeeId = value;
				NotifyPropertyChanged(() => EmployeeId);
			}
		}
		private int _EmployeeId;

		public string LastName
		{ 
			get { return _LastName; }
			set
			{
				if (Equals(value, _LastName)) return;
				_LastName = value;
				NotifyPropertyChanged(() => LastName);
			}
		}
		private string _LastName;

		public string FirstName
		{ 
			get { return _FirstName; }
			set
			{
				if (Equals(value, _FirstName)) return;
				_FirstName = value;
				NotifyPropertyChanged(() => FirstName);
			}
		}
		private string _FirstName;

		public Nullable<System.DateTime> BirthDate
		{ 
			get { return _BirthDate; }
			set
			{
				if (Equals(value, _BirthDate)) return;
				_BirthDate = value;
				NotifyPropertyChanged(() => BirthDate);
			}
		}
		private Nullable<System.DateTime> _BirthDate;

		public Nullable<System.DateTime> HireDate
		{ 
			get { return _HireDate; }
			set
			{
				if (Equals(value, _HireDate)) return;
				_HireDate = value;
				NotifyPropertyChanged(() => HireDate);
			}
		}
		private Nullable<System.DateTime> _HireDate;

		public string City
		{ 
			get { return _City; }
			set
			{
				if (Equals(value, _City)) return;
				_City = value;
				NotifyPropertyChanged(() => City);
			}
		}
		private string _City;

		public string Country
		{ 
			get { return _Country; }
			set
			{
				if (Equals(value, _Country)) return;
				_Country = value;
				NotifyPropertyChanged(() => Country);
			}
		}
		private string _Country;

		public ChangeTrackingCollection<Territory> Territories
		{
			get { return _Territories; }
			set
			{
				if (value != null) value.Parent = this;
				if (Equals(value, _Territories)) return;
				_Territories = value;
				NotifyPropertyChanged(() => Territories);
			}
		}
		private ChangeTrackingCollection<Territory> _Territories;

	}
}
