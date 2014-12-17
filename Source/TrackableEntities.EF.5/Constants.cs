using System;

#if EF_6
namespace TrackableEntities.EF6
#else
namespace TrackableEntities.EF5
#endif
{
    internal static class Constants
    {
        public static class ExceptionMessages
        {
            public const string DeletedWithAddedChildren =
                "An entity may not be marked as Deleted if it has related entities which are marked as Added. " +
                "Remove added related entities before deleting a parent entity.";
        }
    }
}
