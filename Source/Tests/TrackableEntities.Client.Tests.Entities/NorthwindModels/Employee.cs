using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class Employee : ModelBase<Employee>, ITrackable, IEquatable<Employee>
    {
        public Employee()
        {
            _territories = new ChangeTrackingCollection<Territory> { Parent = this };
        }

        private int _employeeId;
        public int EmployeeId
        {
            get { return _employeeId; }
            set
            {
                if (value == _employeeId) return;
                _employeeId = value;
                NotifyPropertyChanged(m => m.EmployeeId);
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
                NotifyPropertyChanged(m => m.LastName);
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
                NotifyPropertyChanged(m => m.FirstName);
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
                NotifyPropertyChanged(m => m.BirthDate);
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
                NotifyPropertyChanged(m => m.HireDate);
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
                NotifyPropertyChanged(m => m.City);
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
                NotifyPropertyChanged(m => m.Country);
            }
        }

        private ChangeTrackingCollection<Territory> _territories;
        public ChangeTrackingCollection<Territory> Territories
        {
            get { return _territories; }
            set
            {
                if (Equals(value, _territories)) return;
                _territories = value;
                NotifyPropertyChanged(m => m.Territories);
            }
        }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }

        bool IEquatable<Employee>.Equals(Employee other)
        {
            if (EntityIdentifier != new Guid())
                return EntityIdentifier == other.EntityIdentifier;
            return EmployeeId.Equals(other.EmployeeId);
        }
        private Guid EntityIdentifier { get; set; }
    }
}
