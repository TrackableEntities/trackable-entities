using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using EntitiesSelectionWizard;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;

namespace TrackableEntities.TemplateWizard
{
    // Entities wizard is used by root project vstemplate
    public class EntitiesWizard : IWizard
    {
        // Use to communicate $entitiestempaltename$ to ChildWizard
        public static Dictionary<string, string> RootDictionary =
            new Dictionary<string, string>();

        // Add global replacement parameters
        public void RunStarted(object automationObject, 
            Dictionary<string, string> replacementsDictionary, 
            WizardRunKind runKind, object[] customParams)
        {
            // Prompt user for entities template
            var dialog = new EntitiesSelectionDialog();
            if (dialog.ShowDialog() == DialogResult.Cancel)
                throw new WizardBackoutException();
            var entitiesTemplateName = GetEntitiesTemplateName(dialog.EntitiesSelection);

            // Place "$saferootprojectname$ in the global dictionary.
            RootDictionary[Constants.DictionaryEntries.SafeRootProjectName] = 
                replacementsDictionary[Constants.DictionaryEntries.SafeProjectName];

            // Place "$entitiestempaltename$ in the global dictionary.
            RootDictionary[Constants.DictionaryEntries.EntitiesTempalteName] = entitiesTemplateName;            
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

        private string GetEntitiesTemplateName(EntitiesSelection entitiesSelection)
        {
            if (entitiesSelection == EntitiesSelection.None)
                return null;

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
