using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TemplateWizard;
using TrackableItemTemplateWizard;

namespace TrackableEntities.WcfItemWizard
{
    public class WcfServiceTypeWizard : IWizard
    {
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, 
            WizardRunKind runKind, object[] customParams)
        {
            // Get referenced types
            var dte2 = (DTE2)automationObject;
            List<Type> modelTypes = GetReferencedTypes(dte2).ToList();

            // Prompt user for Entity and DbContext names
            var dialog = new ModelTypesDialog(modelTypes,
                "Trackable WCF Service Type",
                "Add WCF service contract and type with CRUD operations using Trackable Entities.");
            if (dialog.ShowDialog() == DialogResult.Cancel)
                throw new WizardBackoutException();

            string entityNamespace = dialog.ModelTypesInfo.EntityNamespace;
            string entityName = dialog.ModelTypesInfo.EntityName;
            string entitySetName = dialog.ModelTypesInfo.EntitySetName;
            string dbContextName = dialog.ModelTypesInfo.DbContextName;

            replacementsDictionary.Add("$entityNamespace$", entityNamespace);
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

        private IEnumerable<Type> GetReferencedTypes(DTE2 dte2)
        {
            Project activeProject = dte2.ActiveSolutionProjects[0];
            var project = activeProject.Object as VSLangProj.VSProject;
            if (project != null)
            {
                foreach (VSLangProj.Reference reference in project.References)
                {
                    if (reference.SourceProject != null)
                    {
                        Assembly assembly = Assembly.LoadFrom(reference.Path);
                        foreach (Type type in assembly.GetTypes())
                        {
                            yield return type;
                        }
                    }
                }
            }
        }
    }
}
