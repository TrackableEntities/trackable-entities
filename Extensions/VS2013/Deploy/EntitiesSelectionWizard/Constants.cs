using System;

namespace EntitiesSelectionWizard
{
    internal static class Constants
    {
        public static class Descriptions
        {
            public const string ClientEntities = "Trackable Client Entities:\r\n" +
            "Adds a class library project with code generation templates for reverse engineering trackable client entities from a database.";

            public const string ServiceEntities = "Trackable Service Entities:\r\n" +
            "Adds a class library project with code generation templates for reverse engineering trackable service entities from a database.";

            public const string SharedEntities = "Trackable Shared Entities:\r\n" +
                "Adds a class library project with code generation templates for reverse engineering trackable shared entities from a database.";

            public const string PortableEntities = "{0} Portable Entities:\r\n" +
                "Adds a portable class library project that is compatible with .Net 4.5 or greater, Windows Store 8, Windows Phone 8 or 8.1, iOS and Android.";

            public const string DotNetEntities = "{0} .Net 4.5 Entities:\r\n" +
                "Adds a .Net 4.5 class library project that is compatible with .Net 4.5 or greater.";
        }
    }
}
