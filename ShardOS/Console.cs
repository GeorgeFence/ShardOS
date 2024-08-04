using Cosmos.HAL;
using Cosmos.System.Graphics;
using ShardOS.Apps;
using ShardOS.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ShardOS
{
    public class Shell
    {
        private static int X = 0;
        private static int Y = 0;
        private static int W = 0;
        private static int H = 0;

        public static string TopText = "";

        public static string Text = "";

        public static List<Command> CommandsList = new List<Command>();

        public static void Init(int x, int y, int w, int h)
        {
            X = x; Y = y; W = w; H = h;
            TopText = "~~ ShardOS Shell V1.0 20" + RTC.Year + "/" + RTC.DayOfTheMonth + "/" + RTC.Month + " ~~";
            ShellEngine.shell = new Bitmap((uint)w, (uint)h, ColorDepth.ColorDepth32);
            ASC16.DrawStringShellEngine("Welcome to Shell! ", Color.White, 5, 2);
            ASC16.DrawStringShellEngine(TopText, Color.White, (ShellEngine.shell.Width / 2) - (uint)((TopText.Length / 2) * 8), 2);
            ShellConsole.Init(w, h);
            ShellConsole.DrawInput();
            AddCommands();
        }
        public static void AddCommands()
        {
            CommandsList.Add(new Commands.FileSystem.Cd());
            CommandsList.Add(new Commands.FileSystem.Dir());
        }
        public static void Update()
        {
            if (true)
            {
                //Keyboard Input
                if (KeyboardEx.IsKeyPressed)
                {
                    int Xold = ShellConsole.X;
                    if (KeyboardEx.k.Key != ConsoleKey.Enter)
                    {
                        if (KeyboardEx.k.Key != ConsoleKey.Backspace)
                        {
                            Text += KeyboardEx.k.KeyChar.ToString();
                            ShellConsole.Write(KeyboardEx.k.KeyChar.ToString());
                        }
                        else if(KeyboardEx.k.Key == ConsoleKey.Backspace)
                        {
                            ShellEngine.DrawFilledRectangle(Xold * 8, ShellConsole.Y * 16, Text.Length * 8 + 16, 16, System.Drawing.Color.Black);
                            ShellConsole.FG = System.Drawing.Color.White;
                            ShellConsole.SetCursorPositionChar(Xold, Y);
                            Text = Text.Remove(Text.Length - 1);
                            ShellConsole.Write(Text);
                        }
                    }
                    else
                    {
                        ShellConsole.WriteLine("");
                        ProcessInput(Text);
                        Text = "";
                        ShellConsole.DrawInput();
                    }
                }
            }
        }
        public static void ProcessInput(string line)
        {
            try
            {
                line += " ";
                if (line.Length > 0)
                {
                    String[] args = line.Split(' ');
                    bool error = true;
                    foreach (Command c in CommandsList)
                    {
                        try
                        {

                            // validate command
                            if (args[0].ToLower() == c.Name)
                            {
                                // execute and finish
                                c.Execute(line, args);
                                error = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Kernel.DrawStatusForce("Shell: " + ex.Message);
                            Kernel.DelayCode(2000);
                        }
                    }
                    // invalid command has been entered
                    if (error)
                    {
                        ShellConsole.WriteLine("Invalid command or program!", Color.Red);
                    }

                }
            }
            catch (Exception ex)
            {
                Kernel.DrawStatusForce("Shell: " + ex.Message);
                Kernel.DelayCode(2000);
            }
        }
        public static void Draw(int x, int y)
        {
            Kernel.Canvas.DrawImage(ShellEngine.shell, x, y);
        }
    }
    public static class ShellConsole
    {
        public static Int32 X = 0;
        public static Int32 Y = 3;
        public static System.Drawing.Color BG = System.Drawing.Color.Black;
        public static System.Drawing.Color FG = System.Drawing.Color.White;
        public static Int32 W = 640;
        public static Int32 H = 480;

        public static string TextUndone = "";

        public static void Init(int w, int h)
        {
            W = w;
            H = h;
            X = 0;
            Y = 3;
        }
        public static void Write(string text)
        {
            ShellEngine.DrawFilledRectangle(X * 8, Y * 16, text.Length * 8, 16, BG);
            ASC16.DrawStringShellEngine(text, FG, (uint)X * 8, (uint)Y * 16);
            
            X = X + text.Length;

            Kernel.Canvas.Display();
        }
        public static void Write(string text, System.Drawing.Color color)
        {
            ShellEngine.DrawFilledRectangle(X * 8, Y * 16, text.Length * 8, 16, BG);
            ASC16.DrawStringShellEngine(text, color, (uint)X * 8, (uint)Y * 16);
            X = X + text.Length;

            Kernel.Canvas.Display();
        }

        public static void WriteLine(string text)
        {
            Write(text, FG);
            X = 0;
            Y++;
        }
        public static void WriteLine(string text, System.Drawing.Color color)
        {
            Write(text, color);
            X = 0;
            Y++;
        }

        public static void Clear()
        {
            Kernel.Canvas.Clear(System.Drawing.Color.Black);
            Kernel.Canvas.Display();
        }
        public static void Clear(System.Drawing.Color bg)
        {
            Kernel.Canvas.Clear(bg);
            Kernel.Canvas.Display();
        }

        public static void SetCursorPositionChar(int x_char, int y_char)
        {
            X = (x_char);
            Y = (y_char);
        }

        public static void DrawInput()
        {
            string path = "";
            string end = "> ";
            if(Files.CurrentDirectory != Files.Root)
            {
                path = Files.CurrentDirectory.Substring(0, 2);
            }
            Write(path + end);
        }

        private static void ScrollDown()
        {
            //Kernel.Canvas.DrawFilledRectangle(System.Drawing.Color.Black, 0, 384, 640, 16);
            //SetCursorPositionChar(0, 24);
        }

        internal static void SetCursorPosition(int v, object cursorY)
        {
            throw new NotImplementedException();
        }
    }

    public static class ShellEngine
    {
        public static Bitmap shell;

        public static void Clear()
        {
            shell.RawData.CopyTo(null, 0);
        }

        public static void DrawPixel(int x, int y, Color col)
        {
            int color = Desktop.RgbaToHex(col.R, col.G, col.B, col.A);
            switch (x > 0 && x < shell.Width && y >= 0 && y < shell.Height)
            {
                case true:
                    shell.RawData[y * shell.Width + x] = color;
                    break;
            }
        }

        public static void DrawFilledRectangle(int X, int Y, int Width, int Height, Color col)
        {
            int color = Desktop.RgbaToHex(col.R,col.G,col.B,col.A);
            if (X <= shell.Width)
            {
                int[] line = new int[Width];
                if (X < 0)
                {
                    line = new int[Width + X];
                }
                else if (X + Width > shell.Width)
                {
                    line = new int[Width - (X + Width - shell.Width)];
                }
                Array.Fill(line, color);

                for (int i = Y; i < Y + Height; i++)
                {
                    if (i < shell.Height && i >= 0)
                    {
                        Array.Copy(line, 0, shell.RawData, (i * shell.Width) + X, line.Length);
                    }
                }
            }
        }

    }
}
