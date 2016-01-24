namespace TrackableEntities.EF.Tests.TestData
{
    /// <summary>
    /// Test data item for One to One tests.
    /// </summary>
    public class OneToOneTestDataItem : ITestDataItem
    {
        public StateConfig Customer { get; set; }
        public StateConfig Setting { get; set; }

        public object[] ToArray()
        {
            return new object[]
            {
                this.Customer.InitState,
                this.Customer.FinalState,
                this.Customer.UseInterceptor,
                this.Setting.InitState,
                this.Setting.FinalState,
                this.Setting.UseInterceptor
            };
        }
    }
}