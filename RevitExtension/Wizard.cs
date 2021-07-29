using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLangProj;

namespace RevitExtension
{
    class Wizard : IWizard
    {
        private static string SafeProjectName { get; set; }
        private static string ProjectsDirectory { get; set; }
        DTE VS = null;

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            VS = automationObject as DTE;
            SafeProjectName = replacementsDictionary["$safeprojectname$"];
            ProjectsDirectory = replacementsDictionary["$destinationdirectory$"];

            //foreach (var entry in replacementsDictionary) Debug.WriteLine($"{entry.Key} - {entry.Value}");
            //replacementsDictionary.Add("$custommessage$", "ThisIsTheCustomParameter");
        }

        // This method is called after the solution is created.
        public void RunFinished()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            Project sharedProj = null;
            Project resourceProj = null;
            List<Project> versionProjs = new List<Project>();
            foreach (Project project in VS.Solution.Projects)
            {
                if (project.Name.Contains("Resources")) { resourceProj = project; }
                else if (project.Name == SafeProjectName) { sharedProj = project; }
                else { versionProjs.Add(project); }
            }

            foreach (Project version in versionProjs)
            {
                // Add reference to shared project
                VSProject vsProj = (VSProject)version.Object;
                if (sharedProj != null)
                {
                    Debug.WriteLine(sharedProj.FileName);
                    Debug.WriteLine(sharedProj.UniqueName);
                    Debug.WriteLine(sharedProj.Kind);

                    var codeModel = sharedProj.CodeModel;
                    var mngr = sharedProj.UniqueName;

                    // !!! Shared project should be added as a reference here, but it is causing a critical error. !!!
                    //BuildDependency bldDepends = VS.Solution.SolutionBuild.BuildDependencies.Item(version.UniqueName);
                    //bldDepends.AddProject(sharedProj.FileName);

                    //vsProj.References.Add(sharedProj.FileName);
                    //vsProj.References.AddProject(sharedProj);
                }

                // Add reference to the Resources project
                if (resourceProj != null) { vsProj.References.AddProject(resourceProj); }

                // Add a manifest file
                string manifestFile = generateAppManifest(
                    name: SafeProjectName,
                    assembly: version.Name,
                    addInId: Guid.NewGuid().ToString(),
                    fullClassName: $"{version.Name}.App",
                    vendorId: "BIMDev",
                    vendorDescription: "www.thebimdev.com",
                    manifestFilePath: $"{ProjectsDirectory}\\{version.Name}\\{SafeProjectName}.addin"
                    );
                ProjectItem manifest = version.ProjectItems.AddFromFile(manifestFile);
                manifest.Properties.Item("CopyToOutputDirectory").Value = 1;

                // Define post-build commands
                string versionYear = version.Name.Substring(version.Name.Length - 4, 4);
                string postBuild =
                    $"if exist \"C:\\ProgramData\\Autodesk\\Revit\\Addins\\{versionYear}\" copy \"$(ProjectDir)bin\\debug\\*.addin\" \"C:\\ProgramData\\Autodesk\\Revit\\Addins\\{versionYear}\"\r\n" +
                    $"if exist \"C:\\ProgramData\\Autodesk\\Revit\\Addins\\{versionYear}\" copy \"$(ProjectDir)bin\\debug\\*.dll\" \"C:\\ProgramData\\Autodesk\\Revit\\Addins\\{versionYear}\"";
                version.Properties.Item("PostBuildEvent").Value = postBuild;

                // Point the Debug Start Action at the correct Revit exe
                Configuration config = version.ConfigurationManager.ActiveConfiguration; 
                config.Properties.Item("StartAction").Value = VSLangProj.prjStartAction.prjStartActionProgram;
                config.Properties.Item("StartProgram").Value = $"C:\\Program Files\\Autodesk\\Revit {versionYear}\\Revit.exe";

            }

            // Collapse the Solution Explorer
            UIHierarchy solutionExplorer = VS.Windows.Item(Constants.vsWindowKindSolutionExplorer).Object as UIHierarchy;
            UIHierarchyItem rootNode = solutionExplorer.UIHierarchyItems.Item(1);
            collapseSolutionExplorer(rootNode);

            // Activate the initial App class
        }

        public void ProjectFinishedGenerating(Project project)
        {
            Debug.WriteLine("\r\n----------------------------------------ProjectFinishedGenerating---------------------------------------------------\r\n\r\n");
        }

        // This method is only called for item templates, not for project templates.
        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {

        }
        // This method is only called for item templates, not for project templates.
        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
        // This method is called before opening any item that has the OpenInEditor attribute.
        public void BeforeOpeningFile(ProjectItem projectItem)
        {

        }


        private void collapseSolutionExplorer(UIHierarchyItem node)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            foreach (UIHierarchyItem child in node.UIHierarchyItems)
            {
                collapseSolutionExplorer(child);
            }
            node.UIHierarchyItems.Expanded = false;
        }

        private string generateAppManifest(string name, string assembly, string addInId, string fullClassName, string vendorId, string vendorDescription, string manifestFilePath)
        {
            string content =
            $"<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?>" +
            $"\r\n<RevitAddIns>" +
            $"\r\n   <AddIn Type = \"Application\">" +
            $"\r\n       <Name>{name}</Name>" +
            $"\r\n       <Assembly>{assembly}.dll</Assembly>" +
            $"\r\n       <AddInId>{addInId}</AddInId>" +
            $"\r\n       <FullClassName>{fullClassName}</FullClassName>" +
            $"\r\n       <VendorId>{vendorId}</VendorId>" +
            $"\r\n       <VendorDescription>{vendorDescription}</VendorDescription>" +
            $"\r\n   </AddIn> " +
            $"\r\n</RevitAddIns>";

            File.WriteAllText(manifestFilePath, content);
            return manifestFilePath;
        }
    }
}
