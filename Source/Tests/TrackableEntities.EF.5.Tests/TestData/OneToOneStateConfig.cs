#if EF_6
using System.Data.Entity;
#else
using System.Data;
#endif

namespace TrackableEntities.EF.Tests.TestData
{
    /// <summary>
    /// Configuration of state for One to One tests.
    /// </summary>
    public class OneToOneStateConfig
    {
        public TrackingState InitState { get; set; }
        public EntityState? FinalState { get; set; }
        public bool OverrideState { get; set; }

        public OneToOneStateConfig(TrackingState initState, EntityState? finalState, bool overrideState)
        {
            this.InitState = initState;
            this.FinalState = finalState;
            this.OverrideState = overrideState;
        }
    }
}