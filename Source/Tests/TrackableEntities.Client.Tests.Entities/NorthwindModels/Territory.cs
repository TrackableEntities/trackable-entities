using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class Territory : ModelBase<Territory>, ITrackable
    {
        public Territory()
        {
            _employees = new ChangeTrackingCollection<Employee> { Parent = this };
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

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
