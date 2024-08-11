using Cosmos.Core.Memory;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using CosmosTTF;
using ShardOS.Apps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;

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
        public static Color Dark = Color.FromArgb(30, 30, 30);
        public static Color DarkM = Color.FromArgb(40, 40, 40);
        public static Color DarkL = Color.FromArgb(45, 45, 45);
        public static Color DarkXL = Color.FromArgb(50, 50, 50);

        public static bool CanContinue = false;
        public static bool OnlyWindowsMouse = false;

        public static bool IsShell = false;

        public static void Init(int w, int h)
        {
            MouseManager.ScreenWidth = (uint)w;
            MouseManager.ScreenHeight = (uint)h;
            surface = new CGSSurface(Kernel.Canvas);
            font = new TTFFont(Kernel.rawFontInter);
            CanContinue = true;
            DesktopGrid.Init();
        }

        public static void Update()
        {
            try
            {
                if (CanContinue)
                {
                    if (IsShell)
                    {
                        Shell.Update();
                        Shell.Draw(0, 0);
                        Kernel.Canvas.Display();
                    }
                    else
                    {
                        if (UAS.ActiveUser.Username != UAS.defaultuser.Username)
                        {
                            Kernel.Canvas.DrawImage(wallpaper, 0, 0);
                            if (!OnlyWindowsMouse)
                            {
                                Taskbar.Draw();
                                DesktopGrid.Draw();
                            }
                            WindowManager.Update(Kernel.Canvas);
                            Desktop.DrawImageAlpha(cursor, (int)MouseManager.X, (int)MouseManager.Y);
                            Kernel.Canvas.Display();
                        }
                        else
                        {
                            LogonUI.Draw();
                        }

                        MouseEx.Mouse();
                    }
                }
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

            Kernel.Canvas.DrawFilledRectangle(Color.FromArgb(24, 25, 38), 0, (int)Kernel.Canvas.Mode.Height - BottomTaskbarHeight, (int)Kernel.Canvas.Mode.Width, BottomTaskbarHeight); // Bottom taskbar

            Kernel.Canvas.DrawFilledRectangle(Color.FromArgb(24, 25, 38), 0, 0, (int)Kernel.Canvas.Mode.Width, 24); // Top taskbar
            Kernel.Canvas.DrawImage(Kernel.User, 0, 0, 24, 24);
            Kernel.Canvas.DrawImage(Kernel.Control, (int)(Kernel.Mode.Width - 26), 0, 24, 24);
            Desktop.DrawToSurface(Desktop.surface, 20, 24, -2, UAS.ActiveUser.Username, Color.GhostWhite);
            int i = 00;
            i += RTC.Minute;
            Desktop.DrawToSurface(Desktop.surface, 20, (int)(Kernel.Mode.Width - 86), -2, RTC.Hour + ":" + i, Color.White);

            if (CobaltCore._fps < 30)
            {
                Desktop.DrawToSurface(Desktop.surface, 20, (int)(Kernel.Mode.Width - 116), -2, CobaltCore._fps.ToString(), Color.Red);
            }
            if (CobaltCore._fps >= 30)
            {
                Desktop.DrawToSurface(Desktop.surface, 20, (int)(Kernel.Mode.Width - 116), -2, CobaltCore._fps.ToString(), Color.Green);
            }
            Kernel.Canvas.DrawImage(Kernel.logo512, 0, (int)Kernel.Canvas.Mode.Height - BottomTaskbarHeight, BottomTaskbarHeight, BottomTaskbarHeight); // Start button

            if (MouseEx.IsMouseWithin(0, (int)Kernel.Canvas.Mode.Height - BottomTaskbarHeight, (ushort)BottomTaskbarHeight, (ushort)BottomTaskbarHeight)&& MouseEx.LeftClick)
            {
                Menu.Start();
            }

            for (int j = 0; j < WindowManager.Windows.Count; j++) // Horrible app process... Dont look
            {
                if (WindowManager.Selected == WindowManager.Windows[j].Title)
                {
                    Kernel.Canvas.DrawFilledRectangle(System.Drawing.Color.Gray, X, (Int32)(Kernel.Canvas.Mode.Height - BottomTaskbarHeight), BottomTaskbarHeight, BottomTaskbarHeight);
                }
                Kernel.Canvas.DrawImage(WindowManager.Windows[j].Icon, X + 2, (int)(Kernel.Canvas.Mode.Height - BottomTaskbarHeight + 2), BottomTaskbarHeight - 4, BottomTaskbarHeight - 4);
                if (MouseEx.IsMouseWithin(X, (int)(Kernel.Canvas.Mode.Height - BottomTaskbarHeight), (ushort)BottomTaskbarHeight, (ushort)BottomTaskbarHeight) && MouseEx.LeftClick)
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
        public static Bitmap Selected;
        public static int LastSelect;
        //public static string[] Files;

        public static void Init()
        {
            Selected = new Bitmap(48 + 1, 64 + 1, ColorDepth.ColorDepth32);
            BitmapDraws.DrawFilledRoundedRectangle(Selected, 0, 0, 48, 64, 3, Color.White);
            List<int> l = new List<int>();
            for (int i = 0; i < (Selected.Width * Selected.Height); i++)
            {
                if (Selected.RawData[i] != 0)
                {
                    l.Add(Desktop.RgbaToHex(138, 173, 244, 190));
                }
                else
                {
                    l.Add(0);
                }
            }
            Selected.RawData = l.ToArray();
        }
        public static void Draw()
        {
            //One item 48x64
            for (int item = 0; item < gridItems.Count; item++)
            {
                if (gridItems[item].Selected == true)
                {
                    Desktop.DrawImageAlpha(Selected, gridItems[item].x * 48, gridItems[item].y * 64 + UPoffset);
                    //Kernel.Canvas.DrawRectangle(Color.Blue, gridItems[item].x * 48, gridItems[item].y * 64 + UPoffset, 48, 64);
                }
                //Kernel.Canvas.DrawRectangle(Color.Azure, gridItems[item].x * 48, gridItems[item].y * 64 + UPoffset, 48, 64);
                Kernel.Canvas.DrawImage(gridItems[item].image, gridItems[item].x * 48 + offset, gridItems[item].y * 64 + UPoffset + offset, 48 - offset * 2, 48 - offset * 2);
                Desktop.DrawToSurface(Desktop.surface, 14, gridItems[item].x * 48 + (24 - ((gridItems[item].Title.Length / 2) * 8)), gridItems[item].y * 64 + UPoffset + 48, gridItems[item].Title.ToString(), Color.WhiteSmoke);
                if (MouseEx.IsMouseWithin(gridItems[item].x * 48, gridItems[item].y * 64 + UPoffset, 48, 64) && MouseEx.LeftClick && gridItems[item].Selected)
                {
                    gridItems[item].Selected = false;
                    Execute(gridItems[item].ExecutionID);
                }
                if (MouseEx.IsMouseWithin(gridItems[item].x * 48, gridItems[item].y * 64 + UPoffset, 48, 64) && MouseEx.LeftClick)
                {
                    foreach (GridItem i in gridItems)
                    {
                        i.Selected = false;
                    }
                    gridItems[item].Selected = true;
                }

            }
        }

        public static void Execute(int ExecutionID)
        {
            switch (ExecutionID)
            {
                case 0:
                    //nothing
                    break;
                case 1:
                    //Shell
                    ShellApp.Start();
                    break;
                case 2:
                    //Welcome
                    Welcome.Start();
                    break;
                case 3:

                    break;
                case 4:

                    break;
            }

        }
    }
    public class GridItem
    {
        public string Title;
        public Bitmap image;
        public int x, y;
        public bool Selected = false;
        public int ExecutionID;

        public GridItem(string title, Bitmap image, int x, int y, int executionID)
        {
            Title = title;
            this.image = image;
            this.x = x; this.y = y;
            ExecutionID = executionID;
        }
    }

    public class LogonUI
    {
        public static void Draw()
        {
            WindowManager.Update(Kernel.Canvas);
            Desktop.DrawImageAlpha(Desktop.cursor, (int)MouseManager.X, (int)MouseManager.Y);
            Kernel.Canvas.Display();
        }
    }

    public static class ASC16
    {
        static string ASC16Base64 = "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AAB4zMzM2MzGxsbMAAAAAAAA/sbGwMDAwMDAwAAAAAAAAAAA/mxsbGxsbGwAAAAAAAAA/sZgMBgwYMb+AAAAAAAAAAAAftjY2NjYcAAAAAAAAAAAZmZmZmZ8YGDAAAAAAAAAAHbcGBgYGBgYAAAAAAAAAH4YPGZmZjwYfgAAAAAAAAA4bMbG/sbGbDgAAAAAAAA4bMbGxmxsbGzuAAAAAAAAHjAYDD5mZmZmPAAAAAAAAAAAAH7b29t+AAAAAAAAAAAAAwZ+29vzfmDAAAAAAAAAHDBgYHxgYGAwHAAAAAAAAAB8xsbGxsbGxsYAAAAAAAAAAP4AAP4AAP4AAAAAAAAAAAAYGH4YGAAA/wAAAAAAAAAwGAwGDBgwAH4AAAAAAAAADBgwYDAYDAB+AAAAAAAADhsbGBgYGBgYGBgYGBgYGBgYGBgYGNjY2HAAAAAAAAAAABgYAH4AGBgAAAAAAAAAAAAAdtwAdtwAAAAAAAAAOGxsOAAAAAAAAAAAAAAAAAAAAAAAABgYAAAAAAAAAAAAAAAAAAAAGAAAAAAAAAAADwwMDAwM7GxsPBwAAAAAANhsbGxsbAAAAAAAAAAAAABw2DBgyPgAAAAAAAAAAAAAAAAAfHx8fHx8fAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";
        static MemoryStream ASC16FontMS = new MemoryStream(Convert.FromBase64String(ASC16Base64));

        public static void DrawString(string s, System.Drawing.Color color, uint x, uint y)
        {
            string[] lines = s.Split('\n');
            for (int l = 0; l < lines.Length; l++)
            {
                for (int c = 0; c < lines[l].Length; c++)
                {
                    int offset = (Encoding.ASCII.GetBytes(lines[l][c].ToString())[0] & 0xFF) * 16;
                    ASC16FontMS.Seek(offset, SeekOrigin.Begin);
                    byte[] fontbuf = new byte[16];
                    ASC16FontMS.Read(fontbuf, 0, fontbuf.Length);

                    for (int i = 0; i < 16; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if ((fontbuf[i] & (0x80 >> j)) != 0)
                            {
                                Kernel.Canvas.DrawFilledRectangle(color, (int)((x + j) + (c * 8)), (int)(y + i + (l * 16)), 1, 1);
                            }
                        }
                    }
                }
            }
        }
        public static void DrawStringShellEngine(string s, System.Drawing.Color color, uint x, uint y)
        {
            string[] lines = s.Split('\n');
            for (int l = 0; l < lines.Length; l++)
            {
                for (int c = 0; c < lines[l].Length; c++)
                {
                    int offset = (Encoding.ASCII.GetBytes(lines[l][c].ToString())[0] & 0xFF) * 16;
                    ASC16FontMS.Seek(offset, SeekOrigin.Begin);
                    byte[] fontbuf = new byte[16];
                    ASC16FontMS.Read(fontbuf, 0, fontbuf.Length);

                    for (int i = 0; i < 16; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if ((fontbuf[i] & (0x80 >> j)) != 0)
                            {
                                ShellEngine.DrawFilledRectangle((int)((x + j) + (c * 8)), (int)(y + i + (l * 16)), 1, 1, color);
                            }
                        }
                    }
                }
            }
        }
    }
}
