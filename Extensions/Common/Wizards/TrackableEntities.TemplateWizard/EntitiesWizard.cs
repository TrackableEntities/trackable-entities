using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using EntitiesSelectionWizard;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;

namespace TrackableEntities.TemplateWizard
{
    // Entities wizard is used by entities project vstemplate
    public class EntitiesWizard : IWizard
    {
        // Use to communicate $entitiestempaltename$ to ChildWizard
        public static Dictionary<string, string> RootDictionary =
            new Dictionary<string, string>();

        // Select entities template
        public static void SelectEntitiesTemplate(bool multiproject, string templateName)
        {
            // Disallow shared portable entities for WebApi due to Json.net compat issues:
            // Error: Method not found (Newtonsoft.Json.Serialization.DefaultContractResolver SET_IgnoreSerializableAttribute)
            bool webApiSharedPortable = templateName.Equals(Constants.ProjectTemplates.TrackableWebApi, StringComparison.InvariantCultureIgnoreCase)
                || templateName.Equals(Constants.ProjectTemplates.TrackableWebApiPatterns, StringComparison.InvariantCultureIgnoreCase);

            // Prompt user for entities template
            var dialog = new EntitiesSelectionDialog(multiproject, webApiSharedPortable);
            if (dialog.ShowDialog() == DialogResult.Cancel)
                throw new WizardBackoutException();

            // Place "$entitiestempaltename$ in entities root dictionary
            string entitiesTemplateName = GetEntitiesTemplateName(dialog.EntitiesSelection);
            RootDictionary[Constants.DictionaryEntries.EntitiesTemplateName] = entitiesTemplateName;

            // Place $cliententitiestemplate$ and $serviceentitiestemplate$ in root dictionary
            string[] entitiesTemplateNames = entitiesTemplateName.Split('|');
            RootWizard.RootDictionary[Constants.DictionaryEntries.ClientEntitiesTemplate] = entitiesTemplateNames[0];
            if (entitiesTemplateNames.Length == 2)
                RootWizard.RootDictionary[Constants.DictionaryEntries.ServiceEntitiesTemplate] = entitiesTemplateNames[1];
            else
                RootWizard.RootDictionary[Constants.DictionaryEntries.ServiceEntitiesTemplate] = entitiesTemplateNames[0];
        }

        // Add global replacement parameters
        public void RunStarted(object automationObject, 
            Dictionary<string, string> replacementsDictionary, 
            WizardRunKind runKind, object[] customParams)
        {
            // Select entities template
            SelectEntitiesTemplate(false, string.Empty);

            // Place $parentwizardname$ in root dictionary
            RootWizard.RootDictionary[Constants.DictionaryEntries.ParentWizardName] = 
                Constants.ParentWizards.EntitiesWizard;

            // Place "$saferootprojectname$ in root dictionary.
            RootWizard.RootDictionary[Constants.DictionaryEntries.SafeRootProjectName] = 
                replacementsDictionary[Constants.DictionaryEntries.SafeProjectName];
        }

        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
            // Delete parent of original destination directory
            // NOTE: The directory reappears after install has completed.
            var origDestDirectory = RootDictionary[Constants.DictionaryEntries.OriginalDestinationDirectory];
            var origParentDirectory = new DirectoryInfo(origDestDirectory).Parent;
            if (origParentDirectory != null) Directory.Delete(origParentDirectory.FullName);
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        private static string GetEntitiesTemplateName(EntitiesSelection entitiesSelection)
        {
            if (entitiesSelection == EntitiesSelection.None)
                return null;

            if (entitiesSelection.HasFlag(EntitiesSelection.ClientService))
            {
                var clientSelection = string.Empty;
                if (entitiesSelection.HasFlag(EntitiesSelection.Portable))
                    clientSelection = Constants.EntitiesTemplates.ClientPortable;
                if (entitiesSelection.HasFlag(EntitiesSelection.DotNet45))
                    clientSelection = Constants.EntitiesTemplates.ClientNet45;
                return clientSelection + "|" + Constants.EntitiesTemplates.ServiceNet45;
            }

            if (entitiesSelection.HasFlag(EntitiesSelection.Service))
                return Constants.EntitiesTemplates.ServiceNet45;

            if (entitiesSelection.HasFlag(EntitiesSelection.Client))
            {
                if (entitiesSelection.HasFlag(EntitiesSelection.Portable))
                    return Constants.EntitiesTemplates.ClientPortable;
                if (entitiesSelection.HasFlag(EntitiesSelection.DotNet45))
                    return Constants.EntitiesTemplates.ClientNet45;
            }

            if (entitiesSelection.HasFlag(EntitiesSelection.Shared))
            {
                if (entitiesSelection.HasFlag(EntitiesSelection.Portable))
                    return Constants.EntitiesTemplates.SharedPortable;
                if (entitiesSelection.HasFlag(EntitiesSelection.DotNet45))
                    return Constants.EntitiesTemplates.SharedNet45;
            }
            return null;
        }
    }
}
