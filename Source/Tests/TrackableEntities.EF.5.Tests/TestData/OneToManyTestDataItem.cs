#if EF_6
using System.Data.Entity;
using TrackableEntities.EF6;
#else
using System.Data;
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
                this.Relationship,
                this.Order,
                this.OrderDetail1,
                this.OrderDetail2,
                this.OrderDetail3
            };
        }
    }
}