using System;

namespace TrackableEntities.ItemWizard
{
    public static class Dialogs
    {
        public static class WcfServiceType
        {
            public const string Title = "Trackable WCF Service Type";
            public const string Message = "Add WCF Service Type with read-write operations using Trackable Entities.";
            public const bool GetDbContextName = true;
            public const int DialogWidth = 650;
        }

        public static class EntityController
        {
            public const string Title = "Entity API Controller";
            public const string Message = "Add an Entity API Controller to the Trackable WebApi project.";
            public const bool GetDbContextName = false;
            public const int DialogWidth = 550;
        }

        public static class EntityRepoClass
        {
            public const string Title = "Entity Repository Class";
            public const string Message = "Add an Entity Repository Class to the Trackable Service.EF project.";
            public const bool GetDbContextName = false;
            public const int DialogWidth = 600;
        }

        public static class EntityRepoInterface
        {
            public const string Title = "Entity Repository Interface";
            public const string Message = "Add an Entity Repository Interface to the Service.Persistence project.";
            public const bool GetDbContextName = false;
            public const int DialogWidth = 650;
        }
    }
}
