﻿using Cosmos.System;
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
    public partial class Welcome
    {
        public static Label WelcomeLabel = null!;
        public static Label WelcomeLabel2 = null!;
        public static Button WelcomeButton = null!;
        public static Edittext edittext = null!;
        public static Window window;

        public static int I = 0;

        public static string Title()
        {
            return "Welcome";
        }

        public static void Update()
        {
            if (WelcomeButton.IsClicked)
            {
                Stop();
            }
        }

        public static void Start()
        {
            window = new Window(100, 100, 320, 100, "Welcome", Update, DesignType.Default, PermissionsType.User, Kernel.UnknownApp);
            WelcomeLabel = new Label(3, 3, 8, "Welcome back " + UAS.ActiveUser.Username + "!");
            WelcomeLabel2 = new Label(3, 19, 8, "Enjoy while ShardOS is running");
            edittext = new Edittext(3, 35, 200, 20);
            WelcomeButton = new Button(window.PanelW - 42, window.PanelH - 26, 32, 16, 0, "OK", true, System.Drawing.Color.White, System.Drawing.Color.SteelBlue, System.Drawing.Color.Black, System.Drawing.Color.Red);
            window.Controls.Add(WelcomeLabel);
            window.Controls.Add(WelcomeLabel2);
            window.Controls.Add(WelcomeButton);
            window.Controls.Add(edittext);
            WindowManager.Add(window);
        }

        public static void Stop()
        {
            WindowManager.StopAll();
        }
    }
}
