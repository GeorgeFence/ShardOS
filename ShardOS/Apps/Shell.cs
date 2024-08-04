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
using System.Drawing;

namespace ShardOS.Apps
{
    public partial class ShellApp
    {

        public static Window window;
        public static Label label = null!;

        public static int I = 0;

        public static string Title()
        {
            return "Shell";
        }

        public static void Update()
        {
            Shell.Update();
            Shell.Draw(window.X, window.Y + 32);
        }

        public static void Start()
        {
            window = new Window(600, 300, 640, 400, "Shell", Update, DesignType.Default, PermissionsType.User, Kernel.UnknownApp);
            label = new Label(0, 0, 12, "Shell");
            Shell.Init(window.X, window.Y + 32, window.PanelW, window.PanelH);
            window.Controls.Add(label);
            WindowManager.Add(window);
        }

        public static void Stop()
        {
            WindowManager.Stop(window);
        }
    }
}