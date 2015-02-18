using System;
using System.Collections.Generic;
using TrackableEntities;

namespace WebApiSample.Service.Entities.Models
{
    public partial class Employee : ITrackable, IMergeable
    {
        public Employee()
        {
            this.Territories = new List<Territory>();
        }

        public int EmployeeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public Nullable<System.DateTime> BirthDate { get; set; }
        public Nullable<System.DateTime> HireDate { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<Territory> Territories { get; set; }

        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}
