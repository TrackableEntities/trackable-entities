namespace WebApiSample.Client.Entities.Temp
{
    using System;
    using System.Collections.Generic;
	using TrackableEntities.Client;

    public partial class Territory : EntityBase
    {
        public Territory()
        {
            Employees = new ChangeTrackingCollection<Employee>();
        }

		public string TerritoryId
		{ 
			get { return _TerritoryId; }
			set
			{
				if (Equals(value, _TerritoryId)) return;
				_TerritoryId = value;
				NotifyPropertyChanged();
			}
		}
		private string _TerritoryId;

		public string TerritoryDescription
		{ 
			get { return _TerritoryDescription; }
			set
			{
				if (Equals(value, _TerritoryDescription)) return;
				_TerritoryDescription = value;
				NotifyPropertyChanged();
			}
		}
		private string _TerritoryDescription;

		public ChangeTrackingCollection<Employee> Employees
		{
			get { return _Employees; }
			set
			{
				if (Equals(value, _Employees)) return;
				_Employees = value;
				NotifyPropertyChanged();
			}
		}
		private ChangeTrackingCollection<Employee> _Employees;
    }
}
