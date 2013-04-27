using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using NuGet.VisualStudio;

namespace TrackableTemplateWizard
{
    public class ChildWizard : IWizard
    {
        [Import]
        internal IVsTemplateWizard Wizard { get; set; }

        // Retrieve global replacement parameters
        public void RunStarted(object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            // Add custom parameters.
            replacementsDictionary.Add("$saferootprojectname$",
                RootWizard.GlobalDictionary["$saferootprojectname$"]);

            // Init NuGet Wizard
            Initialize(automationObject);
            Wizard.RunStarted(automationObject, replacementsDictionary, runKind, customParams);
        
        }
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
            Wizard.BeforeOpeningFile(projectItem);
        }

        public void ProjectFinishedGenerating(Project project)
        {
            Wizard.ProjectFinishedGenerating(project);
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            Wizard.ProjectItemFinishedGenerating(projectItem);
        }

        public void RunFinished()
        {
            Wizard.RunFinished();
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return Wizard.ShouldAddProjectItem(filePath);
        }

        private void Initialize(object automationObject)
        {
            using (var provider = new ServiceProvider((IServiceProvider)automationObject))
            {
                var service = (IComponentModel)provider.GetService(typeof(SComponentModel));
                using (var container = new CompositionContainer(new[] { service.DefaultExportProvider }))
                {
                    container.ComposeParts(new object[] { this });
                }
            }
            if (Wizard == null)
            {
                MessageBox.Show("NuGet Package Manager not available.");
                throw new WizardBackoutException("NuGet Package Manager not available.");
            }
        }
    }
}
