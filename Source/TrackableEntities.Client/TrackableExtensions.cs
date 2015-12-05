using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using TrackableEntities.Common;
using Newtonsoft.Json.Serialization;

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
        /// <param name="oneToManyOnly">True if tracking should be set only for OneToMany relations</param>
        public static void SetTracking(this ITrackable item, bool enableTracking, 
            ObjectVisitationHelper visitationHelper = null, bool oneToManyOnly = false)
        {
            // Iterator entity properties
            foreach (var navProp in item.GetNavigationProperties())
            {
                // Skip if 1-M only
                if (!oneToManyOnly)
                {
                    // Set tracking on 1-1 and M-1 properties
                    foreach (var refProp in navProp.AsReferenceProperty())
                    {
                        // Get ref prop change tracker
                        ITrackingCollection refChangeTracker = item.GetRefPropertyChangeTracker(refProp.Property.Name);
                        if (refChangeTracker != null)
                        {
                            // Set tracking on ref prop change tracker
                            refChangeTracker.SetTracking(enableTracking, visitationHelper, oneToManyOnly);
                        }
                    } 
                }

                // Set tracking on 1-M and M-M properties (if not 1-M only)
                foreach (var colProp in navProp.AsCollectionProperty<ITrackingCollection>())
                {
                    bool isOneToMany = !IsManyToManyChildCollection(colProp.EntityCollection);
                    if (!oneToManyOnly || isOneToMany)
                        colProp.EntityCollection.SetTracking(enableTracking, visitationHelper, oneToManyOnly);
                }
            }
        }

        /// <summary>
        /// Recursively set tracking state on trackable entities in an object graph.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <param name="state">Change-tracking state of an entity</param>
        /// <param name="visitationHelper">Circular reference checking helper</param>
        /// <param name="isManyToManyItem">True is an item is treated as part of an M-M collection</param>
        public static void SetState(this ITrackable item, TrackingState state, ObjectVisitationHelper visitationHelper,
            bool isManyToManyItem = false)
        {
            ObjectVisitationHelper.EnsureCreated(ref visitationHelper);

            // Prevent endless recursion
            if (!visitationHelper.TryVisit(item)) return;

            // Recurively set state for unchanged, added or deleted items,
            // but not for M-M child item
            if (!isManyToManyItem && state != TrackingState.Modified)
            {
                // Iterate entity properties
                foreach (var colProp in item.GetNavigationProperties().OfCollectionType<ITrackingCollection>())
                {
                    // Process 1-M and M-M properties
                    // Set state on child entities
                    bool isManyToManyChildCollection = IsManyToManyChildCollection(colProp.EntityCollection);
                    foreach (ITrackable trackableChild in colProp.EntityCollection)
                    {
                        trackableChild.SetState(state, visitationHelper,
                            isManyToManyChildCollection);
                    }
                }
            }

            // Deleted items are treated a bit specially
            if (state == TrackingState.Deleted)
            {
                if (isManyToManyItem)
                {
                    // With M-M properties there is no way to tell if the related entity should be deleted
                    // or simply removed from the relationship, because it is an independent association.
                    // Therefore, deleted children are marked unchanged.
                    if (item.TrackingState != TrackingState.Modified)
                        item.TrackingState = TrackingState.Unchanged;
                    return;
                }
                // When deleting added item, set state to unchanged
                else if (item.TrackingState == TrackingState.Added)
                {
                    item.TrackingState = TrackingState.Unchanged;
                    return;
                }
            }

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
            // Prevent endless recursion
            ObjectVisitationHelper.EnsureCreated(ref visitationHelper);
            if (!visitationHelper.TryVisit(item)) return;

            // Iterate entity properties
            foreach (var colProp in item.GetNavigationProperties().OfCollectionType<ITrackingCollection>())
            {
                // Recursively set modified
                foreach (ITrackable child in colProp.EntityCollection)
                {
                    child.SetModifiedProperties(modified, visitationHelper);
                    child.ModifiedProperties = modified;
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
                if (!visitationHelper.TryVisit(item)) continue;

                // Iterate entity properties
                foreach (var navProp in item.GetNavigationProperties())
                {
                    // Process 1-1 and M-1 properties
                    foreach (var refProp in navProp.AsReferenceProperty())
                    {
                        // Get changed ref prop
                        ITrackingCollection refChangeTracker = item.GetRefPropertyChangeTracker(refProp.Property.Name);

                        // Remove deletes on rep prop
                        if (refChangeTracker != null) refChangeTracker.RemoveRestoredDeletes(visitationHelper);
                    }

                    // Process 1-M and M-M properties
                    foreach (var colProp in navProp.AsCollectionProperty<ITrackingCollection>())
                    {
                        colProp.EntityCollection.RemoveRestoredDeletes(visitationHelper);
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
                if (!visitationHelper.TryVisit(item)) continue;

                // Iterate entity properties
                foreach (var navProp in item.GetNavigationProperties())
                {
                    // Process 1-1 and M-1 properties
                    foreach (var refProp in navProp.AsReferenceProperty())
                    {
                        // Get changed ref prop
                        ITrackingCollection refChangeTracker = item.GetRefPropertyChangeTracker(refProp.Property.Name);
                        
                        // Restore deletes on rep prop
                        if (refChangeTracker != null) refChangeTracker.RestoreDeletes(visitationHelper);
                    }

                    // Process 1-M and M-M properties
                    foreach (var colProp in navProp.AsCollectionProperty<ITrackingCollection>())
                    {
                        colProp.EntityCollection.RestoreDeletes(visitationHelper);
                    }
                }
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

        internal static T CloneObject<T>(T item, IContractResolver contractResolver = null)
            where T : class
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BsonWriter(stream))
                {
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        ContractResolver = contractResolver ?? new EntityNavigationPropertyResolver(),
                        PreserveReferencesHandling = PreserveReferencesHandling.All
                    };
                    var serWr = JsonSerializer.Create(settings);
                    serWr.Serialize(writer, item);

                    stream.Position = 0;
                    using (var reader = new BsonReader(stream))
                    {
                        settings.ContractResolver = new EntityNavigationPropertyResolver();
                        var serRd = JsonSerializer.Create(settings);
                        var copy = serRd.Deserialize<T>(reader);
                        return copy;
                    }
                }
            }
        }

        private class EntityNavigationPropertyResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);
                property.ShouldSerialize =
                    instance =>
                        {
                            var entity = instance as ITrackable;
                            if (entity == null) return true;

                            // The current property is a navigation property and its value is null
                            bool isEmptyNavProp = 
                                (from np in entity.GetNavigationProperties(false)
                                 where np.Property == member
                                 select np.ValueIsNull).Any(isNull => isNull);

                            return !isEmptyNavProp;
                        };
                return property;
            }
        }

        /// <summary>
        /// Get reference property change tracker.
        /// </summary>
        /// <param name="item">ITrackable object</param>
        /// <param name="propertyName">Reference property name</param>
        /// <returns>Reference property change tracker</returns>
        public static ITrackingCollection GetRefPropertyChangeTracker(this ITrackable item, string propertyName)
        {
            var resolver = item as IRefPropertyChangeTrackerResolver;
            if (resolver != null)
                return resolver.GetRefPropertyChangeTracker(propertyName);

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

        internal static IEnumerable<Type> BaseTypes(this Type type)
        {
            for (Type t = type; t != null; t = PortableReflectionHelper.Instance.GetBaseType(t))
                yield return t;
        }

        private static PropertyInfo GetChangeTrackingProperty(Type entityType, string propertyName)
        {
            var property = entityType.BaseTypes()
                .SelectMany(t => PortableReflectionHelper.Instance.GetPrivateInstanceProperties(t))
                .SingleOrDefault(p => p.Name == propertyName + Constants.ChangeTrackingMembers.ChangeTrackingPropEnd);
            return property;
        }
    }
}
