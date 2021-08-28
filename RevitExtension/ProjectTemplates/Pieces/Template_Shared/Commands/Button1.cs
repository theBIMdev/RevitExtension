using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using $ext_safeprojectname$_Resources.Properties;

namespace $ext_safeprojectname$
{
    [Transaction(TransactionMode.Manual)]
    class Button1 : AbstractButton
    {
        // Each button ("command") class must inherit 'AbstractButton' and implement this 'LoadButton' static method.
        // 'LoadButton' is then invoked for each in the 'App' 'OnStartup' to create the UI button.
        public static void LoadButton(UIControlledApplication application)
        {
            ButtonClass = typeof(Button1);
            ButtonName = "Button1";
            ButtonLabel = "Button\rOne";
            PanelLabel = "Panel1";
            TabLabel = "$ext_safeprojectname$"; // OPTIONAL: If no dedicated tab is desired, make this null or empty.
            ImageSmall = Resources.PlaceHolder_16;
            ImageLarge = Resources.PlaceHolder_32;
            Application = application;
            CreateButton();
        }

        public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                TaskDialog.Show("Button1 Clicked!", "Hello World!");
                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }
    }
}
