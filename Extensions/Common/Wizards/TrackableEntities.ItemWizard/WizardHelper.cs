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
            List<Type> modelTypes = GetReferencedTypes(dte2).ToList();

            // Check for trackable entities in referenced projects
            if (FilterModelTypes(modelTypes, typeof(ITrackable)).Count == 0)
            {
                MessageBox.Show("Referenced projects do not contain any trackable entities." +
                    "\r\nAdd service entities using EF Power Tools then build the solution.",
                    "Trackable Entities Not Found");
                throw new WizardBackoutException();
            }

            // Check for DbContext types in referenced projects
            if (getDbContextName && FilterModelTypes(modelTypes, typeof(DbContext)).Count == 0)
            {
                MessageBox.Show("Referenced projects do not contain any DbContext classes." +
                    "\r\nAdd service entities using EF Power Tools then build the solution.",
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
            string baseNamespace = modelTypesDialog.ModelTypesInfo.BaseNamespace;
            string entityName = modelTypesDialog.ModelTypesInfo.EntityName;
            string entitySetName = modelTypesDialog.ModelTypesInfo.EntitySetName;
            string dbContextName = modelTypesDialog.ModelTypesInfo.DbContextName;

            replacementsDictionary.Add("$baseNamespace$", baseNamespace);
            replacementsDictionary.Add("$entityName$", entityName);
            replacementsDictionary.Add("$entitySetName$", entitySetName);
            replacementsDictionary.Add("$dbContextName$", dbContextName);
        }

        private static List<Type> FilterModelTypes
            (IEnumerable<Type> modelTypes, Type canAssignTo)
        {
            var trackableTypes = from t in modelTypes
                                 where canAssignTo.IsAssignableFrom(t)
                                 select t;
            return trackableTypes.ToList();
        }

        private static IEnumerable<Type> GetReferencedTypes(DTE2 dte2)
        {
            const string efFileName = "EntityFramework.dll";
            const string comFileName = "TrackableEntities.Common.dll";
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
                        string destDir = null;
                        string entDestPath = CopyFileToTemp(reference.Path, ref destDir);
                        string sourceDir = Path.GetDirectoryName(reference.Path);
                        string efDestPath = CopyFileToTemp(Path.Combine(sourceDir, efFileName), ref destDir);
                        string comDestPath = CopyFileToTemp(Path.Combine(sourceDir, comFileName), ref destDir);

                        Assembly assembly;
                        try
                        {
                            assembly = Assembly.LoadFrom(entDestPath);
                        }
                        catch (FileNotFoundException)
                        {
                            continue;
                        }

                        // Get types
                        Type[] types;
                        try
                        {
                            types = assembly.GetTypes();
                        }
                        catch (ReflectionTypeLoadException)
                        {
                            continue;
                        }

                        // Return types
                        foreach (Type type in types)
                        {
                            yield return type;
                        }

                        // Clean up
                        DeleteFileFromTemp(efDestPath);
                        DeleteFileFromTemp(comDestPath);
                        DeleteFileFromTemp(entDestPath);
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

        private static void DeleteFileFromTemp(string destPath)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(destPath) && File.Exists(destPath))
                {
                    File.SetAttributes(destPath, FileAttributes.Normal);
                    File.Delete(destPath);
                }
            }
            catch (Exception) { } // Just go on if there's an error
        }
    }
}
