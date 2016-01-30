using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
#if EF_6
using System.Data.Entity;
#else

#endif

namespace TrackableEntities.EF.Tests.TestData.StateInterceptor
{
    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
    public class StateHelper
    {
        public static IEnumerable<TrackingState> GetAllInitStates()
        {
            yield return TrackingState.Unchanged;
            yield return TrackingState.Added;
            yield return TrackingState.Modified;
            yield return TrackingState.Deleted;
        }

        public static IEnumerable<EntityState> GetAllFinalStates()
        {
            yield return EntityState.Unchanged;
            yield return EntityState.Added;
            yield return EntityState.Modified;
            yield return EntityState.Deleted;
        }

        public static EntityState ConvertToEntityState(TrackingState trackingState)
        {
            switch (trackingState)
            {
                case TrackingState.Unchanged:
                    return EntityState.Unchanged;
                case TrackingState.Added:
                    return EntityState.Added;
                case TrackingState.Modified:
                    return EntityState.Modified;
                case TrackingState.Deleted:
                    return EntityState.Deleted;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingState), trackingState, null);

            }
        }

        public static IEnumerable<EntityState> GetOtherStates(EntityState entityState)
        {
            foreach (var state in GetAllFinalStates())
            {
                if (state != entityState)
                    yield return state;
            }
        }

        public static IEnumerable<EntityState> GetFinalStates(TrackingState initState, bool useInterceptor)
        {
            if (useInterceptor)
                foreach (var entityState in GetOtherStates(ConvertToEntityState(initState))) yield return entityState;
            else
                yield return ConvertToEntityState(initState);
        }
    }
}