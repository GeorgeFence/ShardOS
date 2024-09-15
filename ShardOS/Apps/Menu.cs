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
using Image = ShardOS.UI.Controls.Image;
using Cosmos.HAL;

namespace ShardOS.Apps
{
    public partial class Menu
    {
        public static Panel lPanel = null!;
        public static Image Power = null!;
        public static Image Settings = null!;

        public static Window Mwindow;


        public static int I = 0;

        public static string Title()
        {
            return "Menu";
        }

        public static void Update()
        {
            if (Power.IsClicked)
            {
                PowerOptions.Start();
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

            lPanel = new Panel(0, 0, 48, 600, Desktop.DarkL);
            Power = new Image(4,556,40,40, Kernel.PowerShutdown, false, "Power Options");
            Settings = new Image(4, 508, 40, 40, Kernel.Control, false, "Control");

            Mwindow.Controls.Add(lPanel);
            Mwindow.Controls.Add(Power);
            Mwindow.Controls.Add(Settings);
            WindowManager.Add(Mwindow);
        }

        public static void Stop()
        {
            WindowManager.Stop(Mwindow);
        }
    }
}
