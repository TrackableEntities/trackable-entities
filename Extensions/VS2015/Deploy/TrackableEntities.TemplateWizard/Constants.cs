using System;

namespace TrackableEntities.TemplateWizard
{
    internal static class Constants
    {
        public static class ProjectTemplates
        {
            public const string TrackableWebApi = "TrackableWebApi";
            public const string TrackableWcfService = "TrackableWcfService";
            public const string TrackableWebApiPatterns = "TrackableWebApiPatterns";
        }

        public static class ReadMeFiles
        {
            public const string TrackableWcf = "TrackableWcf.ReadMe.txt";
            public const string TrackableWebApi = "TrackableWebApi.ReadMe.txt";
            public const string WebApiPatternsSample = "TrackableWebApi.Patterns.ReadMe.txt";
        }

        public static class ParentWizards
        {
            public const string RootWizard = "RootWizard";
            public const string EntitiesWizard = "EntitiesWizard";
        }

        public static class EntitiesTemplates
        {
            public const string ServiceNet45 = "Entities.Service.Net45";
            public const string ClientPortable = "Entities.Client.Portable";
            public const string ClientNet45 = "Entities.Client.Net45";
            public const string SharedPortable = "Entities.Shared.Portable";
            public const string SharedPortableData = "Entities.Shared.Portable.Data";
            public const string SharedNet45 = "Entities.Shared.Net45";
        }

        public static class DictionaryEntries
        {
            public const string SafeProjectName = "$safeprojectname$";
            public const string SafeRootProjectName = "$saferootprojectname$";
            public const string ParentWizardName = "$parentwizardname$";
            public const string EntitiesTemplateName = "$entitiestemplatename$";
            public const string ClientEntitiesTemplate = "$cliententitiestemplate$";
            public const string ServiceEntitiesTemplate = "$serviceentitiestemplate$";
            public const string SolutionDirectory = "$solutiondirectory$";
            public const string DestinationDirectory = "$destinationdirectory$";
            public const string OriginalDestinationDirectory = "$origdestinationdirectory$";
        }
    }
}
