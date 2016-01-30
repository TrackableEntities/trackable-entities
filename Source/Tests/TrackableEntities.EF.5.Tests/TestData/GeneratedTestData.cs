using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TrackableEntities.EF.Tests.TestData
{
    /// <summary>
    /// Base class for generated test data colleciton used in <see cref="Xunit.ClassDataAttribute"/>.
    /// </summary>
    /// <typeparam name="TDataItem">Type of item for one test.</typeparam>
    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
    public abstract class GeneratedTestData<TDataItem> : IEnumerable<object[]> where TDataItem : ITestDataItem
    {
        /// <summary>
        /// Items excluded from combinations. For empty excludes use <![CDATA[<code>return Enumerable.Empty<TDataItem>();</code>]]>
        /// </summary>
        public abstract IEnumerable<TDataItem> GetExcludedItems();

        /// <summary>
        /// All combinations (some of them will be excluded based on <see cref="GetExcludedItems"/>).
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<TDataItem> GetAllCombinations();

        /// <summary>
        /// All combinations without excluded items.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TDataItem> GetTypedEnumerator()
        {
            foreach (var currentItem in GetAllCombinations())
            {
                if (!GetExcludedItems().Any(excludedItem => excludedItem.Equals(currentItem)))
                    yield return currentItem;
            }
        }

        IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator()
        {
            return GetTypedEnumerator().Select(item => item.AsArray()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<object[]>)this).GetEnumerator();
        }
    }
}