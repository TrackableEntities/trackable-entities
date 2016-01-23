using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using System.Threading;
using TrackableEntities.Common;
#if EF_6
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Metadata.Edm;
#else
using System.Data;
using System.Data.Objects;
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
        /// Update entity state on DbContext for an object graph.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static void ApplyChanges(this DbContext context, ITrackable item)
        {
            // Recursively set entity state for DbContext entry
            ApplyChanges(context, item, null, new ObjectVisitationHelper(), null, null, null);
        }

        /// <summary>
        /// Update entity state on DbContext for more than one object graph.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static void ApplyChanges(this DbContext context, IEnumerable<ITrackable> items)
        {
            // Apply changes to collection of items
            foreach (var item in items)
                ApplyChanges(context, item, null, new ObjectVisitationHelper(), null, null, null);
        }

        /// <summary>
        /// Update entity state on DbContext for an object graph with interception.
        /// </summary>
        /// <param name="pool">Pool of interceptors</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static void ApplyChanges(this InterceptorPool pool, ITrackable item)
        {
            // Recursively set entity state for DbContext entry
            ApplyChanges(pool.DbContext, item, null, new ObjectVisitationHelper(), null, null, pool.Interceptors);
        }

        /// <summary>
        /// Update entity state on DbContext for more than one object graph.
        /// </summary>
        /// <param name="pool">Pool of interceptors</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static void ApplyChanges(this InterceptorPool pool, IEnumerable<ITrackable> items)
        {
            // Apply changes to collection of items
            foreach (var item in items)
                ApplyChanges(pool.DbContext, item, null, new ObjectVisitationHelper(), null, null, pool.Interceptors);
        }

        private static void ApplyChanges(this DbContext context,
            ITrackable item, ITrackable parent, ObjectVisitationHelper visitationHelper,
            string propertyName, TrackingState? state,
            IList<IStateInterceptor> interceptors)
        {
            // Prevent endless recursion
            if (!visitationHelper.TryVisit(item)) return;

            // Check for null args
            if (context == null)
                throw new ArgumentNullException("context");
            if (item == null)
                throw new ArgumentNullException("item");

            // If M-M child, set relationship for added or deleted items
            if (parent != null && propertyName != null
                && (context.IsRelatedProperty(parent.GetType(),
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
                    var entityState = item.TrackingState == TrackingState.Modified
                        ? EntityState.Modified
                        : EntityState.Unchanged;
                    TrySetEntityState(context, item, parent, propertyName, entityState, interceptors);
                    context.SetRelationshipState(item, parent, propertyName,
                        trackingState.ToEntityState());
                }
                else
                {
                    // Set entity state for modified or unchanged
                    if (item.TrackingState == TrackingState.Modified)
                        TrySetEntityState(context, item, parent, propertyName, EntityState.Modified, interceptors);
                    else if (item.TrackingState == TrackingState.Unchanged)
                        TrySetEntityState(context, item, parent, propertyName, EntityState.Unchanged, interceptors);
                }

                // Set state for child collections
                context.ApplyChangesOnProperties(item, visitationHelper, null, interceptors);
                return;
            }

            // Exit if item is not deleted, parent is deleted, and not a M-1 relation
            if (parent != null
                && item.TrackingState != TrackingState.Deleted
                && parent.TrackingState == TrackingState.Deleted
                && !context.IsRelatedProperty(parent.GetType(),
                propertyName, RelationshipType.ManyToOne))
            {
                SetStateByInterceptors(context, item, parent, propertyName, interceptors);
                return;
            }

            // If it is a M-1 relation and item state is deleted,
            // set to unchanged and exit
            if (parent != null
                && (context.IsRelatedProperty(parent.GetType(),
                    propertyName, RelationshipType.ManyToOne)
                    && item.TrackingState == TrackingState.Deleted))
            {
                try
                {
                    TrySetEntityState(context, item, parent, propertyName, EntityState.Unchanged, interceptors);
                }
                catch (InvalidOperationException invalidOpEx)
                {
                    throw new InvalidOperationException(Constants.ExceptionMessages.DeletedWithAddedChildren, invalidOpEx);
                }
                return;
            }

            // Set state to Added if item marked as Added
            if (item.TrackingState == TrackingState.Added
                && (state == null || state == TrackingState.Added))
            {
                TrySetEntityState(context, item, parent, propertyName, EntityState.Added, interceptors);
                context.ApplyChangesOnProperties(item, visitationHelper, null, interceptors);
                return;
            }

            // Or if parent has been set to Added and 1-M or 1-1 relation
            if (parent != null
                && context.Entry(parent).State == EntityState.Added
                && (context.IsRelatedProperty(parent.GetType(), propertyName, RelationshipType.OneToMany)
                    || context.IsRelatedProperty(parent.GetType(), propertyName, RelationshipType.OneToOne)))
            {
                context.ApplyChangesOnProperties(item, visitationHelper, null, interceptors);
                return;
            }

            // Set state to Deleted on children and parent
            if (item.TrackingState == TrackingState.Deleted
                && (state == null || state == TrackingState.Deleted))
            {
                context.SetChanges(item, EntityState.Unchanged, visitationHelper.Clone(), null, null, interceptors); // Clone to avoid interference
                context.SetChanges(item, EntityState.Deleted, visitationHelper, null, null, interceptors);
                return;
            }

            // Set entity state
            if (state == null
                || state == TrackingState.Unchanged
                || state == TrackingState.Modified
                || (state == TrackingState.Added
                    && item.TrackingState != TrackingState.Deleted))
            {
                // Set added state for reference or child properties
                context.ApplyChangesOnProperties(item, visitationHelper.Clone(), TrackingState.Added, interceptors); // Clone to avoid interference

                // Delete children prior to parent
                if (item.TrackingState == TrackingState.Deleted)
                {
                    context.ApplyChangesOnProperties(item, visitationHelper.Clone(), TrackingState.Deleted, interceptors);
                }

                // Set modified properties
                if (item.TrackingState == TrackingState.Modified
                    && (state == null || state == TrackingState.Modified)
                    && item.ModifiedProperties != null
                    && item.ModifiedProperties.Count > 0)
                {
                    // Mark modified properties
                    TrySetEntityState(context, item, parent, propertyName, EntityState.Unchanged, interceptors);
                    foreach (var property in item.ModifiedProperties)
                        context.Entry(item).Property(property).IsModified = true;
                }
                else
                {
                    // Set entity state
                    TrySetEntityState(context, item, parent, propertyName, item.TrackingState.ToEntityState(), interceptors);
                }

                // Set other state for reference or child properties
                context.ApplyChangesOnProperties(item, visitationHelper.Clone(), TrackingState.Unchanged, interceptors); // Clone to avoid interference
                if (item.TrackingState == TrackingState.Deleted)
                {
                    context.ApplyChangesOnProperties(item, visitationHelper, TrackingState.Modified, interceptors); // Clone to avoid interference
                }
                else
                {
                    context.ApplyChangesOnProperties(item, visitationHelper.Clone(), TrackingState.Modified, interceptors); // Clone to avoid interference
                    context.ApplyChangesOnProperties(item, visitationHelper, TrackingState.Deleted, interceptors); 
                }
            }
        }

        /// <summary>
        /// For the given entity type return the EntitySet name qualified by container name.
        /// </summary>
        /// <param name="dbContext">Used to query and save changes to a database</param>
        /// <param name="entityType">Type of an entity</param>
        /// <returns></returns>
        public static string GetEntitySetName(this DbContext dbContext, Type entityType)
        {
            MetadataWorkspace workspace = ((IObjectContextAdapter)dbContext)
                .ObjectContext.MetadataWorkspace;
            var containers = workspace.GetItems<EntityContainer>(DataSpace.CSpace);
            var entitySetName =
                (from c in containers
                 from es in c.BaseEntitySets
                 where GetEntityTypes(dbContext, entityType).Contains(es.ElementType)
                 select es.EntityContainer.Name + "." + es.Name).SingleOrDefault();
            return entitySetName;
        }

        /// <summary>
        /// Load related entities for an object graph.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implement ITrackable</param>
        /// <param name="loadAll">True to load all related entities, false to load only added entities</param>
        public static void LoadRelatedEntities(this DbContext context,
            ITrackable item, bool loadAll = false)
        {
            LoadRelatedEntities(context, new[] {item}, null, CreateVisitationHelperWithIdMatching(context), loadAll);
        }

        /// <summary>
        /// Load related entities for more than one object graph.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implements ITrackable</param>
        /// <param name="loadAll">True to load all related entities, false to load only added entities</param>
        public static void LoadRelatedEntities(this DbContext context, IEnumerable<ITrackable> items,
            bool loadAll = false)
        {
            LoadRelatedEntities(context, items, null, CreateVisitationHelperWithIdMatching(context), loadAll);
        }

#if EF_6
        /// <summary>
        /// Load related entities for an object graph asynchronously.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implement ITrackable</param>
        /// <param name="loadAll">True to load all related entities, false to load only added entities</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static Task LoadRelatedEntitiesAsync(this DbContext context,
            ITrackable item, bool loadAll = false)
        {
            return LoadRelatedEntitiesAsync(context, new[] {item}, null, CreateVisitationHelperWithIdMatching(context),
                CancellationToken.None, loadAll);
        }

        /// <summary>
        /// Load related entities for an object graph asynchronously.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implement ITrackable</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="loadAll">True to load all related entities, false to load only added entities</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task LoadRelatedEntitiesAsync(this DbContext context,
            ITrackable item, CancellationToken cancellationToken, bool loadAll = false)
        {
            return LoadRelatedEntitiesAsync(context, new[] { item }, null, CreateVisitationHelperWithIdMatching(context),
                cancellationToken, loadAll);
        }

        /// <summary>
        /// Load related entities for more than one object graph asynchronously.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implements ITrackable</param>
        /// <param name="loadAll">True to load all related entities, false to load only added entities</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task LoadRelatedEntitiesAsync(this DbContext context,
            IEnumerable<ITrackable> items, bool loadAll = false)
        {
            return LoadRelatedEntitiesAsync(context, items, null, CreateVisitationHelperWithIdMatching(context),
                CancellationToken.None, loadAll);
        }

        /// <summary>
        /// Load related entities for more than one object graph asynchronously.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implements ITrackable</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="loadAll">True to load all related entities, false to load only added entities</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task LoadRelatedEntitiesAsync(this DbContext context, IEnumerable<ITrackable> items,
            CancellationToken cancellationToken, bool loadAll = false)
        {
            return LoadRelatedEntitiesAsync(context, items, null, CreateVisitationHelperWithIdMatching(context),
                cancellationToken, loadAll);
        }
#endif

        private static void LoadRelatedEntities(this DbContext context,
            IEnumerable<ITrackable> items, ITrackable parent, ObjectVisitationHelper visitationHelper, bool loadAll)
        {
            // Return if no items
            if (items == null) return;

            // Get selected items
            var selectedItems = loadAll ? items
                : items.Where(t => t.TrackingState == TrackingState.Added
                    || (parent != null && parent.TrackingState == TrackingState.Added));
            var entities = selectedItems.Cast<object>().Where(i => !visitationHelper.IsVisited(i));

            // Collection 'items' can contain entities of different types (due to inheritance)
            // We collect a superset of all properties of all items of type ITrackable
            var allProps = (from entity in entities
                            from prop in entity.GetType().GetProperties()
                            where typeof(ITrackable).IsAssignableFrom(prop.PropertyType)
                            select prop).Distinct();

            // Populate related entities on all items
            foreach (var prop in allProps)
            {
                // Get related entities
                string propertyName = prop.Name;
                Type propertyType = prop.PropertyType;
                IEnumerable<object> relatedEntities = context.GetRelatedEntities(entities,
                    prop.DeclaringType, propertyName, propertyType);

                // Continue if there are no related entities
                if (!relatedEntities.Any()) continue;

                // ObjectVisitationHelper serves here as an identity cache
                relatedEntities = relatedEntities.Select(e => visitationHelper.FindVisited(e) ?? e);

                // Set related entities
                context.SetRelatedEntities(entities, relatedEntities, prop,
                    prop.DeclaringType, propertyName, propertyType);
            }

            // Recursively populate related entities on ref and child properties
            foreach (var item in items)
            {
                // Avoid endless recursion
                if (!visitationHelper.TryVisit(item)) continue;

                bool loadAllRelated = loadAll 
                    || item.TrackingState == TrackingState.Added
                    || (parent  != null && parent.TrackingState == TrackingState.Added);
                context.LoadRelatedEntitiesOnProperties(item, visitationHelper, loadAllRelated);
            }
        }

#if EF_6
        private static async Task LoadRelatedEntitiesAsync(this DbContext context,
            IEnumerable<ITrackable> items, ITrackable parent, ObjectVisitationHelper visitationHelper,
            CancellationToken cancellationToken, bool loadAll)
        {
            // Return if no items
            if (items == null) return;

            // Get selected items
            var selectedItems = loadAll ? items
                : items.Where(t => t.TrackingState == TrackingState.Added
                    || (parent != null && parent.TrackingState == TrackingState.Added));
            var entities = selectedItems.Cast<object>().Where(i => !visitationHelper.IsVisited(i));

            // Collection 'items' can contain entities of different types (due to inheritance)
            // We collect a superset of all properties of all items of type ITrackable
            var allProps = (from entity in entities
                            from prop in entity.GetType().GetProperties()
                            where typeof(ITrackable).IsAssignableFrom(prop.PropertyType)
                            select prop).Distinct();

            // Populate related entities on all items
            foreach (var prop in allProps)
            {
                // Get related entities
                string propertyName = prop.Name;
                Type propertyType = prop.PropertyType;
                IEnumerable<object> relatedEntities = await context.GetRelatedEntitiesAsync(entities,
                    prop.DeclaringType, propertyName, propertyType, cancellationToken);

                // Continue if there are no related entities
                if (!relatedEntities.Any()) continue;

                // ObjectVisitationHelper serves here as an identity cache
                relatedEntities = relatedEntities.Select(e => visitationHelper.FindVisited(e) ?? e);

                // Set related entities
                context.SetRelatedEntities(entities, relatedEntities, prop,
                    prop.DeclaringType, propertyName, propertyType);
            }

            // Recursively populate related entities on ref and child properties
            foreach (var item in items)
            {
                // Avoid endless recursion
                if (!visitationHelper.TryVisit(item)) continue;

                bool loadAllRelated = loadAll
                    || item.TrackingState == TrackingState.Added
                    || (parent != null && parent.TrackingState == TrackingState.Added);
                await context.LoadRelatedEntitiesOnPropertiesAsync(item, visitationHelper,
                    cancellationToken, loadAllRelated);
            }
        }

        private async static Task LoadRelatedEntitiesOnPropertiesAsync(this DbContext context,
            ITrackable item, ObjectVisitationHelper visitationHelper, CancellationToken cancellationToken, bool loadAll)
        {
            // Recursively load related entities
            foreach (var navProp in item.GetNavigationProperties())
            {
                // Apply changes to 1-1 and M-1 properties
                foreach (var refProp in navProp.AsReferenceProperty())
                {
                    await context.LoadRelatedEntitiesAsync(new[] { refProp.EntityReference }, item, visitationHelper,
                        cancellationToken, loadAll);
                }

                // Apply changes to 1-M and M-M properties
                foreach (var colProp in navProp.AsCollectionProperty())
                {
                    await context.LoadRelatedEntitiesAsync(colProp.EntityCollection, item, visitationHelper,
                        cancellationToken, loadAll);
                }
            }
        }
#endif

        private static void LoadRelatedEntitiesOnProperties(this DbContext context,
            ITrackable item, ObjectVisitationHelper visitationHelper, bool loadAll)
        {
            // Recursively load related entities
            foreach (var navProp in item.GetNavigationProperties())
            {
                // Apply changes to 1-1 and M-1 properties
                foreach (var refProp in navProp.AsReferenceProperty())
                {
                    context.LoadRelatedEntities(new[] { refProp.EntityReference }, item, visitationHelper, loadAll);
                }

                // Apply changes to 1-M and M-M properties
                foreach (var colProp in navProp.AsCollectionProperty())
                {
                    context.LoadRelatedEntities(colProp.EntityCollection, item, visitationHelper, loadAll);
                }
            }
        }

        private static EntityType GetEdmSpaceType(this DbContext dbContext, Type entityType)
        {
            MetadataWorkspace workspace = ((IObjectContextAdapter)dbContext)
                .ObjectContext.MetadataWorkspace;

            StructuralType oType = workspace.GetItems<StructuralType>(DataSpace.OSpace)
                .Where(e => e.FullName == entityType.FullName).SingleOrDefault();

            if (oType == null) return null;

            return workspace.GetEdmSpaceType(oType) as EntityType;
        }

        #region ApplyChanges Helpers

        private static void ApplyChangesOnProperties(this DbContext context,
            ITrackable item, ObjectVisitationHelper visitationHelper,
            TrackingState? state, IList<IStateInterceptor> interceptors)
        {
            // Recursively apply changes
            foreach (var navProp in item.GetNavigationProperties())
            {
                // Apply changes to 1-1 and M-1 properties
                foreach (var refProp in navProp.AsReferenceProperty())
                    context.ApplyChanges(refProp.EntityReference, item, visitationHelper, navProp.Property.Name, state, interceptors);

                // Apply changes to 1-M and M-M properties
                foreach (var colProp in navProp.AsCollectionProperty<IList>())
                {
                    // Apply changes on collection property for each state.
                    // Process added items first, then process others.
                    ApplyChangesOnCollectionProperties(TrackingState.Added, true,
                        navProp, colProp, context, item, visitationHelper, state, interceptors);
                    ApplyChangesOnCollectionProperties(TrackingState.Added, false,
                        navProp, colProp, context, item, visitationHelper, state, interceptors);
                }
            }
        }

        private static void ApplyChangesOnCollectionProperties(TrackingState stateFilter, bool includeState,
            EntityNavigationProperty navProp, EntityCollectionProperty<IList> colProp,
            DbContext context, ITrackable item, ObjectVisitationHelper visitationHelper,
            TrackingState? state, IList<IStateInterceptor> interceptors)
        {
            // Apply changes to 1-M and M-M properties filtering by tracking state
            var count = colProp.EntityCollection.Count;
            for (int i = count - 1; i > -1; i--)
            {
                var trackableChild = colProp.EntityCollection[i] as ITrackable;
                if (trackableChild != null)
                {
                    bool condition = includeState
                        ? trackableChild.TrackingState == stateFilter
                        : trackableChild.TrackingState != stateFilter;
                    if (condition)
                        context.ApplyChanges(trackableChild, item, visitationHelper, navProp.Property.Name, state, interceptors);                    
                } 
            }
        }

        private static void SetChanges(this DbContext context,
            ITrackable item, EntityState state,
            ObjectVisitationHelper visitationHelper,
            ITrackable parent, string propertyName,
            IList<IStateInterceptor> interceptors)
        {
            // Set state for child collections
            foreach (var navProp in item.GetNavigationProperties())
            {
                string propName = navProp.Property.Name;

                // Apply changes to 1-1 and M-1 properties
                foreach (var refProp in navProp.AsReferenceProperty())
                {
                    ITrackable trackableReference = refProp.EntityReference;
                    if (visitationHelper.IsVisited(trackableReference)) continue;

                    context.ApplyChanges(trackableReference, item, visitationHelper, propName, null, interceptors);
                    if (context.IsRelatedProperty(item.GetType(), propName, RelationshipType.OneToOne))
                        context.SetChanges(trackableReference, state, visitationHelper, item, propName, interceptors);
                }

                // Apply changes to 1-M and M-M properties
                foreach (var colProp in navProp.AsCollectionProperty())
                {
                    foreach (ITrackable trackableChild in colProp.EntityCollection.Reverse())
                    {
                        // Prevent endless recursion
                        if (visitationHelper.TryVisit(trackableChild))
                        {
                            // TRICKY: we have just visited the item
                            // As a side effect, ApplyChanges will never be called for it.
                            context.SetChanges(trackableChild, state, visitationHelper, item, propName, interceptors);
                        }
                    }
                }
            }

            // If M-M child, set relationship for deleted items
            if (state == EntityState.Deleted && parent != null && propertyName != null
                && (context.IsRelatedProperty(parent.GetType(),
                propertyName, RelationshipType.ManyToMany)))
            {
                var entityState = item.TrackingState == TrackingState.Modified
                    ? EntityState.Modified
                    : EntityState.Unchanged;
                TrySetEntityState(context, item, parent, propertyName, entityState, interceptors);
                context.SetRelationshipState(item, parent, propertyName, EntityState.Deleted);
                return;
            }

            // Set entity state
            TrySetEntityState(context, item, parent, propertyName, state, interceptors);
        }

        // TODO: refactor to use GetRelationshipType() method
        private static bool IsRelatedProperty(this DbContext dbContext,
            Type entityType, string propertyName, RelationshipType relationshipType)
        {
            // Get navigation property
            var edmEntityType = dbContext.GetEdmSpaceType(entityType);
            if (edmEntityType == null) return false;
            var navProp = edmEntityType.NavigationProperties
                .SingleOrDefault(p => p.Name == propertyName);
            if (navProp == null) return false;

            switch (relationshipType)
            {
                case RelationshipType.OneToOne:
                    return navProp.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One
                           && (navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne
                            || navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One);
                case RelationshipType.ManyToOne:
                    return navProp.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many
                           && (navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne
                            || navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One);
                case RelationshipType.ManyToMany:
                    return navProp.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many
                           && navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many;
                case RelationshipType.OneToMany:
                    return (navProp.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One
                            || navProp.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne)
                           && navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many;
                default:
                    return false;
            }
        }

        private static void SetRelationshipState(this DbContext context,
            ITrackable item, ITrackable parent, string propertyName, EntityState entityState)
        {
            // PR #43: Before calling ChangeRelationshipState make sure parent entry is attached
            var parentEntry = context.Entry(parent);
            if (parentEntry.State == EntityState.Detached)
                parentEntry.State = EntityState.Unchanged;

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

        private static RelationshipType GetRelationshipType(this DbContext dbContext, Type entityType, string propertyName)
        {
            // Get navigation property
            var edmEntityType = dbContext.GetEdmSpaceType(entityType);
            if (edmEntityType == null)
                throw new ArgumentException("Getting entity type from metadata failed.", "entityType");
            var navProp = edmEntityType.NavigationProperties
                .SingleOrDefault(p => p.Name == propertyName);
            if (navProp == null)
                throw new ArgumentException("Getting navigation property failed.", "propertyName");

            if (navProp.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One
                && (navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne
                    || navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One))
                return RelationshipType.OneToOne;

            if (navProp.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many
                && (navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne
                    || navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One))
                return RelationshipType.ManyToOne;

            if (navProp.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many
                && navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
                return RelationshipType.ManyToMany;

            if ((navProp.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One
                 || navProp.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne)
                && navProp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
                return RelationshipType.OneToMany;

            throw new InvalidOperationException(String.Format("Cannot determine relationship type for {0} property on {1}.", propertyName, entityType.FullName));
        }

        private static bool SetStateByInterceptors(DbContext context,
            ITrackable item, ITrackable parent, string propertyName,
            IList<IStateInterceptor> interceptors)
        {
            // If there are no state interceptors, do not use them
            if (interceptors == null || interceptors.Count <= 0)
                return false;

            var interceptionStateUsed = false;
            RelationshipType? relationType = null;

            if (parent != null && propertyName != null)
                relationType = GetRelationshipType(context, parent.GetType(), propertyName);

            foreach (IStateInterceptor interceptor in interceptors)
            {
                // If current interceptor returns the state, use it
                var entityState = interceptor.GetEntityState(item, relationType);
                if (entityState != null)
                {
                    context.Entry(item).State = (EntityState)entityState;
                    interceptionStateUsed = true;
                }
            }

            return interceptionStateUsed;
        }

        private static void TrySetEntityState(DbContext context,
            ITrackable item, ITrackable parent, string propertyName,
            EntityState state, IList<IStateInterceptor> interceptors)
        {
            // Set state normally if we cannot perform interception
            if (interceptors == null || interceptors.Count == 0)
            {
                context.Entry(item).State = state;
                return;
            }

            // Try to use interceptors to set the state
            // If no interceptor has changed the state, set state normally
            if (!SetStateByInterceptors(context, item, parent, propertyName, interceptors))
                context.Entry(item).State = state;
        }

        #endregion

        #region LoadRelatedEntities Helpers

        private static List<object> GetRelatedEntities(this DbContext context,
            IEnumerable<object> items, Type entityType, string propertyName, Type propertyType)
        {
            // Get entity sql
            string entitySql = context.GetRelatedEntitiesSql(items, entityType, propertyName, propertyType);
            if (string.IsNullOrWhiteSpace(entitySql)) return new List<object>();

            // Get related entities
            List<object> entities = context.ExecuteQueryEntitySql(entitySql);
            return entities;
        }

#if EF_6
        private static async Task<List<object>> GetRelatedEntitiesAsync(this DbContext context,
            IEnumerable<object> items, Type entityType, string propertyName, Type propertyType,
            CancellationToken cancellationToken)
        {
            // Get entity sql
            string entitySql = context.GetRelatedEntitiesSql(items, entityType, propertyName, propertyType);
            if (string.IsNullOrWhiteSpace(entitySql)) return new List<object>();

            // Get related entities
            List<object> entities = await context.ExecuteQueryEntitySqlAsync(entitySql, cancellationToken);
            return entities;
        } 
#endif

        private static string GetRelatedEntitiesSql(this DbContext context,
            IEnumerable<object> items, Type entityType, string propertyName, Type propertyType)
        {
            // Get entity set name
            string entitySetName = context.GetEntitySetName(propertyType);
            if (string.IsNullOrEmpty(entitySetName)) return null;

            // Get foreign key name
            string[] foreignKeyNames = context.GetForeignKeyNames(entityType, propertyName);
            if (foreignKeyNames == null || foreignKeyNames.Length == 0) return null;

            // Get entity sql based on key values
            string entitySql;
            if (foreignKeyNames.Length == 1)
            {
                object[] foreignKeyValues = GetKeyValuesFromEntites(foreignKeyNames[0], items);
                if (foreignKeyValues.Length == 0) return null;
                entitySql = GetQueryEntitySql(entitySetName, foreignKeyNames[0], foreignKeyValues);
            }
            else
            {
                List<Dictionary<string, object>> foreignKeyValues = GetForeignKeyValues(foreignKeyNames, items);
                if (foreignKeyValues.Count == 0) return null;
                entitySql = GetQueryEntitySql(entitySetName, foreignKeyValues);
            }
            return entitySql;
        }

        private static IEnumerable<EntityType> GetEntityTypes(DbContext dbContext, Type entityType)
        {
            // First get concrete entity type
            yield return dbContext.GetEdmSpaceType(entityType);

            // Then get base entity types
            var baseType = entityType.BaseType;
            while (baseType != null && baseType != typeof (object))
            {
                yield return dbContext.GetEdmSpaceType(baseType);
                baseType = baseType.BaseType;
            }
        }

        private static void SetRelatedEntities(this DbContext context, 
            IEnumerable<object> entities, IEnumerable<object> relatedEntities, PropertyInfo referenceProperty,
            Type entityType, string propertyName, Type propertyType)
        {
            // Get names of entity foreign key and related entity primary key
            string[] foreignKeyNames = context.GetForeignKeyNames(entityType, propertyName);
            string[] primaryKeyNames = context.GetPrimaryKeyNames(propertyType);

            // Continue if we can't get foreign or primary key names
            if (foreignKeyNames == null || primaryKeyNames == null) return;

            foreach (var entity in entities)
            {
                // Get key values
                var foreignKeyValues = GetKeyValuesFromEntity(foreignKeyNames, entity);

                // Get related entity
                var relatedEntity = (from e in relatedEntities
                    let relatedKeyValues = GetKeyValuesFromEntity(foreignKeyNames, e)
                    where KeyValuesAreEqual(relatedKeyValues, foreignKeyValues)
                    select e).SingleOrDefault();

                // Set reference prop to related entity
                if (relatedEntity != null)
                    referenceProperty.SetValue(entity, relatedEntity);
            }
        }

        private static object[] GetKeyValuesFromEntites(string foreignKeyName, IEnumerable<object> items)
        {
            var values = from item in items
                         let prop = item.GetType().GetProperty(foreignKeyName)
                         select prop != null ? prop.GetValue(item) : null;
            return values.Where(v => v != null).Distinct().ToArray();
        }

        private static Dictionary<string, object> GetKeyValuesFromEntity(string[] keyNames, object entity)
        {
            var keyValues = new Dictionary<string, object>();
            if (keyNames == null || keyNames.Length == 0)
                return keyValues;

            foreach (var keyName in keyNames)
            {
                // Get key value
                var keyProp = entity.GetType().GetProperty(keyName);
                if (keyProp == null) break;
                var keyValue = keyProp.GetValue(entity);
                if (keyValue == null) break;

                keyValues.Add(keyName, keyValue);
            }
            return keyValues;
        }

        private static bool KeyValuesAreEqual(Dictionary<string, object> primaryKeys,
            Dictionary<string, object> foreignKeys)
        {
            bool areEqual = false;

            foreach (KeyValuePair<string, object> primaryKey in primaryKeys)
            {
                object foreignKeyValue;
                if (!foreignKeys.TryGetValue(primaryKey.Key, out foreignKeyValue))
                {
                    areEqual = false;
                    break;
                }
                areEqual = KeyValuesAreEqual(primaryKey.Value, foreignKeyValue);
            }
            return areEqual;
        }

        private static bool KeyValuesAreEqual(object primaryKeyValue, object foreignKeyValue)
        {
            // Compare normalized strings
            if (primaryKeyValue is string && foreignKeyValue is string)
            {
                return ((string)primaryKeyValue).Normalize() 
                    == ((string)foreignKeyValue).Normalize();
            }

            // Then compare key values
            return primaryKeyValue.Equals(foreignKeyValue);
        }

        private static List<Dictionary<string, object>> GetForeignKeyValues(string[] foreignKeyNames, IEnumerable<object> items)
        {
            var foreignKeyValues = new List<Dictionary<string, object>>();

            foreach (object item in items)
            {
                var foreignKeyValue = new Dictionary<string, object>();
                foreach (var foreignKeyName in foreignKeyNames)
                {
                    var prop = item.GetType().GetProperty(foreignKeyName);
                    var value = prop != null ? prop.GetValue(item) : null;
                    if (value != null)
                        foreignKeyValue.Add(foreignKeyName, value);
                }
                if (foreignKeyValue.Count > 0)
                    foreignKeyValues.Add(foreignKeyValue);
            }

            return foreignKeyValues;
        }

        private static string[] GetPrimaryKeyNames(this DbContext dbContext, Type entityType)
        {
            var edmEntityType = dbContext.GetEdmSpaceType(entityType);
            if (edmEntityType == null) return null;

            // Get key names
            var primaryKeyNames = edmEntityType.KeyMembers.Select(k => k.Name).ToArray();
            return primaryKeyNames;
        }

        private static string[] GetForeignKeyNames(this DbContext dbContext,
            Type entityType, string propertyName)
        {
            // Get navigation property association
            var edmEntityType = dbContext.GetEdmSpaceType(entityType);
            if (edmEntityType == null) return null;
            var navProp = edmEntityType.NavigationProperties
                .SingleOrDefault(p => p.Name == propertyName);
            if (navProp == null) return null;
            var assoc = navProp.RelationshipType as AssociationType;
            if (assoc == null) return null;

            // Get foreign key names
            var fkPropNames = assoc.ReferentialConstraints[0].FromProperties
                .Select(p => p.Name).ToArray();
            return fkPropNames;
        }

        private static string GetQueryEntitySql(string entitySetName,
            string foreignKeyName, params object[] keyValues)
        {
            if (keyValues.Length == 0) return null;
            var ids = from k in keyValues
                      select k is string ? string.Format("'{0}'", k) : k.ToString();
            string csvIds = string.Join(",", ids);
            string entitySql = string.Format
                ("SELECT VALUE x FROM {0} AS x WHERE x.{1} IN {{{2}}}",
                entitySetName, foreignKeyName, csvIds);
            return entitySql;
        }

        private static string GetQueryEntitySql(string entitySetName,
            List<Dictionary<string, object>> primaryKeysList)
        {
            string whereSql = GetWhereSql(primaryKeysList);
            string entitySql = string.Format("SELECT VALUE x FROM {0} AS x {1}", 
                entitySetName, whereSql);
            return entitySql;
        }

        static string GetWhereSql(List<Dictionary<string, object>> primaryKeysList)
        {
            string whereSql = string.Empty;

            foreach (var primaryKeys in primaryKeysList)
            {
                if (whereSql.Length == 0)
                    whereSql += "WHERE ";
                else
                    whereSql += " OR ";

                string itemSql = string.Empty;
                foreach (var primaryKey in primaryKeys)
                {
                    if (itemSql.Length == 0)
                        itemSql = "(";
                    else
                        itemSql += " AND ";

                    itemSql += string.Format("x.{0} = {1}",
                        primaryKey.Key, primaryKey.Value);
                }
                if (itemSql.Length > 0)
                    itemSql += ")";
                whereSql += itemSql;
            }

            return whereSql;
        }

        private static List<object> ExecuteQueryEntitySql(this DbContext dbContext, string entitySql)
        {
            var objContext = ((IObjectContextAdapter)dbContext).ObjectContext;
            var objQuery = new ObjectQuery<object>(entitySql, objContext);
            ObjectResult<object> result = objQuery.Execute(MergeOption.NoTracking);
            return result.ToList();
        }

#if EF_6
        private static async Task<List<object>> ExecuteQueryEntitySqlAsync(this DbContext dbContext, string entitySql,
            CancellationToken cancellationToken)
        {
            var objContext = ((IObjectContextAdapter)dbContext).ObjectContext;
            var objQuery = new ObjectQuery<object>(entitySql, objContext);
            ObjectResult<object> result = await objQuery.ExecuteAsync(MergeOption.NoTracking, cancellationToken);
            return result.ToList();
        }
#endif

        private class IdMatcher : IEqualityComparer<object>
        {
            public DbContext DbContext;

            bool IEqualityComparer<object>.Equals(object x, object y)
            {
                if (!x.GetType().Equals(y.GetType())) return false;
                return KeyValuesAreEqual(GetKeyValue(x), GetKeyValue(y));
            }

            int IEqualityComparer<object>.GetHashCode(object obj)
            {
                object primaryKeyValue = GetKeyValue(obj);

                // Normalized string PKs
                if (primaryKeyValue is string)
                {
                    primaryKeyValue = ((string)primaryKeyValue).Normalize();
                }

                return Tuple.Create(obj.GetType(), primaryKeyValue).GetHashCode();
            }

            private object GetKeyValue(object entity)
            {
                Type entityType = entity.GetType();
                string primaryKeyName = DbContext.GetPrimaryKeyNames(entityType).FirstOrDefault();
                return entityType.GetProperty(primaryKeyName).GetValue(entity);
            }
        }

        private static ObjectVisitationHelper CreateVisitationHelperWithIdMatching(DbContext dbContext)
        {
            var visitationHelper = new ObjectVisitationHelper(new IdMatcher() { DbContext = dbContext });
            return visitationHelper;
        }

        #endregion
    }
}
