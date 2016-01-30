using System;
using System.Data;
#if EF_6
using System.Data.Entity;
#else

#endif

namespace TrackableEntities.EF.Tests.TestData.StateInterceptor
{
    /// <summary>
    /// Configuration of state.
    /// </summary>
    public class StateConfig : ITestDataItem, IEquatable<StateConfig>
    {
        public TrackingState InitState { get; }
        public EntityState FinalState { get; }
        public bool UseInterceptor { get; }

        public StateConfig(TrackingState initState, EntityState finalState, bool useInterceptor = true)
        {
            InitState = initState;
            FinalState = finalState;
            UseInterceptor = useInterceptor;
        }

        public object[] AsArray()
        {
            return new object[] { this };
        }

        #region R# Generated IEquatable<StateConfig>

        public bool Equals(StateConfig other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return InitState == other.InitState && FinalState == other.FinalState && UseInterceptor == other.UseInterceptor;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((StateConfig)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)InitState;
                hashCode = (hashCode * 397) ^ (int)FinalState;
                hashCode = (hashCode * 397) ^ UseInterceptor.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(StateConfig left, StateConfig right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StateConfig left, StateConfig right)
        {
            return !Equals(left, right);
        }

        #endregion
        
        public override string ToString()
        {
            return $"{GetType().Name} {{{InitState} -> {FinalState}, {UseInterceptor}}}";
        }
    }
}