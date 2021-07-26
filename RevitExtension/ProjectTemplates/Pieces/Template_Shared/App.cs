using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace $safeprojectname$
{
    class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            try 
            {
                // Subscribe to events
                application.ControlledApplication.DocumentCreated += new EventHandler<DocumentCreatedEventArgs>(AppDocCreated);

                // RegisterCommands
                // RegisterUpdaters
            }
            catch
            {
                return Result.Failed;
            }
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            try
            {
                // UnregisterCommands(application);
                // UnregisterUpdaters(application);
            }
            catch
            {
                return Result.Failed;
            }
            return Result.Succeeded;
        }

        // Application Event Handlers
        public void AppDocCreated(object sender, DocumentCreatedEventArgs args)
        {

        }
    }
}
