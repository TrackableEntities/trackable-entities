using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrackableEntities.Client.Tests.Entities.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class AppEntityBase : EntityBase
    {
        public Dictionary<string, ITrackable> DeletedEntities { get; set; }
    }
}
