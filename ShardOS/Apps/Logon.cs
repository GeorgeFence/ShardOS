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
using System.Threading;
using System.Drawing;

namespace ShardOS.Apps
{
    public partial class Logon
    {
        public static Label Label = null!;
        public static Button ButtonLogin = null!;
        public static Edittext edittext = null!;
        public static Window window;

        public static string UserName = "";

        public static int I = 0;

        public static string Title()
        {
            return "LogonUI";
        }

        public static void Update()
        {
            Label.Text = "Welcome back " + UserName + "!";
            if (ButtonLogin.IsClicked)
            {
                foreach (User us in UAS.Users)
                {
                    if(us.Username == UserName)
                    {
                        if(us.Password == edittext.Text)
                        {
                            UAS.ActiveUser = us;
                            Stop();
                        }
                        else
                        {
                            Kernel.DrawStatusForce("Password incorrect", Color.Red);
                            Kernel.DelayCode(1000);
                            edittext.Text = "";
                        }
                    }
                }
            }
        }

        public static void Start()
        {
            window = new Window((int)(Kernel.Mode.Width / 2 - 160), (int)(Kernel.Mode.Height / 2 - 120), 320, 240, "LogonUI", Update, DesignType.Blank, PermissionsType.User, Kernel.UnknownApp);
            Label = new Label(3, 3, 8, "Welcome back " + UserName + "!");
            edittext = new Edittext(3, 35, 200, 20);
            ButtonLogin = new Button(window.PanelW - 42, window.PanelH - 26, 32, 16, 0, "OK", true, System.Drawing.Color.White, System.Drawing.Color.Black, System.Drawing.Color.Red);
            window.Controls.Add(Label);
            window.Controls.Add(ButtonLogin);
            window.Controls.Add(edittext);
            WindowManager.Add(window);
        }

        public static void Stop()
        {
            WindowManager.Stop(window);
        }
    }
}
