using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShardOS
{
    public static class MouseEx
    {
        public static bool LeftClick = false;
        public static bool RightClick = false;

        public static int OldX = 0;
        public static int OldY = 0;
        public static Bitmap mouse;

        public static bool IsClickFired
        {
            get
            {
                if (MouseManager.MouseState == MouseState.None)
                {
                    return MouseManager.LastMouseState != MouseState.None;
                }

                return false;
            }
        }

        public static bool IsClicked => MouseManager.MouseState != MouseState.None;
        public static void Init()
        {
            mouse = Kernel.Canvas.GetImage((int)MouseManager.X, (int)MouseManager.Y, 12, 19);
            BitmapDraws.DrawImageAlpha(mouse, Desktop.cursor, 0, 0);
        }
        public static void Eficcient()
        {
            if (OldX != MouseManager.X)
            {
                OldX = (int)MouseManager.X;
                mouse = Kernel.Canvas.GetImage((int)MouseManager.X, (int)MouseManager.Y, 12, 19);
                BitmapDraws.DrawImageAlpha(mouse, Desktop.cursor, 0, 0);
            }
            if (OldY != MouseManager.Y)
            {
                OldY = (int)MouseManager.Y;
                mouse = Kernel.Canvas.GetImage((int)MouseManager.X, (int)MouseManager.Y, 12, 19);
                BitmapDraws.DrawImageAlpha(mouse, Desktop.cursor, 0, 0);
            }
            Kernel.Canvas.DrawImage(mouse, (int)MouseManager.X, (int)MouseManager.Y);
        }
        public static void Mouse()
        {
            if( MouseManager.MouseState == MouseState.Left && MouseManager.LastMouseState != MouseState.Left)
            {
                LeftClick = true;
            }
            else { LeftClick = false; }
            if (MouseManager.MouseState == MouseState.Right && MouseManager.LastMouseState != MouseState.Right)
            {
                RightClick = true;
            }
            else { RightClick = false; }
        }

        public static bool IsMouseWithin(int X, int Y, ushort Width, ushort Height)
        {
            checked
            {
                if (MouseManager.X >= X && MouseManager.X <= X + unchecked((int)Width) && MouseManager.Y >= Y)
                {
                    return MouseManager.Y <= Y + unchecked((int)Height);
                }

                return false;
            }
        }
    }
}
