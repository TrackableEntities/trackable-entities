using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TrackableEntities.EF.Tests.TestData
{
    /// <summary>
    /// Base class for static test data collection used in <see cref="Xunit.ClassDataAttribute"/>.
    /// </summary>
    /// <typeparam name="TItem">Type of item for one test.</typeparam>
    public abstract class StaticTestData<TItem> : IEnumerable<object[]> where TItem : ITestDataItem
    {
        /// <summary>
        /// Test data items.
        /// </summary>
        public abstract IList<TItem> Items { get; }

        public IEnumerator<object[]> GetEnumerator()
        {
            return Items.Select(item => item.AsArray()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<object[]>)this).GetEnumerator();
        }
    }
}