using System;

namespace TrackableEntities.ItemWizard
{
    public static class Dialogs
    {
        public static class WcfServiceType
        {
            public const string Title = "Trackable WCF Service Type";
            public const string Message = "Add a WCF Service Type with read-write operations using Trackable Entities.";
            public const bool GetDbContextName = true;
            public const int DialogWidth = 650;
        }

        public static class WebApiController
        {
            public const string Title = "Entity Web API Controller";
            public const string Message = "Add an Entity Web API Controller to the WebApi project.";
            public const bool GetDbContextName = true;
            public const int DialogWidth = 550;
        }

        public static class EntityController
        {
            public const string Title = "Entity Web API Controller";
            public const string Message = "Add an Entity Web API Controller to the Patterns WebApi project.";
            public const bool GetDbContextName = false;
            public const int DialogWidth = 550;
        }

        public static class EntityRepoClass
        {
            public const string Title = "Entity Repository Class";
            public const string Message = "Add an Entity Repository Class to the Patterns EF project.";
            public const bool GetDbContextName = false;
            public const int DialogWidth = 600;
        }

        public static class EntityRepoInterface
        {
            public const string Title = "Entity Repository Interface";
            public const string Message = "Add an Entity Repository Interface to the Patterns Persistence project.";
            public const bool GetDbContextName = false;
            public const int DialogWidth = 650;
        }
    }
}
