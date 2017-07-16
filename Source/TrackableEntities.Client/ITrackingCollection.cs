﻿using System;
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
        /// Notification that an entity in the graph has changed.
        /// </summary>
        event EventHandler EntityChanged;
  
        /// <summary>
        /// Turn change-tracking on and off.
        /// </summary>
        bool Tracking { get; set; }

        /// <summary>
        /// Turn change-tracking on and off without graph traversal (internal use).
        /// </summary>
        bool InternalTracking { set; }

        /// <summary>
        /// For internal use.
        /// </summary>
        void SetTracking(bool value, Common.ObjectVisitationHelper visitationHelper, bool oneToManyOnly);

        /// <summary>
        /// Get entities that have been marked as Added, Modified or Deleted.
        /// </summary>
        /// <returns>Collection containing only changed entities</returns>
        ITrackingCollection GetChanges();

        /// <summary>
        /// Get deleted entities which have been cached. 
        /// </summary>
        ICollection CachedDeletes { get; }

        /// <summary>
        /// Remove deleted entities which have been cached.
        /// </summary>
        void RemoveCachedDeletes();

        /// <summary>
        /// Properties to exclude from change tracking.
        /// </summary>
        IList<string> ExcludedProperties { get; }

        /// <summary>
        /// ITrackable parent referencing items in this collection.
        /// </summary>
        ITrackable Parent { get; set; }
    }
}
