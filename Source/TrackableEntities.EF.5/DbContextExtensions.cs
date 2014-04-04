using System;
using System.Linq;
using System.Collections;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
#if EF_6
using System.Data.Entity.Core.Metadata.Edm;
#else
using System.Data.Metadata.Edm;
#endif

#if EF_6
namespace TrackableEntities.EF6
#else
namespace TrackableEntities.EF5
#endif
{
    /// <summary>
    /// Extension methods for DbContext to persist trackable entities.
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Recursively set entity state for DbContext entry.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static void ApplyChanges(this DbContext context, ITrackable item)
        {
            // Recursively set entity state for DbContext entry
            ApplyChanges(context, item, null, null);
        }

        private static void ApplyChanges(this DbContext context,
            ITrackable item, ITrackable parent, string propertyName)
        {
            // Check for null args
            if (context == null)
                throw new ArgumentNullException("context");
            if (item == null)
                throw new ArgumentNullException("item");

            // If M-M child, set relationship for added or deleted items
            if (parent != null && propertyName != null
                && (IsManyToManyProperty(context, parent.GetType().Name, propertyName)))
            {
                // If parent is added set tracking state to match
                var trackingState = item.TrackingState;
                if (parent.TrackingState == TrackingState.Added)
                    trackingState = parent.TrackingState;

                // If tracking state is added set entity to unchanged,
                // then add or delete relation.
                if (trackingState == TrackingState.Added
                    || trackingState == TrackingState.Deleted)
                {
                    context.Entry(item).State = item.TrackingState == TrackingState.Modified
                        ? EntityState.Modified
                        : EntityState.Unchanged;                    
                    context.SetRelationshipState(item, parent, propertyName,
                        trackingState.ToEntityState());
                }
                else
                {
                    // Set entity state for modified
                    if (item.TrackingState == TrackingState.Modified)
                        context.Entry(item).State = EntityState.Modified;
                }

                // Set state for child collections
                context.ApplyChangesToChildren(item, parent);
                return;
            }   

            // Exit if parent was added or deleted
            if (parent != null && (parent.TrackingState == TrackingState.Added
                || parent.TrackingState == TrackingState.Deleted))
            {
                return;
            }

            // Set state to Added on parent only
            if (item.TrackingState == TrackingState.Added)
            {
                context.Entry(item).State = EntityState.Added;
                context.ApplyChangesToChildren(item, parent);
                return;
            }

            // Set state to Deleted on children and parent
            if (item.TrackingState == TrackingState.Deleted)
            {
                context.SetChanges(item, EntityState.Unchanged);
                context.SetChanges(item, EntityState.Deleted);
                return;
            }

            // Set modified properties
            if (item.TrackingState == TrackingState.Modified
                && item.ModifiedProperties != null
                && item.ModifiedProperties.Count > 0)
            {
                // Mark modified properties
                context.Entry(item).State = EntityState.Unchanged;
                foreach (var property in item.ModifiedProperties)
                    context.Entry(item).Property(property).IsModified = true;
            }
            // Set entity state
            else
            {
                context.Entry(item).State = item.TrackingState.ToEntityState();
            }

            // Set state for child collections
            context.ApplyChangesToChildren(item, parent);
        }

        private static void ApplyChangesToChildren(this DbContext context, 
            ITrackable item, ITrackable parent)
        {
            foreach (var prop in item.GetType().GetProperties())
            {
                var items = prop.GetValue(item, null) as IList;
                if (items != null)
                {
                    for (int i = items.Count - 1; i > -1; i--)
                    {
                        // Stop recursion if trackable is same type as parent
                        var trackable = items[i] as ITrackable;
                        if (trackable != null
                            && (parent == null || trackable.GetType() != parent.GetType()))
                            context.ApplyChanges(trackable, item, prop.Name);
                    }
                }
            }
        }

        private static void SetChanges(this DbContext context,
            ITrackable item, EntityState state,
            ITrackable parent = null, string propertyName = null)
        {
            // Set state for child collections
            foreach (var prop in item.GetType().GetProperties())
            {
                var items = prop.GetValue(item, null) as IList;
                if (items != null)
                {
                    for (int i = items.Count - 1; i > -1; i--)
                    {
                        // Stop recursion if trackable is same type as parent
                        var trackable = items[i] as ITrackable;
                        if (trackable != null
                            && (parent == null || trackable.GetType() != parent.GetType()))
                            context.SetChanges(trackable, state, item, prop.Name);
                    }
                }
            }

            // If M-M child, set relationship for deleted items
            if (state == EntityState.Deleted && parent != null && propertyName != null
                && (IsManyToManyProperty(context, parent.GetType().Name, propertyName)))
            {
                context.Entry(item).State = item.TrackingState == TrackingState.Modified
                    ? EntityState.Modified
                    : EntityState.Unchanged;
                context.SetRelationshipState(item, parent, propertyName, EntityState.Deleted);
                return;
            }

            // Set entity state
            context.Entry(item).State = state;
        }

        private static bool IsManyToManyProperty(this DbContext dbContext, 
            string entityTypeName, string propertyName)
        {
            // Use metadata workspace to see if an entity relationship is M-M
            MetadataWorkspace workspace = ((IObjectContextAdapter)dbContext)
                .ObjectContext.MetadataWorkspace;

            var entityType = workspace.GetItems<EntityType>(DataSpace.CSpace)
                .SingleOrDefault(e => e.Name == entityTypeName);
            if (entityType == null) return false;

            var navProp = entityType.NavigationProperties
                .SingleOrDefault(p => p.Name == propertyName);
            if (navProp == null) return false;

            bool isManyToMany = navProp.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many
                                && navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many;
            return isManyToMany;
        }

        private static void SetRelationshipState(this DbContext context,
            ITrackable item, ITrackable parent, string propertyName, EntityState entityState)
        {
            // Set relationship state for independent association
            var stateManager = ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager;
            stateManager.ChangeRelationshipState(parent, item, propertyName, entityState);
        }

        private static EntityState ToEntityState(this TrackingState state)
        {
            switch (state)
            {
                case TrackingState.Added:
                    return EntityState.Added;
                case TrackingState.Modified:
                    return EntityState.Modified;
                case TrackingState.Deleted:
                    return EntityState.Deleted;
                default:
                    return EntityState.Unchanged;
            }
        }
    }
}
