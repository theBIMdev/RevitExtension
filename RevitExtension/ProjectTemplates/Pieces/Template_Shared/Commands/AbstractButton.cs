using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System.Diagnostics;

namespace $ext_safeprojectname$
{
    // Inherit this class into each button ("command") class to easily implement IExternalCommand and create the associated UI button.
    public abstract class AbstractButton : IExternalCommand
    {
        public abstract Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements);

        protected static string AssemblyName { get; } = System.Reflection.Assembly.GetExecutingAssembly().Location;
        protected static UIControlledApplication Application { get; set; }
        protected static Type ButtonClass { get; set; }
        protected static string ButtonName { get; set; }
        protected static string ButtonLabel { get; set; }
        protected static string PanelLabel { get; set; }
        protected static string TabLabel { get; set; }
        protected static Bitmap ImageSmall { get; set; }
        protected static Bitmap ImageLarge { get; set; }

        protected static void CreateButton()
        {
            try
            {
                RibbonPanel ribbonPanel;

                // If no dedicated tab is desired, place the button in the default "Add-Ins" tab.
                if (string.IsNullOrEmpty(TabLabel))
                {
                    List<RibbonPanel> panels = Application.GetRibbonPanels();
                    ribbonPanel = panels.Find(p => p.Name == PanelLabel);
                    if (ribbonPanel is null) { ribbonPanel = Application.CreateRibbonPanel(PanelLabel); }
                }
                else
                {
                    // If a tab with this name does not exist, create it;
                    bool tabMissing = true;
                    foreach (var tab in Autodesk.Windows.ComponentManager.Ribbon.Tabs)
                    { if (tab.Name == TabLabel) { tabMissing = false; break; } }
                    if (tabMissing) { Application.CreateRibbonTab(TabLabel); }

                    // If a panel already exists in the tab, retrieve it. Otherwise create it.
                    List<RibbonPanel> panels = Application.GetRibbonPanels(TabLabel);
                    ribbonPanel = panels.Find(p => p.Name == PanelLabel);
                    if (ribbonPanel is null) { ribbonPanel = Application.CreateRibbonPanel(TabLabel, PanelLabel); }
                }

                // Create the button in the panel.
                string className = $"{ButtonClass.Namespace}.{ButtonClass.Name}";
                ribbonPanel.AddItem(new PushButtonData(ButtonName, ButtonLabel, AssemblyName, className)
                {
                    Image = bitmapToImageSource(ImageSmall),
                    LargeImage = bitmapToImageSource(ImageLarge)
                });
            }
            catch
            {
                Debug.WriteLine("Could not load a UI button. Please verify that none of the properties in the 'AbstractButton' class are null.");
            }
        }

    private static BitmapImage bitmapToImageSource(Bitmap bitmap)
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
