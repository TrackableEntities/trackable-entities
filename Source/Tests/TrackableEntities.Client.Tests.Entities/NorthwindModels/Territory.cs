using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class Territory : ModelBase<Territory>, ITrackable, IEquatable<Territory>
    {
        public Territory()
        {
            _employees = new ChangeTrackingCollection<Employee> { Parent = this };
            _customers = new ChangeTrackingCollection<Customer> { Parent = this };
        }

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
                AreaChangeTracker = _area == null ? null
                    : new ChangeTrackingCollection<Area> { _area };
            }
        }
        private ChangeTrackingCollection<Area> AreaChangeTracker { get; set; }

        private ChangeTrackingCollection<Employee> _employees;
        public ChangeTrackingCollection<Employee> Employees
        {
            get { return _employees; }
            set
            {
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
            return TerritoryId.Equals(other.TerritoryId);
        }
    }
}
