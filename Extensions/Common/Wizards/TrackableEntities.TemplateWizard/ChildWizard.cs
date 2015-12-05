using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using NuGet.VisualStudio;

namespace TrackableEntities.TemplateWizard
{
    public class ChildWizard : IWizard
    {
        [Import]
        internal IVsTemplateWizard Wizard { get; set; }

        protected DTE2 Dte2;

        // Retrieve global replacement parameters
        public void RunStarted(object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            // Add custom parameters
            replacementsDictionary.Add(Constants.DictionaryEntries.SafeRootProjectName,
                RootWizard.RootDictionary[Constants.DictionaryEntries.SafeRootProjectName]);
            replacementsDictionary.Add(Constants.DictionaryEntries.ClientEntitiesTemplate,
                RootWizard.RootDictionary[Constants.DictionaryEntries.ClientEntitiesTemplate]);
            replacementsDictionary.Add(Constants.DictionaryEntries.ServiceEntitiesTemplate,
                RootWizard.RootDictionary[Constants.DictionaryEntries.ServiceEntitiesTemplate]);

            // Process entities template
            var templateName = Path.GetFileNameWithoutExtension((string)customParams[0]);
            ProcessEntitiesTemplate(replacementsDictionary, templateName);

            // Get DTE
            Dte2 = (DTE2)automationObject;

            // Init NuGet Wizard
            Initialize(automationObject);
            Wizard.RunStarted(automationObject, replacementsDictionary, runKind, customParams);
        }

        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
            MoveGeneratedProject(ref project);
            Wizard.ProjectFinishedGenerating(project);
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

        protected virtual void ProcessEntitiesTemplate(
            Dictionary<string, string> replacementsDictionary, string templateName)
        {            
        }

        protected virtual void MoveGeneratedProject(ref Project project)
        {
        }

        private void Initialize(object automationObject)
        {
            using (var provider = new ServiceProvider((IServiceProvider)automationObject))
            {
                var service = (IComponentModel)provider.GetService(typeof(SComponentModel));
                using (var container = new CompositionContainer(service.DefaultExportProvider))
                {
                    container.ComposeParts(this);
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
