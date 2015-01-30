using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace TrackableEntities.Client
{
    /// <summary>
    /// Base class for model entities
    /// </summary>
    [DataContract(IsReference = true)]
    public abstract class EntityBase : INotifyPropertyChanged, ITrackable, IIdentifiable
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
        /// Change-tracking state of an entity.
        /// </summary>
        public TrackingState TrackingState { get; set; }

        /// <summary>
        /// Properties on an entity that have been modified.
        /// </summary>
        public ICollection<string> ModifiedProperties { get; set; }

        [JsonProperty]
        private Guid EntityIdentifier { get; set; }

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
            EntityIdentifier = ((EntityBase)other).EntityIdentifier;
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

            return EntityIdentifier.Equals(((EntityBase)other).EntityIdentifier);
        }

        bool IEquatable<IIdentifiable>.Equals(IIdentifiable other)
        {
            return IsEquatable(other);
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj">An object to compare with this object</param>
        public override bool Equals(object obj)
        {
            var other = obj as EntityBase;
            if (other == null)
                return base.Equals(obj);

            return IsEquatable(other);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        public override int GetHashCode()
        {
            if (EntityIdentifier == default(Guid))
                return base.GetHashCode();

            return EntityIdentifier.GetHashCode();
        }
    }
}
