using System;
using System.Collections;
using System.Collections.Generic;

namespace TrackableEntities.Client
{
    /// <summary>
    /// Interface implemented by trackable collections.
    /// </summary>
    public interface ITrackingCollection : ICollection
    {
        /// <summary>
        /// Notification that an entity has changed.
        /// </summary>
        event EventHandler EntityChanged;

        /// <summary>
        /// Turn change-tracking on and off.
        /// </summary>
        bool Tracking { get; set; }

        /// <summary>
        /// Get entities that have been marked as Added, Modified or Deleted.
        /// </summary>
        /// <returns>Collection containing only changed entities</returns>
        ITrackingCollection GetChanges();

        /// <summary>
        /// Properties to exclude from change tracking.
        /// </summary>
        IList<string> ExcludedProperties { get; }
    }
}
