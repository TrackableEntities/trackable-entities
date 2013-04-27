using System.Collections.Generic;

namespace TrackableEntities.Client
{
    /// <summary>
    /// Interface implemented by trackable collections.
    /// </summary>
    public interface ITrackingCollection<T> : ICollection<T>
        where T : class, ITrackable
    {
        /// <summary>
        /// Get entities that have been marked as Added, Modified or Deleted.
        /// </summary>
        /// <returns>Collection containing only changed entities</returns>
        ITrackingCollection<T> GetChanges();
    }
}
