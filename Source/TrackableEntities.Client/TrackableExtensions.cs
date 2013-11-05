using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace TrackableEntities.Client
{
    internal static class TrackableExtensions
    {
        /// <summary>
        /// Recursively enable or disable tracking on child collections in an object graph.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <param name="enableTracking">Enable or disable change-tracking</param>
        public static void SetTracking(this ITrackable item, bool enableTracking)
        {
            foreach (var prop in item.GetType().GetProperties())
            {
                var trackingColl = prop.GetValue(item, null) as ITrackingCollection;
                if (trackingColl != null)
                {
                    // Recursively set change-tracking
                    foreach (ITrackable child in trackingColl)
                    {
                        child.SetTracking(enableTracking);
                    }

                    // Set tracking
                    trackingColl.Tracking = enableTracking;
                }
            }
        }

        /// <summary>
        /// Recursively set tracking state on child collections in an object graph.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <param name="state">Change-tracking state of an entity</param>
        public static void SetState(this ITrackable item, TrackingState state)
        {
            foreach (var prop in item.GetType().GetProperties())
            {
                var trackingColl = prop.GetValue(item, null) as ITrackingCollection;
                if (trackingColl != null)
                {
                    // Recursively set state
                    foreach (ITrackable child in trackingColl)
                    {
                        child.SetState(state);
                        child.TrackingState = state;
                    }
                }
            }
        }

        /// <summary>
        /// Recursively set tracking state on child collections in an object graph.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <param name="modified">Properties on an entity that have been modified</param>
        public static void SetModifiedProperties(this ITrackable item, ICollection<string> modified)
        {
            foreach (var prop in item.GetType().GetProperties())
            {
                var trackingColl = prop.GetValue(item, null) as ITrackingCollection;
                if (trackingColl != null)
                {
                    // Recursively set modified
                    foreach (ITrackable child in trackingColl)
                    {
                        child.SetModifiedProperties(modified);
                        child.ModifiedProperties = modified;
                    }
                }
            }
        }

        /// <summary>
        /// Recursively set child collections in an object graph to changed items.
        /// </summary>
        /// <param name="item">Trackable object</param>
        public static void SetChanges(this ITrackable item)
        {
            foreach (var prop in item.GetType().GetProperties())
            {
                var trackingColl = prop.GetValue(item, null) as ITrackingCollection;
                if (trackingColl != null)
                {
                    // Recursively set changes
                    foreach (ITrackable child in trackingColl)
                    {
                        child.SetChanges();
                    }

                    // Set collection property on cloned item
                    ITrackingCollection changes = trackingColl.GetChanges();
                    prop.SetValue(item, changes, null);
                }
            }
        }

        /// <summary>
        /// Recursively restore deletes from child collections in an object graph.
        /// </summary>
        /// <param name="item">Trackable object</param>
        public static void RestoreDeletes(this ITrackable item)
        {
            foreach (var prop in item.GetType().GetProperties())
            {
                var trackingColl = prop.GetValue(item, null) as ITrackingCollection;
                if (trackingColl != null)
                {
                    // Recursively restore deletes
                    foreach (ITrackable child in trackingColl)
                    {
                        child.RestoreDeletes();
                    }

                    // Restore deletes on collection
                    trackingColl.RestoreDeletes();
                }
            }
        }

        /// <summary>
        /// Recursively remove deletes from child collections in an object graph.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <param name="enableTracking">Set to true to track removed items</param>
        public static void RemoveDeletes(this ITrackable item, bool enableTracking)
        {
            foreach (var prop in item.GetType().GetProperties())
            {
                var trackingColl = prop.GetValue(item, null) as ITrackingCollection;
                if (trackingColl != null)
                {
                    // Recursively remove deletes
                    foreach (ITrackable child in trackingColl)
                    {
                        child.RemoveDeletes(enableTracking);
                    }

                    // Restore deletes on collection
                    trackingColl.RemoveDeletes(enableTracking);
                }
            }
        }

        /// <summary>
        /// Restore deletes to a trackable collection.
        /// </summary>
        /// <param name="trackingColl">Trackable collection</param>
        public static void RestoreDeletes(this ITrackingCollection trackingColl)
        {
            var trackingList = trackingColl as IList;
            if (trackingList == null) return;
            foreach (ITrackable trackable in trackingColl.GetChanges())
            {
                ITrackable delete = trackable.TrackingState == TrackingState.Deleted
                    ? trackable : null;
                if (delete != null)
                {
                    var isTracking = trackingColl.Tracking;
                    trackingColl.Tracking = false;
                    trackingList.Add(delete);
                    trackingColl.Tracking = isTracking;
                }
            }
        }

        /// <summary>
        /// Remove deletes from a trackable collection.
        /// </summary>
        /// <param name="trackingColl">Trackable collection</param>
        /// <param name="enableTracking">Set to true to track removed items</param>
        public static void RemoveDeletes(this ITrackingCollection trackingColl, bool enableTracking)
        {
            var trackingList = trackingColl as IList;
            if (trackingList == null) return;
            for (int i = trackingList.Count - 1; i >= 0; i--)
            {
                var trackable = (ITrackable)trackingList[i];
                ITrackable delete = trackable.TrackingState == TrackingState.Deleted
                    ? trackable : null;
                if (delete != null)
                {
                    var isTracking = trackingColl.Tracking;
                    trackingColl.Tracking = enableTracking;
                    if (trackingColl.Tracking) 
                        delete.TrackingState = TrackingState.Unchanged;
                    trackingList.Remove(delete);
                    trackingColl.Tracking = isTracking;
                }
            }
        }

        /// <summary>
        /// Recursively get items with child collections that have changes.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <returns>True if item has child collections that have changes</returns>
        public static bool HasChanges(this ITrackable item)
        {
            foreach (var prop in item.GetType().GetProperties())
            {
                var trackingColl = prop.GetValue(item, null) as ITrackingCollection;
                if (trackingColl != null)
                {
                    // Recursively check for child collection changes
                    foreach (ITrackable child in trackingColl)
                    {
                        bool hasChanges = child.HasChanges();
                        if (hasChanges) return true;
                    }

                    // Return true if child collection has changes
                    ITrackingCollection changes = trackingColl.GetChanges();
                    if (changes.Count > 0) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Performs a deep copy using JsonSerializer.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="item">Trackable object</param>
        /// <returns>Cloned entity</returns>
        public static T Clone<T>(this ITrackable item)
            where T : class, ITrackable
        {
            using (var stream = new MemoryStream())
            {
                var ser = new JsonSerializer();
                var writer = new BsonWriter(stream);
                ser.Serialize(writer, item);
                stream.Position = 0;
                var reader = new BsonReader(stream);
                var copy = ser.Deserialize<T>(reader);
                return copy;
            }
        }
    }
}
