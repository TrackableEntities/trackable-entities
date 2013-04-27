using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace TrackableEntities.Client
{
    /// <summary>
    /// Base class for model entities to support INotifyPropertyChanged, ITrackable
    /// </summary>
    /// <typeparam name="TModel">Entity type</typeparam>
    [DataContract(IsReference = true)]
    public abstract class ModelBase<TModel> : INotifyPropertyChanged, ITrackable
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
        protected virtual void NotifyPropertyChanged<TResult>
            (Expression<Func<TModel, TResult>> property)
        {
            string propertyName = ((MemberExpression)property.Body).Member.Name;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// State of a trackable entity.
        /// </summary>
        [DataMember]
        public TrackingState TrackingState { get; set; }

        /// <summary>
        /// List of properties on entity that have been modified.
        /// </summary>
        [DataMember]
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
