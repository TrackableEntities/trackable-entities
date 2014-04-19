
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;
using TrackableEntities.Client;

namespace WebApiSample.Client.Entities.Models
{
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Territory : ModelBase<Territory>, IEquatable<Territory>, ITrackable
    {
		public Territory()
		{
			this.Employees = new ChangeTrackingCollection<Employee>();
		}

		[DataMember]
		public string TerritoryId
		{ 
			get { return _TerritoryId; }
			set
			{
				if (Equals(value, _TerritoryId)) return;
				_TerritoryId = value;
				NotifyPropertyChanged(m => m.TerritoryId);
			}
		}
		private string _TerritoryId;

		[DataMember]
		public string TerritoryDescription
		{ 
			get { return _TerritoryDescription; }
			set
			{
				if (Equals(value, _TerritoryDescription)) return;
				_TerritoryDescription = value;
				NotifyPropertyChanged(m => m.TerritoryDescription);
			}
		}
		private string _TerritoryDescription;

		[DataMember]
		public ChangeTrackingCollection<Employee> Employees
		{
			get { return _Employees; }
			set
			{
				if (value != null) value.Parent = this;
				if (Equals(value, _Employees)) return;
				_Employees = value;
				NotifyPropertyChanged(m => m.Employees);
			}
		}
		private ChangeTrackingCollection<Employee> _Employees;

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

		bool IEquatable<Territory>.Equals(Territory other)
		{
			if (EntityIdentifier != default(Guid))
				return EntityIdentifier == other.EntityIdentifier;
			return false;
		}
        #endregion
	}
}
