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

namespace ShardOS.Apps
{
    public partial class MessageBox
    {
        public static Label Label = null!;
        public static Button Button = null!;
        public static Panel bottom = null!;
        public static Window window;

        public static int I = 0;

        public static string titleText = "";


        public static string Title()
        {
            return titleText;
        }

        public static void Update()
        {
            if (Button.IsClicked)
            {
                Stop();
            }
        }

        public static void Start()
        {
            window = new Window(100, 100, 320, 100, Title(), Update, DesignType.Default, PermissionsType.System, Kernel.UnknownApp);
            Label = new Label(3, 3, 8, "");
            Button = new Button(window.PanelW - 42, window.PanelH - 26, 32, 16, 0, "OK", true, System.Drawing.Color.White, System.Drawing.Color.SteelBlue, System.Drawing.Color.Black, System.Drawing.Color.Red);
            window.Controls.Add(Label);
            window.Controls.Add(Button);
            WindowManager.Add(window);
        }

        public static void Show(string Title, string Description)
        {
            titleText = Title;
            Start();

        }

        public static void Stop()
        {
            WindowManager.Stop(window);
        }
    }
}
