using Cosmos.System;
using Cosmos.System.Graphics;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ShardOS.UI.Controls
{
    public class Button : Control
    {
        private System.Drawing.Color Bg;

        private System.Drawing.Color Bg2;

        private System.Drawing.Color Fg;

        private System.Drawing.Color Outline;

        public bool IsClicked = false;

        private bool Center;


        public string Text;

        public int Xpos;
        public int Ypos;
        public int W;
        public int H;

        public Button(int X, int Y, ushort Width, ushort Height, ushort Radius, string Text, bool Center, System.Drawing.Color Bg,Color BgClick, System.Drawing.Color Fg, System.Drawing.Color Outline)
            : base(X, Y, Width, Height)
        {
            this.Bg = Bg;
            this.Bg2 = BgClick;
            this.Fg = Fg;
            this.Xpos = X;
            this.Ypos = Y;
            this.W = Width;
            this.H = Height;
            this.Outline = Outline;
            base.Radius = Radius;
            this.Text = Text;
            this.Center = Center;
        }


        public override void Update(Canvas canvas, int X, int Y, bool sel)
        {
            canvas.DrawFilledRectangle(Bg, X + Xpos, Y + Ypos, W, H);
            canvas.DrawRectangle(Outline, X + Xpos, Y + Ypos, W, H);
            Desktop.DrawToSurface(Desktop.surface, Kernel.DefaultFontHeight, (X + W / 2 + Xpos) - ((Text.Length / 2) * 8), (Y + H / 4 + Ypos - 4), Text, Fg);
            if (MouseEx.IsMouseWithin(X + Xpos, Y + Ypos, (ushort)W, (ushort)H))
            {
                if (Desktop.MouseState == MouseState.Left && Desktop.prevMouseState != MouseState.Left && sel)
                {
                    IsClicked = true;
                    canvas.DrawFilledRectangle(Bg2, X + Xpos, Y + Ypos, W, H);
                    canvas.DrawRectangle(Outline, X + Xpos, Y + Ypos, W, H);
                    Desktop.DrawToSurface(Desktop.surface, Kernel.DefaultFontHeight, (X + W / 2 + Xpos) - ((Text.Length / 2) * 8), (Y + H / 4 + Ypos - 4), Text, Fg);
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
