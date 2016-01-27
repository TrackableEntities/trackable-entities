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
                Customer.InitState,
                Customer.FinalState,
                Customer.UseInterceptor,
                Setting.InitState,
                Setting.FinalState,
                Setting.UseInterceptor
            };
        }
    }
}