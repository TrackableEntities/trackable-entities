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

namespace TrackableEntities.EF.Tests
{
    internal static class DbContextHelper
    {
        // Recursively get entity states
        public static IEnumerable<EntityState> GetEntityStates(this DbContext context,
            ITrackable item, EntityState? entityState = null,
            ITrackable parent = null)
        {
            foreach (var prop in item.GetType().GetProperties())
            {
                var trackingColl = prop.GetValue(item, null) as ICollection;
                if (trackingColl != null)
                {
                    foreach (ITrackable child in trackingColl)
                    {
                        if (parent == null || child.GetType() != parent.GetType())
                        {
                            foreach (var state in context.GetEntityStates(child, parent: item))
                            {
                                if (entityState == null || state == entityState)
                                    yield return state;
                            }
                        }
                    }
                }
            }
            yield return context.Entry(item).State;
        }

        // Returns true if target entity has been added to relationship with source entity
        public static bool RelatedItemHasBeenAdded(this DbContext context,
            object source, object target)
        {
            return context.GetAddedRelationships()
                .Any(r => ReferenceEquals(r.Item1, target)
                    && ReferenceEquals(r.Item2, source));
        }

        // Returns true if target entity has been removed from relationship with source entity
        public static bool RelatedItemHasBeenRemoved(this DbContext context,
            object source, object target)
        {
            return context.GetDeletedRelationships()
                .Any(r => ReferenceEquals(r.Item1, target)
                    && ReferenceEquals(r.Item2, source));
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
