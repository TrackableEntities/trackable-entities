using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Reflection;
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
        /// <param name="parent">ITrackable parent of item</param>
        public static void SetTracking(this ITrackable item, 
            bool enableTracking, ITrackable parent = null)
        {
            foreach (var prop in item.GetType().GetProperties(BindingFlags.Instance
                | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var trackingColl = prop.GetValue(item, null) as ITrackingCollection;
                if (trackingColl != null)
                {
                    // Recursively set change-tracking
                    bool stopRecursion = false;
                    foreach (ITrackable child in trackingColl)
                    {
                        // Stop recursion if trackable is same type as parent
                        if (parent != null && (child.GetType() == parent.GetType()))
                        {
                            stopRecursion = true;
                            break;
                        }
                        child.SetTracking(enableTracking, item);
                    }

                    // Enable tracking if we have not stopped recursion
                    if (!stopRecursion)
                    {
                        trackingColl.Parent = item;
                        trackingColl.Tracking = enableTracking;
                    }
                }
            }
        }

        /// <summary>
        /// Recursively set tracking state on child collections in an object graph.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <param name="state">Change-tracking state of an entity</param>
        /// <param name="parent">ITrackable parent of item</param>
        public static void SetState(this ITrackable item, TrackingState state, 
            ITrackable parent = null)
        {
            foreach (var prop in item.GetType().GetProperties(BindingFlags.Instance
                | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var trackingColl = prop.GetValue(item, null) as ITrackingCollection;
                if (trackingColl != null)
                {
                    // Continue if setting reference prop to added or deleted
                    var propGetter = prop.GetGetMethod(true);
                    if (propGetter != null && propGetter.IsPrivate
                        && (state == TrackingState.Added || state == TrackingState.Deleted))
                        continue;

                    // Recursively set state
                    foreach (ITrackable child in trackingColl)
                    {
                        // Stop recursion if trackable is same type as parent
                        if (parent != null && (child.GetType() == parent.GetType()))
                            break;
                        child.SetState(state, item);
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
        /// <param name="parent">ITrackable parent of item</param>
        public static void SetModifiedProperties(this ITrackable item,
            ICollection<string> modified, ITrackable parent = null)
        {
            foreach (var prop in item.GetType().GetProperties(BindingFlags.Instance
                | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var trackingColl = prop.GetValue(item, null) as ITrackingCollection;
                if (trackingColl != null)
                {
                    // Recursively set modified
                    foreach (ITrackable child in trackingColl)
                    {
                        // Stop recursion if trackable is same type as parent
                        if (parent != null && (child.GetType() == parent.GetType()))
                            break;
                        child.SetModifiedProperties(modified, item);
                        child.ModifiedProperties = modified;
                    }
                }
            }
        }

        /// <summary>
        /// Recursively set child collections in an object graph to changed items.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <param name="parent">ITrackable parent of item</param>
        public static void SetChanges(this ITrackable item, ITrackable parent = null)
        {
            foreach (var prop in item.GetType().GetProperties(BindingFlags.Instance
                | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var trackingColl = prop.GetValue(item, null) as ITrackingCollection;
                if (trackingColl != null)
                {
                    // Recursively set changes
                    bool stopRecursion = false;
                    foreach (ITrackable child in trackingColl)
                    {
                        // Stop recursion if trackable is same type as parent
                        if (parent != null && (child.GetType() == parent.GetType()))
                        {
                            stopRecursion = true;
                            break;
                        }
                        child.SetChanges(item);
                    }

                    // Set collection property on cloned item
                    if (!stopRecursion)
                    {
                        ITrackingCollection changes = trackingColl.GetChanges();
                        prop.SetValue(item, changes, null); 
                    }
                }
            }
        }

        /// <summary>
        /// Recursively restore deletes from child collections in an object graph.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <param name="parent">ITrackable parent of item</param>
        public static void RestoreDeletes(this ITrackable item, ITrackable parent = null)
        {
            foreach (var prop in item.GetType().GetProperties(BindingFlags.Instance
                | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var trackingColl = prop.GetValue(item, null) as ITrackingCollection;
                if (trackingColl != null)
                {
                    // Recursively restore deletes
                    bool stopRecursion = false;
                    foreach (ITrackable child in trackingColl)
                    {
                        // Stop recursion if trackable is same type as parent
                        if (parent != null && (child.GetType() == parent.GetType()))
                        {
                            stopRecursion = true;
                            break;
                        }
                        child.RestoreDeletes(item);
                    }

                    // Restore deletes on collection
                    if (!stopRecursion)
                        trackingColl.RestoreDeletes();
                }
            }
        }

        /// <summary>
        /// Recursively remove deletes from child collections in an object graph.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <param name="enableTracking">Set to true to track removed items</param>
        /// <param name="parent">ITrackable parent of item</param>
        public static void RemoveDeletes(this ITrackable item, bool enableTracking,
            ITrackable parent = null)
        {
            foreach (var prop in item.GetType().GetProperties(BindingFlags.Instance
                | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var trackingColl = prop.GetValue(item, null) as ITrackingCollection;
                if (trackingColl != null)
                {
                    // Recursively remove deletes
                    bool stopRecursion = false;
                    foreach (ITrackable child in trackingColl)
                    {
                        if (parent != null && (child.GetType() == parent.GetType()))
                        {
                            stopRecursion = true;
                            break;
                        }
                        child.RemoveDeletes(enableTracking, item);
                    }

                    // Remove deletes on collection
                    if (!stopRecursion)
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
        /// <param name="parent">ITrackable parent of item</param>
        public static bool HasChanges(this ITrackable item, ITrackable parent = null)
        {
            foreach (var prop in item.GetType().GetProperties(BindingFlags.Instance
                | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var trackingColl = prop.GetValue(item, null) as ITrackingCollection;
                if (trackingColl != null)
                {
                    // Recursively check for child collection changes
                    bool stopRecursion = false;
                    foreach (ITrackable child in trackingColl)
                    {
                        // Stop recursion if trackable is same type as parent
                        if (parent != null && (child.GetType() == parent.GetType()))
                        {
                            stopRecursion = true;
                            break;
                        }
                        bool hasChanges = child.HasChanges(item);
                        if (hasChanges) return true;
                    }

                    // Return true if child collection has changes
                    if (!stopRecursion)
                    {
                        ITrackingCollection changes = trackingColl.GetChanges();
                        if (changes.Count > 0) return true; 
                    }
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
