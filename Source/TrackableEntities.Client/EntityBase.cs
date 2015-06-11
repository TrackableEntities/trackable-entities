using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace TrackableEntities.Client
{
    /// <summary>
    /// Base class for model entities
    /// </summary>
    public abstract partial class EntityBase : INotifyPropertyChanged, ITrackable, IIdentifiable
    {
        /// <summary>
        /// Event for notification of property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fire PropertyChanged event.
        /// </summary>
        /// <typeparam name="TResult">Property return type</typeparam>
        /// <param name="property">Lambda expression for property</param>
        protected void NotifyPropertyChanged<TResult>
            (Expression<Func<TResult>> property)
        {
            string propertyName = ((MemberExpression)property.Body).Member.Name;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Generate entity identifier used for correlation with MergeChanges (if not yet done)
        /// </summary>
        public void SetEntityIdentifier()
        {
            if (EntityIdentifier == default(Guid))
                EntityIdentifier = Guid.NewGuid();
        }

        /// <summary>
        /// Copy entity identifier used for correlation with MergeChanges from another entity
        /// </summary>
        /// <param name="other">Other trackable object</param>
        public void SetEntityIdentifier(IIdentifiable other)
        {
            var otherEntity = other as EntityBase;
            if (otherEntity != null)
                EntityIdentifier = otherEntity.EntityIdentifier;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same
        /// type. The comparison is based on EntityIdentifier.
        /// 
        /// If the local EntityIdentifier is empty, then return false.
        /// </summary>
        /// <param name="other">An object to compare with this object</param>
        /// <returns></returns>
        public bool IsEquatable(IIdentifiable other)
        {
            if (EntityIdentifier == default(Guid))
                return false;

            var otherEntity = other as EntityBase;
            if (otherEntity == null)
                return false;

            return EntityIdentifier.Equals(otherEntity.EntityIdentifier);
        }

        bool IEquatable<IIdentifiable>.Equals(IIdentifiable other)
        {
            return IsEquatable(other);
        }
    }
}
