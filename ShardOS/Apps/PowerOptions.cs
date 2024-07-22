using Cosmos.System;
using ShardOS.UI.Controls;
using ShardOS.UI;
using ShardOS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System.Drawing;

namespace ShardOS.Apps
{
    public partial class PowerOptions
    {
        public static Button ShutdownButton = null!;
        public static Button RebootButton = null!;
        public static Window Swindow;

        public static Bitmap OldWallpaper;

        public static int I = 0;

        public static List<bool> WindowOldValue;

        public static string Title()
        {
            return "Shutdown Dialog";
        }

        public static void Update()
        {
            if (ShutdownButton.IsClicked)
            {
                Kernel.Shutdown(0);
            }
            if (RebootButton.IsClicked)
            {
                Kernel.Shutdown(1);
            }
        }

        public static void Start()
        {
            OldWallpaper = Desktop.wallpaper;
            Bitmap b = Kernel.Canvas.GetImage(0, 0, (int)Kernel.Canvas.Mode.Width, (int)Kernel.Canvas.Mode.Height);
            List<int> data = new List<int>();
            foreach (int i in b.RawData)
            {
                var (red, green, blue, alpha) = Desktop.HexToRgba(i);
                int t = (red + green + blue) / 3;
                data.Add(Desktop.RgbaToHex(t,t,t,alpha));
            }
            b.RawData = data.ToArray();
            Swindow = new Window((int)(Kernel.Canvas.Mode.Width / 2 - 150), (int)(Kernel.Canvas.Mode.Height / 2 - 64), 300, 128, "Power Options", Update, DesignType.Default,PermissionsType.Service, Kernel.PowerShutdown);
            Swindow.CanMove = false;
            ShutdownButton = new Button(0, 0, (ushort)Swindow.PanelW, (ushort)(Swindow.PanelH / 2), 0, "SHUTDOWN", true, System.Drawing.Color.Blue, System.Drawing.Color.White, System.Drawing.Color.DarkBlue, System.Drawing.Color.DarkBlue);
            RebootButton = new Button(0, (ushort)(Swindow.PanelH / 2), (ushort)Swindow.PanelW, (ushort)(Swindow.PanelH / 2), 0, "REBOOT", true, System.Drawing.Color.Blue, System.Drawing.Color.White, System.Drawing.Color.DarkBlue, System.Drawing.Color.DarkBlue);
            Swindow.Controls.Add(ShutdownButton);
            Swindow.Controls.Add(RebootButton);
            WindowManager.Add(Swindow);
            Desktop.OnlyWindowsMouse = true;
            Desktop.wallpaper = b;

            for (int i = 0; i < WindowManager.Windows.Count; i++)
            {
                if (WindowManager.Windows[i].Title == Title())
                {
                    //WindowOldValue.Add(WindowManager.Windows[i].IsVisible);
                    WindowManager.Windows[i].IsVisible = false;
                }
            }
        }

        public static void Stop()
        {
            Desktop.wallpaper = OldWallpaper;
            Desktop.OnlyWindowsMouse = false;
            WindowManager.Stop(Swindow);
            for (int i = 0; i < WindowManager.Windows.Count; i++)
            {
                WindowManager.Windows[i].IsVisible = WindowOldValue[i];
            }
        }


    }
}
