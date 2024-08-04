using Cosmos.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Console = System.Console;
using Sys = Cosmos.System;


namespace ShardOS.Commands.FileSystem
{
    class Dir : Command
    {
        public Dir()
        {
            Name = "dir";
            Help = "show files and folders in current directory";

        }

        public override void Execute(string line, string[] args)
        {
            if(line.Length > 3)
            {
                // show contents of active directory
                if (args[1].Length == 0)
                {
                    ListContents(Files.CurrentDirectory);
                }
                if (line.Length == 3)
                {
                    ListContents(Files.CurrentDirectory);

                }
                // show contents of specified directory
                else if (line.Length > 4)
                {
                    // parse path
                    string path = args[1];
                    if (path.EndsWith('\\')) { path = path.Remove(path.Length - 1, 1); }
                    path += "\\";

                    if (Files.FolderExists(path))
                    {
                        if (path.StartsWith(Files.CurrentDirectory)) { ListContents(path); }
                        else if (path.StartsWith(@"0:\")) { ListContents(path); }
                        else if (!path.StartsWith(Files.CurrentDirectory) && !path.StartsWith(@"0:\"))
                        {
                            if (Files.FolderExists(Files.CurrentDirectory + path)) ListContents(Files.CurrentDirectory + path);
                            else { ShellConsole.WriteLine("Could not locate directory \"" + path + "\"", Color.Red); }
                        }
                        else { ShellConsole.WriteLine("Could not locate directory \"" + path + "\"", Color.Red); }
                    }
                    else { ShellConsole.WriteLine("Could not locate directory!", Color.Red); }
                }
                else { ShellConsole.WriteLine("Invalid argument! Path expected.", Color.Red); }
            }
        }

        private void ListContents(string path)
        {
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                string[] folders = Files.GetFolders(path);
                string[] files = Files.GetFiles(path);

                ShellConsole.WriteLine("Showing contents of directory \"" + path + "\"");

                // draw folders
                for (int i = 0; i < folders.Length; i++)
                {
                    ShellConsole.WriteLine(folders[i], Color.Yellow);
                }

                // draw files
                for (int i = 0; i < files.Length; i++)
                {
                    Cosmos.System.FileSystem.Listing.DirectoryEntry attr = Files.GetFileInfo(path + files[i]);
                    if (attr != null)
                    {
                        ShellConsole.Write(files[i], Color.White);
                        ShellConsole.SetCursorPositionChar(30, ShellConsole.Y);
                        ShellConsole.WriteLine(attr.mSize.ToString() + " BYTES", Color.Gray);
                    }
                    else { ShellConsole.WriteLine("Error retrieiving file info", Color.Red); }
                }

                ShellConsole.WriteLine("");
                ShellConsole.Write("Total folders: " + folders.Length.ToString());
                ShellConsole.WriteLine("        Total files: " + files.Length.ToString());
            }
            catch (Exception ex) { }
#pragma warning restore CS0168 // Variable is declared but never used
        }
    }
}

