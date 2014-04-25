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
            else if (_templateName == Constants.ProjectTemplates.TrackableWebApi)
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

        private string GetDestReadMePath(Solution2 solution, string templateName)
        {
            var solutionPath = solution.Properties.Item("Path").Value as string;
            if (string.IsNullOrWhiteSpace(solutionPath)) return null;
            string solutionDir = new FileInfo(solutionPath).DirectoryName;
            if (string.IsNullOrWhiteSpace(solutionDir)) return null;
            string readMeFileName = GetReadMeFileName(templateName);
            string destReadMePath = Path.Combine(solutionDir, readMeFileName);
            return destReadMePath;
        }

        private string GetReadMeFileName(string templateName)
        {
            switch (templateName)
            {
                case Constants.ProjectTemplates.TrackableWcfService:
                    return Constants.ReadMeFiles.TrackableWcf;
                case Constants.ProjectTemplates.TrackableWebApi:
                    return Constants.ReadMeFiles.TrackableWebApi;
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
            string parentPath = Directory.GetParent(dirPath).FullName;
            string filePath = Path.Combine(parentPath, fileName);
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

    }
}
