using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ShardOS.UI.Controls
{
    public class Image : Control
    {
        internal string InternalContents;

        public Bitmap image;

        public bool IsClicked = false;

        public int W;
        public int H;

        public int Xpos;
        public int Ypos;
        public bool Alpha = false;
        public string Text = "";

        public Image(int X, int Y,int W,int H, Bitmap img, bool alpha, string title = "")
            : base(X, Y, 0, 0)
        {
            this.W = W;
            this.H = H;
            this.Xpos = X;
            this.Ypos = Y;
            image = img;
            this.Alpha = alpha;
            this.Text = title;
        }

        public override void Update(Canvas Canvas, int X, int Y, bool sel)
        {
            if (Alpha)
            {
                Desktop.DrawImageAlpha(image,X + Xpos,Y + Ypos);
            }
            else
            {
                Kernel.Canvas.DrawImage(image, X + Xpos, Y + Ypos, W, H);
            }

            if (MouseEx.IsMouseWithin(X + Xpos, Y + Ypos, (ushort)W,(ushort)H))
            {
                if(Text != "")
                {
                    Kernel.Canvas.DrawFilledRectangle(System.Drawing.Color.DarkGray,(int)(X + Xpos -((Text.Length /2 ) * 8) + (image.Height / 2)) ,(int)(Y + Ypos + image.Height - 16), Text.Length * 8, 16);
                    Desktop.DrawToSurface(Desktop.surface, Kernel.DefaultFontHeight, (int)(X + Xpos - ((Text.Length / 2) * 8) + (image.Height / 2)), (int)(Y + Ypos + image.Height - 16),Text, Color.White);
                }
                if (MouseManager.MouseState == MouseState.Left && sel)
                {
                    IsClicked = true;
                    
                }
                else
                {
                    IsClicked = false;
                }
            }
            else
            {
                IsClicked = false;
            }
        }
    }
}
