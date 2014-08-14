using System;
using System.Collections.Generic;
using System.Linq;

namespace TrackableEntities.Common
{
    /// <summary>
    /// This class facilitates proper checking for circular references when iterating the graph nodes.
    /// It is designed as an immutable collection of visited node. Thus it is not possible to add
    /// new objects into an existing collection, but the proper way is always to create a new
    /// instance of ObjectVisitationHelper which contains all the previous nodes plus the new one.
    /// Such design significantly reduced the complexity of the graph visitation algorithms.
    /// </summary>
    public class ObjectVisitationHelper : IEqualityComparer<object>
    {
        private readonly IEnumerable<object> _objectSet;
        private IEqualityComparer<object> _equalityComparer;

        /// <summary>
        /// Customizable equality comparer used by IsVisited method
        /// </summary>
        public IEqualityComparer<object> EqualityComparer
        {
            get
            {
                return _equalityComparer ?? this;
            }
            set
            {
                _equalityComparer = value;
            }
        }

        /// <summary>
        /// Helper method which initializes the given reference to ObjectVisitationHelper
        /// if it is null.
        /// </summary>
        /// <param name="visitationHelper">Reference to ObjectVisitationHelper</param>
        public static void EnsureCreated(ref ObjectVisitationHelper visitationHelper)
        {
            if (visitationHelper == null)
                visitationHelper = new ObjectVisitationHelper();
        }

        /// <summary>
        /// The collection will contain a single object or will be initialized empty
        /// if no/null object is provided.
        /// </summary>
        public ObjectVisitationHelper(object obj = null)
        {
            _objectSet = Enumerable.Repeat<object>(obj, obj == null ? 0 : 1);
        }

        /// <summary>
        /// Constructs a new collection which contains all the previously visited nodes plus the new one.
        /// An attempt to add the same object twice will throw InvalidOperationException.
        /// <param name="obj">New object to be put into the collection</param>
        /// </summary>
        /// <returns>The new collection</returns>
        public ObjectVisitationHelper With(object obj)
        {
            return new ObjectVisitationHelper(this, obj);
        }

        private ObjectVisitationHelper(ObjectVisitationHelper other, object obj)
        {
            if (obj == null)
                throw new NullReferenceException("obj");
            if (other.IsVisited(obj))
                throw new InvalidOperationException(string.Format("Object {0} has beed already visited", obj));

            _equalityComparer = other._equalityComparer;
            _objectSet = other._objectSet.Union(Enumerable.Repeat<object>(obj, 1), EqualityComparer);
        }

        /// <summary>
        /// Checks if the given graph node has already been visited (is contained in the collection)
        /// <param name="obj">An object to be checked</param>
        /// </summary>
        public bool IsVisited(object obj)
        {
            if (obj == null)
                throw new NullReferenceException("obj");
            return _objectSet.Contains(obj, EqualityComparer);
        }

        /// <summary>
        /// Finds a visited object which matches the given object by EqualityComparer
        /// <param name="obj">An object to be found</param>
        /// </summary>
        public object FindVisited(object obj)
        {
            if (obj == null)
                throw new NullReferenceException("obj");
            return _objectSet.Where(o => EqualityComparer.Equals(o, obj)).SingleOrDefault();
        }

        #region Default IEqualityComparer based on object references

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            // Reference equality
            return ReferenceEquals(x, y);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            // Reference hash
            return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }

        #endregion Default IEqualityComparer based on object references
    }
}
