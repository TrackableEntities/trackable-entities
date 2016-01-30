using System;
using System.Collections.Generic;
using System.Linq;

namespace TrackableEntities.EF.Tests.TestData
{
    public abstract class TestDataItemAsArray<TItem> : ITestDataItem, IEquatable<TestDataItemAsArray<TItem>>
        where TItem : ITestDataItem, IEquatable<TItem>
    {
        public IList<TItem> Items { get; }

        protected TestDataItemAsArray(IList<TItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (items.Count == 0)
                throw new ArgumentException("There must be at least one item.", nameof(items));

            Items = items;
        }

        protected TestDataItemAsArray(params TItem[] items)
            : this(new List<TItem>(items))
        {
        }

        /// <summary>
        /// Returns test data item as array of objects.
        /// </summary>
        /// <returns></returns>
        public virtual object[] AsArray()
        {
            return Items.Cast<object>().ToArray();
        }

        #region IEquatable<TestDataItemAsArray<TItem>>

        public bool Equals(TestDataItemAsArray<TItem> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (Items.Count != other.Items.Count)
                return false;

            if (Items.Any(sc => !other.Items.Contains(sc)))
                return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            var other = obj as TestDataItemAsArray<TItem>;

            if (other == null)
                return false;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Items?.GetHashCode() ?? 0;
        }

        public static bool operator ==(TestDataItemAsArray<TItem> left, TestDataItemAsArray<TItem> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TestDataItemAsArray<TItem> left, TestDataItemAsArray<TItem> right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}