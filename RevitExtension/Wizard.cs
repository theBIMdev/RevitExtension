using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitExtension
{
    class Wizard : IWizard
    {
        DTE VS = null;
        IEnumerable<string> _packages;

        // This method is called before opening any item that has the OpenInEditor attribute.
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
            Debug.WriteLine("\r\n----------------------------------------ProjectFinishedGenerating---------------------------------------------------\r\n");
            //Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            //try
            //{
            //    var temp0 = project.Collection;
            //    var temp1 = project.ProjectItems;
            //    var temp2 = project.DTE;
            //    var temp3 = project.ExtenderNames;
            //    var temp4 = project.Globals;
            //    var temp5 = project.UniqueName;

            //    Debug.WriteLine(project.Name);
            //}
            //catch
            //{

            //}
            //Debug.WriteLine("\r\n----------------------------------------ProjectFinishedGenerating---------------------------------------------------\r\n\r\n");
        }

        // This method is only called for item templates,
        // not for project templates.
        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            Debug.WriteLine("----------------------------------------ProjectItemFinishedGenerating---------------------------------------------------");
            //var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            //var _installer = componentModel.GetService<IVsPackageInstaller2>();

            //foreach (var package in _packages)
            //{
            //    _installer.InstallLatestPackage(null, project, package, false, false);
            //}
        }

        // This method is called after the project is created.
        public void RunFinished()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Debug.WriteLine("\r\n\r\n----------------------------------------RunFinished---------------------------------------------------\r\n");



            var projects = VS.Solution.Projects;

            //for (int i = 0; i < projects.Count; i++)
            foreach (var proj in projects)
            {
                if (proj is Project project)
                {
                    var temp0 = project.Name;
                    var temp1 = project.FileName;
                    var temp2 = project.DTE;
                    var temp3 = project.ProjectItems;
                    foreach (Property prop in project.Properties)
                    {
                        try
                        {
                            if (prop.Name == "Description")
                            {
                                prop.let_Value("THIS IS THE DESCRIPTION!!!");
                                prop.Value = "THIS IS THE DESCRIPTION!!!";
                                Debug.WriteLine($"-{prop.Name}");
                                Debug.WriteLine($"-{prop.GetType()}");
                                Debug.WriteLine($"-{prop.NumIndices}");
                                Debug.WriteLine($"-{prop.Value}");
                            }
                            //Debug.WriteLine($"{prop.Name} - {prop.Value.ToString()}");
                        }
                        catch { Debug.WriteLine($"{prop.Name}"); }
                    }
                    //Debug.WriteLine("\r\n-----------------Globals---------------------");
                    //foreach (String s in (Array)project.Globals.VariableNames)
                    //{
                    //    Debug.WriteLine(s);
                    //}
                }
            }

            Debug.WriteLine("\r\n----------------------------------------RunFinished---------------------------------------------------\r\n\r\n");
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Debug.WriteLine("\r\n\r\n----------------------------------------RunStarted---------------------------------------------------\r\n");

            VS = automationObject as DTE;
            try
            {
                Debug.WriteLine($"customParams - {customParams.Length}");
                //if (customParams.Length > 0)
                //{
                //    var vstemplate = XDocument.Load((string)customParams[0]);
                //    _packages = vstemplate.Root
                //        .ElementsNoNamespace("WizardData")
                //        .ElementsNoNamespace("packages")
                //        .ElementsNoNamespace("package")
                //        .Select(e => e.Attribute("id").Value)
                //        .ToList();
                //}
            }
            catch (Exception)
            {

            }
            //var temp2 = runKind.ToString();
            foreach (var entry in replacementsDictionary) Debug.WriteLine($"{entry.Key} - {entry.Value}");


            replacementsDictionary.Add("$custommessage$", "ThisIsTheCustomParameter");

            Debug.WriteLine("\r\n----------------------------------------RunStarted---------------------------------------------------\r\n\r\n");
        }

        // This method is only called for item templates, not for project templates.
        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
