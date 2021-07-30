using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace $safeprojectname$
{
    class UILoader
    {
        private static string AssemblyName { get; } = System.Reflection.Assembly.GetExecutingAssembly().Location;

        private RibbonPanel GetRibbonPanel(string TabName, string panelName, UIControlledApplication application)
        {
            List<RibbonPanel> panels = application.GetRibbonPanels(TabName);

            // If a panel already exists in the tab, retrieve it. Otherwise create it.
            if (panels.Find(p => p.Name == panelName) is RibbonPanel panel) { return panel; }
            else { return application.CreateRibbonPanel(TabName, panelName); }
        }

        private void LoadButton(RibbonPanel panel)
        {
            string commandClassName = "Click_ExcavateVolume";
            string buttonName = "Excavate Volume";
            string buttonLabel = "Excavate" + System.Environment.NewLine + "Volume";
            PushButtonData btnData = new PushButtonData(buttonName, buttonLabel, AssemblyName, commandClassName)
            {
                Image = BitmapToImageSource(Template_Resources.Properties.Resources.PlaceHolder_16),
                LargeImage = BitmapToImageSource(Template_Resources.Properties.Resources.PlaceHolder_32)
            };
            panel.AddItem(btnData);
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            // Convert System.Drawing.Bitmap to a BitmapImage that Revit can use for buttons
            using (MemoryStream mem = new MemoryStream())
            {
                bitmap.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
                mem.Position = 0;

                BitmapImage bitmapImg = new BitmapImage();
                bitmapImg.BeginInit();
                bitmapImg.StreamSource = mem;
                bitmapImg.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImg.EndInit();

                return bitmapImg;
            }
        }
    }
}
