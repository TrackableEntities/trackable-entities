using System;

namespace TrackableEntities.EF.Tests
{
    public enum CreateDbOptions
    {
        CreateDatabaseIfNotExists,
        DropCreateDatabaseAlways,
        DropCreateDatabaseIfModelChanges,
        DropCreateDatabaseSeed
    }
}
