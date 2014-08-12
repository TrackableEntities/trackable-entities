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

        private static void AcceptChanges(this ITrackable item, ObjectVisitationHelper visitationHelper)
        {
            ObjectVisitationHelper.EnsureCreated(ref visitationHelper);

            // Prevent endless recursion
            if (visitationHelper.IsVisited(item)) return;

            // Set tracking state for child collections
            foreach (var prop in item.GetType().GetProperties())
            {
                // Apply changes to 1-1 and M-1 properties
                var trackableRef = prop.GetValue(item, null) as ITrackable;

                if (trackableRef != null)
                    trackableRef.AcceptChanges(visitationHelper.With(item));

                // Apply changes to 1-M properties
                var items = prop.GetValue(item, null) as IList;
                if (items != null)
                {
                    var count = items.Count;
                    for (int i = count - 1; i > -1; i--)
                    {
                        // Stop recursion if trackable hasn't been visited
                        var trackable = items[i] as ITrackable;
                        if (trackable != null
                            && (!visitationHelper.IsVisited(trackable)))
                        {
                            if (trackable.TrackingState == TrackingState.Deleted)
                                // Remove items marked as deleted
                                items.RemoveAt(i);
                            else
                                // Recursively accept changes on trackable
                                trackable.AcceptChanges(visitationHelper.With(item));
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
