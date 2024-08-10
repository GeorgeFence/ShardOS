using Cosmos.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Diagnostics;
using IL2CPU.API.Attribs;
using Cosmos.System.Graphics;

namespace ShardOS.UI;  

public class Window : Control
{

    public readonly List<Control> ShelfControls;

    public List<Control> Controls;

    internal bool IsMoving;

    public bool CanMove = true;

    public bool IsMaximized = false;

    public bool IsSelected = false;

    public bool IsVisible = true;

    public int ID;

    public string Title = "";

    public int PanelW;
    public int PanelH;
    public int WinW;
    public int WinH;

    internal int IX;
    internal int IY;

    public Bitmap WindowSkelet;
    public Bitmap WindowCloser;

    public Bitmap backgroundimage;

    public Bitmap Icon;
    public DesignType Wtype;
    public PermissionsType Ptype;
    public Window(int X, int Y, ushort Width, ushort Height, string TitleStr, Action action, DesignType type, PermissionsType perType, Bitmap Icon) : base(X, Y, Width, Height, action)
    {
        ShelfControls = new List<Control>();
        Controls = new List<Control>();
        Title = TitleStr;
        if(DesignType.Blank == type)
        {
            PanelW = Width;
            PanelH = Height;
        }
        else if (DesignType.Default == type)
        {
            PanelW = Width - 6;
            PanelH = Height - 27;
        }
        else
        {
            PanelW = Width - 12;
            PanelH = Height - 39;
        }
        WinW = Width;
        WinH = Height;
        Wtype = type;
        Ptype = perType;
        this.Icon = Icon;
        WindowCloser = new Bitmap(25, 25, ColorDepth.ColorDepth32);
        BitmapDraws.DrawFilledRoundedRectangle(WindowCloser, 0, 0, 24, 24, 5, Color.FromArgb(36, 39, 59));
        if (type == DesignType.Default)
        {
            WindowSkelet = new Bitmap((uint)(Width + 1), (uint)(Height + 1), ColorDepth.ColorDepth32);
            BitmapDraws.DrawFilledRoundedRectangle(WindowSkelet, 0, 0, Width, Height, 5, Color.FromArgb(30, 32, 48));
            BitmapDraws.DrawFilledRoundedRectangle(WindowSkelet, 3, 24, Width - 6, Height - 27, 5, Color.FromArgb(36, 39, 59));
        }
        if (type == DesignType.LUI)
        {
            WindowSkelet = new Bitmap((uint)(320 + 1), (uint)(240 + 1), ColorDepth.ColorDepth32);
            BitmapDraws.DrawFilledRoundedRectangle(WindowSkelet, 0, 0, 320, 240, 5, Color.FromArgb(30, 32, 48));
            BitmapDraws.DrawFilledRoundedRectangle(WindowSkelet, 2, 2, 320 - 4, 240 - 4, 5, Color.FromArgb(36, 39, 59));
        }
        if (type == DesignType.Blank)
        {
            WindowSkelet = new Bitmap((uint)(Width + 1), (uint)(Height + 1), ColorDepth.ColorDepth32);
            BitmapDraws.DrawFilledRoundedRectangle(WindowSkelet, 0, 0, Width, Height, 5, Color.FromArgb(30, 32, 48));
            BitmapDraws.DrawFilledRoundedRectangle(WindowSkelet, 2, 2, Width - 4, Height - 4, 5, Color.FromArgb(36, 39, 59));
        }
    }
    public void ProcessControls(int X, int Y, List<Control> Controls, ConsoleKeyInfo? Key, bool sel)
    {
        for (int i = 0; i < Controls.Count; i++)
        {
            Controls[i].Update(Kernel.Canvas, X, Y, sel);
        }
    }

    public override void Update(Canvas Canvas, int X, int Y, bool sel)
    {
        try
        {
            if(IsVisible)
            {
                int w = (WinW - 255) / 2;
                switch (Wtype)
                {
                    case DesignType.Default:

                        Desktop.DrawImageAlpha(WindowSkelet,base.X,base.Y);

                        ProcessControls(base.X + 3, base.Y + 24, Controls, KeyboardEx.k, sel);
                        Desktop.DrawToSurface(Desktop.surface, Kernel.DefaultFontHeight + 2, base.X + 3, base.Y + 2, Title, Color.White);
                        if(MouseEx.IsMouseWithin((int)(base.X + WindowSkelet.Width - 24), base.Y, 24,24))
                        {
                            Desktop.DrawImageAlpha(WindowCloser, (int)(base.X + WindowSkelet.Width - 24), base.Y);
                        }
                        Kernel.Canvas.DrawImage(Kernel.ExitApp, (int)(base.X + WindowSkelet.Width - 21), base.Y + 3,18,18);
                        break;

                    case DesignType.Blank:
                        Desktop.DrawImageAlpha(WindowSkelet, base.X, base.Y);
                        ProcessControls(base.X, base.Y, Controls, KeyboardEx.k, sel);
                        break;

                    case DesignType.LUI:
                        Canvas.DrawImage(backgroundimage, 0, 0);
                        Desktop.DrawImageAlpha(WindowSkelet, base.X + PanelW / 2 - 160, base.Y + PanelH / 2 - 120);
                        ProcessControls(base.X, base.Y, Controls, KeyboardEx.k, sel);
                        break;
                }

                base.act();
            }

        }
        catch (Exception ex)
        {
            
        }
    }
}
