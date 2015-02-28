using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class Employee : EntityBase
    {
        private int _employeeId;
        public int EmployeeId
        {
            get { return _employeeId; }
            set
            {
                if (value == _employeeId) return;
                _employeeId = value;
                NotifyPropertyChanged(() => EmployeeId);
            }
        }
        
        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (value == _lastName) return;
                _lastName = value;
                NotifyPropertyChanged(() => LastName);
            }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (value == _firstName) return;
                _firstName = value;
                NotifyPropertyChanged(() => FirstName);
            }
        }

        private DateTime? _birthDate;
        public DateTime? BirthDate
        {
            get { return _birthDate; }
            set
            {
                if (value == _birthDate) return;
                _birthDate = value;
                NotifyPropertyChanged(() => BirthDate);
            }
        }

        private DateTime? _hireDate;
        public DateTime? HireDate
        {
            get { return _hireDate; }
            set
            {
                if (value == _hireDate) return;
                _hireDate = value;
                NotifyPropertyChanged(() => HireDate);
            }
        }

        private string _city;
        public string City
        {
            get { return _city; }
            set
            {
                if (value == _city) return;
                _city = value;
                NotifyPropertyChanged(() => City);
            }
        }

        private string _country;
        public string Country
        {
            get { return _country; }
            set
            {
                if (value == _country) return;
                _country = value;
                NotifyPropertyChanged(() => Country);
            }
        }

        private ChangeTrackingCollection<Territory> _territories;
        public ChangeTrackingCollection<Territory> Territories
        {
            get { return _territories; }
            set
            {
                if (value != null) value.Parent = this;
                if (Equals(value, _territories)) return;
                _territories = value;
                NotifyPropertyChanged(() => Territories);
            }
        }
    }
}
