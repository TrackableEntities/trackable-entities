using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;

namespace TrackableEntities.WcfItemWizard
{
    public class WcfServiceTypeWizard : IWizard
    {
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, 
            WizardRunKind runKind, object[] customParams)
        {
            // Prompt user for Entity and DbContext names
            var dialog = new GetParamatersDialog();
            if (dialog.ShowDialog() == DialogResult.Cancel)
                throw new WizardBackoutException();

            string rootNamespace = dialog.RootNamespace;
            string entityName = dialog.EntityName;
            string entitySetName = dialog.EntitySetName;
            string dbContextName = dialog.DbContextName;

            replacementsDictionary.Add("$rootNamespace$", rootNamespace);
            replacementsDictionary.Add("$entityName$", entityName);
            replacementsDictionary.Add("$entitySetName$", entitySetName);
            replacementsDictionary.Add("$dbContextName$", dbContextName);
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
