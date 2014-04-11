using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TrackableEntities.Client;

namespace TrackableEntities.Client_Lame
{
    /// <summary>
    /// ITrackable extension methods.
    /// </summary>
    public static class Trackable
    {
        /// <summary>
        /// <para>Merge changes from updated ITrackable object into original ITrackable object.</para>
        /// <para>First call GetChanges on change tracker, passing changes to service update operation.</para>
        /// <para>Then call MergeChanges on updated entity, passing original entity by reference.</para>
        /// <para>Original entity reference will point to updated entity with unchanged child entities merged into it.</para>
        /// <code>
        /// // Usage:
        /// // Start change-tracking originalEntity (new or retrieved from service)
        /// var changeTracker = new ChangeTrackingCollection(originalEntity);
        /// 
        /// // Make changes to originalEntity (including reference and child entities)
        /// originalEntity.ShipDate = DateTime.Today;
        /// originalEntity.Customer.City = "New York";
        /// originalEntity.Details[0].Quantity++;
        /// originalEntity.Details.RemoveAt(1);
        /// originalEntity.Details.Add(new detail);
        /// 
        /// // Get only changed entities (exclude unchanged child entities)
        /// var changedEntity = changeTracker.GetChanges().SingleOrDefault();
        /// 
        /// // Pass changedEntity to service update operation
        /// var updatedEntity = service.Update(changedEntity);
        /// 
        /// // Merge updates from updatedEntity back into originalEntity
        /// updatedEntity.MergeChanges(ref originalEntity, changeTracker);
        /// </code>
        /// </summary>
        /// <param name="updatedItem">Entity updated with changes from a service update operation</param>
        /// <param name="originalItem">Original change-tracked entity</param>
        /// <param name="changeTracker">Change tracker used to track changes on original entity</param>
        public static void MergeChanges<TEntity>(this TEntity updatedItem, 
            ref TEntity originalItem, ChangeTrackingCollection<TEntity> changeTracker)
                where TEntity : class, ITrackable, INotifyPropertyChanged
        {
            // Check for null items
            if (updatedItem == null) throw new ArgumentNullException("updatedItem");
            if (originalItem == null) throw new ArgumentNullException("originalItem");

            // Recursively set tracking state for child collections
            ITrackable originalObj = originalItem;
            updatedItem.MergeChanges(ref originalObj, changeTracker, null);
            originalItem = (TEntity)originalObj;
        }

        private static void MergeChanges(this ITrackable updatedItem, ref ITrackable originalItem, 
            ITrackingCollection originalItemChangeTracker, ITrackable updatedItemParent)
        {
            // Check for null items
            if (updatedItem == null) throw new ArgumentNullException("updatedItem");
            if (originalItem == null) throw new ArgumentNullException("originalItem");

            // Get unchanged child entities on original item
            foreach (var prop in updatedItem.GetType().GetProperties())
            {
                // Apply changes to 1-1 and M-1 properties
                var origTrackableRef = prop.GetValue(originalItem, null) as ITrackable;
                var updatedTrackableRef = prop.GetValue(updatedItem, null) as ITrackable;

                // Stop recursion if trackable is same type as parent
                if (origTrackableRef != null && updatedTrackableRef != null
                    && (updatedItemParent == null || updatedTrackableRef.GetType() != updatedItemParent.GetType()))
                {
                    // Get ref prop change tracker
                    ITrackingCollection refPropChangeTracker = GetRefPropChangeTracker(originalItem, prop.Name);
                    if (refPropChangeTracker != null)
                        updatedTrackableRef.MergeChanges(ref origTrackableRef, refPropChangeTracker, updatedItem);
                }

                var updatedItems = prop.GetValue(updatedItem, null) as IList;
                var originalItems = prop.GetValue(originalItem, null) as IList;
                var origItemsChangeTracker = originalItems as ITrackingCollection;
                if (originalItems != null && updatedItems != null 
                    && origItemsChangeTracker != null)
                {
                    for (int i = originalItems.Count - 1; i > -1; i--)
                    {
                        // Continue if orig item is added or deleted
                        var origTrackable = originalItems[i] as ITrackable;
                        if (origTrackable == null) continue;
                        if (origTrackable.TrackingState == TrackingState.Added
                            || origTrackable.TrackingState == TrackingState.Deleted)
                            continue;

                        // Stop recursion if trackable is same type as parent
                        var updatedTrackable = updatedItems.OfType<ITrackable>()
                            .FindEquatableItem(origTrackable);
                        if (updatedTrackable != null && (updatedItemParent == null 
                            || origTrackable.GetType() != updatedItemParent.GetType()))
                        {
                            updatedTrackable.MergeChanges(ref origTrackable,
                                origItemsChangeTracker, updatedItem);
                        }

                        // If orig trackable not found in updated items,
                        // add unchanged original item to updated items.
                        if (updatedTrackable == null && origTrackable.TrackingState == TrackingState.Unchanged)
                        {
                            updatedItems.Add(origTrackable);
                            FixUpParentReference(originalItemChangeTracker, origTrackable, updatedItem);
                        }
                    }
                }
            }

            // Track updated items
            originalItemChangeTracker.TrackUpdatedItem(originalItem, updatedItem);

            // Set original item to updated item with unchanged items merged in
            originalItem = updatedItem;
        }

        private static ITrackingCollection GetRefPropChangeTracker(ITrackable originalItem, string propertyName)
        {
            PropertyInfo refChangeTrackerProp = GetRefChangeTrackerProperty(originalItem.GetType(), propertyName);
            if (refChangeTrackerProp == null) return null;
            var refPropChangeTracker = refChangeTrackerProp.GetValue(originalItem, null) as ITrackingCollection;
            return refPropChangeTracker;
        }

        private static PropertyInfo GetRefChangeTrackerProperty(Type type, string propertyName)
        {
            var prop = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
                .SingleOrDefault(p => p.Name == propertyName + "ChangeTracker");
            return prop;
        }

        private static ITrackable FindEquatableItem(this IEnumerable<ITrackable> updatedTrackables, 
            ITrackable origTrackable)
        {
            ITrackable updatedTrackable = updatedTrackables
                .FirstOrDefault(t => ItemsAreEquatable(t, origTrackable));
            return updatedTrackable;
        }

        static bool ItemsAreEquatable(object a, object b)
        {
            Type type = a.GetType();
            var method = GetEquatableMethod(type);
            if (method != null)
            {
                return (bool)method.Invoke(a, new[] { b });
            }
            return false;
        }

        private static MethodInfo GetEquatableMethod(Type type)
        {
            var method = type.GetMethods(BindingFlags.Instance
                                         | BindingFlags.NonPublic)
                            .SingleOrDefault(m => m.Name.StartsWith("System.IEquatable<")
                                && m.Name.EndsWith(".Equals"));
            return method;
        }

        private static void FixUpParentReference(ITrackingCollection changeTracker, 
            object child, object parent)
        {
            bool isTracking = changeTracker.Tracking;
            foreach (var prop in child.GetType().GetProperties())
            {
                if (prop.PropertyType == parent.GetType())
                {
                    var childParent = prop.GetValue(child, null);
                    if (childParent != null && !ReferenceEquals(childParent, parent))
                    {
                        changeTracker.Tracking = false;
                        prop.SetValue(child, parent, null);
                        changeTracker.Tracking = isTracking;
                    }
                }
            }
        }

        private static void TrackUpdatedItem(this ITrackingCollection changeTracker,
            object originalItem, object updatedItem)
        {
            // Track updated item
            var list = changeTracker as IList;
            if (list == null) return;
            bool isTracking = changeTracker.Tracking;
            changeTracker.Tracking = false;
            list.Remove(originalItem);
            list.Add(updatedItem);
            changeTracker.Tracking = isTracking;
        }
    }
}
