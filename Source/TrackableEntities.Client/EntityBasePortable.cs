using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace TrackableEntities.Client
{
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true)]
    public abstract partial class EntityBase
    {
        /// <summary>
        /// Change-tracking state of an entity.
        /// </summary>
        [DataMember]
        public TrackingState TrackingState { get; set; }

        /// <summary>
        /// Properties on an entity that have been modified.
        /// </summary>
        [DataMember]
        public ICollection<string> ModifiedProperties { get; set; }

        /// <summary>
        /// Identifier used for correlation with MergeChanges.
        /// </summary>
        [DataMember]
        public Guid EntityIdentifier { get; set; }
    }
}
