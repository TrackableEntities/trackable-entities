using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using ItemTemplateParametersWizard;
using Microsoft.VisualStudio.TemplateWizard;

namespace TrackableEntities.ItemWizard
{
    internal static class WizardHelper
    {
        public static void AddCustomParameters(object automationObject,
            Dictionary<string, string> replacementsDictionary, 
            string dialogTitle, string dialogMessage, bool getDbContextName)
        {
            // Get referenced types
            var dte2 = (DTE2)automationObject;
            List<ModelTypeInfo> modelTypes = GetReferencedTypes(dte2).ToList();
            const string noEntitiesMessage = "Referenced projects do not contain any {0}." +
                    "\r\nAdd service entities by reverse engineering Code First classes from an existing database, then re-build the solution.";

            // Check for trackable entities in referenced projects
            if (modelTypes.Count(t => t.ModelType == ModelType.Trackable) == 0)
            {
                MessageBox.Show(string.Format(noEntitiesMessage, "Trackable model classes"),
                    "Trackable Entities Not Found");
                throw new WizardBackoutException();
            }

            // Check for DbContext types in referenced projects
            if (getDbContextName && modelTypes.Count(t => t.ModelType == ModelType.DbContext) == 0)
            {
                MessageBox.Show(string.Format(noEntitiesMessage, "DbContext classes"),
                    "DbContext Class Not Found");
                throw new WizardBackoutException();
            }

            // Prompt user for Entity and DbContext names
            Form dialog;
            if (getDbContextName)
                dialog = new ModelTypesContextDialog(modelTypes, dialogTitle, dialogMessage); 
            else
                dialog = new ModelTypesDialog(modelTypes, dialogTitle, dialogMessage);
            if (dialog.ShowDialog() == DialogResult.Cancel)
                throw new WizardBackoutException();

            var modelTypesDialog = ((IModelTypes) dialog);
            string baseNamespace = modelTypesDialog.ModelTypesDialogInfo.BaseNamespace;
            string entityName = modelTypesDialog.ModelTypesDialogInfo.EntityName;
            string entitySetName = modelTypesDialog.ModelTypesDialogInfo.EntitySetName;
            string dbContextName = modelTypesDialog.ModelTypesDialogInfo.DbContextName;

            replacementsDictionary.Add("$baseNamespace$", baseNamespace);
            replacementsDictionary.Add("$entityName$", entityName);
            replacementsDictionary.Add("$entitySetName$", entitySetName);
            replacementsDictionary.Add("$dbContextName$", dbContextName);
        }

        private static IEnumerable<ModelTypeInfo> GetReferencedTypes(DTE2 dte2)
        {
            Project activeProject = dte2.ActiveSolutionProjects[0];
            var project = activeProject.Object as VSLangProj.VSProject;
            if (project != null)
            {
                foreach (VSLangProj.Reference reference in project.References)
                {
                    if (reference.SourceProject != null)
                    {
                        if (!File.Exists(reference.Path))
                        {
                            continue;
                        }

                        // Get type info from assembly
                        List<ModelTypeInfo> modelTypes = ModelReflectionHelper.GetModelTypes
                            (new FileInfo(reference.Path));
                        foreach (var modelType in modelTypes)
                        {
                            yield return modelType;
                        }
                    }
                }
            }
        }

        private static string CopyFileToTemp(string sourcePath, ref string destDir)
        {
            if (!File.Exists(sourcePath)) return null;
            string destFileName = Path.GetFileName(sourcePath);
            if (string.IsNullOrWhiteSpace(destDir))
                destDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string destPath = Path.Combine(destDir, destFileName);
            Directory.CreateDirectory(destDir);
            File.Copy(sourcePath, destPath);
            return destPath;
        }

        private static void DeleteFilesFromTemp(string destDir)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(destDir) && Directory.Exists(destDir))
                {
                    Directory.Delete(destDir);
                }
            }
            catch (Exception) { } // Just go on if there's an error
        }
    }
}
