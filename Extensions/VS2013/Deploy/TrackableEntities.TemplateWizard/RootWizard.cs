using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TemplateWizard;

// This assembly must be signed and either installed in the GAC
// or included as Content in a VSIX project.
// Then it is referenced from vstemplates.
// Custom replacement can be used in projects and files.

namespace TrackableEntities.TemplateWizard
{
    // Root wizard is used by root project vstemplate
    public class RootWizard : IWizard
    {
        // Use to communicate $saferootprojectname$ to ChildWizard
        public static Dictionary<string, string> GlobalDictionary =
            new Dictionary<string, string>();

        // Fields
        private DTE2 _dte2;
        private string _templateName;

        // Add global replacement parameters
        public void RunStarted(object automationObject, 
            Dictionary<string, string> replacementsDictionary, 
            WizardRunKind runKind, object[] customParams)
        {
            // Place "$saferootprojectname$ in the global dictionary.
            // Copy from $safeprojectname$ passed in my root vstemplate
            GlobalDictionary["$saferootprojectname$"] = replacementsDictionary["$safeprojectname$"];
            
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
            string readMePath = GetTemplateReadMe(_templateName);
            if (File.Exists(readMePath))
                _dte2.ItemOperations.AddExistingItem(readMePath);

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

        private string GetTextFilePath(string fileName)
        {
            string dirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePath = Path.Combine(dirPath, fileName);
            return filePath;
        }

        private Project GetProject(string extension)
        {
            string projectName = GlobalDictionary["$saferootprojectname$"] + extension;
            foreach (Project project in _dte2.Solution.Projects)
            {
                if (Path.GetFileNameWithoutExtension(project.FullName).Equals(projectName))
                {
                    return project;
                }
            }
            return null;
        }

        private string GetTemplateReadMe(string templateName)
        {
            switch (templateName)
            {
                case Constants.ProjectTemplates.TrackableWcfService:
                    return GetTextFilePath(Constants.ReadMeFiles.TrackableWcf);
                case Constants.ProjectTemplates.TrackableWebApi:
                    return GetTextFilePath(Constants.ReadMeFiles.TrackableWebApi);
                case Constants.ProjectTemplates.TrackableWebApiPatterns:
                    return GetTextFilePath(Constants.ReadMeFiles.WebApiPatternsSample);
                default:
                    return null;
            }
        }
    }
}
