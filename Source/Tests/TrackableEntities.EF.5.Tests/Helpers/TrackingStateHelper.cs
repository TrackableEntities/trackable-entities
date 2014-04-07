using System;
using System.Collections;
using System.Collections.Generic;

namespace TrackableEntities.EF.Tests
{
    internal static class TrackingStateHelper
    {
        // Recursively set tracking state
        public static void SetTrackingState(this ITrackable item,
            TrackingState state, ITrackable parent = null)
        {
            foreach (var prop in item.GetType().GetProperties())
            {
                var trackableRef = prop.GetValue(item, null) as ITrackable;
                if (trackableRef != null
                    && (parent == null || trackableRef.GetType() != parent.GetType()))
                {
                    trackableRef.SetTrackingState(state, parent);
                    trackableRef.TrackingState = state;
                }

                var trackingColl = prop.GetValue(item, null) as ICollection;
                if (trackingColl != null)
                {
                    foreach (ITrackable child in trackingColl)
                    {
                        if (parent == null || child.GetType() != parent.GetType())
                        {
                            child.SetTrackingState(state, parent);
                            child.TrackingState = state;
                        }
                    }
                }
            }
        }

        // Recursively get tracking states
        public static IEnumerable<TrackingState> GetTrackingStates
            (this ITrackable item, TrackingState? trackingState = null,
            ITrackable parent = null)
        {
            foreach (var prop in item.GetType().GetProperties())
            {
                var trackableRef = prop.GetValue(item, null) as ITrackable;
                if (trackableRef != null
                    && (parent == null || trackableRef.GetType() != parent.GetType()))
                {
                    foreach (var state in trackableRef.GetTrackingStates(parent: item))
                    {
                        if (trackingState == null || state == trackingState)
                            yield return state;
                    }
                }

                var trackingColl = prop.GetValue(item, null) as ICollection;
                if (trackingColl != null)
                {
                    foreach (ITrackable child in trackingColl)
                    {
                        if (parent == null || child.GetType() != parent.GetType())
                        {
                            foreach (var state in child.GetTrackingStates(parent: item))
                            {
                                if (trackingState == null || state == trackingState)
                                    yield return state;
                            }
                        }
                    }
                }
            }
            yield return item.TrackingState;
        }

        // Recursively get modified properties
        public static IEnumerable<IEnumerable<string>> GetModifiedProperties
            (this ITrackable item, ITrackable parent = null)
        {
            foreach (var prop in item.GetType().GetProperties())
            {
                var trackableRef = prop.GetValue(item, null) as ITrackable;
                if (trackableRef != null
                    && (parent == null || trackableRef.GetType() != parent.GetType()))
                {
                    foreach (var modifiedProps in trackableRef.GetModifiedProperties(item))
                    {
                        yield return modifiedProps;
                    }
                }

                var trackingColl = prop.GetValue(item, null) as ICollection;
                if (trackingColl != null)
                {
                    foreach (ITrackable child in trackingColl)
                    {
                        if (parent == null || child.GetType() != parent.GetType())
                        {
                            foreach (var modifiedProps in child.GetModifiedProperties(item))
                            {
                                yield return modifiedProps;
                            }
                        }
                    }
                }
            }
            yield return item.ModifiedProperties;
        }
    }
}
