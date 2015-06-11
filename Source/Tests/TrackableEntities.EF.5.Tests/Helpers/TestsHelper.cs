using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using TrackableEntities.EF.Tests.Contexts;
using Xunit;

namespace TrackableEntities.EF.Tests
{
    internal static class TestsHelper
    {
        public static FamilyDbContext CreateFamilyDbContext(CreateDbOptions createDbOptions)
        {
            // Create new context for all tests
            var context = new FamilyDbContext(createDbOptions);
            Assert.True(context.Parents.Count() >= 0);
            return context;
        }

        public static NorthwindDbContext CreateNorthwindDbContext(CreateDbOptions createDbOptions)
        {
            // Create new context for all tests
            var context = new NorthwindDbContext(createDbOptions);
            Assert.True(context.Products.Count() >= 0);
            return context;
        }
    }
}
