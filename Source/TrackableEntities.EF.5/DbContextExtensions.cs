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
                && (context.IsRelatedProperty(parent.GetType().Name, 
                propertyName, RelationshipType.ManyToMany)))
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
                context.ApplyPropertyChanges(item, parent);
                return;
            }   

            // Exit if parent is added or deleted,
            // and it's not a M-1 relation
            if (parent != null
                && (parent.TrackingState == TrackingState.Added
                    || parent.TrackingState == TrackingState.Deleted)
                && !context.IsRelatedProperty(parent.GetType().Name,
                    propertyName, RelationshipType.ManyToOne))
                return;

            // If it is a M-1 relation and item state is deleted,
            // set to unchanged and exit
            if (parent != null
                && (context.IsRelatedProperty(parent.GetType().Name,
                    propertyName, RelationshipType.ManyToOne)
                    && item.TrackingState == TrackingState.Deleted))
            {
                context.Entry(item).State = EntityState.Unchanged;
                return;
            }

            // Set state to Added on parent only
            if (item.TrackingState == TrackingState.Added)
            {
                context.Entry(item).State = EntityState.Added;
                context.ApplyPropertyChanges(item, parent);
                return;
            }

            // Set state to Deleted on children and parent
            if (item.TrackingState == TrackingState.Deleted)
            {
                context.SetChanges(item, EntityState.Unchanged, parent);
                context.SetChanges(item, EntityState.Deleted, parent);
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
            context.ApplyPropertyChanges(item, parent);
        }

        private static void ApplyPropertyChanges(this DbContext context, 
            ITrackable item, ITrackable parent)
        {
            foreach (var prop in item.GetType().GetProperties())
            {
                // Apply changes to 1-1 and M-1 properties
                var trackableReference = prop.GetValue(item, null) as ITrackable;

                // Stop recursion if trackable is same type as parent
                if (trackableReference != null
                    && (parent == null || trackableReference.GetType() != parent.GetType()))
                    context.ApplyChanges(trackableReference, item, prop.Name);

                // Apply changes to 1-M and M-M properties
                var items = prop.GetValue(item, null) as IList;
                if (items != null)
                {
                    for (int i = items.Count - 1; i > -1; i--)
                    {
                        // Stop recursion if trackable is same type as parent
                        var trackableChild = items[i] as ITrackable;
                        if (trackableChild != null
                            && (parent == null || trackableChild.GetType() != parent.GetType()))
                            context.ApplyChanges(trackableChild, item, prop.Name);
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
                // Apply changes to 1-1 and M-1 properties
                var trackableReference = prop.GetValue(item, null) as ITrackable;

                // Stop recursion if trackable is same type as parent
                if (trackableReference != null
                    && (parent == null || trackableReference.GetType() != parent.GetType()))
                    context.ApplyChanges(trackableReference, item, prop.Name);

                // Apply changes to 1-M and M-M properties
                var items = prop.GetValue(item, null) as IList;
                if (items != null)
                {
                    for (int i = items.Count - 1; i > -1; i--)
                    {
                        // Stop recursion if trackable is same type as parent
                        var trackableChild = items[i] as ITrackable;
                        if (trackableChild != null
                            && (parent == null || trackableChild.GetType() != parent.GetType()))
                            context.SetChanges(trackableChild, state, item, prop.Name);
                    }
                }
            }

            // If M-M child, set relationship for deleted items
            if (state == EntityState.Deleted && parent != null && propertyName != null
                && (context.IsRelatedProperty(parent.GetType().Name, 
                propertyName, RelationshipType.ManyToMany)))
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

        private static bool IsRelatedProperty(this DbContext dbContext, 
            string entityTypeName, string propertyName, RelationshipType relationshipType)
        {
            // Use metadata workspace to check relationship type
            MetadataWorkspace workspace = ((IObjectContextAdapter)dbContext)
                .ObjectContext.MetadataWorkspace;

            var entityTypes = workspace.GetItems<EntityType>(DataSpace.CSpace);
            if (entityTypes == null) return false;
            var entityType = entityTypes.SingleOrDefault(e => e.Name == entityTypeName);
            if (entityType == null) return false;

            var navProp = entityType.NavigationProperties
                .SingleOrDefault(p => p.Name == propertyName);
            if (navProp == null) return false;

            switch (relationshipType)
            {
                case RelationshipType.ManyToOne:
                    return navProp.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many
                           && (navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne
                            || navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One);
                case RelationshipType.ManyToMany:
                    return navProp.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many
                           && navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many;
                default:
                    return false;
            }
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

        enum RelationshipType
        {
            ManyToOne,
            ManyToMany
        }
    }
}
