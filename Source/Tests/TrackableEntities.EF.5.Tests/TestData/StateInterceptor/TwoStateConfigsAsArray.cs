namespace TrackableEntities.EF.Tests.TestData.StateInterceptor
{
    public class TwoStateConfigsAsArray : TestDataItemAsArray<StateConfig>
    {
        public TwoStateConfigsAsArray(StateConfig stateConfig1, StateConfig stateConfig2)
            : base(stateConfig1, stateConfig2)
        {
        }
    }
}