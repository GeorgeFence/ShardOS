using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using IL2CPU.API.Attribs;
using ShardOS.Apps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using Console = System.Console;
using Power = Cosmos.HAL.Power;
using Sys = Cosmos.System;

namespace ShardOS
{
    public class Kernel : Sys.Kernel
    {
        public static Canvas Canvas;
        public static Mode Mode;
        public static Bitmap logo512;
        public static bool IsBooting = true;
        public static int Ypos = 0;

        public static int DefaultFontHeight = 14; //14 best :)

        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.logo.bmp")] public static byte[] rawLogo512;
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.logo30.bmp")] public static byte[] rawLogo30;
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.wallpaper.bmp")] public static byte[] rawWallpaper;
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.cursor.bmp")] public static byte[] rawCursor;

        [ManifestResourceStream(ResourceName = "ShardOS.build.txt")] public static byte[] rawBuildNumber;

        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.exit.bmp")] public static byte[] rawExit;
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.serviceapp.bmp")] public static byte[] rawServiceApp;
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.systemapp.bmp")] public static byte[] rawSystemApp;
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.userapp.bmp")] public static byte[] rawUserApp;
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.application.bmp")] public static byte[] rawUnknownApp;
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.user.bmp")] public static byte[] rawUser;
        public static Bitmap ExitApp = new Bitmap(rawExit);
        public static Bitmap ServiceApp = new Bitmap(rawServiceApp);
        public static Bitmap SystemApp = new Bitmap(rawSystemApp);
        public static Bitmap UserApp = new Bitmap(rawUserApp);
        public static Bitmap UnknownApp = new Bitmap(rawUnknownApp);
        public static Bitmap User = new Bitmap(rawUser);

        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.setting.bmp")] public static byte[] rawSetting;
        public static Bitmap Settings = new Bitmap(rawSetting);
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.control.bmp")] public static byte[] rawControl;
        public static Bitmap Control = new Bitmap(rawControl);
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.power.bmp")] public static byte[] rawPower;
        public static Bitmap PowerShutdown = new Bitmap(rawPower);


        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.menu.bmp")] public static byte[] rawMenu;
        public static Bitmap Menu = new Bitmap(rawMenu);



        [ManifestResourceStream(ResourceName = "ShardOS.Files.ttf.Ubuntu.ttf")] public static byte[] rawFontUbuntu;

        public static string BuildNumber = "";
        protected override void OnBoot()
        {
            base.OnBoot();
            IsBooting = true;
            Mode = new Mode(1920,1080,ColorDepth.ColorDepth32);
            Canvas = FullScreenCanvas.GetFullScreenCanvas(Mode);
            logo512 = new Bitmap(rawLogo512);
            Canvas.DrawImageAlpha(logo512,(int)(Mode.Width / 2 - 256), (int)(Mode.Height / 5));
            Canvas.Display();
            BuildNumber = System.Text.Encoding.UTF8.GetString(rawBuildNumber);
            DelayCode(500);
            DrawStatus("Starting Shard OS");
            DelayCode(500);
            DrawStatus("Running build " + BuildNumber.ToString());
            DelayCode(1500);
        }
        protected override void BeforeRun() 
        {
            DrawStatus("Now running BeforeRun");
            DelayCode(200);
            Files.Initialize();
            DelayCode(500);
            DrawStatus("Initializing Registers");
            RegistryManager.Initialize();
            DrawStatus("Initializing UAS");
            UAS.Initialize();
            DelayCode(500); 
            DrawStatus("Initializing GUI");
            Desktop.Init((int)Mode.Width,(int)Mode.Height);
            DelayCode(500);
            DrawStatus("Starting GUI");
            DelayCode(500);
            
            try
            {
                DesktopGrid.gridItems.Add(new GridItem("App 1", UnknownApp, 0, 0));
                DesktopGrid.gridItems.Add(new GridItem("App 2", UnknownApp, 1, 0));
                DesktopGrid.gridItems.Add(new GridItem("App 3", UnknownApp, 0, 1));
            }
            catch (Exception ex)
            {
                DrawStatus("DesktopGrid: " + ex.Message, Color.Red);
            }
            IsBooting = false;
            Logon.Start();
        }

        protected override void Run()
        {
            //Keyboard Input
            KeyboardEx.k = new ConsoleKeyInfo();
            KeyboardEx.IsKeyPressed = false;
            //PS2Keyboard.WaitForKey(); Halts CPU, waits until HW update
            if (KeyboardManager.TryReadKey(out var key))
            {
                KeyboardEx.k = new ConsoleKeyInfo(key.KeyChar, key.Key.ToConsoleKey()
                , key.Modifiers == ConsoleModifiers.Shift, key.Modifiers == ConsoleModifiers.Alt
                , key.Modifiers == ConsoleModifiers.Control);
                KeyboardEx.IsKeyPressed = true;
            }

            //Gui
            Desktop.Update();
        }

        public static void DelayCode(uint milliseconds)
        {
            Cosmos.HAL.PIT pit = new Cosmos.HAL.PIT();
            pit.Wait(milliseconds);
            pit = new Cosmos.HAL.PIT();
        }

        public static void DrawStatus(string text)
        {
            if (IsBooting)
            {
                Canvas.DrawFilledRectangle(Color.Black, 0, (int)((Mode.Height / 5) * 4), (int)Mode.Width, 16); 
                Canvas.DrawString(text,PCScreenFont.Default, Color.White, (int)(Mode.Width / 2 - (text.Length / 2 * 8)), (int)((Mode.Height / 5) * 4));
                Canvas.Display();
            }
        }
        public static void DrawStatus(string text, Color color)
        {
            if (IsBooting)
            {
                Canvas.DrawFilledRectangle(Color.Black, 0, (int)((Mode.Height / 5) * 4), (int)Mode.Width, 16);
                Canvas.DrawString(text, PCScreenFont.Default, color, (int)(Mode.Width / 2 - (text.Length / 2 * 8)), (int)((Mode.Height / 5) * 4));
                Canvas.Display();
            }
        }

        public static void DrawStatusForce(string text)
        {
            Canvas.DrawFilledRectangle(Color.Black, 0, (int)((Mode.Height / 5) * 4), (int)Mode.Width, 16);
            Canvas.DrawString(text, PCScreenFont.Default, Color.White, (int)(Mode.Width / 2 - (text.Length / 2 * 8)), (int)((Mode.Height / 5) * 4));
            Canvas.Display();
        }
        public static void DrawStatusForce(string text, Color color)
        {
            Canvas.DrawFilledRectangle(Color.Black, 0, (int)((Mode.Height / 5) * 4), (int)Mode.Width, 16);
            Canvas.DrawString(text, PCScreenFont.Default, color, (int)(Mode.Width / 2 - (text.Length / 2 * 8)), (int)((Mode.Height / 5) * 4));
            Canvas.Display();
        }

        public static void Shutdown(int mode)
        {
            if(mode == 0)
            {
                Desktop.CanContinue = false;
                Canvas = FullScreenCanvas.GetFullScreenCanvas(Mode);
                Canvas.DrawImageAlpha(logo512, (int)(Mode.Width / 2 - 256), (int)(Mode.Height / 5));
                Canvas.Display();
                DrawStatusForce("Shutting down");
                DelayCode(1500);
                Power.ACPIShutdown();
            }
            else if(mode == 1)
            {
                Desktop.CanContinue = false;
                Canvas = FullScreenCanvas.GetFullScreenCanvas(Mode);
                Canvas.DrawImageAlpha(logo512, (int)(Mode.Width / 2 - 256), (int)(Mode.Height / 5));
                Canvas.Display();
                DrawStatusForce("Restarting");
                DelayCode(1500);
                Power.CPUReboot();
            }
            else if(mode == 2)
            {
                DrawStatus("Reseting registry", Color.Yellow);
                Files.WriteAllText("0:\\registers.reg", "");
                RegistryManager.AddRegistry(RegisterType.REG_STR, "BOOT_STRING", "Testing Registry xd");
                RegistryManager.AddRegistry(RegisterType.REG_STR, "BOOT_STRING_TWO", "TESTIng two");
                RegistryManager.AddRegistry(RegisterType.REG_STR, "BOOT_STRING_THREE", "TESTIng three");
                RegistryManager.Reload();
                DrawStatus("Done without error!", Color.Green);
                DelayCode(1000);
            }
            else if (mode == 3)
            {
                Canvas.DrawString("---       Create new user       ---  * Entered text is hidden", PCScreenFont.Default, Color.White,0,(Ypos * 16));
                Canvas.Display();
                Ypos++;
                Canvas.DrawString("Enter new Username: ", PCScreenFont.Default, Color.White, 0, (Ypos * 16));
                Canvas.Display();
                Ypos++;
                string username = Console.ReadLine();
                Canvas.DrawString("Enter password for " + username + ": ", PCScreenFont.Default, Color.White, 0, (Ypos * 16));
                Canvas.Display();
                Ypos++;
                string password = Console.ReadLine();
                Canvas.DrawString("Please confirm creation of user (Y/n)  " + username + ", " + password, PCScreenFont.Default, Color.White, 0, (Ypos * 16));
                Canvas.Display();
                Ypos++;
                if(Console.ReadLine().ToLower() == "y")
                {
                    UAS.CreateUser(new User(username, password, "0:\\Users\\"+ username + "\\"));
                    UAS.Reload();
                    Canvas.DrawString("Done", PCScreenFont.Default, Color.White, 0, (Ypos * 16));
                    Canvas.Display();
                    Ypos++;
                }
                else
                {
                    Canvas.DrawString("Operation aborted", PCScreenFont.Default, Color.White, 0, (Ypos * 16));
                    Canvas.Display();
                    Ypos++;
                    DelayCode(1000);
                }
            }
        }
    }
}
