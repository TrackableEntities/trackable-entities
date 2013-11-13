using System;
using System.Collections;

#if EF_6
namespace TrackableEntities.EF6
#else
namespace TrackableEntities.EF5
#endif
{
    /// <summary>
    /// Extension methods for entities that implement ITrackable.
    /// </summary>
    public static class TrackableExtensions
    {
        /// <summary>
        /// Set tracking state to Unchanged on an entity and its child collections.
        /// </summary>
        /// <param name="item">Trackable object</param>
        public static void AcceptChanges(this ITrackable item)
        {
            // Recursively set tracking state for child collections
            foreach (var prop in item.GetType().GetProperties())
            {
                var items = prop.GetValue(item, null) as IList;
                if (items != null)
                {
                    for (int i = items.Count - 1; i > -1; i--)
                    {
                        var trackable = items[i] as ITrackable;
                        if (trackable != null)
                            trackable.AcceptChanges();
                    }
                }
            }

            // Set tracking state and clear modified properties
            item.TrackingState = TrackingState.Unchanged;
            item.ModifiedProperties = null;
        }
    }
}
