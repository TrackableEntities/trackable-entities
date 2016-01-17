using System;

namespace TrackableEntities.Extended
{
    /// <summary>
    /// Interface implemented by entities that need extended change-tracking.
    /// </summary>
    interface ITrackableExtended<TPropertyType> : ITrackable
    {
        /// <summary>
        /// Extended change-tracking state of an entity.
        /// </summary>
        TPropertyType TrackgingStateExtended { get; set; }
    }
}
