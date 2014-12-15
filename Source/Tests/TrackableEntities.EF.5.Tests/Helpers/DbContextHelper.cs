using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
#if EF_6
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
#else
using System.Data.Objects;
#endif
using System.Data.Entity.Infrastructure;
using System.Linq;
using TrackableEntities.Common;

namespace TrackableEntities.EF.Tests
{
    internal static class DbContextHelper
    {
        // Recursively get entity states
        public static IEnumerable<EntityState> GetEntityStates(this DbContext context,
            ITrackable item, EntityState? entityState = null,
            ObjectVisitationHelper visitationHelper = null)
        {
            // Prevent endless recursion
            ObjectVisitationHelper.EnsureCreated(ref visitationHelper);
            if (!visitationHelper.TryVisit(item)) yield break;

            foreach (var colProp in item.GetNavigationProperties().OfCollectionType())
            {
                foreach (ITrackable child in colProp.EntityCollection)
                {
                    foreach (var state in context.GetEntityStates(child, visitationHelper: visitationHelper))
                    {
                        if (entityState == null || state == entityState)
                            yield return state;
                    }
                }
            }
            yield return context.Entry(item).State;
        }

        // Returns true if target entity has been added to relationship with source entity
        public static bool RelatedItemHasBeenAdded(this DbContext context,
            object source, object target)
        {
            var addedRels = context.GetAddedRelationships();
            bool itemAdded = addedRels
                .Any(r => ReferenceEquals(r.Item1, source)
                    && ReferenceEquals(r.Item2, target));
            return itemAdded;
        }

        // Returns true if target entity has been removed from relationship with source entity
        public static bool RelatedItemHasBeenRemoved(this DbContext context,
            object source, object target)
        {
            var deletedRels = context.GetDeletedRelationships();
            bool itemRemoved = deletedRels
                .Any(r => ReferenceEquals(r.Item1, source)
                    && ReferenceEquals(r.Item2, target));
            return itemRemoved;
        }

        // Returns tuples with ends for added relationships
        public static IEnumerable<Tuple<object, object>> GetAddedRelationships
            (this DbContext context)
        {
            return GetRelationships(context, EntityState.Added, (e, i) => e.CurrentValues[i]);
        }

        // Returns tuples with ends for added relationships
        public static IEnumerable<Tuple<object, object>> GetDeletedRelationships
            (this DbContext context)
        {
            return GetRelationships(context, EntityState.Deleted, (e, i) => e.OriginalValues[i]);
        }

        private static IEnumerable<Tuple<object, object>> GetRelationships(
            this DbContext context,
            EntityState relationshipState,
            Func<ObjectStateEntry, int, object> getValue)
        {
            context.ChangeTracker.DetectChanges();
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;

            return objectContext.ObjectStateManager
                .GetObjectStateEntries(relationshipState)
                .Where(e => e.IsRelationship)
                .Select(
                    e => Tuple.Create(
                        objectContext.GetObjectByKey((EntityKey)getValue(e, 0)),
                        objectContext.GetObjectByKey((EntityKey)getValue(e, 1))));
        }
    }
}
