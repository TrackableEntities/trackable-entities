#if EF_6
using TrackableEntities.EF6;
#else
using TrackableEntities.EF5;
#endif

namespace TrackableEntities.EF.Tests.TestData
{
    /// <summary>
    /// Test data item for One to Many tests.
    /// </summary>
    public class OneToManyTestDataItem : ITestDataItem
    {
        public RelationshipType Relationship { get; set; }
        public StateConfig Order { get; set; }
        public StateConfig OrderDetail1 { get; set; }
        public StateConfig OrderDetail2 { get; set; }
        public StateConfig OrderDetail3 { get; set; }
        
        public object[] ToArray()
        {
            return new object[]
            {
                Relationship,
                Order,
                OrderDetail1,
                OrderDetail2,
                OrderDetail3
            };
        }
    }
}