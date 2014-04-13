using System;

namespace TrackableEntities.Client
{
    /// <summary>
    /// TrackableEntities constants.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Change-tracking property names.
        /// </summary>
        public static class TrackingProperties
        {
            /// <summary>TrackingState property name</summary>
            public const string TrackingState = "TrackingState";
            /// <summary>ModifiedProperties property name</summary>
            public const string ModifiedProperties = "ModifiedProperties";
        }

        /// <summary>
        /// Equatable member names.
        /// </summary>
        public static class EquatableMembers
        {
            /// <summary>Equatable method start</summary>
            public const string EquatableMethodStart = "System.IEquatable<";

            /// <summary>Equatable method end</summary>
            public const string EquatableMethodEnd = ">.Equals";

            /// <summary>Entity identifier property</summary>
            public const string EntityIdentifierProperty = "EntityIdentifier";

            /// <summary>Entity identify field</summary>
            public const string EntityIdentifyField = "_entityIdentity";
        }

        /// <summary>
        /// Change-tracking member names.
        /// </summary>
        public static class ChangeTrackingMembers
        {
            /// <summary>Change-tracking property end</summary>
            public const string ChangeTrackingPropEnd = "ChangeTracker";
        }
    }
}
