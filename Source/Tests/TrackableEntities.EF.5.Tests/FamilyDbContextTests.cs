using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using NUnit.Framework;
#if EF_6
using TrackableEntities.EF6;
#else
using TrackableEntities.EF5;
#endif
using TrackableEntities.EF.Tests;
using TrackableEntities.EF.Tests.FamilyModels;
using TrackableEntities.EF.Tests.Mocks;
using TrackableEntities.EF.Tests.NorthwindModels;

#if EF_6
namespace TrackableEntities.EF6.Tests
#else
namespace TrackableEntities.EF5.Tests
#endif
{
    [TestFixture]
    public class FamilyDbContextTests
    {
        const CreateDbOptions CreateFamilyDbOptions = CreateDbOptions.DropCreateDatabaseIfModelChanges;

        [Test]
        public void Apply_Changes_Should_Mark_Added_Parent()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var parent = new Parent("Parent");
            parent.TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.AreEqual(EntityState.Added, context.Entry(parent).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Modified_Parent()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var parent = new Parent("Parent");
            parent.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.AreEqual(EntityState.Modified, context.Entry(parent).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Modified_Parent_Properties()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var parent = new Parent("Parent");
            parent.Name += "_Changed";
            parent.TrackingState = TrackingState.Modified;
            parent.ModifiedProperties = new List<string> {"Name"};

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.AreEqual(EntityState.Modified, context.Entry(parent).State);
            Assert.IsTrue(context.Entry(parent).Property("Name").IsModified);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Deleted_Parent()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var parent = new Parent("Parent");
            parent.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.AreEqual(EntityState.Deleted, context.Entry(parent).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Added_Parent_With_Children()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var parent = new Parent("Parent")
            {
                Children = new List<Child>
                    {
                        new Child("Child1"), 
                        new Child("Child2"),
                        new Child("Child3")
                    }
            };
            parent.TrackingState = TrackingState.Added;
            parent.Children[0].TrackingState = TrackingState.Added;
            parent.Children[1].TrackingState = TrackingState.Added;
            parent.Children[2].TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.AreEqual(EntityState.Added, context.Entry(parent).State);
            Assert.AreEqual(EntityState.Added, context.Entry(parent.Children[0]).State);
            Assert.AreEqual(EntityState.Added, context.Entry(parent.Children[1]).State);
            Assert.AreEqual(EntityState.Added, context.Entry(parent.Children[2]).State);
        }

        [Test]
        public void Apply_Changes_Should_Mark_Family_Unchanged()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var parent = new MockFamily().Parent;

            // Act
            context.ApplyChanges(parent);

            // Assert
            IEnumerable<EntityState> states = context.GetEntityStates(parent, EntityState.Unchanged);
            Assert.AreEqual(40, states.Count());
        }

        [Test]
        public void Apply_Changes_Should_Mark_Family_Added()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var parent = new MockFamily().Parent;
            parent.TrackingState = TrackingState.Added;
            parent.SetTrackingState(TrackingState.Added);

            // Act
            context.ApplyChanges(parent);

            // Assert
            IEnumerable<EntityState> states = context.GetEntityStates(parent, EntityState.Added);
            Assert.AreEqual(40, states.Count());
        }

        [Test]
        public void Apply_Changes_Should_Mark_Family_Modified()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var parent = new MockFamily().Parent;
            parent.TrackingState = TrackingState.Modified;
            parent.SetTrackingState(TrackingState.Modified);

            // Act
            context.ApplyChanges(parent);

            // Assert
            IEnumerable<EntityState> states = context.GetEntityStates(parent, EntityState.Modified);
            Assert.AreEqual(40, states.Count());
        }

        [Test]
        public void Apply_Changes_Should_Mark_Parent_With_Added_Modified_Deleted_Children()
        {
            // Arrange
            var context = TestsHelper.CreateFamilyDbContext(CreateFamilyDbOptions);
            var child1 = new Child("Child1");
            var child2 = new Child("Child2");
            var child3 = new Child("Child3");
            var parent = new Parent("Parent")
            {
                Children = new List<Child> { child1, child2, child3 }
            };
            child1.TrackingState = TrackingState.Added;
            child2.TrackingState = TrackingState.Modified;
            child3.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.AreEqual(EntityState.Unchanged, context.Entry(parent).State);
            Assert.AreEqual(EntityState.Added, context.Entry(child1).State);
            Assert.AreEqual(EntityState.Modified, context.Entry(child2).State);
            Assert.AreEqual(EntityState.Deleted, context.Entry(child3).State);
        }
    }
}
