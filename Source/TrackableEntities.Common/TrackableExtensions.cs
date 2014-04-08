using System;
using System.Collections;
using System.Collections.Generic;

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

        /// <summary>
        /// Set tracking state to Unchanged on entities and their child collections.
        /// </summary>
        /// <param name="items">Trackable objects</param>
        public static void AcceptChanges(this IEnumerable<ITrackable> items)
        {
            // Recursively set tracking state for child collections
            foreach (var item in items)
                item.AcceptChanges(null);
        }

        private static void AcceptChanges(this ITrackable item, ITrackable parent)
        {
            // Set tracking state for child collections
            foreach (var prop in item.GetType().GetProperties())
            {
                // Apply changes to 1-1 and M-1 properties
                var trackableReference = prop.GetValue(item, null) as ITrackable;

                // Stop recursion if trackable is same type as parent
                if (trackableReference != null
                    && (parent == null || trackableReference.GetType() != parent.GetType()))
                    trackableReference.AcceptChanges(item);

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
