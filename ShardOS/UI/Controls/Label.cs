using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShardOS.UI.Controls
{
    public class Label : Control
    {
        internal string InternalContents;

        public string Text = "";

        public int Xpos;
        public int Ypos;
        public int Px;


        public Label(int X, int Y, int Px, string Contents)
            : base(X, Y, 0, 0)
        {
            this.Xpos = X;
            this.Ypos = Y;
            this.Text = Contents;
            this.Px = Px;
        }

        public override void Update(Canvas Canvas, int X, int Y, bool sel)
        {
            Desktop.DrawToSurface(Desktop.surface, Kernel.DefaultFontHeight, (X + Xpos), (Y + Ypos), Text, Color.Black);
        }
    }
}
