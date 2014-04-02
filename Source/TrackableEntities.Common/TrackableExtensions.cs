using System;
using System.Collections;

namespace TrackableEntities.Common
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
            item.AcceptChanges(null);
        }

        private static void AcceptChanges(this ITrackable item, ITrackable parent)
        {
            // Set tracking state for child collections
            foreach (var prop in item.GetType().GetProperties())
            {
                var items = prop.GetValue(item, null) as IList;
                if (items != null)
                {
                    for (int i = items.Count - 1; i > -1; i--)
                    {
                        // Stop recursion if trackable is same type as parent
                        var trackable = items[i] as ITrackable;
                        if (trackable != null 
                            && (parent == null || trackable.GetType() != parent.GetType()))
                        {
                            if (trackable.TrackingState == TrackingState.Deleted)
                                // Remove items marked as deleted
                                items.RemoveAt(i);
                            else
                                // Recursively accept changes on trackable
                                trackable.AcceptChanges(item);
                        }
                    }
                }
            }

            // Set tracking state and clear modified properties
            item.TrackingState = TrackingState.Unchanged;
            item.ModifiedProperties = null;
        }
    }
}
