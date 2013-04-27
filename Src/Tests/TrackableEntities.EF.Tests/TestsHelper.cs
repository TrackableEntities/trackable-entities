using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using NUnit.Framework;
using TrackableEntities.EF.Tests.Contexts;

namespace TrackableEntities.EF.Tests
{
    internal static class TestsHelper
    {
        public static FamilyDbContext CreateFamilyDbContext(CreateDbOptions createDbOptions)
        {
            // Create new context for all tests
            var context = new FamilyDbContext(createDbOptions);
            Assert.GreaterOrEqual(context.Parents.Count(), 0);
            return context;
        }

        public static NorthwindDbContext CreateNorthwindDbContext(CreateDbOptions createDbOptions)
        {
            // Create new context for all tests
            var context = new NorthwindDbContext(createDbOptions);
            Assert.GreaterOrEqual(context.Products.Count(), 0);
            return context;
        }

        // Recursively set tracking state
        public static void SetTrackingState(this ITrackable item, TrackingState state)
        {
            foreach (var prop in item.GetType().GetProperties())
            {
                var trackingColl = prop.GetValue(item, null) as ICollection;
                if (trackingColl != null)
                {
                    foreach (ITrackable child in trackingColl)
                    {
                        child.SetTrackingState(state);
                        child.TrackingState = state;
                    }
                }
            }
        }

        // Recursively get entity states
        public static IEnumerable<EntityState> GetEntityStates(this DbContext context, ITrackable item,
            EntityState? entityState = null)
        {
            foreach (var prop in item.GetType().GetProperties())
            {
                var trackingColl = prop.GetValue(item, null) as ICollection;
                if (trackingColl != null)
                {
                    foreach (ITrackable child in trackingColl)
                    {
                        foreach (var state in context.GetEntityStates(child))
                        {
                            if (entityState == null || state == entityState)
                                yield return state;
                        }
                    }
                }
            }
            yield return context.Entry(item).State;
        }
    }
}
