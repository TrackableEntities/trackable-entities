using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TrackableEntities.Client;

namespace TrackableEntities.Tests.Acceptance.ClientEntities
{
    [JsonObject(IsReference = true)]
    public class Territory : EntityBase
    {
        private string _territoryId;
        public string TerritoryId
        {
            get { return _territoryId; }
            set
            {
                if (value == _territoryId) return;
                _territoryId = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }
        private ChangeTrackingCollection<Area> AreaChangeTracker { get; set; }

        private ChangeTrackingCollection<Employee> _employees;
        public ChangeTrackingCollection<Employee> Employees
        {
            get { return _employees; }
            set
            {
                if (value != null) value.Parent = this;
                if (Equals(value, _employees)) return;
                _employees = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }
    }
}
