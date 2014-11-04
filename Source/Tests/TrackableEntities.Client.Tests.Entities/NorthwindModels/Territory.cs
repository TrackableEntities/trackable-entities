using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class Territory : ModelBase<Territory>, ITrackable, IEquatable<Territory>,
        IRefPropertyChangeTrackerResolver
    {
        private string _territoryId;
        public string TerritoryId
        {
            get { return _territoryId; }
            set
            {
                if (value == _territoryId) return;
                _territoryId = value;
                NotifyPropertyChanged(m => m.TerritoryId);
            }
        }

        private string _territoryDescription;
        public string TerritoryDescription
        {
            get { return _territoryDescription; }
            set
            {
                if (value == _territoryDescription) return;
                _territoryDescription = value;
                NotifyPropertyChanged(m => m.TerritoryDescription);
            }
        }

        private string _data;
        public string Data
        {
            get { return _data; }
            set
            {
                if (value == _data) return;
                _data = value;
                NotifyPropertyChanged(m => m.Data);
            }
        }

        private int _areaId;
        public int AreaId
        {
            get { return _areaId; }
            set
            {
                if (value == _areaId) return;
                _areaId = value;
                NotifyPropertyChanged(m => m.AreaId);
            }
        }

        private Area _area;
        public Area Area
        {
            get { return _area; }
            set
            {
                if (value == _area) return;
                _area = value;
                AreaChangeTracker_NON_STANDARD = _area == null ? null
                    : new ChangeTrackingCollection<Area> { _area };
                NotifyPropertyChanged(m => m.Area);
            }
        }
        private ChangeTrackingCollection<Area> AreaChangeTracker_NON_STANDARD { get; set; }

        private ChangeTrackingCollection<Employee> _employees;
        public ChangeTrackingCollection<Employee> Employees
        {
            get { return _employees; }
            set
            {
                if (value != null) value.Parent = this;
                if (Equals(value, _employees)) return;
                _employees = value;
                NotifyPropertyChanged(m => m.Employees);
            }
        }

        private ChangeTrackingCollection<Customer> _customers;
        public ChangeTrackingCollection<Customer> Customers
        {
            get { return _customers; }
            set
            {
                if (Equals(value, _customers)) return;
                _customers = value;
                NotifyPropertyChanged(m => m.Customers);
            }
        }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }

        bool IEquatable<Territory>.Equals(Territory other)
        {
            if (EntityIdentifier != default(Guid))
                return EntityIdentifier == other.EntityIdentifier;
            return false;
        }

#pragma warning disable 414
        [JsonProperty]
        private Guid EntityIdentifier { get; set; }
        [JsonProperty]
        private Guid _entityIdentity = default(Guid);
#pragma warning restore 414

        ITrackingCollection IRefPropertyChangeTrackerResolver.GetRefPropertyChangeTracker(string propertyName)
        {
            return propertyName == "Area" ? AreaChangeTracker_NON_STANDARD : null;
        }
    }
}
