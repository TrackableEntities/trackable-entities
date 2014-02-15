using System.Collections.Generic;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;

namespace TrackableEntities.PatternItemsWizard
{
    public class EntityRepoClassWizard : IWizard
    {
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, 
            WizardRunKind runKind, object[] customParams)
        {
            // Prompt user for Entity and DbContext names
            var dialog = new GetParamatersDialog("Entity Repository Class",
                "Add Entity Repository Class to the Trackable Service.EF project.");
            if (dialog.ShowDialog() == DialogResult.Cancel)
                throw new WizardBackoutException();

            string entitiesNamespace = dialog.EntitiesNamespace;
            string entityName = dialog.EntityName;
            string entitySetName = dialog.EntitySetName;

            replacementsDictionary.Add("$entitiesNamespace$", entitiesNamespace);
            replacementsDictionary.Add("$entityName$", entityName);
            replacementsDictionary.Add("$entitySetName$", entitySetName);
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
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
