using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackableEntities.Client
{
    /// <summary>
    /// Supplies data about the object that raised the event.
    /// </summary>
    public class EntityChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EntityChangedEventArgs"/>.    /// </summary>
        /// <param name="entity"></param>
        /// <param name="parent"></param>
        /// <param name="propertyName"></param>
        public EntityChangedEventArgs(ITrackable entity, ITrackable parent, string propertyName)
        {
            Entity = entity;
            Parent = parent;
            PropertyName = propertyName;
        }

        /// <summary>
        /// To be added.
        /// </summary>
        public ITrackable Entity { get;  }
        /// <summary>
        /// To be added.
        /// </summary>
        public ITrackable Parent { get; }
        /// <summary>
        /// To be added.
        /// </summary>
        public string PropertyName { get; }
    }
}
