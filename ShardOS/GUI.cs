using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using CosmosTTF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
            if (once) { start = Cosmos.HAL.RTC.Hour * 3600 + Cosmos.HAL.RTC.Minute * 60 + Cosmos.HAL.RTC.Second + 1; once = false; }
            if (start == ((Cosmos.HAL.RTC.Hour * 3600 + Cosmos.HAL.RTC.Minute * 60 + Cosmos.HAL.RTC.Second))) { once = true; fps = Count; FPS = fps; Count = 0; }

            Kernel.Canvas.DrawImage(wallpaper, 0, 0);
            Taskbar.Draw();
            DesktopGrid.Draw();
            DrawToSurface(surface, 14, 0, 0, FPS.ToString(), Color.Green);
            Kernel.Canvas.DrawImageAlpha(cursor, (int)MouseManager.X, (int)MouseManager.Y);
            Kernel.Canvas.Display();
            if (Frames == 4)
            {
                Heap.Collect();
                Frames = 0;
            }
            Frames++;
            Count++;
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
        public static void Draw()
        {
            Kernel.Canvas.DrawFilledRectangle(Desktop.Dark,0,(int)Kernel.Canvas.Mode.Height - 30,(int)Kernel.Canvas.Mode.Width, 30);
            Kernel.Canvas.DrawImageAlpha(logo30,0,(int)Kernel.Canvas.Mode.Height - 30);
        }
    }

    public class DesktopGrid
    {
        public static List<GridItem> gridItems = new List<GridItem>();
        public static void Draw()
        {
            string[] files = Files.GetFiles("0:\\Users\\" + UAS.ActiveUser.Username + "\\");
            //One item 86x86
            for (int item = 0; item < gridItems.Count; item++)
            {
                Kernel.Canvas.DrawRectangle(Color.Azure, gridItems[item].x * 86, gridItems[item].y * 86, 86, 86);
                Kernel.Canvas.DrawImageAlpha(gridItems[item].image, gridItems[item].x * 86 + 11, gridItems[item].y * 86 + 6);
                Desktop.DrawToSurface(Desktop.surface, 14, gridItems[item].x * 86 + (43 - ((gridItems[item].Title.Length / 2) * 8)), gridItems[item].y * 86 + 64, gridItems[item].Title.ToString(), Color.WhiteSmoke);
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
