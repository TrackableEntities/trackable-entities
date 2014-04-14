using System.Collections.Generic;
using System.ComponentModel;

namespace TrackableEntities.Client
{
    /// <summary>
    /// Interface implemented by trackable collections.
    /// </summary>
    public interface ITrackingCollection<T> : ICollection<T>
        where T : class, ITrackable, INotifyPropertyChanged
    {
        /// <summary>
        /// Get entities that have been marked as Added, Modified or Deleted.
        /// </summary>
        /// <returns>Collection containing only changed entities</returns>
        ChangeTrackingCollection<T> GetChanges();
    }
}
