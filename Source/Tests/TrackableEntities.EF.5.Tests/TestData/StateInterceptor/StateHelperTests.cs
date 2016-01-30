using System.Data;
using System.Linq;
using Xunit;

namespace TrackableEntities.EF.Tests.TestData.StateInterceptor
{
    public class StateHelperTests
    {
        [Fact]
        public void All_States_Should_Not_Contain_Detached_State()
        {
            // Arrange
 
            // Act
            var allStates = StateHelper.GetAllFinalStates().ToList();

            // Assert
            Assert.Equal(4, allStates.Count);
            Assert.DoesNotContain(EntityState.Detached, allStates);
        }

        [Fact]
        public void Other_States_Should_Not_Return_Unchanged_State()
        {
            // Arrange
            var inputState = EntityState.Unchanged;

            // Act
            var otherStates = StateHelper.GetOtherStates(inputState).ToList();

            // Assert
            Assert.Equal(3, otherStates.Count);
            Assert.Contains(EntityState.Added, otherStates);
            Assert.Contains(EntityState.Modified, otherStates);
            Assert.Contains(EntityState.Deleted, otherStates);
        }

        [Fact]
        public void Other_States_Should_Not_Return_Added_State()
        {
            // Arrange
            var inputState = EntityState.Added;

            // Act
            var otherStates = StateHelper.GetOtherStates(inputState).ToList();

            // Assert
            Assert.Equal(3, otherStates.Count);
            Assert.Contains(EntityState.Unchanged, otherStates);
            Assert.Contains(EntityState.Modified, otherStates);
            Assert.Contains(EntityState.Deleted, otherStates);
        }

        [Fact]
        public void Other_States_Should_Not_Return_Modified_State()
        {
            // Arrange
            var inputState = EntityState.Modified;

            // Act
            var otherStates = StateHelper.GetOtherStates(inputState).ToList();

            // Assert
            Assert.Equal(3, otherStates.Count);
            Assert.Contains(EntityState.Unchanged, otherStates);
            Assert.Contains(EntityState.Added, otherStates);
            Assert.Contains(EntityState.Deleted, otherStates);
        }

        [Fact]
        public void Other_States_Should_Not_Return_Deleted_State()
        {
            // Arrange
            var inputState = EntityState.Deleted;

            // Act
            var otherStates = StateHelper.GetOtherStates(inputState).ToList();

            // Assert
            Assert.Equal(3, otherStates.Count);
            Assert.Contains(EntityState.Unchanged, otherStates);
            Assert.Contains(EntityState.Added, otherStates);
            Assert.Contains(EntityState.Modified, otherStates);
        }

        [Fact]
        public void Final_State_Without_Interceptor_Should_Return_Unchanged()
        {
            // Arrange
            bool useInterceptor = false;
            var inputState = TrackingState.Unchanged;

            // Act
            var finalStates = StateHelper.GetFinalStates(inputState, useInterceptor).ToList();

            // Assert
            Assert.Equal(1, finalStates.Count);
            Assert.Contains(EntityState.Unchanged, finalStates);
        }

        [Fact]
        public void Final_State_Without_Interceptor_Should_Return_Added()
        {
            // Arrange
            bool useInterceptor = false;
            var inputState = TrackingState.Added;

            // Act
            var finalStates = StateHelper.GetFinalStates(inputState, useInterceptor).ToList();

            // Assert
            Assert.Equal(1, finalStates.Count);
            Assert.Contains(EntityState.Added, finalStates);
        }

        [Fact]
        public void Final_State_Without_Interceptor_Should_Return_Modified()
        {
            // Arrange
            bool useInterceptor = false;
            var inputState = TrackingState.Modified;

            // Act
            var finalStates = StateHelper.GetFinalStates(inputState, useInterceptor).ToList();

            // Assert
            Assert.Equal(1, finalStates.Count);
            Assert.Contains(EntityState.Modified, finalStates);
        }

        [Fact]
        public void Final_State_Without_Interceptor_Should_Return_Deleted()
        {
            // Arrange
            bool useInterceptor = false;
            var inputState = TrackingState.Deleted;

            // Act
            var finalStates = StateHelper.GetFinalStates(inputState, useInterceptor).ToList();

            // Assert
            Assert.Equal(1, finalStates.Count);
            Assert.Contains(EntityState.Deleted, finalStates);
        }

        [Fact]
        public void Final_State_With_Interceptor_Should_Return_Other_Than_Unchanged()
        {
            // Arrange
            bool useInterceptor = true;
            var inputState = TrackingState.Unchanged;

            // Act
            var finalStates = StateHelper.GetFinalStates(inputState, useInterceptor).ToList();

            // Assert
            Assert.Equal(3, finalStates.Count);
            Assert.Contains(EntityState.Added, finalStates);
            Assert.Contains(EntityState.Modified, finalStates);
            Assert.Contains(EntityState.Deleted, finalStates);
        }

        [Fact]
        public void Final_State_With_Interceptor_Should_Return_Other_Than_Add()
        {
            // Arrange
            bool useInterceptor = true;
            var inputState = TrackingState.Added;

            // Act
            var finalStates = StateHelper.GetFinalStates(inputState, useInterceptor).ToList();

            // Assert
            Assert.Equal(3, finalStates.Count);
            Assert.Contains(EntityState.Unchanged, finalStates);
            Assert.Contains(EntityState.Modified, finalStates);
            Assert.Contains(EntityState.Deleted, finalStates);
        }

        [Fact]
        public void Final_State_With_Interceptor_Should_Return_Other_Than_Modified()
        {
            // Arrange
            bool useInterceptor = true;
            var inputState = TrackingState.Modified;

            // Act
            var finalStates = StateHelper.GetFinalStates(inputState, useInterceptor).ToList();

            // Assert
            Assert.Equal(3, finalStates.Count);
            Assert.Contains(EntityState.Unchanged, finalStates);
            Assert.Contains(EntityState.Added, finalStates);
            Assert.Contains(EntityState.Deleted, finalStates);
        }

        [Fact]
        public void Final_State_With_Interceptor_Should_Return_Other_Than_Deleted()
        {
            // Arrange
            bool useInterceptor = true;
            var inputState = TrackingState.Deleted;

            // Act
            var finalStates = StateHelper.GetFinalStates(inputState, useInterceptor).ToList();

            // Assert
            Assert.Equal(3, finalStates.Count);
            Assert.Contains(EntityState.Unchanged, finalStates);
            Assert.Contains(EntityState.Added, finalStates);
            Assert.Contains(EntityState.Modified, finalStates);
        }
    }
}
