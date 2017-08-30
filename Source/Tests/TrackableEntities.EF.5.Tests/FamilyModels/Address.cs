using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackableEntities.EF.Tests.FamilyModels
{
    [ComplexType]
    public class Address : ITrackable
    {
        public string StreetName { get; set; }
        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
    }
}                                                                                             