using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TrackableEntities.Common
{
    /// <summary>
    /// Compares objects using reference equality.
    /// </summary>
    /// <typeparam name="T">Type of the object</typeparam>
    public sealed class ObjectReferenceEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        private ObjectReferenceEqualityComparer()
        {
        }

        static ObjectReferenceEqualityComparer()
        {
            Default = new ObjectReferenceEqualityComparer<T>();
        }

        /// <summary>
        /// Single instance of the comparer.
        /// </summary>
        public static ObjectReferenceEqualityComparer<T> Default { get; private set; }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(T x, T y)
        {
            return object.ReferenceEquals(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The System.Object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
