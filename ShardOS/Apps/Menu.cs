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

namespace ShardOS.Apps
{
    public partial class Menu
    {
        public static Button ShutdownDialog = null!;
        public static Button Console = null!;
        public static Button Settings = null!;
        public static Panel downPanel = null!;
        public static Panel fpsBgPanel = null!;
        public static Panel fpsFgPanel = null!;
        public static Label fps = null!;

        public static Window Mwindow;


        public static int I = 0;

        public static string Title()
        {
            return "Menu";
        }

        public static void Update()
        {
            fps.Text = Desktop.FPS.ToString();
            fpsFgPanel.Widt = Desktop.FPS * 2;
            if (ShutdownDialog.IsClicked)
            {
                
            }
            if (Settings.IsClicked)
            {
                
            }
            if (Console.IsClicked)
            {
                
            }
            if(WindowManager.Selected != Title())
            {
                Stop();
            }
        }

        public static void Start()
        {
            Mwindow = new Window(15, (int)(Kernel.Canvas.Mode.Height - 658), 400, 600, "Menu", Update, DesignType.Blank,PermissionsType.User, Kernel.Menu);
            Mwindow.CanMove = false;
            Mwindow.Background = Desktop.DarkS;
            ShutdownDialog = new Button(5, Mwindow.PanelH - 29, 32, 24, 0, "S/R", true, System.Drawing.Color.Blue, System.Drawing.Color.White, System.Drawing.Color.DarkBlue, System.Drawing.Color.Blue);
            Console = new Button(42, Mwindow.PanelH - 29, 64, 24, 0, "Console", true, System.Drawing.Color.Red, System.Drawing.Color.White, System.Drawing.Color.DarkBlue, System.Drawing.Color.Blue);
            Settings = new Button(Mwindow.PanelW - 101, Mwindow.PanelH - 29, 96, 24, 0, "Settings", true, System.Drawing.Color.DimGray, System.Drawing.Color.White, System.Drawing.Color.DarkBlue, System.Drawing.Color.Blue);
            downPanel = new Panel(0, 600 - 34, 400, 34, System.Drawing.Color.Gray);
            fpsBgPanel = new Panel(0, 0, 400, 16, Desktop.DarkM);
            fpsFgPanel = new Panel(0, 0, 200, 16, Desktop.DarkXL);
            fps = new Label((200 - (Desktop.FPS.ToString().Length / 2)), 0,14, "FPS");
            Mwindow.Controls.Add(downPanel);
            Mwindow.Controls.Add(ShutdownDialog);
            Mwindow.Controls.Add(Console);
            Mwindow.Controls.Add(Settings);
            Mwindow.Controls.Add(fpsBgPanel);
            Mwindow.Controls.Add(fpsFgPanel);
            Mwindow.Controls.Add(fps);
            WindowManager.Add(Mwindow);
        }

        public static void Stop()
        {
            WindowManager.Stop(Mwindow);
        }
    }
}
