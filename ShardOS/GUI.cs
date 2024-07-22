using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using CosmosTTF;
using ShardOS.Apps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ShardOS
{
    public class Desktop
    {
        static int Frames = 0;
        public static Bitmap wallpaper = new Bitmap(Kernel.rawWallpaper);
        public static Bitmap cursor = new Bitmap(Kernel.rawCursor);

        public static CGSSurface surface;
        public static TTFFont font;

        public static Color DarkS = Color.FromArgb(15, 15, 15);
        public static Color Dark = Color.FromArgb(30,30,30);
        public static Color DarkM = Color.FromArgb(40, 40, 40);
        public static Color DarkL = Color.FromArgb(45, 45, 45);
        public static Color DarkXL = Color.FromArgb(50, 50, 50);

        public static MouseState MouseState;
        public static MouseState prevMouseState;

        public static bool once = true;
        public static int start;
        public static int Count;
        public static int FPS;

        public static int MouseX = 0;
        public static int MouseY = 0;

        public static bool CanContinue = false;
        public static bool OnlyWindowsMouse = false;

        public static void Init(int w, int h)
        {
            MouseManager.ScreenWidth = (uint)w;
            MouseManager.ScreenHeight = (uint)h;
            surface = new CGSSurface(Kernel.Canvas);
            font = new TTFFont(Kernel.rawFontUbuntu);
            CanContinue = true;
            Welcome.Start();
        }

        public static void Update()
        {
            try
            {
                if (CanContinue)
                {
                    if (MouseManager.X != MouseX)
                    {
                        MouseX = (int)MouseManager.X;
                    }
                    if (MouseManager.Y != MouseY)
                    {
                        MouseY = (int)MouseManager.Y;
                    }

                    if (once) { start = Cosmos.HAL.RTC.Hour * 3600 + Cosmos.HAL.RTC.Minute * 60 + Cosmos.HAL.RTC.Second + 1; once = false; }
                    if (start == ((Cosmos.HAL.RTC.Hour * 3600 + Cosmos.HAL.RTC.Minute * 60 + Cosmos.HAL.RTC.Second))) { once = true; FPS = Count; Count = 0; }

                    if (UAS.ActiveUser.Username != "defaultUser")
                    {
                        Kernel.Canvas.DrawImage(wallpaper, 0, 0);
                        if (OnlyWindowsMouse==false)
                        {
                            Taskbar.Draw();
                            DesktopGrid.Draw();
                        }
                        WindowManager.Update(Kernel.Canvas);
                        //DrawToSurface(surface, 14, 0, 24, FPS.ToString(), Color.Green);
                        Kernel.Canvas.DrawImageAlpha(cursor, MouseX, MouseY);
                        Kernel.Canvas.Display();
                    }
                    else
                    {
                        LogonUI.Draw();
                    }

                    if (Frames == 20)
                    {
                        Heap.Collect();
                        Frames = 0;
                    }
                    Frames++;
                    Count++;
                    prevMouseState = MouseManager.MouseState;
                }
            }
            catch (Exception ex)
            {
                Kernel.DrawStatusForce("GUI: " + ex.Message, Color.Red);
                Thread.Sleep(5000);
            }

        }
        public static int RgbaToHex(int red, int green, int blue, int alpha)
        {
            // Ensure the values are within the valid range
            red = Math.Clamp(red, 0, 255);
            green = Math.Clamp(green, 0, 255);
            blue = Math.Clamp(blue, 0, 255);
            alpha = Math.Clamp(alpha, 0, 255);

            // Combine the values into a single integer
            int hex = (alpha << 24) | (red << 16) | (green << 8) | blue;
            return hex;
        }
        public static (int red, int green, int blue, int alpha) HexToRgba(int hex)
        {
            int alpha = (hex >> 24) & 0xFF;
            int red = (hex >> 16) & 0xFF;
            int green = (hex >> 8) & 0xFF;
            int blue = hex & 0xFF;
            return (red, green, blue, alpha);
        }

        public static void DrawImageAlpha(Cosmos.System.Graphics.Image image, int x, int y)
        {
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color color = Color.FromArgb(image.RawData[i + j * image.Width]);
                    if (color.A > 0)
                        Kernel.Canvas.DrawPoint(color, x + i, y + j);
                }
            }
        }
        public static void DrawToSurface(ITTFSurface surface, int px, int x, int y, string text, Color color)
        {
            int num = 0;
            Rune left = new Rune('\0');
            foreach (Rune item in text.EnumerateRunes())
            {
                GlyphResult? glyphResult = font.RenderGlyphAsBitmap(item, color, px);
                if (glyphResult.HasValue)
                {
                    GlyphResult value = glyphResult.Value;
                    font.GetGlyphHMetrics(item, px, out var advWidth, out var lsb);
                    font.GetKerning(left, item, px, out var kerning);
                    num += lsb + kerning;
                    surface.DrawBitmap(value.bmp, x + num, y + value.offY + px);
                    num = ((kerning <= 0) ? (num + (advWidth - lsb)) : (num - lsb));
                    left = item;
                }
            }
        }
    }

    public class Taskbar
    {
        static Bitmap logo30 = new Bitmap(Kernel.rawLogo30);
        public static int BottomTaskbarHeight = 42;
        public static void Draw()
        {
            int X = BottomTaskbarHeight + 2;

            Kernel.Canvas.DrawFilledRectangle(Desktop.Dark,0,(int)Kernel.Canvas.Mode.Height - BottomTaskbarHeight,(int)Kernel.Canvas.Mode.Width, BottomTaskbarHeight); // Bottom taskbar

            Kernel.Canvas.DrawFilledRectangle(Desktop.Dark, 0, 0, (int)Kernel.Canvas.Mode.Width, 24); // Top taskbar
            Kernel.Canvas.DrawImage(Kernel.User, 0, 0,24,24);
            Kernel.Canvas.DrawImage(Kernel.Control, (int)(Kernel.Mode.Width - 26), 0,24,24);
            Desktop.DrawToSurface(Desktop.surface, 20, 24, -2, UAS.ActiveUser.Username, Color.GhostWhite);
            Desktop.DrawToSurface(Desktop.surface, 20,(int)(Kernel.Mode.Width - 86), -2, RTC.Hour + ":" + RTC.Minute, Color.White);

            Kernel.Canvas.DrawImage(Kernel.logo512,0,(int)Kernel.Canvas.Mode.Height - BottomTaskbarHeight, BottomTaskbarHeight,BottomTaskbarHeight); // Start button

            if(MouseEx.IsMouseWithin(0, (int)Kernel.Canvas.Mode.Height - BottomTaskbarHeight, (ushort)BottomTaskbarHeight, (ushort)BottomTaskbarHeight) && Desktop.MouseState == MouseState.Left && Desktop.prevMouseState != MouseState.Left)
            {
                Menu.Start();
            }

            for (int j = 0; j < WindowManager.Windows.Count; j++) // Horrible app process... Dont look
            {
                if (WindowManager.Selected == WindowManager.Windows[j].Title)
                {
                    Kernel.Canvas.DrawFilledRectangle(System.Drawing.Color.Gray, X, (Int32)(Kernel.Canvas.Mode.Height - BottomTaskbarHeight), BottomTaskbarHeight, BottomTaskbarHeight);
                }
                Kernel.Canvas.DrawImage(WindowManager.Windows[j].Icon, X + 2, (int)(Kernel.Canvas.Mode.Height - BottomTaskbarHeight + 2),BottomTaskbarHeight - 4,BottomTaskbarHeight - 4);
                if (MouseEx.IsMouseWithin(X, (int)(Kernel.Canvas.Mode.Height - BottomTaskbarHeight), (ushort)BottomTaskbarHeight, (ushort)BottomTaskbarHeight) && MouseManager.MouseState == MouseState.Left && Desktop.prevMouseState != MouseState.Left)
                {
                    WindowManager.Selected = WindowManager.Windows[j].Title;
                }
                X = X + BottomTaskbarHeight;
            }
        }
    }

    public class DesktopGrid
    {
        public static List<GridItem> gridItems = new List<GridItem>();
        public static int offset = 3;
        public static int UPoffset = 48;
        //public static string[] Files;
        public static void Draw()
        {
            //One item 86x86
            for (int item = 0; item < gridItems.Count; item++)
            {
                //Kernel.Canvas.DrawRectangle(Color.Azure, gridItems[item].x * 48, gridItems[item].y * 64 + UPoffset, 48, 64);
                Kernel.Canvas.DrawImage(gridItems[item].image, gridItems[item].x * 48 + offset, gridItems[item].y * 64 + UPoffset + offset,48 - offset*2,48 - offset*2);
                Desktop.DrawToSurface(Desktop.surface, 14, gridItems[item].x * 48 + (24 - ((gridItems[item].Title.Length / 2) * 8)), gridItems[item].y * 64 + UPoffset + 48, gridItems[item].Title.ToString(), Color.WhiteSmoke);
            }
        }
    }
    public class GridItem
    {
        public string Title;
        public Bitmap image;
        public int x, y;

        public GridItem(string title, Bitmap image, int x, int y)
        {
            Title = title;
            this.image = image;
            this.x = x; this.y = y;
        }
    }

    public class LogonUI
    {
        public static void Draw()
        {
            WindowManager.Update(Kernel.Canvas);
            Kernel.Canvas.DrawImageAlpha(Desktop.cursor, (int)MouseManager.X, (int)MouseManager.Y);
            Kernel.Canvas.Display();
        }
    }
}
