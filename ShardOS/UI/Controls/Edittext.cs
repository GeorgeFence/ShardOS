﻿using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Console = System.Console;

namespace ShardOS.UI.Controls
{
    public class Edittext : Control
    {
        internal string InternalContents;

        public string Text = "";

        public bool Selected = false;

        public int Xpos;
        public int Ypos;

        public int W;
        public int H;

        public Color bg = Color.White;
        public Color fg = Color.Black;

        public Edittext(int X, int Y, int W, int H)
            : base(X, Y, 0, 0)
        {
            this.Xpos = X;
            this.Ypos = Y;
            this.W = W;
            this.H = H;
        }

        public override void Update(Canvas Canvas, int X, int Y, bool sel)
        {
            Canvas.DrawFilledRectangle(bg, Xpos + X, Ypos + Y, W, H);
            Canvas.DrawRectangle(System.Drawing.Color.Black, Xpos + X, Ypos + Y, W, H);
            if (MouseEx.IsMouseWithin(Xpos + X, Ypos + Y, (ushort)W,(ushort)H))
            {
                if(  MouseEx.LeftClick)
                {
                    Selected = true;
                }
            }
            if (!sel)
            {
                Selected = false;
            }

            //Keyboard Input
            if (Selected && KeyboardEx.IsKeyPressed)
            {
                if (KeyboardEx.k.Key == ConsoleKey.Backspace && Text != "")
                {
                    Text = Text.Substring(0, Text.Length - 1);
                }
                else if(KeyboardEx.k.Key != ConsoleKey.Enter)
                {
                    Text += KeyboardEx.k.KeyChar.ToString();
                }
            }
            
            //Scroll
            if (Text.Length > (W / 8 - 3))
            {
                string s = " ";
                for (int i = MouseManager.ScrollDelta; i < Text.Length; i++)
                {
                    if (MouseManager.ScrollDelta < 0)
                    {
                        if (i - MouseManager.ScrollDelta < (W / 8 - 3))
                        {
                            s = s + Text[i];
                        }
                    }
                }
                Desktop.DrawToSurface(Desktop.surface, Kernel.DefaultFontHeight, (X + Xpos + 3), (Y + Ypos + 3), s, fg);
                Canvas.DrawFilledRectangle(System.Drawing.Color.Blue,(X + Xpos + (W - (Text.Length -(W / 8)))), (Y + Ypos + H - 2),20, 2);
            }
            else
            {
                Desktop.DrawToSurface(Desktop.surface, Kernel.DefaultFontHeight, (X + Xpos + 3), (Y + Ypos + 3), Text, fg);
            }

            if (Selected)
            {
                Canvas.DrawFilledRectangle(System.Drawing.Color.DimGray,(X + Xpos + 3) + ((Text.Length) * 6), (Y + Ypos + 3), 2,16);
            }
        }
    }
}
