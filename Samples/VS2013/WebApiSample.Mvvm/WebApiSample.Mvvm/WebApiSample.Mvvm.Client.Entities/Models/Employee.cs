using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;
using TrackableEntities.Client;

namespace WebApiSample.Mvvm.Client.Entities.Models
{
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Employee : ModelBase<Employee>, IEquatable<Employee>, ITrackable
    {
		public Employee()
		{
			this.Territories = new ChangeTrackingCollection<Territory>();
		}

		[DataMember]
		public int EmployeeId
		{ 
			get { return _EmployeeId; }
			set
			{
				if (Equals(value, _EmployeeId)) return;
				_EmployeeId = value;
				NotifyPropertyChanged(m => m.EmployeeId);
			}
		}
		private int _EmployeeId;

		[DataMember]
		public string LastName
		{ 
			get { return _LastName; }
			set
			{
				if (Equals(value, _LastName)) return;
				_LastName = value;
				NotifyPropertyChanged(m => m.LastName);
			}
		}
		private string _LastName;

		[DataMember]
		public string FirstName
		{ 
			get { return _FirstName; }
			set
			{
				if (Equals(value, _FirstName)) return;
				_FirstName = value;
				NotifyPropertyChanged(m => m.FirstName);
			}
		}
		private string _FirstName;

		[DataMember]
		public Nullable<System.DateTime> BirthDate
		{ 
			get { return _BirthDate; }
			set
			{
				if (Equals(value, _BirthDate)) return;
				_BirthDate = value;
				NotifyPropertyChanged(m => m.BirthDate);
			}
		}
		private Nullable<System.DateTime> _BirthDate;

		[DataMember]
		public Nullable<System.DateTime> HireDate
		{ 
			get { return _HireDate; }
			set
			{
				if (Equals(value, _HireDate)) return;
				_HireDate = value;
				NotifyPropertyChanged(m => m.HireDate);
			}
		}
		private Nullable<System.DateTime> _HireDate;

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
		public ChangeTrackingCollection<Territory> Territories
		{
			get { return _Territories; }
			set
			{
				if (value != null) value.Parent = this;
				if (Equals(value, _Territories)) return;
				_Territories = value;
				NotifyPropertyChanged(m => m.Territories);
			}
		}
		private ChangeTrackingCollection<Territory> _Territories;

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

		bool IEquatable<Employee>.Equals(Employee other)
		{
			if (EntityIdentifier != default(Guid))
				return EntityIdentifier == other.EntityIdentifier;
			return false;
		}
        #endregion
	}
}
