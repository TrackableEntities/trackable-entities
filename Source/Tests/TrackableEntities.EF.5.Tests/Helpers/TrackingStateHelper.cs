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
            if (!visitationHelper.TryVisit(item)) return;

            foreach (var navProp in item.GetNavigationProperties())
            {
                foreach (var refProp in navProp.AsReferenceProperty())
                {
                    refProp.EntityReference.SetTrackingState(state, visitationHelper);
                    refProp.EntityReference.TrackingState = state;
                }

                foreach (var colProp in navProp.AsCollectionProperty())
                {
                    foreach (ITrackable child in colProp.EntityCollection)
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
            if (!visitationHelper.TryVisit(item)) yield break;

            foreach (var navProp in item.GetNavigationProperties())
            {
                foreach (var refProp in navProp.AsReferenceProperty())
                {
                    foreach (var state in refProp.EntityReference.GetTrackingStates(visitationHelper: visitationHelper))
                    {
                        if (trackingState == null || state == trackingState)
                            yield return state;
                    }
                }

                foreach (var colProp in navProp.AsCollectionProperty())
                {
                    foreach (ITrackable child in colProp.EntityCollection)
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
            if (!visitationHelper.TryVisit(item)) yield break;

            foreach (var navProp in item.GetNavigationProperties())
            {
                foreach (var refProp in navProp.AsReferenceProperty())
                {
                    foreach (var modifiedProps in refProp.EntityReference.GetModifiedProperties(visitationHelper))
                    {
                        yield return modifiedProps;
                    }
                }

                foreach (var colProp in navProp.AsCollectionProperty())
                {
                    foreach (ITrackable child in colProp.EntityCollection)
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
