using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using TrackableEntities.Common;

namespace TrackableEntities.Client
{
    internal static class TrackableExtensions
    {
        /// <summary>
        /// Recursively enable or disable tracking on trackable entities in an object graph.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <param name="enableTracking">Enable or disable change-tracking</param>
        /// <param name="visitationHelper">Circular reference checking helper</param>
        public static void SetTracking(this ITrackable item,
            bool enableTracking, ObjectVisitationHelper visitationHelper = null)
        {
            ObjectVisitationHelper.EnsureCreated(ref visitationHelper);

            // Prevent endless recursion
            if (visitationHelper.IsVisited(item)) return;

            // Iterator entity properties
            foreach (var prop in item.GetType().GetProperties())
            {
                // Set tracking on 1-1 and M-1 properties
                var trackableRef = prop.GetValue(item, null) as ITrackable;

                // Continue recursion
                if (trackableRef != null)
                {
                    // Get ref prop change tracker
                    ITrackingCollection refChangeTracker = item.GetRefPropertyChangeTracker(prop.Name);
                    if (refChangeTracker != null)
                    {
                        // Set tracking on ref prop change tracker
                        refChangeTracker.SetTracking(enableTracking, visitationHelper.With(item));
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
                        // Stop recursion if trackable has been already visited
                        if (visitationHelper.IsVisited(child))
                        {
                            stopRecursion = true;
                            break;
                        }
                    }

                    // Enable tracking if we have not stopped recursion
                    if (!stopRecursion)
                    {
                        bool isManyToMany = IsManyToManyChildCollection(trackingColl);
                        trackingColl.SetTracking(enableTracking, visitationHelper.With(item));
                    }
                }
            }
        }

        /// <summary>
        /// Recursively set tracking state on trackable entities in an object graph.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <param name="state">Change-tracking state of an entity</param>
        /// <param name="visitationHelper">Circular reference checking helper</param>
        public static void SetState(this ITrackable item, TrackingState state, ObjectVisitationHelper visitationHelper)
        {
            ObjectVisitationHelper.EnsureCreated(ref visitationHelper);

            // Prevent endless recursion
            if (visitationHelper.IsVisited(item)) return;

            // Recurively set state for unchanged, added or deleted items,
            // or if recursion has already begun.
            if (state == TrackingState.Unchanged || state == TrackingState.Added || state == TrackingState.Deleted)
            {
                // Iterate entity properties
                foreach (var prop in item.GetType().GetProperties())
                {
                    // Process 1-M and M-M properties
                    var trackingColl = prop.GetValue(item, null) as ITrackingCollection;
                    if (trackingColl != null)
                    {
                        // Set state on child entities
                        foreach (ITrackable trackableChild in trackingColl)
                        {
                            // Continue recursion if trackable hasn't been visited
                            if (trackableChild != null &&
                                (!visitationHelper.IsVisited(trackableChild)))
                            {
                                // Cascade state for Unchanged - AcceptChanges
                                // Also cascade state for Added, Deleted
                                switch (state)
                                {
                                    // Cascade unchanged and added state
                                    case TrackingState.Unchanged:
                                    case TrackingState.Added:
                                        // Cascade added state for 1-M properties
                                        if (!IsManyToManyChildCollection(trackingColl))
                                            trackableChild.SetState(state, visitationHelper.With(item));
                                        // M-M: No recursion
                                        else
                                            trackableChild.TrackingState = TrackingState.Added;
                                        break;
                                    case TrackingState.Deleted:
                                        // Cascade deleted state for 1-M properties
                                        // Deleting an added item will mark it as Unchanged (that is, not added)
                                        if (!IsManyToManyChildCollection(trackingColl))
                                            trackableChild.SetState((trackableChild.TrackingState == TrackingState.Added)
                                                ? TrackingState.Unchanged : state, visitationHelper.With(item));
                                        // Set deleted for M-M as unchanged state for added.
                                        // (Cached M-M child deletes will remain deleted.)
                                        else if (trackableChild.TrackingState == TrackingState.Added)
                                            trackableChild.TrackingState = TrackingState.Unchanged;
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            // When deleting added item, set state to unchanged,
            // otherwise set entity state
            if (state == TrackingState.Deleted && item.TrackingState == TrackingState.Added)
                item.TrackingState = TrackingState.Unchanged;
            else
                item.TrackingState = state;
        }

        /// <summary>
        /// Recursively set tracking state on trackable properties in an object graph.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <param name="modified">Properties on an entity that have been modified</param>
        /// <param name="visitationHelper">Circular reference checking helper</param>
        public static void SetModifiedProperties(this ITrackable item,
            ICollection<string> modified, ObjectVisitationHelper visitationHelper = null)
        {
            ObjectVisitationHelper.EnsureCreated(ref visitationHelper);

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
                        // Stop recursion if trackable has been already visited
                        if (visitationHelper.IsVisited(child))
                            break;
                        child.SetModifiedProperties(modified, visitationHelper.With(child));
                        child.ModifiedProperties = modified;
                    }
                }
            }
        }

        /// <summary>
        /// Recursively remove items marked as deleted.
        /// </summary>
        /// <param name="changeTracker">Change-tracking collection</param>
        /// <param name="visitationHelper">Circular reference checking helper</param>
        public static void RemoveRestoredDeletes(this ITrackingCollection changeTracker, ObjectVisitationHelper visitationHelper = null)
        {
            ObjectVisitationHelper.EnsureCreated(ref visitationHelper);

            // Prevent endless recursion
            if (visitationHelper.IsVisited(changeTracker)) return;

            visitationHelper = visitationHelper.With(changeTracker);

            // Iterate items in change-tracking collection
            var items = changeTracker as IList;
            if (items == null) return;
            var count = items.Count;

            for (int i = count - 1; i > -1; i--)
            {
                // Get trackable item
                var item = items[i] as ITrackable;
                if (item == null) continue;

                // Prevent endless recursion
                if (visitationHelper.IsVisited(item)) continue;

                // Iterate entity properties
                foreach (var prop in item.GetType().GetProperties())
                {
                    // Process 1-1 and M-1 properties
                    var trackableRef = prop.GetValue(item, null) as ITrackable;

                    // Continue recursion
                    if (trackableRef != null)
                    {
                        // Get changed ref prop
                        ITrackingCollection refChangeTracker = item.GetRefPropertyChangeTracker(prop.Name);

                        // Remove deletes on rep prop
                        if (refChangeTracker != null) refChangeTracker.RemoveRestoredDeletes(visitationHelper.With(item));
                    }

                    // Process 1-M and M-M properties
                    var trackingColl = prop.GetValue(item, null) as ITrackingCollection;

                    // Remove deletes on child collection
                    if (trackingColl != null)
                    {
                        trackingColl.RemoveRestoredDeletes(visitationHelper.With(item));
                    }
                }

                // Remove item if marked as deleted
                var removedDeletes = changeTracker.GetChanges(true).Cast<ITrackable>().ToList();
                if (item.TrackingState == TrackingState.Deleted)
                {
                    var isTracking = changeTracker.Tracking;
                    changeTracker.Tracking = false;
                    if (removedDeletes.Contains(item))
                        items.Remove(item);
                    changeTracker.Tracking = isTracking;
                }
            }
        }

        /// <summary>
        /// Restore items marked as deleted.
        /// </summary>
        /// <param name="changeTracker">Change-tracking collection</param>
        /// <param name="visitationHelper">Circular reference checking helper</param>
        public static void RestoreDeletes(this ITrackingCollection changeTracker, ObjectVisitationHelper visitationHelper = null)
        {
            ObjectVisitationHelper.EnsureCreated(ref visitationHelper);

            // Prevent endless recursion
            if (visitationHelper.IsVisited(changeTracker)) return;

            visitationHelper = visitationHelper.With(changeTracker);

            // Get cached deletes
            var removedDeletes = changeTracker.GetChanges(true).Cast<ITrackable>().ToList();

            // Restore deleted items
            if (removedDeletes.Any())
            {
                var isTracking = changeTracker.Tracking;
                changeTracker.Tracking = false;
                foreach (var delete in removedDeletes)
                {
                    var items = changeTracker as IList;
                    if (items != null && !items.Contains(delete))
                        items.Add(delete);
                }
                changeTracker.Tracking = isTracking;
            }

            foreach (var item in changeTracker.Cast<ITrackable>())
            {
                // Prevent endless recursion
                if (visitationHelper.IsVisited(item)) continue;

                // Iterate entity properties
                foreach (var prop in item.GetType().GetProperties())
                {
                    // Process 1-1 and M-1 properties
                    var trackableRef = prop.GetValue(item, null) as ITrackable;

                    // Continue recursion
                    if (trackableRef != null)
                    {
                        // Get changed ref prop
                        ITrackingCollection refChangeTracker = item.GetRefPropertyChangeTracker(prop.Name);
                        
                        // Restore deletes on rep prop
                        if (refChangeTracker != null) refChangeTracker.RestoreDeletes(visitationHelper.With(item));
                    }

                    // Process 1-M and M-M properties
                    var trackingColl = prop.GetValue(item, null) as ITrackingCollection;

                    // Restore deletes on child collection
                    if (trackingColl != null)
                    {
                        trackingColl.RestoreDeletes(visitationHelper.With(item));
                    }
                }
            }
        }

        /// <summary>
        /// Get entities that have been added, modified or deleted, including trackable 
        /// reference and child entities.
        /// </summary>
        /// <param name="items">Collection of ITrackable objects</param>
        /// <param name="visitationHelper">Circular reference checking helper</param>
        /// <returns>Collection containing only added, modified or deleted entities</returns>
        internal static IEnumerable<ITrackable> GetChanges(this IEnumerable<ITrackable> items, ObjectVisitationHelper visitationHelper)
        {
            // Iterate items in change-tracking collection
            foreach (ITrackable item in items)
            {
                // Prevent endless recursion
                if (visitationHelper.IsVisited(item)) continue;

                // Downstream changes flag
                bool hasDownstreamChanges = false;

                // Iterate entity properties
                foreach (var prop in item.GetType().GetProperties())
                {
                    // Process 1-1 and M-1 properties
                    var trackableRef = prop.GetValue(item, null) as ITrackable;

                    // Continue recursion if trackable hasn't been visited
                    if (trackableRef != null
                        && (!visitationHelper.IsVisited(trackableRef)))
                    {
                        // Get changed ref prop
                        ITrackingCollection refChangeTracker = item.GetRefPropertyChangeTracker(prop.Name);
                        if (refChangeTracker != null)
                        {
                            // Get downstream changes
                            IEnumerable<ITrackable> refPropItems = refChangeTracker.Cast<ITrackable>();
                            IEnumerable<ITrackable> refPropChanges = refPropItems.GetChanges(visitationHelper.With(item));

                            // Set flag for downstream changes
                            hasDownstreamChanges = refPropChanges.Any(t => t.TrackingState != TrackingState.Deleted) ||
                                                   trackableRef.TrackingState == TrackingState.Added ||
                                                   trackableRef.TrackingState == TrackingState.Modified;

                            // Set ref prop to null if unchanged or deleted
                            if (!hasDownstreamChanges && 
                                (trackableRef.TrackingState == TrackingState.Unchanged
                                || trackableRef.TrackingState == TrackingState.Deleted))
                            {
                                prop.SetValue(item, null, null);
                                continue;
                            }
                        }
                    }

                    // Process 1-M and M-M properties
                    var trackingItems = prop.GetValue(item, null) as IList;
                    var trackingColl = trackingItems as ITrackingCollection;

                    // Get changes on child collection
                    if (trackingItems != null && trackingColl != null
                        && trackingColl.Count > 0)
                    {
                        // Continue recursion if trackable hasn't been visited
                        if (!visitationHelper.IsVisited(trackingColl))
                        {
                            // Get changes on child collection
                            var trackingCollChanges = IsManyToManyChildCollection(trackingColl) ?
                                trackingColl.Cast<ITrackable>().Where(t => t.TrackingState != TrackingState.Unchanged).ToList() :
                                trackingColl.Cast<ITrackable>().GetChanges(visitationHelper.With(trackingColl)).ToList();

                            // Set flag for downstream changes
                            hasDownstreamChanges = hasDownstreamChanges || trackingCollChanges.Any();

                            // Remove child items without changes
                            var count = trackingItems.Count;
                            for (int i = count - 1; i > -1; i--)
                            {
                                if (!trackingCollChanges.Any(e => ReferenceEquals(trackingItems[i], e)))
                                    trackingItems.Remove(trackingItems[i]);
                            }
                        }
                    }
                }

                // Return item if it has changes
                if (hasDownstreamChanges || item.TrackingState != TrackingState.Unchanged)
                    yield return item;
            }
        }

        /// <summary>
        /// Performs a deep copy using Json binary serializer.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="item">Trackable object</param>
        /// <returns>Cloned Trackable object</returns>
        public static T Clone<T>(this T item)
            where T : class, ITrackable
        {
            return CloneObject(item);
        }

        /// <summary>
        /// Performs a deep copy using Json binary serializer.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="items">Collection of Trackable objects</param>
        /// <returns>Cloned collection of Trackable object</returns>
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> items)
            where T : class, ITrackable
        {
            return CloneObject(new CollectionSerializationHelper<T>() { Result = items }).Result;
        }

        private class CollectionSerializationHelper<T>
        {
            [JsonProperty]
            public IEnumerable<T> Result;
        }

        private static T CloneObject<T>(T item)
            where T : class
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

        /// <summary>
        /// Determine if an entity is a child of a many-to-many change-tracking collection property.
        /// </summary>
        /// <param name="changeTracker">Change-tracking collection</param>
        /// <returns></returns>
        public static bool IsManyToManyChildCollection(ITrackingCollection changeTracker)
        {
            // Entity is a M-M child if change-tracking collection has a non-null Parent property
            bool isManyToManyChild = changeTracker.Parent != null;
            return isManyToManyChild;
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
