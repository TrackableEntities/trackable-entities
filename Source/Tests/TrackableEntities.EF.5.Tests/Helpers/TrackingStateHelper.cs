using System;
using System.Collections;
using System.Collections.Generic;
using TrackableEntities.Common;

namespace TrackableEntities.EF.Tests
{
    internal static class TrackingStateHelper
    {
        // Recursively set tracking state
        public static void SetTrackingState(this ITrackable item,
            TrackingState state, ObjectVisitationHelper visitationHelper = null)
        {
            // Prevent endless recursion
            ObjectVisitationHelper.EnsureCreated(ref visitationHelper);
            if (visitationHelper.IsVisited(item)) return;
            visitationHelper = visitationHelper.With(item);

            foreach (var prop in item.GetType().GetProperties())
            {
                var trackableRef = prop.GetValue(item, null) as ITrackable;
                if (trackableRef != null)
                {
                    trackableRef.SetTrackingState(state, visitationHelper);
                    trackableRef.TrackingState = state;
                }

                var trackingColl = prop.GetValue(item, null) as ICollection;
                if (trackingColl != null)
                {
                    foreach (ITrackable child in trackingColl)
                    {
                        child.SetTrackingState(state, visitationHelper);
                        child.TrackingState = state;
                    }
                }
            }
        }

        // Recursively get tracking states
        public static IEnumerable<TrackingState> GetTrackingStates
            (this ITrackable item, TrackingState? trackingState = null,
            ObjectVisitationHelper visitationHelper = null)
        {
            // Prevent endless recursion
            ObjectVisitationHelper.EnsureCreated(ref visitationHelper);
            if (visitationHelper.IsVisited(item)) yield break;
            visitationHelper = visitationHelper.With(item);

            foreach (var prop in item.GetType().GetProperties())
            {
                var trackableRef = prop.GetValue(item, null) as ITrackable;
                if (trackableRef != null)
                {
                    foreach (var state in trackableRef.GetTrackingStates(visitationHelper: visitationHelper))
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
                        foreach (var state in child.GetTrackingStates(visitationHelper: visitationHelper))
                        {
                            if (trackingState == null || state == trackingState)
                                yield return state;
                        }
                    }
                }
            }
            yield return item.TrackingState;
        }

        // Recursively get modified properties
        public static IEnumerable<IEnumerable<string>> GetModifiedProperties
            (this ITrackable item, ObjectVisitationHelper visitationHelper = null)
        {
            // Prevent endless recursion
            ObjectVisitationHelper.EnsureCreated(ref visitationHelper);
            if (visitationHelper.IsVisited(item)) yield break;
            visitationHelper = visitationHelper.With(item);

            foreach (var prop in item.GetType().GetProperties())
            {
                var trackableRef = prop.GetValue(item, null) as ITrackable;
                if (trackableRef != null)
                {
                    foreach (var modifiedProps in trackableRef.GetModifiedProperties(visitationHelper))
                    {
                        yield return modifiedProps;
                    }
                }

                var trackingColl = prop.GetValue(item, null) as ICollection;
                if (trackingColl != null)
                {
                    foreach (ITrackable child in trackingColl)
                    {
                        foreach (var modifiedProps in child.GetModifiedProperties(visitationHelper))
                        {
                            yield return modifiedProps;
                        }
                    }
                }
            }
            yield return item.ModifiedProperties;
        }
    }
}
