using System;
using System.Data.Entity;
using TrackableEntities.EF.Tests.FamilyModels;

namespace TrackableEntities.EF.Tests.Contexts
{
    public class FamilyDbContext : DbContext
    {
#if EF_6
        private const string TestDbName = "FamilyTestDbv6";        
#else
        private const string TestDbName = "FamilyTestDbv5";
#endif
        public FamilyDbContext(CreateDbOptions createDbOptions = CreateDbOptions.CreateDatabaseIfNotExists)
            : base(TestDbName)
        {
            switch (createDbOptions)
            {
                case CreateDbOptions.DropCreateDatabaseAlways:
                    Database.SetInitializer(new DropCreateDatabaseAlways<FamilyDbContext>());
                    break;
                case CreateDbOptions.DropCreateDatabaseSeed:
                    throw new NotSupportedException("DropCreateDatabaseSeed not supported");
                case CreateDbOptions.DropCreateDatabaseIfModelChanges:
                    Database.SetInitializer(new DropCreateDatabaseIfModelChanges<FamilyDbContext>());
                    break;
                default:
                    Database.SetInitializer(new CreateDatabaseIfNotExists<FamilyDbContext>());
                    break;
            }
            //Parents = Set<Parent>();
            //Children = Set<Child>();
        }

        public DbSet<Parent> Parents { get; set; }
        public DbSet<Child> Children { get; set; }
    }
}
