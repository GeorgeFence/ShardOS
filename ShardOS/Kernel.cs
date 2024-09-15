using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
using Cosmos.HAL.BlockDevice;
using Cosmos.HAL.Network;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using IL2CPU.API.Attribs;
using ShardOS.Apps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using static Cosmos.HAL.BlockDevice.ATA_PIO;
using static System.Net.Mime.MediaTypeNames;
using Console = System.Console;
using Global = Cosmos.System.Global;
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
        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.error.bmp")] public static byte[] rawError;
        public static Bitmap Error = new Bitmap(rawError);


        [ManifestResourceStream(ResourceName = "ShardOS.Files.bmp.menu.bmp")] public static byte[] rawMenu;
        public static Bitmap Menu = new Bitmap(rawMenu);



        [ManifestResourceStream(ResourceName = "ShardOS.Files.ttf.Ubuntu.ttf")] public static byte[] rawFontUbuntu;
        [ManifestResourceStream(ResourceName = "ShardOS.Files.ttf.Inter.ttf")] public static byte[] rawFontInter;

        public static string BuildNumber = "";
        protected override void OnBoot()
        {
            base.OnBoot();
            IsBooting = true;
            Mode = new Mode(1920,1080,ColorDepth.ColorDepth32);
            Canvas = FullScreenCanvas.GetFullScreenCanvas(Mode);
            logo512 = new Bitmap(rawLogo512);
            Canvas.DrawImageAlpha(logo512,(int)(Mode.Width / 2 - 258), (int)(Mode.Height / 5));
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

            
            try
            {
                DesktopGrid.gridItems.Add(new GridItem("Shell", UnknownApp, 0, 0,1));
                DesktopGrid.gridItems.Add(new GridItem("Welcome", UnknownApp, 1, 0,2));
                DesktopGrid.gridItems.Add(new GridItem("App 3", UnknownApp, 0, 1,0));
            }
            catch (Exception ex)
            {
                DrawStatus("DesktopGrid: " + ex.Message, Color.Red);
            }
            DrawStatus("Initializing GUI");
            Desktop.Init((int)Mode.Width, (int)Mode.Height);
            //Kernel.Canvas.Clear();
            DrawStatus("Starting GUI");
            DelayCode(500); 
            IsBooting = false;
            //Logon.Start();
            
        }

        protected override void Run()
        {
            try
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
                CobaltCore.CallCore();
            }
            catch(Exception ex)
            {
                KernelPanic(ex.Message);
            }
        }
        public static void DarkenBitmap(Bitmap image, float darkeningFactor)
        {
            darkeningFactor = Math.Clamp(darkeningFactor, 0f, 1f);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color originalColor = image.GetPointColor(x, y);

                    int r = (int)(originalColor.R * darkeningFactor);
                    int g = (int)(originalColor.G * darkeningFactor);
                    int b = (int)(originalColor.B * darkeningFactor);

                    Color darkenedColor = Color.FromArgb(originalColor.A, r, g, b);

                    image.SetPixel(darkenedColor, x, y);
                }
            }
        }
        public static void ApplyBlur(Bitmap bitmap, int blurRadius)
        {
            uint width = bitmap.Width;
            uint height = bitmap.Height;

            // Loop through every pixel in the image
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int total = 0;

                    // Loop through each surrounding pixel within the blur radius
                    for (int blurX = -blurRadius; blurX <= blurRadius; blurX++)
                    {
                        for (int blurY = -blurRadius; blurY <= blurRadius; blurY++)
                        {
                            int pixelX = x + blurX;
                            int pixelY = y + blurY;

                            // Ensure pixel is within image bounds
                            if (pixelX >= 0 && pixelX < width && pixelY >= 0 && pixelY < height)
                            {
                                Color pixelColor = bitmap.GetPointColor(pixelX, pixelY);
                                avgR += pixelColor.R;
                                avgG += pixelColor.G;
                                avgB += pixelColor.B;
                                total++;
                            }
                        }
                    }

                    // Calculate the average color for the current pixel
                    avgR /= total;
                    avgG /= total;
                    avgB /= total;

                    // Set the blurred pixel color
                    bitmap.SetPixel(Color.FromArgb(avgR, avgG, avgB), x, y);
                }
            }
        }

        public static void KernelPanic(string message)
        {
            int state = 0;
            while (state == 0)
            {
                // Clear the screen with a background color
                Canvas.Clear(Color.DarkBlue);

                Canvas.DrawImage(Error, (int)(Mode.Width / 2 - 128), (int)(Mode.Height / 5), 256, 256);

                // Draw the kernel panic message
                Canvas.DrawString("KERNEL PANIC", PCScreenFont.Default, Color.Red, 20, 20);
                Canvas.DrawString("==============", PCScreenFont.Default, Color.White, 20, 50);
                Canvas.DrawString("A fatal error has occurred, and the system has halted.", PCScreenFont.Default, Color.White, 20, 80);
                Canvas.DrawString("Error Details:", PCScreenFont.Default, Color.White, 20, 110);
                Canvas.DrawString(message, PCScreenFont.Default, Color.White, 20, 140); ;
                Canvas.DrawString("Please restart your computer.", PCScreenFont.Default, Color.White, 20, 170);


                //Shutdown
                //Kernel.Canvas.DrawFilledRectangle(Color.Orange, (int)Kernel.Canvas.Mode.Width / 2 - 128, (int)Kernel.Canvas.Mode.Height / 3 * 2, 256, 64);
                Kernel.Canvas.DrawRectangle(Color.Red, (int)Kernel.Canvas.Mode.Width / 2 - 128, (int)Kernel.Canvas.Mode.Height / 3 * 2, 256, 64);
                Canvas.DrawString("Shutdown", PCScreenFont.Default, Color.Green, (int)Kernel.Canvas.Mode.Width / 2 - 32, (int)Kernel.Canvas.Mode.Height / 3 * 2 + 24);
                //Reboot
                //Kernel.Canvas.DrawFilledRectangle(Color.Orange, (int)Kernel.Canvas.Mode.Width / 2 - 128, (int)Kernel.Canvas.Mode.Height / 3 * 2 + 65, 256, 64);
                Kernel.Canvas.DrawRectangle(Color.Red, (int)Kernel.Canvas.Mode.Width / 2 - 128, (int)Kernel.Canvas.Mode.Height / 3 * 2 + 65, 256, 64);
                Canvas.DrawString("Reboot", PCScreenFont.Default, Color.Green, (int)Kernel.Canvas.Mode.Width / 2 - 24, (int)Kernel.Canvas.Mode.Height / 3 * 2 + 65 + 24);
                //Reboot
                //Kernel.Canvas.DrawFilledRectangle(Color.Orange, (int)Kernel.Canvas.Mode.Width / 2 - 128, (int)Kernel.Canvas.Mode.Height / 3 * 2 + 130, 256, 64);
                Kernel.Canvas.DrawRectangle(Color.Red, (int)Kernel.Canvas.Mode.Width / 2 - 128, (int)Kernel.Canvas.Mode.Height / 3 * 2 + 130, 256, 64);
                Canvas.DrawString("Enter SAFE mode", PCScreenFont.Default, Color.Red, (int)Kernel.Canvas.Mode.Width / 2 - 64, (int)Kernel.Canvas.Mode.Height / 3 * 2 + 130 + 24);

                if(MouseEx.IsMouseWithin((int)Kernel.Canvas.Mode.Width / 2 - 128, (int)Kernel.Canvas.Mode.Height / 3 * 2, 256, 64) && MouseEx.LeftClick)
                {
                    state = 1;
                }
                if (MouseEx.IsMouseWithin((int)Kernel.Canvas.Mode.Width / 2 - 128, (int)Kernel.Canvas.Mode.Height / 3 * 2 + 65, 256, 64) && MouseEx.LeftClick)
                {
                    state = 2;
                }
                if (MouseEx.IsMouseWithin((int)Kernel.Canvas.Mode.Width / 2 - 128, (int)Kernel.Canvas.Mode.Height / 3 * 2 + 130, 256, 64) && MouseEx.LeftClick)
                {
                    state = 3;
                }

                Desktop.DrawImageAlpha(Desktop.cursor, (int)MouseManager.X, (int)MouseManager.Y);
                Canvas.Display();

                Heap.Collect();
                MouseEx.Mouse();
            }
            if (state == 1)
            {
                ACPI.Shutdown();
            }
            if (state == 2)
            {
                Power.CPUReboot();
            }
            if (state == 3)
            {
                Kernel.DrawStatusForce("Not implemented yet! Rebooting now...");
                Kernel.DelayCode(2000);
                Power.CPUReboot();
            }
            // Halt the CPU
            CPU.Halt();
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
