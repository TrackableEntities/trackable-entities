namespace WebApiSample.Client.Entities.Temp
{
    using System;
    using System.Collections.Generic;
	using TrackableEntities.Client;

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
