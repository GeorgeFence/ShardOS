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
        if(type == DesignType.Default)
        {
            WindowSkelet = new Bitmap((uint)(Width + 1), (uint)(Height + 1), ColorDepth.ColorDepth32);
            BitmapDraws.DrawFilledRoundedRectangle(WindowSkelet, 0, 0, Width, Height, 5, Color.FromArgb(30, 32, 48));
            BitmapDraws.DrawFilledRoundedRectangle(WindowSkelet, 3, 24, Width - 6, Height - 27, 5, Color.FromArgb(24, 25, 38));
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

                        Kernel.Canvas.DrawImageAlpha(WindowSkelet,base.X,base.Y);

                        ProcessControls(base.X + 3, base.Y + 24, Controls, KeyboardEx.k, sel);
                        Desktop.DrawToSurface(Desktop.surface, Kernel.DefaultFontHeight + 2, base.X + 3, base.Y + 14 - ((Kernel.DefaultFontHeight + 2) / 2), Title, Color.White);
                        break;

                    case DesignType.Classic:
                        if (Ptype == PermissionsType.User)
                        {
                            Canvas.DrawFilledRectangle(System.Drawing.Color.SteelBlue, base.X, base.Y, 0, 32);
                        }
                        else if (Ptype == PermissionsType.System)
                        {
                            Canvas.DrawFilledRectangle(System.Drawing.Color.Red, base.X, base.Y, 0, 32);
                        }
                        else if (Ptype == PermissionsType.Service)
                        {
                            Canvas.DrawFilledRectangle(System.Drawing.Color.Green, base.X, base.Y, 0, 32);
                        }
                        Canvas.DrawFilledRectangle(System.Drawing.Color.GhostWhite, base.X, base.Y + 32, (ushort)(base.Width - 32), base.Height);
                        ProcessControls(base.X + 6, base.Y + 33, Controls, KeyboardEx.k, sel);
                        Desktop.DrawToSurface(Desktop.surface, Kernel.DefaultFontHeight + 2, base.X + 8, base.Y + (32 / 2) - ((Kernel.DefaultFontHeight + 2) / 2), Title, Color.White);
                        break;

                    case DesignType.Modern:
                        if (Ptype == PermissionsType.User)
                        {

                            Canvas.DrawFilledRectangle(System.Drawing.Color.FromArgb(0, 0, 255), X, Y, (ushort)w, WinH);
                            Canvas.DrawImage(Kernel.UserApp, X + w, Y);
                            Canvas.DrawImage(Kernel.UserApp, X + w, Y + WinH - 32);
                            Canvas.DrawFilledRectangle(System.Drawing.Color.FromArgb(255, 0, 255), (X + w + 255), Y, (ushort)((WinW - 255) / 2), WinH);
                            Canvas.DrawImageAlpha(Kernel.ExitApp, X + WinW - 27, Y + 5);


                            Canvas.DrawFilledRectangle(System.Drawing.Color.Black, X + 5, Y + 32, (ushort)(WinW - 10), (ushort)(WinH - 37));
                            Canvas.DrawFilledRectangle(System.Drawing.Color.White, X + 6, Y + 33, (ushort)(WinW - 12), (ushort)(WinH - 39));
                            ProcessControls(base.X + 6, base.Y + 33, Controls, KeyboardEx.k, sel);
                            Desktop.DrawToSurface(Desktop.surface, Kernel.DefaultFontHeight, base.X + 5, base.Y + 8, Title, Color.White);
                        }
                        else if (Ptype == PermissionsType.System)
                        {
                            Canvas.DrawFilledRectangle(System.Drawing.Color.FromArgb(255, 0, 0), X, Y, (ushort)w, WinH);
                            Canvas.DrawImage(Kernel.SystemApp, X + w, Y);
                            Canvas.DrawImage(Kernel.SystemApp, X + w, Y + WinH - 32);
                            Canvas.DrawFilledRectangle(System.Drawing.Color.FromArgb(255, 0, 255), (X + w + 255), Y, (ushort)((WinW - 255) / 2), WinH);
                            Canvas.DrawImageAlpha(Kernel.ExitApp, X + WinW - 27, Y + 5);

                            Canvas.DrawFilledRectangle(System.Drawing.Color.Black, X + 5, Y + 32, (ushort)(WinW - 10), (ushort)(WinH - 37));
                            Canvas.DrawFilledRectangle(System.Drawing.Color.White, X + 6, Y + 33, (ushort)(WinW - 12), (ushort)(WinH - 39));
                            ProcessControls(base.X + 6, base.Y + 33, Controls, KeyboardEx.k, sel);
                            Desktop.DrawToSurface(Desktop.surface, Kernel.DefaultFontHeight, base.X + 5, base.Y + 8, Title, Color.White);

                        }
                        else if (Ptype == PermissionsType.Service)
                        {
                            Canvas.DrawFilledRectangle(System.Drawing.Color.FromArgb(0, 255, 0), X, Y, (ushort)w, WinH);
                            Canvas.DrawImage(Kernel.ServiceApp, X + w, Y);
                            Canvas.DrawImage(Kernel.ServiceApp, X + w, Y + WinH - 32);
                            Canvas.DrawFilledRectangle(System.Drawing.Color.FromArgb(255, 0, 255), (X + w + 255), Y, (ushort)((WinW - 255) / 2), WinH);
                            Canvas.DrawImageAlpha(Kernel.ExitApp, X + WinW - 27, Y + 5);

                            Canvas.DrawFilledRectangle(System.Drawing.Color.Black, X + 5, Y + 32, (ushort)(WinW - 10), (ushort)(WinH - 37));
                            Canvas.DrawFilledRectangle(System.Drawing.Color.White, X + 6, Y + 33, (ushort)(WinW - 12), (ushort)(WinH - 39));
                            ProcessControls(base.X + 6, base.Y + 33, Controls, KeyboardEx.k, sel);
                            Desktop.DrawToSurface(Desktop.surface, Kernel.DefaultFontHeight, base.X + 5, base.Y + 8, Title, Color.White);

                        }
                        break;

                    case DesignType.Blank:
                        Canvas.DrawFilledRectangle(Background, base.X, base.Y, WinW, WinH);
                        ProcessControls(base.X, base.Y, Controls, KeyboardEx.k, sel);
                        break;

                    case DesignType.LUI:
                        Canvas.DrawImage(backgroundimage, 0, 0);
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
