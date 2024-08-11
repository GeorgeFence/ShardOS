using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Console = System.Console;

namespace ShardOS.UI.Controls
{
    public class RichEdittext : Control
    {
        internal string InternalContents;

        public List<string> text = new List<string>();

        public bool Selected = false;

        public int Xpos;
        public int Ypos;

        public int W;
        public int H;


        public RichEdittext(int X, int Y, int W, int H)
            : base(X, Y, 0, 0)
        {
            this.Xpos = X;
            this.Ypos = Y;
            this.W = W;
            this.H = H;
            text.Add("");
        }

        public override void Update(Canvas Canvas, int X, int Y, bool sel)
        {
            Canvas.DrawFilledRectangle(System.Drawing.Color.White, Xpos + X, Ypos + Y, W, H);
            Canvas.DrawRectangle(System.Drawing.Color.Black, Xpos + X, Ypos + Y, W, H);;
            if (MouseEx.IsMouseWithin(Xpos + X, Ypos + Y, (ushort)W,(ushort)H))
            {
                if(MouseEx.LeftClick)
                {
                    Selected = true;
                }
            }
            if (!sel)
            {
                Selected = false;
            }
            if (Key.KeyPressed && Selected)
            {
                if(Key.keyevent.Key == ConsoleKey.Backspace)
                {
                    if(text[text.Count - 1] != "")
                    {
                        text[text.Count - 1] = text[text.Count - 1].Remove(text[text.Count - 1].Length - 1);
                    }
                    else if(text[text.Count - 1] == "" && (text.Count - 1) != 0)
                    {
                        text.RemoveAt(text.Count - 1);
                    }
                }
                else if (Key.keyevent.Key == ConsoleKey.Enter)
                {
                    text.Add("");
                }
                else
                {
                    text[text.Count - 1] = text[text.Count - 1] + Key.keyevent.KeyChar.ToString();
                }
            }
            if(text.Count > ((H - 3) / 16))
            {
                int y = 0;
                for (int i = 0; i < (H / 16); i++)
                {
                    y++;
                    Desktop.DrawToSurface(Desktop.surface, Kernel.DefaultFontHeight, (X + Xpos + 3), (Y + Ypos + 3 + (y * 16)), text[i], Color.Black);
                }
            }
            else
            {
                for (int i = 0; i < text.Count; i++)
                {
                    Desktop.DrawToSurface(Desktop.surface, Kernel.DefaultFontHeight, (X + Xpos + 3), (Y + Ypos + 3 + (i * 16)), text[i], Color.Black);
                }
            }

            if (Selected)
            {
                Canvas.DrawFilledRectangle(System.Drawing.Color.DimGray,(X + Xpos + 3) + ((text[text.Count - 1]).Length * 8), (Y + Ypos + 3 + ((text.Count - 1) * 16)), 2,16);
            }
        }
    }
}
