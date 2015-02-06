using System;

namespace TrackableEntities.Client
{
    /// <summary>
    /// Interface implemented by entities, 1-1 and M-1 properties of which have
    /// non-standard names or locations of the corresponding change trackers.
    /// </summary>
    public interface IRefPropertyChangeTrackerResolver
    {
        /// <summary>
        /// Get change tracker corresponding to a given 1-1 or M-1 property.
        /// <param name="propertyName">Name of 1-1 or M-1 property</param>
        /// </summary>
        ITrackingCollection GetRefPropertyChangeTracker(string propertyName);
    }
}
