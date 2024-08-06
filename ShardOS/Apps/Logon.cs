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
using Cosmos.System.Graphics;
using static System.Net.Mime.MediaTypeNames;

namespace ShardOS.Apps
{
    public partial class Logon
    {
        public static Label Label = null!;
        public static Button ButtonLogin = null!;
        public static Edittext edittext = null!;
        public static Panel panel = null!;
        public static Window window;

        public static Bitmap wallpaper = new Bitmap(Kernel.rawWallpaper);

        public static string UserName = "";

        public static int I = 0;

        public static string Title()
        {
            return "LogonUI";
        }

        public static void Update()
        {
            for (int i = 0; i < UAS.Users.Count; i++)
            {
                Color col = Desktop.Dark;
                if (MouseEx.IsMouseWithin(0, (int)(Kernel.Mode.Height - (UAS.Users.Count * 64) + (i * 64)), 192, 64))
                {
                    if (MouseManager.MouseState == MouseState.Left && Desktop.prevMouseState != MouseState.Left)
                    {
                        col = Color.FromArgb(45, 45, 45);
                        Logon.UserName = UAS.Users[i].Username;
                    }
                }
                if (Logon.UserName == UAS.Users[i].Username)
                {
                    col = Color.FromArgb(45, 45, 45);
                }
                Kernel.Canvas.DrawFilledRectangle(col, 0, (int)(Kernel.Mode.Height - (UAS.Users.Count * 64) + (i * 64)), 192, 64);
                Desktop.DrawToSurface(Desktop.surface, 40, 3, (int)(Kernel.Mode.Height - (UAS.Users.Count * 64) + (i * 64) - 6), UAS.Users[i].Username, Color.Gray);
                Desktop.DrawToSurface(Desktop.surface, 20, 3, (int)(Kernel.Mode.Height - (UAS.Users.Count * 64) + (i * 64) + 40), UAS.Users[i].UserFolderPath, Color.DarkGray);
            }

            Label.Text = "Welcome back " + UserName + "!";
            bool con2 = false;
            if (KeyboardEx.IsKeyPressed)
            {
                if (KeyboardEx.k.Key == ConsoleKey.Enter)
                {
                    con2 = true;
                }
            }
            if (ButtonLogin.IsClicked || con2)
            {
                foreach (User us in UAS.Users)
                {
                    if(us.Username == UserName)
                    {
                        if(us.Password == edittext.Text)
                        {
                            UAS.ActiveUser = us;
                            Stop();
                            //Welcome.Start();
                            //ShellApp.Start();
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
            WindowManager.StopAll();
            window = new Window(0,0,(ushort)Kernel.Canvas.Mode.Width,(ushort)Kernel.Canvas.Mode.Height, "LogonUI", Update, DesignType.LUI, PermissionsType.User, Kernel.UnknownApp);
            window.backgroundimage = wallpaper;
            panel = new Panel(window.PanelW/2 - 160, window.PanelH/2 - 120, 320, 240, Desktop.Dark);
            Label = new Label(panel.X + 5, panel.Y + 5, 8, "Welcome back " + UserName + "!");
            Label.fg = Color.White;
            edittext = new Edittext(panel.X + 5, panel.Y + 22, 310, 20);
            edittext.fg = Color.White;
            edittext.bg = Desktop.DarkL;
            edittext.Selected = true;
            ButtonLogin = new Button(panel.X + panel.Widt - 37, panel.Y + panel.Heig - 21, 32, 16, 0, "OK", true, Desktop.Dark,Desktop.DarkL, System.Drawing.Color.White, Color.Black);
            window.Controls.Add(panel);
            window.Controls.Add(Label);
            window.Controls.Add(ButtonLogin);
            window.Controls.Add(edittext);
            WindowManager.Add(window);
            if (UserName == "")
            {
                UserName = UAS.Users[0].Username;
            }
        }

        public static void Stop()
        {
            WindowManager.Stop(window);
        }
    }
}
