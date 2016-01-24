namespace TrackableEntities.EF.Tests.TestData
{
    /// <summary>
    /// Test data item for One to One tests.
    /// </summary>
    public class OneToOneTestDataItem : ITestDataItem
    {
        public OneToOneStateConfig Customer { get; set; }
        public OneToOneStateConfig Setting { get; set; }

        public object[] ToArray()
        {
            return new object[]
            {
                this.Customer.InitState,
                this.Customer.FinalState,
                this.Customer.OverrideState,
                this.Setting.InitState,
                this.Setting.FinalState,
                this.Setting.OverrideState
            };
        }
    }
}