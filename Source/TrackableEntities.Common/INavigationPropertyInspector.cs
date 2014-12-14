using System.Collections.Generic;

namespace TrackableEntities.Common
{
    /// <summary>
    /// If an entity wishes to provide information about its properties in
    /// a non-standard way, then it must implement this interface.
    ///
    /// Possible application: return null for uninitialized lazy-loaded
    /// properties instead of getting the property value thus triggerring
    /// the unwanted LoadProperty call.
    /// </summary>
    public interface INavigationPropertyInspector
    {
        /// <summary>
        /// Return navigation properties of an entity.
        /// </summary>
        IEnumerable<EntityNavigationProperty> GetNavigationProperties();
    }
}