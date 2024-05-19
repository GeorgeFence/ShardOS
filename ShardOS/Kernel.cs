using Cosmos.HAL;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using IL2CPU.API.Attribs;
using ShardOS.Apps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
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
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.unknownapp.bmp")] public static byte[] rawUnknownApp;
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.user24.bmp")] public static byte[] rawUser24;
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.settings24.bmp")] public static byte[] rawSettings24;
        public static Bitmap ExitApp = new Bitmap(rawExit);
        public static Bitmap ServiceApp = new Bitmap(rawServiceApp);
        public static Bitmap SystemApp = new Bitmap(rawSystemApp);
        public static Bitmap UserApp = new Bitmap(rawUserApp);
        public static Bitmap UnknownApp = new Bitmap(rawUnknownApp);
        public static Bitmap User24 = new Bitmap(rawUser24);
        public static Bitmap Settings24 = new Bitmap(rawSettings24);

        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.APP.bmp")] public static byte[] rawFileApp;
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.NONE.bmp")] public static byte[] rawFileNone;
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.REG.bmp")] public static byte[] rawFileReg;
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.TXT.bmp")] public static byte[] rawFileTxt;
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.WIN.bmp")] public static byte[] rawWinApp;

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
            if(UAS.Users.Count == 1)
            {
                UAS.ActiveUser = UAS.Users[0];
            }
            try
            {
                DesktopGrid.gridItems.Add(new GridItem("Registry", new Bitmap(rawFileReg), 0, 0));
                DesktopGrid.gridItems.Add(new GridItem("Browser", new Bitmap(rawWinApp), 1, 0));
                DesktopGrid.gridItems.Add(new GridItem("Text file", new Bitmap(rawFileTxt), 0, 1));
            }
            catch (Exception ex)
            {
                DrawStatus("DesktopGrid: " + ex.Message, Color.Red);
            }
            Welcome.Start();
            IsBooting = false;
        }

        protected override void Run()
        {
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
                Power.ACPIShutdown();
            }
            else if(mode == 1)
            {
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
