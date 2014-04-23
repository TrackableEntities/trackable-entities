using System;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;

namespace TrackableEntities.ItemWizard
{
    public class EntityRepoInterfaceWizard : IWizard
    {
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, 
            WizardRunKind runKind, object[] customParams)
        {
            // Add custom parameters
            WizardHelper.AddCustomParameters(automationObject, replacementsDictionary,
                Dialogs.EntityRepoInterface.Title,
                Dialogs.EntityRepoInterface.Message,
                Dialogs.EntityRepoInterface.GetDbContextName,
                Dialogs.EntityRepoInterface.DialogWidth);
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
