using System;
using System.Collections.Generic;

namespace TrackableEntities.Client
{
    public abstract partial class EntityBase
    {
        /// <summary>
        /// Change-tracking state of an entity.
        /// </summary>
        public TrackingState TrackingState { get; set; }

        /// <summary>
        /// Properties on an entity that have been modified.
        /// </summary>
        public ICollection<string> ModifiedProperties { get; set; }

        /// <summary>
        /// Identifier used for correlation with MergeChanges.
        /// </summary>
        public Guid EntityIdentifier { get; set; }
    }
}
