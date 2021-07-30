using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit;
using Autodesk.Revit.Attributes;
using Autodesk.Windows;
using Autodesk.Internal.Windows;
using Autodesk.Revit.ApplicationServices;

namespace $safeprojectname$
{
    /// <summary> The starting point of the ConcoTools Addin. </summary>
    public class App : IExternalApplication
    {
        public static Document ActiveDocument { get; set; }

        public Result OnStartup(UIControlledApplication app)
        {
            try
            {
                // OPTIONAL: Add buttons and register commands

                // OPTIONAL: Subscribe to events
                // See https://www.revitapidocs.com/2022/fa23dcb9-fd52-b8ac-eec4-7ce03eac4b7d.htm

                // OPTIONAL: Register updaters and triggers
                // See https://www.revitapidocs.com/2022/9585644f-5bbd-03ab-45f1-0473d2c2b0da.htm
                // See https://www.revitapidocs.com/2022/018aac7e-0c20-c988-b6ab-f592d61a4772.htm

                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication app)
        {
            try
            {
                return Result.Succeeded;
            }
            catch 
            {
                return Result.Failed;
            }
        }

        // Revit Application Event Handlers
        public void AppDocCreated(object sender, DocumentCreatedEventArgs args)
        {

        }
    }
}



//using System;
//using System.Collections.Generic;
//using System.Text;
//using Autodesk.Revit.DB.Events;
//using Autodesk.Revit.UI;

//namespace $safeprojectname$
//{
//    class App : IExternalApplication
//    {
//        public Result OnStartup(UIControlledApplication application)
//        {
//            try 
//            {
//                // Subscribe to events
//                application.ControlledApplication.DocumentCreated += new EventHandler<DocumentCreatedEventArgs>(AppDocCreated);

//                // RegisterCommands
//                // RegisterUpdaters
//            }
//            catch
//            {
//                return Result.Failed;
//            }
//            return Result.Succeeded;
//        }

//        public Result OnShutdown(UIControlledApplication application)
//        {
//            try
//            {
//                // UnregisterCommands(application);
//                // UnregisterUpdaters(application);
//            }
//            catch
//            {
//                return Result.Failed;
//            }
//            return Result.Succeeded;
//        }

//        // Application Event Handlers
//        public void AppDocCreated(object sender, DocumentCreatedEventArgs args)
//        {

//        }
//    }
//}
