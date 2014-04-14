using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Linq;
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
            // Include private props to get ref prop change tracker
            foreach (var prop in item.GetType().GetProperties(BindingFlags.Instance
                | BindingFlags.Public | BindingFlags.NonPublic))
            {
                // Set tracking on 1-1 and M-1 properties
                var trackableRef = prop.GetValue(item, null) as ITrackable;

                // Stop recursion if trackable is same type as parent
                if (trackableRef != null
                    && (parent == null || trackableRef.GetType() != parent.GetType()))
                {
                    // Get ref prop change tracker
                    ITrackingCollection refChangeTracker = item.GetRefPropertyChangeTracker(prop.Name);
                    if (refChangeTracker != null)
                    {
                        // Set tracking on ref prop change tracker
                        refChangeTracker.Parent = item;
                        refChangeTracker.Tracking = enableTracking;

                        // Reset parent because ref prop can have more than one parent
                        refChangeTracker.Parent = null; 
                    }
                }

                // Set tracking on 1-M and M-M properties
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
            // Include private props to get ref prop change tracker
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
            // Include private props to get ref prop change tracker
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
            // Include private props to get ref prop change tracker
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
            // Include private props to get ref prop change tracker
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
        /// Recursively remove items marked as deleted.
        /// </summary>
        /// <param name="changeTracker">Change-tracking collection</param>
        /// <param name="enableTracking">True to cache deleted items</param>
        /// <param name="parent">Parent ITrackable object</param>
        public static void RemoveDeletes(this ITrackingCollection changeTracker, 
            bool enableTracking, ITrackable parent = null)
        {
            // Iterate items in change-tracking collection
            var items = changeTracker as IList;
            if (items == null) return;
            for (int i = changeTracker.Count - 1; i > -1; i--)
            {
                // Get trackable item
                var item = items[i] as ITrackable;
                if (item == null) continue;

                // Iterate entity properties
                foreach (var prop in item.GetType().GetProperties())
                {
                    // Process 1-1 and M-1 properties
                    var trackableRef = prop.GetValue(item, null) as ITrackable;

                    // Stop recursion if trackable is same type as parent
                    if (trackableRef != null
                        && (parent == null || trackableRef.GetType() != parent.GetType()))
                    {
                        // Get changed ref prop
                        ITrackingCollection refChangeTracker = item.GetRefPropertyChangeTracker(prop.Name);
                        if (refChangeTracker != null) refChangeTracker.RemoveDeletes(enableTracking, item);
                    }

                    // Process 1-M and M-M properties
                    var trackingItems = prop.GetValue(item, null) as IList;
                    var trackingColl = trackingItems as ITrackingCollection;

                    // Remove deletes on child collection
                    if (trackingItems != null && trackingColl != null
                        && trackingColl.Count > 0)
                    {
                        // Stop recursion if trackable is same type as parent
                        var trackableChild = trackingItems[0] as ITrackable;
                        if (parent == null || (trackableChild != null && trackableChild.GetType() != parent.GetType()))
                        {
                            // Remove deletes on child collection
                            trackingColl.RemoveDeletes(enableTracking, item);
                        }
                    }
                }

                // Remove item if marked as deleted
                if (item.TrackingState == TrackingState.Deleted)
                {
                    var isTracking = changeTracker.Tracking;
                    changeTracker.Tracking = enableTracking;
                    items.Remove(item);
                    changeTracker.Tracking = isTracking;
                }
            }
        }

        /* /// <summary>
        /// Recursively remove deletes from child collections in an object graph.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <param name="enableTracking">Set to true to track removed items</param>
        /// <param name="parent">ITrackable parent of item</param>
        [Obsolete("Remove this method after refactor")]
        public static void RemoveDeletes(this ITrackable item, bool enableTracking,
            ITrackable parent = null)
        {
            // Include private props to get ref prop change tracker
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
        /// Remove deletes from a trackable collection.
        /// </summary>
        /// <param name="trackingColl">Trackable collection</param>
        /// <param name="enableTracking">Set to true to track removed items</param>
        [Obsolete("Remove this method after refactor")]
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
        }*/

        /// <summary>
        /// Recursively get items with child collections that have changes.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <returns>True if item has child collections that have changes</returns>
        /// <param name="parent">ITrackable parent of item</param>
        public static bool HasChanges(this ITrackable item, ITrackable parent = null)
        {
            // Include private props to get ref prop change tracker
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

        /// <summary>
        /// Determines if two entities have the same identifier.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <param name="other">Other trackable object</param>
        /// <returns>True if item is equatable, otherwise false</returns>
        public static bool IsEquatable(this ITrackable item, ITrackable other)
        {
            var method = GetEquatableMethod(item.GetType());
            if (method != null)
            {
                return (bool)method.Invoke(item, new object[] { other });
            }
            return false;
        }

        /// <summary>
        /// Get entity identifier for correlation with MergeChanges.
        /// </summary>
        /// <param name="item">ITrackable object</param>
        /// <returns>Entity identifier used for correlation with MergeChanges</returns>
        public static Guid GetEntityIdentifier(this ITrackable item)
        {
            var property = GetEntityIdentifierProperty(item.GetType());
            if (property == null) return default(Guid);
            return (Guid)property.GetValue(item, null);
        }

        /// <summary>
        /// Set entity identifier used for correlation with MergeChanges.
        /// </summary>
        /// <param name="item">ITrackable object</param>
        /// <param name="value">Unique identifier (optional)</param>
        public static void SetEntityIdentifier(this ITrackable item, Guid? value = null)
        {
            // Get entity identifier property
            var property = GetEntityIdentifierProperty(item.GetType());
            if (property == null) return;

            // Set entity identifier prop value explicitly
            if (value != null)
            {
                if ((Guid)value != default(Guid))
                    item.SetEntityIdentity((Guid)value);
                property.SetValue(item, value, null);
                return;
            }

            // Get entity identifier prop value
            var entityIdentifier = (Guid)property.GetValue(item, null);

            // If entity identifier prop has not been set yet,
            // set it based on entity identity field.
            if (entityIdentifier == default(Guid))
            {
                var entityIdentity = item.GetOrSetEntityIdentity();
                property.SetValue(item, entityIdentity, null);
            }
        }

        /// <summary>
        /// Get value of entity identity used for setting EntityIdentifier.
        /// </summary>
        /// <param name="item">ITrackable object</param>
        /// <returns>Value of entity identity field</returns>
        public static Guid GetEntityIdentity(this ITrackable item)
        {
            var field = GetEntityIdentifyField(item.GetType());
            if (field == null) return default(Guid);
            return (Guid)field.GetValue(item);
        }

        /// <summary>
        /// Set value of entity identity used for setting EntityIdentifier.
        /// </summary>
        /// <param name="item">ITrackable object</param>
        /// <param name="value">Value for entity identity field</param>
        public static void SetEntityIdentity(this ITrackable item, Guid value)
        {
            var field = GetEntityIdentifyField(item.GetType());
            if (field == null) return;
            field.SetValue(item, value);
        }

        /// <summary>
        /// Get reference property change tracker.
        /// </summary>
        /// <param name="item">ITrackable object</param>
        /// <param name="propertyName">Reference property name</param>
        /// <returns>Reference property change tracker</returns>
        public static ITrackingCollection GetRefPropertyChangeTracker(this ITrackable item, string propertyName)
        {
            var property = GetChangeTrackingProperty(item.GetType(), propertyName);
            if (property == null) return null;
            return property.GetValue(item, null) as ITrackingCollection;
        }

        private static PropertyInfo GetChangeTrackingProperty(Type entityType, string propertyName)
        {
            var property = entityType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
                .SingleOrDefault(m => m.Name == propertyName + Constants.ChangeTrackingMembers.ChangeTrackingPropEnd);
            return property;
        }

        private static MethodInfo GetEquatableMethod(Type type)
        {
            var method = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .SingleOrDefault(m => m.Name.StartsWith(Constants.EquatableMembers.EquatableMethodStart)
                    && m.Name.EndsWith(Constants.EquatableMembers.EquatableMethodEnd));
            return method;
        }

        private static PropertyInfo GetEntityIdentifierProperty(Type type)
        {
            var property = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
                .SingleOrDefault(m => m.Name == Constants.EquatableMembers.EntityIdentifierProperty);
            return property;
        }

        private static FieldInfo GetEntityIdentifyField(Type type)
        {
            var property = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .SingleOrDefault(m => m.Name == Constants.EquatableMembers.EntityIdentifyField);
            return property;
        }

        private static Guid GetOrSetEntityIdentity(this ITrackable item)
        {
            var newIdentity = Guid.NewGuid();
            var field = GetEntityIdentifyField(item.GetType());
            if (field != null)
            {
                var entityIdentity = (Guid)field.GetValue(item);
                if (entityIdentity != default(Guid))
                    return entityIdentity;
                field.SetValue(item, newIdentity);
            }
            return newIdentity;
        }
    }
}
