using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using System.Threading;
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
            ApplyChanges(context, item, null, null);
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
                context.ApplyChangesOnProperties(item, parent);
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
                context.ApplyChangesOnProperties(item, parent);
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
            context.ApplyChangesOnProperties(item, parent);
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
            LoadRelatedEntities(context, new[] {item}, null, loadAll);
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
            LoadRelatedEntities(context, items.ToArray(), null, loadAll);
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
            return LoadRelatedEntitiesAsync(context, new[] {item}, null, 
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
            return LoadRelatedEntitiesAsync(context, new[] { item }, null, 
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
            return LoadRelatedEntitiesAsync(context, items.ToArray(), null,
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
            return LoadRelatedEntitiesAsync(context, items.ToArray(), null, 
                cancellationToken, loadAll);
        }
#endif

        private static void LoadRelatedEntities(this DbContext context, 
            ITrackable[] items, ITrackable parent, bool loadAll)
        {
            // Return if no items
            if (items == null || items.Length == 0) return;

            // Populate related entities on all items
            var entity = items[0];
            foreach (var prop in entity.GetType().GetProperties())
            {
                // Continue if prop is not ITrackable
                if (!typeof(ITrackable).IsAssignableFrom(prop.PropertyType))
                    continue;

                // Continue if trackable prop is same type as parent
                if (parent != null && prop.PropertyType == parent.GetType())
                    continue;

                // Get selected items
                var selectedItems = loadAll ? items
                    : items.Where(t => t.TrackingState == TrackingState.Added
                        || (parent != null && parent.TrackingState == TrackingState.Added)).ToArray();
                if (selectedItems.Length == 0) continue;
                var entities = selectedItems.Cast<object>().ToArray();

                // Get related entities
                string entityTypeName = entity.GetType().Name;
                string propertyName = prop.Name;
                string propertyTypeName = prop.PropertyType.Name;
                List<object> relatedEntities = context.GetRelatedEntities(entities,
                    entityTypeName, propertyName, propertyTypeName);

                // Continue if there are no related entities
                if (!relatedEntities.Any()) continue;

                // Set related entities
                context.SetRelatedEntities(entities, relatedEntities, prop,
                    entityTypeName, propertyName, propertyTypeName);
            }

            // Recursively populate related entities on ref and child properties
            foreach (var item in items)
            {
                bool loadAllRelated = loadAll 
                    || item.TrackingState == TrackingState.Added
                    || (parent  != null && parent.TrackingState == TrackingState.Added);
                context.LoadRelatedEntitiesOnProperties(item, parent, loadAllRelated);
            }
        }

#if EF_6
        private static async Task LoadRelatedEntitiesAsync(this DbContext context,
            ITrackable[] items, ITrackable parent, CancellationToken cancellationToken, bool loadAll)
        {
            // Return if no items
            if (items == null || items.Length == 0) return;

            // Populate related entities on all items
            var entity = items[0];
            foreach (var prop in entity.GetType().GetProperties())
            {
                // Continue if prop is not ITrackable
                if (!typeof(ITrackable).IsAssignableFrom(prop.PropertyType))
                    continue;

                // Continue if trackable prop is same type as parent
                if (parent != null && prop.PropertyType == parent.GetType())
                    continue;

                // Get selected items
                var selectedItems = loadAll ? items
                    : items.Where(t => t.TrackingState == TrackingState.Added
                        || (parent != null && parent.TrackingState == TrackingState.Added)).ToArray();
                if (selectedItems.Length == 0) continue;
                var entities = selectedItems.Cast<object>().ToArray();

                // Get related entities
                string entityTypeName = entity.GetType().Name;
                string propertyName = prop.Name;
                string propertyTypeName = prop.PropertyType.Name;
                List<object> relatedEntities = await context.GetRelatedEntitiesAsync(entities,
                    entityTypeName, propertyName, propertyTypeName, cancellationToken);

                // Continue if there are no related entities
                if (!relatedEntities.Any()) continue;

                // Set related entities
                context.SetRelatedEntities(entities, relatedEntities, prop,
                    entityTypeName, propertyName, propertyTypeName);
            }

            // Recursively populate related entities on ref and child properties
            foreach (var item in items)
            {
                bool loadAllRelated = loadAll
                    || item.TrackingState == TrackingState.Added
                    || (parent != null && parent.TrackingState == TrackingState.Added);
                await context.LoadRelatedEntitiesOnPropertiesAsync(item, parent,
                    cancellationToken, loadAllRelated);
            }
        }

        private async static Task LoadRelatedEntitiesOnPropertiesAsync(this DbContext context,
            ITrackable item, ITrackable parent, CancellationToken cancellationToken, bool loadAll)
        {
            // Recursively load related entities
            foreach (var prop in item.GetType().GetProperties())
            {
                // Apply changes to 1-1 and M-1 properties
                var trackableRef = prop.GetValue(item) as ITrackable;

                // Stop recursion if trackable is same type as parent
                if (trackableRef != null
                    && (parent == null || trackableRef.GetType() != parent.GetType()))
                {
                    await context.LoadRelatedEntitiesAsync(new[] { trackableRef }, item,
                        cancellationToken, loadAll);
                }

                // Apply changes to 1-M and M-M properties
                var childItems = prop.GetValue(item, null) as IList;
                if (childItems != null && childItems.Count > 0)
                {
                    // Stop recursion if trackable is same type as parent
                    var trackableChild = childItems[0] as ITrackable;
                    if (trackableChild != null
                        && (parent == null || trackableChild.GetType() != parent.GetType()))
                    {
                        var trackableChildren = childItems.OfType<ITrackable>().ToArray();
                        await context.LoadRelatedEntitiesAsync(trackableChildren, item,
                            cancellationToken, loadAll);
                    }
                }
            }
        }
#endif

        private static void LoadRelatedEntitiesOnProperties(this DbContext context,
            ITrackable item, ITrackable parent, bool loadAll)
        {
            // Recursively load related entities
            foreach (var prop in item.GetType().GetProperties())
            {
                // Apply changes to 1-1 and M-1 properties
                var trackableRef = prop.GetValue(item) as ITrackable;

                // Stop recursion if trackable is same type as parent
                if (trackableRef != null
                    && (parent == null || trackableRef.GetType() != parent.GetType()))
                {
                    context.LoadRelatedEntities(new[] { trackableRef }, item, loadAll);
                }

                // Apply changes to 1-M and M-M properties
                var childItems = prop.GetValue(item, null) as IList;
                if (childItems != null && childItems.Count > 0)
                {
                    // Stop recursion if trackable is same type as parent
                    var trackableChild = childItems[0] as ITrackable;
                    if (trackableChild != null
                        && (parent == null || trackableChild.GetType() != parent.GetType()))
                    {
                        var trackableChildren = childItems.OfType<ITrackable>().ToArray();
                        context.LoadRelatedEntities(trackableChildren, item, loadAll);
                    }
                }
            }
        }

        #region ApplyChanges Helpers

        private static void ApplyChangesOnProperties(this DbContext context,
            ITrackable item, ITrackable parent)
        {
            // Recursively apply changes
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
                    var count = items.Count;
                    for (int i = count - 1; i > -1; i--)
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
                {
                    context.ApplyChanges(trackableReference, item, prop.Name);
                    if (context.IsRelatedProperty(item.GetType().Name, prop.Name, RelationshipType.OneToOne))
                        context.SetChanges(trackableReference, state, item, prop.Name);
                }


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
            // Get navigation property
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
            OneToOne,
            ManyToMany,
        }

        #endregion

        #region LoadRelatedEntities Helpers

        private static List<object> GetRelatedEntities(this DbContext context,
            object[] items, string entityTypeName, string propertyName, string propertyTypeName)
        {
            // Get entity sql
            string entitySql = context.GetRelatedEntitiesSql(items, entityTypeName, propertyName, propertyTypeName);

            // Get related entities
            List<object> entities = context.ExecuteQueryEntitySql(entitySql);
            return entities;
        }

#if EF_6
        private static async Task<List<object>> GetRelatedEntitiesAsync(this DbContext context,
            object[] items, string entityTypeName, string propertyName, string propertyTypeName,
            CancellationToken cancellationToken)
        {
            // Get entity sql
            string entitySql = context.GetRelatedEntitiesSql(items, entityTypeName, propertyName, propertyTypeName);

            // Get related entities
            List<object> entities = await context.ExecuteQueryEntitySqlAsync(entitySql, cancellationToken);
            return entities;
        } 
#endif

        private static string GetRelatedEntitiesSql(this DbContext context,
            object[] items, string entityTypeName, string propertyName, string propertyTypeName)
        {
            // Get entity set name
            string entitySetName = context.GetEntitySetName(propertyTypeName);

            // Get foreign key name
            string foreignKeyName = context.GetForeignKeyName(entityTypeName, propertyName);

            // Get key values
            var keyValues = GetKeyValues(foreignKeyName, items);

            // Get entity sql
            return GetQueryEntitySql(entitySetName, foreignKeyName, keyValues);
        }

        private static void SetRelatedEntities(this DbContext context, 
            IEnumerable<object> entities, List<object> relatedEntities, PropertyInfo referenceProperty,
            string entityTypeName, string propertyName, string propertyTypeName)
        {
            // Get names of entity foreign key and related entity primary key
            string foreignKeyName = context.GetForeignKeyName(entityTypeName, propertyName);
            string primaryKeyName = context.GetPrimaryKeyName(propertyTypeName);

            // Continue if we can't get foreign or primary key names
            if (foreignKeyName == null || primaryKeyName == null) return;

            foreach (var entity in entities)
            {
                // Get foreign key id
                var foreignKeyProp = entity.GetType().GetProperty(foreignKeyName);
                if (foreignKeyProp == null) break;
                var foreignKeyId = foreignKeyProp.GetValue(entity);
                if (foreignKeyId == null) break;

                // Get related entity
                var relatedEntity = (from e in relatedEntities
                    let p = e.GetType().GetProperty(primaryKeyName)
                    let primaryKeyId = p != null ? p.GetValue(e) : null
                    where KeyValuesAreEqual(primaryKeyId, foreignKeyId)
                    select e).SingleOrDefault();

                // Set reference prop to related entity
                if (relatedEntity != null)
                    referenceProperty.SetValue(entity, relatedEntity);
            }
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

        private static object[] GetKeyValues(string foreignKeyName, params object[] items)
        {
            var values = from item in items
                         let prop = item.GetType().GetProperty(foreignKeyName)
                         select prop != null ? prop.GetValue(item) : null;
            return values.Where(v => v != null).Distinct().ToArray();
        }

        private static string GetEntitySetName(this DbContext dbContext, string propertyTypeName)
        {
            MetadataWorkspace workspace = ((IObjectContextAdapter)dbContext)
                .ObjectContext.MetadataWorkspace;
            var containers = workspace.GetItems<EntityContainer>(DataSpace.CSpace);
            if (containers == null) return null;
            var entitySet = containers.First().BaseEntitySets
                .SingleOrDefault(es => es.ElementType.Name == propertyTypeName);
            if (entitySet == null) return null;
            return entitySet.Name;
        }

        private static string GetPrimaryKeyName(this DbContext dbContext, string entityTypeName)
        {
            // Get navigation property association
            MetadataWorkspace workspace = ((IObjectContextAdapter)dbContext)
                .ObjectContext.MetadataWorkspace;
            var entityTypes = workspace.GetItems<EntityType>(DataSpace.CSpace);
            if (entityTypes == null) return null;
            var entityType = entityTypes.SingleOrDefault(e => e.Name == entityTypeName);
            if (entityType == null) return null;

            // We're not supporting multiple primary keys for reference types
            if (entityType.KeyMembers.Count > 1) return null;

            // Get name 
            var primaryKeyName = entityType.KeyMembers.Select(k => k.Name).FirstOrDefault();
            return primaryKeyName;
        }

        private static string GetForeignKeyName(this DbContext dbContext,
            string entityTypeName, string propertyName)
        {
            // Get navigation property association
            MetadataWorkspace workspace = ((IObjectContextAdapter)dbContext)
                .ObjectContext.MetadataWorkspace;
            var entityTypes = workspace.GetItems<EntityType>(DataSpace.CSpace);
            if (entityTypes == null) return null;
            var entityType = entityTypes.SingleOrDefault(e => e.Name == entityTypeName);
            if (entityType == null) return null;
            var navProp = entityType.NavigationProperties
                .SingleOrDefault(p => p.Name == propertyName);
            if (navProp == null) return null;
            var assoc = navProp.RelationshipType as AssociationType;
            if (assoc == null) return null;

            // Get foreign key name
            var fkPropName = assoc.ReferentialConstraints[0].FromProperties[0].Name;
            return fkPropName;
        }

        private static string GetQueryEntitySql(string entitySetName,
            string foreignKeyName, params object[] keyValues)
        {
            var ids = from k in keyValues
                      select k is string ? string.Format("'{0}'", k) : k.ToString();
            string csvIds = string.Join(",", ids.ToArray());
            string entitySql = string.Format
                ("SELECT VALUE x FROM {0} AS x WHERE x.{1} IN {{{2}}}",
                entitySetName, foreignKeyName, csvIds);
            return entitySql;
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
        #endregion
    }
}
