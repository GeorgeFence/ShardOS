using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShardOS.Commands.FileSystem
{
    public class Cd : Command
    {

        public Cd()
        {
            Name = "cd";
            Help = "go to directory";
        }

        public override void Execute(string line, string[] args)
        {
            string path = args[1].Replace("/",@"\");
            if (args[1].StartsWith(Files.CurrentDirectory))
            {
                if (Files.FolderExists(args[1]))
                {
                    if (path.EndsWith(@"\"))
                    {
                        Files.CurrentDirectory = args[1];
                    }
                    else
                    {
                        Files.CurrentDirectory = args[1] + @"\";
                    }
                }
                else
                {
                    ShellConsole.WriteLine("Could not locate directory \"" + args[1] + "\"", Color.Red);
                }
            }
            
            else if (args[1] == "..")
            {
                if (Files.CurrentDirectory == Files.Root)
                {
                    ShellConsole.WriteLine("Can not go under root directory!", Color.Red);
                }
                else
                {
                    string[] dir = Files.CurrentDirectory.Split(@"\");
                    string Out = "";
                    for (int i = 0; i < (dir.Length - 1); i++)
                    {
                        Out = Out + dir[i];
                    }
                    Files.CurrentDirectory = Out;
                }
            }
            else if(args[1].StartsWith(Files.CurrentDirectory) == false && args[1] != "..")
            {
                if (Files.FolderExists(args[1]))
                {
                    if (path.EndsWith(@"\"))
                    {
                        Files.CurrentDirectory = Files.CurrentDirectory + args[1];
                    }
                    else
                    {
                        Files.CurrentDirectory = Files.CurrentDirectory + args[1] + @"\";
                    }
                }
                else
                {
                    ShellConsole.WriteLine("Could not locate directory \"" + Files.CurrentDirectory + args[1] + @"\", Color.Red);
                }
            }
        }
    }
}
