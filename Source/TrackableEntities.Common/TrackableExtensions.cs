using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

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
            if (!visitationHelper.TryVisit(item)) return;

            // Set tracking state for child collections
            foreach (var navProp in item.GetNavigationProperties())
            {
                // Apply changes to 1-1 and M-1 properties
                foreach (var refProp in navProp.AsReferenceProperty())
                    refProp.EntityReference.AcceptChanges(visitationHelper);

                // Apply changes to 1-M properties
                foreach (var colProp in navProp.AsCollectionProperty<IList>())
                {
                    var items = colProp.EntityCollection;
                    var count = items.Count;
                    for (int i = count - 1; i > -1; i--)
                    {
                        // Stop recursion if trackable hasn't been visited
                        var trackable = items[i] as ITrackable;
                        if (trackable != null)
                        {
                            if (trackable.TrackingState == TrackingState.Deleted)
                                // Remove items marked as deleted
                                items.RemoveAt(i);
                            else
                                // Recursively accept changes on trackable
                                trackable.AcceptChanges(visitationHelper);
                        }
                    }
                }
            }

            // Set tracking state and clear modified properties
            item.TrackingState = TrackingState.Unchanged;
            item.ModifiedProperties = null;
        }

        /// <summary>
        /// Get a list of all navigation properties (entity references and entity collections)
        /// of a given entity.
        /// </summary>
        /// <param name="entity">Entity object</param>
        /// <param name="skipNulls">Null properties are skipped</param>
        public static IEnumerable<EntityNavigationProperty>
            GetNavigationProperties(this ITrackable entity, bool skipNulls = true)
        {
            INavigationPropertyInspector inspector = entity as INavigationPropertyInspector;
            if (inspector == null)
                inspector = new DefaultNavigationPropertyInspector(entity);

            foreach (var navProp in inspector.GetNavigationProperties())
            {
                // 1-1 and M-1 properties
                foreach (var refProp in navProp.AsReferenceProperty())
                {
                    if (skipNulls && refProp.EntityReference == null) continue; // skip nulls
                    yield return refProp;
                }

                // 1-M and M-M properties
                foreach (var colProp in navProp.AsCollectionProperty())
                {
                    if (skipNulls && colProp.EntityCollection == null) continue; // skip nulls
                    yield return colProp;
                }
            }
        }

        /// <summary>
        /// Get an entity collection property (1-M or M-M) for the given entity.
        /// </summary>
        /// <typeparam name="TEntityCollection">Type of entity collection</typeparam>
        /// <param name="entity">Entity object</param>
        /// <param name="property">Property information</param>
        public static EntityCollectionProperty<TEntityCollection>
            GetEntityCollectionProperty<TEntityCollection>(this ITrackable entity,
                PropertyInfo property)
            where TEntityCollection : class
        {
            return entity.GetNavigationProperties(false)
                .Where(np => np.Property == property)
                .OfCollectionType<TEntityCollection>()
                .Single();
        }

        /// <summary>
        /// Get an entity collection property (1-M or M-M) for the given entity.
        /// </summary>
        /// <param name="entity">Entity object</param>
        /// <param name="property">Property information</param>
        public static EntityCollectionProperty<IEnumerable<ITrackable>>
            GetEntityCollectionProperty(this ITrackable entity, PropertyInfo property)
        {
            return entity.GetEntityCollectionProperty<IEnumerable<ITrackable>>(property);
        }

        /// <summary>
        /// Get an entity reference property (1-1 or M-1) for the given entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">Entity object</param>
        /// <param name="property">Property information</param>
        public static EntityReferenceProperty<TEntity>
            GetEntityReferenceProperty<TEntity>(this ITrackable entity, PropertyInfo property)
            where TEntity : class, ITrackable
        {
            return entity.GetNavigationProperties(false)
                .Where(np => np.Property == property)
                .OfReferenceType<TEntity>()
                .Single();
        }

        /// <summary>
        /// Get an entity reference property (1-1 or M-1) for the given entity.
        /// </summary>
        /// <param name="entity">Entity object</param>
        /// <param name="property">Property information</param>
        public static EntityReferenceProperty<ITrackable>
            GetEntityReferenceProperty(this ITrackable entity, PropertyInfo property)
        {
            return entity.GetEntityReferenceProperty<ITrackable>(property);
        }

        /// <summary>
        /// Pick only properties of type entity reference.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity</typeparam>
        /// <param name="navigationProperties">All nagivation properties</param>
        public static IEnumerable<EntityReferenceProperty<TEntity>>
            OfReferenceType<TEntity>(this IEnumerable<EntityNavigationProperty> navigationProperties)
            where TEntity : class, ITrackable
        {
            return navigationProperties.SelectMany(np => np.AsReferenceProperty<TEntity>());
        }

        /// <summary>
        /// Pick only properties of type entity reference.
        /// </summary>
        /// <param name="navigationProperties">All nagivation properties</param>
        public static IEnumerable<EntityReferenceProperty>
            OfReferenceType(this IEnumerable<EntityNavigationProperty> navigationProperties)
        {
            return navigationProperties.OfReferenceType<ITrackable>();
        }

        /// <summary>
        /// Pick only properties of type entity collection.
        /// </summary>
        /// <typeparam name="TEntityCollection">Type of entity collection</typeparam>
        /// <param name="navigationProperties">All nagivation properties</param>
        public static IEnumerable<EntityCollectionProperty<TEntityCollection>>
            OfCollectionType<TEntityCollection>(this IEnumerable<EntityNavigationProperty> navigationProperties)
            where TEntityCollection : class
        {
            return navigationProperties.SelectMany(np => np.AsCollectionProperty<TEntityCollection>());
        }

        /// <summary>
        /// Pick only properties of type entity collection.
        /// </summary>
        /// <param name="navigationProperties">All nagivation properties</param>
        public static IEnumerable<EntityCollectionProperty>
            OfCollectionType(this IEnumerable<EntityNavigationProperty> navigationProperties)
        {
            return navigationProperties.OfCollectionType<IEnumerable<ITrackable>>();
        }

        /// <summary>
        /// Default implementation of INavigationPropertyInspector used if an entity doesn't provide
        /// its own implementation.
        /// DefaultNavigationPropertyInspector simply loops over all entity properties
        /// and yields those, whose values are either ITrackable or IEnumerable&lt;ITrackable&gt;.
        /// </summary>
        private sealed class DefaultNavigationPropertyInspector : INavigationPropertyInspector
        {
            private readonly ITrackable Entity;

            public DefaultNavigationPropertyInspector(ITrackable entity)
            {
                this.Entity = entity;
            }

            public IEnumerable<EntityNavigationProperty> GetNavigationProperties()
            {
                foreach (var prop in Entity.GetType().GetProperties())
                {
                    // 1-1 and M-1 properties
                    if (typeof(ITrackable).IsAssignableFrom(prop.PropertyType))
                    {
                        var trackableRef = prop.GetValue(Entity, null) as ITrackable;
                        yield return new EntityReferenceProperty(prop, trackableRef);
                    }

                    // 1-M and M-M properties
                    if (typeof(IEnumerable<ITrackable>).IsAssignableFrom(prop.PropertyType))
                    {
                        var items = prop.GetValue(Entity, null) as IEnumerable<ITrackable>;
                        yield return new EntityCollectionProperty(prop, items);
                    }
                }
            }
        }
    }
}
