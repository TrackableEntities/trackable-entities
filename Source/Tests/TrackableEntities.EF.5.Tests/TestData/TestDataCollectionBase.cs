using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TrackableEntities.EF.Tests.TestData
{
    /// <summary>
    /// Base class for test data used in <see cref="Xunit.ClassDataAttribute"/>.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public abstract class TestDataCollectionBase<TItem> : IEnumerable<object[]> where TItem : ITestDataItem
    {
        /// <summary>
        /// Test data items.
        /// </summary>
        public abstract IList<TItem> Items { get; }

        public IEnumerator<object[]> GetEnumerator()
        {
            return Items.Select(item => item.ToArray()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}