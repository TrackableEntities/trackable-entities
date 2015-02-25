using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TemplateWizard;

namespace TrackableEntities.TemplateWizard
{
    // Root wizard is used by root project vstemplate
    public class RootWizard : IWizard
    {
        // Use to communicate $saferootprojectname$ to ChildWizard
        public static Dictionary<string, string> RootDictionary =
            new Dictionary<string, string>();

        // Fields
        private DTE2 _dte2;
        private string _templateName;

        // Add global replacement parameters
        public void RunStarted(object automationObject, 
            Dictionary<string, string> replacementsDictionary, 
            WizardRunKind runKind, object[] customParams)
        {
            // Select entities template
            EntitiesWizard.SelectEntitiesTemplate(true);

            // Place $parentwizardname$ in root dictionary
            RootDictionary[Constants.DictionaryEntries.ParentWizardName] = 
                Constants.ParentWizards.RootWizard;

            // Place "$saferootprojectname$ in the global dictionary.
            RootDictionary[Constants.DictionaryEntries.SafeRootProjectName] = 
                replacementsDictionary[Constants.DictionaryEntries.SafeProjectName];

            // Get template name
            _templateName = Path.GetFileNameWithoutExtension((string)customParams[0]);

            // Get DTE
            _dte2 = (DTE2)automationObject;
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
            // Add ReadMe file to solution folder
            var solution = (Solution2)_dte2.Solution;
            solution.AddSolutionFolder("Solution Items");

            // Copy readme file to solution directory and add to solution folder
            string sourceReadMePath = GetTemplateReadMePath(_templateName);
            string destReadMePath = GetDestReadMePath(solution, _templateName);
            File.Copy(sourceReadMePath, destReadMePath);
            if (File.Exists(destReadMePath))
                _dte2.ItemOperations.AddExistingItem(destReadMePath);

            // Set startup project
            string extension = null;
            if (_templateName == Constants.ProjectTemplates.TrackableWcfService)
                extension = ".Service.Web";
            else if (_templateName == Constants.ProjectTemplates.TrackableWebApi ||
                _templateName == Constants.ProjectTemplates.TrackableWebApiPatterns)
                extension = ".WebApi";
            if (extension != null)
            {
                Project startupProject = GetProject(extension);
                if (startupProject != null)
                {
                    _dte2.Solution.SolutionBuild.StartupProjects = startupProject.UniqueName;
                } 
            }
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        private Project GetProject(string extension)
        {
            string projectName = RootDictionary["$saferootprojectname$"] + extension;
            foreach (Project project in _dte2.Solution.Projects)
            {
                if (Path.GetFileNameWithoutExtension(project.FullName).Equals(projectName))
                {
                    return project;
                }
            }
            return null;
        }

        private string GetDestReadMePath(Solution2 solution, string templateName)
        {
            var solutionPath = solution.Properties.Item("Path").Value as string;
            if (string.IsNullOrWhiteSpace(solutionPath)) return null;
            string solutionDir = new FileInfo(solutionPath).DirectoryName;
            if (string.IsNullOrWhiteSpace(solutionDir)) return null;
            string readMeFileName = GetReadMeFileName(templateName);
            return Path.Combine(solutionDir, readMeFileName);
        }

        private string GetReadMeFileName(string templateName)
        {
            switch (templateName)
            {
                case Constants.ProjectTemplates.TrackableWcfService:
                    return Constants.ReadMeFiles.TrackableWcf;
                case Constants.ProjectTemplates.TrackableWebApi:
                    return Constants.ReadMeFiles.TrackableWebApi;
                case Constants.ProjectTemplates.TrackableWebApiPatterns:
                    return Constants.ReadMeFiles.WebApiPatternsSample;
                default:
                    return null;
            }
        }

        private string GetTemplateReadMePath(string templateName)
        {
            string readMeFileName = GetReadMeFileName(templateName);
            if (readMeFileName == null) return null;
            string templateReadMePath = GetTemplateFilePath(readMeFileName);
            return templateReadMePath;
        }

        private string GetTemplateFilePath(string fileName)
        {
            string dirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(dirPath, fileName);
        }
    }
}
