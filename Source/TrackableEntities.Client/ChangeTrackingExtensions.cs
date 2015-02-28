using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TrackableEntities.Common;

namespace TrackableEntities.Client
{
    /// <summary>
    /// ITrackable extension methods.
    /// </summary>
    public static class ChangeTrackingExtensions
    {
        /// <summary>
        /// <para>Merge changes from a one or more updated entities into original entities.</para>
        /// <para>First call GetChanges on the change tracker, passing changes to a service update operation.</para>
        /// <para>Then call MergeChanges, passing one or more updated entities from the service update operation.</para>
        /// <para>Properties on original entities will be set to those from updated entities.</para>
        /// <code>
        /// // Usage:
        /// // Start change-tracking originalEntity (new or retrieved from service)
        /// var changeTracker = new ChangeTrackingCollection(originalEntity);
        /// 
        /// // Make changes to originalEntity, including reference and child entities
        /// 
        /// // Get changes
        /// var changedEntity = changeTracker.GetChanges().SingleOrDefault();
        /// 
        /// // Pass changes to service update operation
        /// var updatedEntity = service.Update(changedEntity);
        /// 
        /// // Merge updates from updated entity back into the original entity
        /// changeTracker.MergeChanges(updatedEntity);
        /// </code>
        /// </summary>
        /// <typeparam name="TEntity">Trackable entity type</typeparam>
        /// <param name="changeTracker">Change tracker used to track changes on original entities</param>
        /// <param name="updatedItems">One or more entities updated with changes from a service update operation</param>
        /// <exception cref="ArgumentException">
        /// <para>Entity must implement IEquatable(TEntity)</para>
        /// <para>Update Trackable Entities Visual Studio Extension to v 2.0 or later, then re-generate client entities.</para>
        /// </exception>
        public static void MergeChanges<TEntity>(this ChangeTrackingCollection<TEntity> changeTracker,
            params TEntity[] updatedItems)
                where TEntity : class, ITrackable, IIdentifiable, INotifyPropertyChanged
        {
            // Check for no items
            if (updatedItems == null) throw new ArgumentNullException("updatedItems");

            // Recursively set tracking state for child collections
            changeTracker.MergeChanges(updatedItems, null);
        }

        private static void MergeChanges(this ITrackingCollection originalChangeTracker,
            IEnumerable<ITrackable> updatedItems, ObjectVisitationHelper visitationHelper, bool isTrackableRef = false)
        {
            ObjectVisitationHelper.EnsureCreated(ref visitationHelper);

            // Process each updated item
            foreach (var updatedItem in updatedItems)
            {
                // Prevent endless recursion
                if (!visitationHelper.TryVisit(updatedItem)) continue;

                // Get matching orig item
                var origItem = originalChangeTracker.Cast<ITrackable>()
                    .GetEquatableItem(updatedItem, isTrackableRef);
                if (origItem == null) continue;

                // Back fill entity identity on trackable ref
                if (isTrackableRef)
                {
                    var origItemIdentifiable = (IIdentifiable)origItem;
                    origItemIdentifiable.SetEntityIdentifier();
                    ((IIdentifiable)updatedItem).SetEntityIdentifier(origItemIdentifiable);
                }
                
                // Iterate entity properties
                foreach (var navProp in updatedItem.GetNavigationProperties())
                {
                    // Set to 1-1 and M-1 properties
                    foreach (var refProp in navProp.AsReferenceProperty())
                    {
                        ITrackingCollection refPropChangeTracker = origItem.GetRefPropertyChangeTracker(navProp.Property.Name);
                        if (refPropChangeTracker != null)
                            refPropChangeTracker.MergeChanges(new[] { refProp.EntityReference }, visitationHelper, true);
                    }

                    // Set 1-M and M-M properties
                    foreach (var colProp in navProp.AsCollectionProperty())
                    {
                        var origItemsChangeTracker = origItem.GetEntityCollectionProperty<ITrackingCollection>(navProp.Property).EntityCollection;
                        if (origItemsChangeTracker != null)
                        {
                            // Merge changes into trackable children
                            origItemsChangeTracker.MergeChanges(colProp.EntityCollection, visitationHelper);
                        }
                    }
                }

                // Set properties on orig item
                origItem.SetEntityProperties(updatedItem, originalChangeTracker);

                // Accept changes
                origItem.AcceptChanges();
            }

            // Remove cached deletes
            originalChangeTracker.RemoveCachedDeletes();
        }

        /// <summary>
        /// See if there are changes in an object graph.
        /// </summary>
        /// <param name="item">Trackable object</param>
        /// <returns>True if there are changes in the object graph</returns>
        public static bool HasChanges(this ITrackable item)
        {
            var visitationHelper = new ObjectVisitationHelper();
            bool hasChanges = item.HasChanges(visitationHelper,
                new Dictionary<ITrackable, bool>(ObjectReferenceEqualityComparer<ITrackable>.Default));
            return hasChanges;
        }

        private static bool HasChanges(this ITrackable item,
            ObjectVisitationHelper visitationHelper,
            Dictionary<ITrackable, bool> cachedResults)
        {
            // Prevent endless recursion
            if (!visitationHelper.TryVisit(item))
            {
                bool result;
                if (cachedResults.TryGetValue(item, out result)) return result;

                // if the circle closes and we reach again the item for which
                // we are currently determining "HasChanges" and so far we encounter
                // no changed items along the way, then just assume "no change".
                // However after inspection of other branches this result may still be corrected.
                return false;
            }

            // See if item has changes
            bool itemHasChanges = item.TrackingState != TrackingState.Unchanged;
            if (itemHasChanges) return true;

            // Recursively see if trackable properties have changes
            foreach (var navProp in item.GetNavigationProperties())
            {
                // Process 1-1 and M-1 properties
                foreach (var refProp in navProp.AsReferenceProperty())
                {
                    // See if ref prop has changes
                    bool refPropHasChanges = refProp.EntityReference.HasChanges(visitationHelper, cachedResults);
                    cachedResults[refProp.EntityReference] = refPropHasChanges;
                    if (refPropHasChanges) return true;
                }

                // Process 1-M and M-M properties
                foreach (var colProp in navProp.AsCollectionProperty<ITrackingCollection>())
                {
                    // See if there are any cached deletes
                    var cachedDeletes = colProp.EntityCollection.GetChanges(true);
                    if (cachedDeletes.Count > 0) return true;

                    // See if child entities have changes
                    foreach (ITrackable trackableChild in colProp.EntityCollection)
                    {
                        // Continue recursion
                        // REVISIT: shall we make a "shallow" scan in case of M-M?
                        if (trackableChild != null)
                        {
                            bool childHasChanges = trackableChild.HasChanges(visitationHelper, cachedResults);
                            cachedResults[trackableChild] = childHasChanges;
                            if (childHasChanges) return true;
                        }
                    }
                }
            }

            // Return false if there are no changes
            return false;
        }

        /// <summary>
        /// <para>Create a clone of the ChangeTrackingCollection using a JSON binary serializer.</para>
        /// <para>Changes can be rolled back by reverting to the cloned ChangeTrackingCollection.</para>
        /// </summary>
        /// <param name="changeTracker">Change tracker used to track changes on entities</param>
        /// <typeparam name="TEntity">Trackable entity type</typeparam>
        /// <returns>Deep copy of the ChangeTrackingCollection</returns>
        public static ChangeTrackingCollection<TEntity> Clone<TEntity>(this ChangeTrackingCollection<TEntity> changeTracker)
                where TEntity : class, ITrackable, INotifyPropertyChanged
        {
            IEnumerable<TEntity> clonedEntities = changeTracker.CloneEntities();
            return new ChangeTrackingCollection<TEntity>(clonedEntities, true);
        }

        private static IEnumerable<T> CloneEntities<T>(this IEnumerable<T> items)
            where T : class, ITrackable
        {
            return items.Clone();
        }

        private static ITrackable GetEquatableItem
            (this IEnumerable<ITrackable> sourceItems, ITrackable sourceItem, bool isTrackableRef)
        {
            // Get first matching item
            if (isTrackableRef) return sourceItems.FirstOrDefault();
            return sourceItems.Cast<IIdentifiable>()
                .FirstOrDefault(t => t.Equals((IIdentifiable)sourceItem)) as ITrackable;
        }

        private static void SetEntityProperties(this ITrackable targetItem, ITrackable sourceItem,
            ITrackingCollection changeTracker)
        {
            // List of 'prop.SetValue' actions
            var actions = new List<Action>();

            // Iterate simple properties
#if SILVERLIGHT || NET40
            foreach (var prop in targetItem.GetType().GetProperties().Where(p => p.CanWrite)
#else
            foreach (var prop in targetItem.GetType().GetTypeInfo().DeclaredProperties
                .Where(p => p.CanWrite && !p.GetMethod.IsPrivate)
#endif
                .Except(targetItem.GetNavigationProperties(false).Select(np => np.Property)))
            {
                // Skip tracking properties
                if (prop.Name == Constants.TrackingProperties.TrackingState
                    || prop.Name == Constants.TrackingProperties.ModifiedProperties)
                    continue;

                // Get source item prop value
                object sourceValue = prop.GetValue(sourceItem, null);
                object targetValue = prop.GetValue(targetItem, null);

                // Continue if source is null or source and target equal
                if (sourceValue == null || sourceValue.Equals(targetValue))
                    continue;

                // Deferred 'SetValue'
                actions.Add(() => prop.SetValue(targetItem, sourceValue, null));
            }

            // Iterate entity reference properties (skip collections)
            foreach (var refProp in targetItem
                .GetNavigationProperties(false)
                .OfReferenceType()
                .Where(np => np.Property.CanWrite))
            {
                ITrackable targetValue = refProp.EntityReference;

                // Skip non-null trackable
                if (targetValue != null) continue;

                ITrackable sourceValue = sourceItem.GetEntityReferenceProperty(refProp.Property).EntityReference;

                // Continue if source is null
                if (sourceValue == null) continue;

                // Deferred 'SetValue'
                actions.Add(() => refProp.Property.SetValue(targetItem, sourceValue, null));
            }

            // Nothing to do?
            if (!actions.Any()) return;

            // Turn off change-tracking
            bool isTracking = changeTracker.Tracking;
            changeTracker.Tracking = false;

            // Set target item prop value
            foreach (var action in actions)
            {
                action();
            }

            // Reset change-tracking
            changeTracker.Tracking = isTracking;
        }
    }
}
