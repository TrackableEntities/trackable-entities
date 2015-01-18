using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace TrackableEntities.Client
{
    /// <summary>
    /// Base class for model entities to support INotifyPropertyChanged
    /// </summary>
    /// <typeparam name="TModel">Entity type</typeparam>
    [DataContract(IsReference = true)]
    public abstract class ModelBase<TModel> : INotifyPropertyChanged
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
        /// Fire PropertyChanged event.
        /// </summary>
        /// <typeparam name="TResult">Property return type</typeparam>
        /// <typeparam name="TSpecificModel">Specific entity type</typeparam>
        /// <param name="property">Lambda expression for property</param>
        protected virtual void NotifyPropertyChanged<TSpecificModel, TResult>
            (Expression<Func<TSpecificModel, TResult>> property)
        {
            string propertyName = ((MemberExpression)property.Body).Member.Name;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
