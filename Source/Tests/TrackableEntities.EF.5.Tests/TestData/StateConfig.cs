#if EF_6
using System.Data.Entity;
#else
using System.Data;
#endif

namespace TrackableEntities.EF.Tests.TestData
{
    /// <summary>
    /// Configuration of state.
    /// </summary>
    public class StateConfig
    {
        public TrackingState InitState { get; set; }
        public EntityState FinalState { get; set; }
        public bool UseInterceptor { get; set; }

        public StateConfig(TrackingState initState, EntityState finalState, bool useInterceptor = true)
        {
            this.InitState = initState;
            this.FinalState = finalState;
            this.UseInterceptor = useInterceptor;
        }
    }
}