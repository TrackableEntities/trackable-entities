using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;

namespace TrackableEntities.TemplateWizard
{
    public class EntitiesChildWizard : ChildWizard
    {
        protected override void ProcessEntitiesTemplate(
            Dictionary<string, string> replacementsDictionary, string templateName)
        {
            // Get $entitiestempaltename$ dictionary entry and return if null or not present
            var entitiesTemplate = EntitiesWizard.RootDictionary[Constants.DictionaryEntries.EntitiesTempalteName];
            if (entitiesTemplate == null) return;

            // Get destination directory
            DirectoryInfo directory = null;
            string directoryPath = replacementsDictionary[Constants.DictionaryEntries.DestinationDirectory];
            if (!string.IsNullOrWhiteSpace(directoryPath))
                directory = new DirectoryInfo(directoryPath);

            // If present and not null, back out if does not match current template name
            if (!templateName.Equals(entitiesTemplate, StringComparison.InvariantCultureIgnoreCase))
            {
                // Delete folder and cancel
                if (directory != null) directory.Delete();
                throw new WizardBackoutException();
            }

            // Add $entitiestempaltename$ to replacements dictionary
            replacementsDictionary.Add(Constants.DictionaryEntries.EntitiesTempalteName, entitiesTemplate);

            // Store destination directory info
            string solutionPath = replacementsDictionary[Constants.DictionaryEntries.SolutionDirectory];
            string projectName = replacementsDictionary[Constants.DictionaryEntries.SafeProjectName];
            var destDirectory = new DirectoryInfo(Path.Combine(solutionPath, projectName));
            EntitiesWizard.RootDictionary[Constants.DictionaryEntries.SafeProjectName] = projectName;
            EntitiesWizard.RootDictionary[Constants.DictionaryEntries.OriginalDestinationDirectory] =
                replacementsDictionary[Constants.DictionaryEntries.DestinationDirectory];
            EntitiesWizard.RootDictionary[Constants.DictionaryEntries.DestinationDirectory] = destDirectory.FullName;

            // Set $destinationdirectory$ in replacements dictionary
            replacementsDictionary[Constants.DictionaryEntries.DestinationDirectory] = destDirectory.FullName;
        }

        protected override void ProcessRootTemplate(Dictionary<string, string> replacementsDictionary)
        {
            replacementsDictionary.Add(Constants.DictionaryEntries.SafeRootProjectName,
                EntitiesWizard.RootDictionary[Constants.DictionaryEntries.SafeRootProjectName]);
        }

        protected override void PostProjectFinishedGenerating(Project project)
        {
            // Get directory and project info
            var origDestDirectory = EntitiesWizard.RootDictionary[Constants.DictionaryEntries.OriginalDestinationDirectory];
            var newDestDirectory = EntitiesWizard.RootDictionary[Constants.DictionaryEntries.DestinationDirectory];
            var projectName = EntitiesWizard.RootDictionary[Constants.DictionaryEntries.SafeProjectName];
            
            // Move project up a level
            if (!string.IsNullOrWhiteSpace(origDestDirectory) && 
                !string.IsNullOrWhiteSpace(newDestDirectory) && 
                !string.IsNullOrWhiteSpace(projectName))
            {
                Dte2.Solution.Remove(project);
                Directory.Move(origDestDirectory, newDestDirectory);
                string newDestProject = Path.Combine(newDestDirectory, projectName + ".csproj");
                Dte2.Solution.AddFromFile(newDestProject);
            }
        }
    }
}
