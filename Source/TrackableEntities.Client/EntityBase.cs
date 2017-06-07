using System;
using System.ComponentModel;
using System.Linq.Expressions;
#if !(SILVERLIGHT || NET40)
using System.Runtime.CompilerServices;
#endif

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
            NotifyPropertyChanged(propertyName);
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.</param>
#if SILVERLIGHT || NET40
        protected void NotifyPropertyChanged(string propertyName) =>
#else
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null) =>
#endif
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


#if !(SILVERLIGHT || NET40)
        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            NotifyPropertyChanged(propertyName);

            return true;
        }
#endif 

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
            if (other is EntityBase otherEntity)
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