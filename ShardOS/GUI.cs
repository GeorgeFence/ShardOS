using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using CosmosTTF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShardOS
{
    public class Desktop
    {
        static int Frames = 0;
        static Bitmap wallpaper = new Bitmap(Kernel.rawWallpaper);
        static Bitmap cursor = new Bitmap(Kernel.rawCursor);

        public static CGSSurface surface;
        public static TTFFont font;

        public static Color Dark = Color.FromArgb(30,30,30);

        public static MouseState prevMouseState;

        private static int Count = 0;
        static bool once = true;
        static int start = 0;
        static int fps = 0;
        private static int FPS;

        public static void Init(int w, int h)
        {
            MouseManager.ScreenWidth = (uint)w;
            MouseManager.ScreenHeight = (uint)h;
            surface = new CGSSurface(Kernel.Canvas);
            font = new TTFFont(Kernel.rawFontUbuntu);
        }

        public static void Update()
        {
            try
            {
                if (once) { start = Cosmos.HAL.RTC.Hour * 3600 + Cosmos.HAL.RTC.Minute * 60 + Cosmos.HAL.RTC.Second + 1; once = false; }
                if (start == ((Cosmos.HAL.RTC.Hour * 3600 + Cosmos.HAL.RTC.Minute * 60 + Cosmos.HAL.RTC.Second))) { once = true; fps = Count; FPS = fps; Count = 0; DesktopGrid.Files = Files.GetFiles("0:\\Users\\" + UAS.ActiveUser.Username + "\\"); }

                Kernel.Canvas.DrawImage(wallpaper, 0, 0);
                Taskbar.Draw();
                DesktopGrid.Draw();
                WindowManager.Update(Kernel.Canvas);
                DrawToSurface(surface, 14, 0, 24, FPS.ToString(), Color.Green);
                Kernel.Canvas.DrawImageAlpha(cursor, (int)MouseManager.X, (int)MouseManager.Y);
                Kernel.Canvas.Display();
                if (Frames == 4)
                {
                    Heap.Collect();
                    Frames = 0;
                }
                Frames++;
                Count++;

                prevMouseState = MouseManager.MouseState;
            }
            catch (Exception ex)
            {
                Kernel.DrawStatusForce("GUI: " + ex.Message, Color.Red);
                Thread.Sleep(5000);
            }

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
        public static string LastApp = "";
        public static int start = 0;
        public static void Draw()
        {
            int X = 34;

            Kernel.Canvas.DrawFilledRectangle(Desktop.Dark,0,(int)Kernel.Canvas.Mode.Height - 30,(int)Kernel.Canvas.Mode.Width, 30); // Bottom taskbar

            Kernel.Canvas.DrawFilledRectangle(Desktop.Dark, 0, 0, (int)Kernel.Canvas.Mode.Width, 24); // Top taskbar
            Kernel.Canvas.DrawImageAlpha(Kernel.User24, 0, 0);
            Kernel.Canvas.DrawImageAlpha(Kernel.Settings24, (int)(Kernel.Mode.Width - 24), 0);
            Desktop.DrawToSurface(Desktop.surface, 20, 24, -2, UAS.ActiveUser.Username, Color.GhostWhite);
            Desktop.DrawToSurface(Desktop.surface, 20,(int)(Kernel.Mode.Width - 86), -2, RTC.Hour + ":" + RTC.Minute, Color.White);

            Kernel.Canvas.DrawImageAlpha(logo30,0,(int)Kernel.Canvas.Mode.Height - 30); // Start button

            for (int j = 0; j < WindowManager.Windows.Count; j++) // Horrible app process... Dont look
            {
                if (WindowManager.Selected == WindowManager.Windows[j].Title)
                {
                    Kernel.Canvas.DrawFilledRectangle(System.Drawing.Color.Gray, X - 3, (Int32)(Kernel.Canvas.Mode.Height - 30), 30, 30);
                }
                Kernel.Canvas.DrawImageAlpha(WindowManager.Windows[j].Icon, X - 1, (int)(Kernel.Canvas.Mode.Height - 28));
                if (MouseEx.IsMouseWithin(X, (int)(Kernel.Canvas.Mode.Height - 30), 30, 30) && MouseManager.MouseState == MouseState.Left && Desktop.prevMouseState != MouseState.Left)
                {
                    WindowManager.Selected = WindowManager.Windows[j].Title;
                }
                if (MouseEx.IsMouseWithin(X, (int)(Kernel.Canvas.Mode.Height - 28), 30, 30) && MouseManager.MouseState == MouseState.Right && Desktop.prevMouseState != MouseState.Right)
                {
                    LastApp = WindowManager.Windows[j].Title;
                    start = (Cosmos.HAL.RTC.Hour * 3600 + Cosmos.HAL.RTC.Minute * 60 + Cosmos.HAL.RTC.Second);

                }
                else if (MouseEx.IsMouseWithin(X, (int)(Kernel.Canvas.Mode.Height - 28), 30, 30) && MouseManager.MouseState == MouseState.None && !((start + 3) > ((Cosmos.HAL.RTC.Hour * 3600 + Cosmos.HAL.RTC.Minute * 60 + Cosmos.HAL.RTC.Second))))
                {
                    Kernel.Canvas.DrawFilledRectangle(System.Drawing.Color.FromArgb(25, 25, 25), (X + 13) - ((WindowManager.Windows[j].Title.Length * 8) / 2) - 2, (int)(Kernel.Canvas.Mode.Height - 52), (WindowManager.Windows[j].Title.Length * 8) + 4, 18);
                    Desktop.DrawToSurface(Desktop.surface, Kernel.DefaultFontHeight, (X + 13) - ((WindowManager.Windows[j].Title.Length * 8) / 2), (int)(Kernel.Mode.Height - 53), WindowManager.Windows[j].Title, Color.White);
                }
                if (LastApp == WindowManager.Windows[j].Title && (start + 3) > ((Cosmos.HAL.RTC.Hour * 3600 + Cosmos.HAL.RTC.Minute * 60 + Cosmos.HAL.RTC.Second)))
                {
                    Kernel.Canvas.DrawFilledRectangle(System.Drawing.Color.FromArgb(25, 25, 25), X, (int)(Kernel.Canvas.Mode.Height - 80), 32, 32);
                    Kernel.Canvas.DrawRectangle(System.Drawing.Color.Red, X, (int)(Kernel.Canvas.Mode.Height - 80), 32, 32);
                    Kernel.Canvas.DrawImageAlpha(Kernel.ExitApp, X + 5, (int)(Kernel.Canvas.Mode.Height - 74));
                    if (MouseManager.MouseState == MouseState.Left && Desktop.prevMouseState != MouseState.Left && MouseEx.IsMouseWithin(X, (int)(Kernel.Canvas.Mode.Height - 80), 26, 26))
                    {
                        WindowManager.Stop(WindowManager.Windows[j]);
                    }
                }
                X = X + 30;
            }
        }
    }

    public class DesktopGrid
    {
        public static List<GridItem> gridItems = new List<GridItem>();
        public static string[] Files;
        public static void Draw()
        {
            //One item 86x86
            for (int item = 0; item < gridItems.Count; item++)
            {
                //Kernel.Canvas.DrawRectangle(Color.Azure, gridItems[item].x * 86, gridItems[item].y * 86, 86, 86);
                Kernel.Canvas.DrawImageAlpha(gridItems[item].image, gridItems[item].x * 86 + 11, gridItems[item].y * 86 + 32);
                Desktop.DrawToSurface(Desktop.surface, 14, gridItems[item].x * 86 + (43 - ((gridItems[item].Title.Length / 2) * 8)), gridItems[item].y * 86 + 64 + 32, gridItems[item].Title.ToString(), Color.WhiteSmoke);
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
}
