using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShardOS
{
    public static class FileExecutor
    {
        public static List<File> list = new List<File>();
        public static void Initialize()
        {
            list.Clear();
        }
        public static void Execute(string path)
        {
            
        }

        public static void Reload()
        {
            try
            {
                list.Clear();
                string datafile = Files.ReadText("0:\\list.reg");
                string[] s = datafile.Split(';');

                for (int i = 0; i < s.Count(); i++)
                {
                    string[] s3 = s[i].Split(",");

                    list.Add(new File(s3[0]));
                }
            }
            catch (Exception ex) { Kernel.DrawStatusForce("UAS Exception: " + ex.Message, Color.Red); Kernel.DelayCode(2000); }

        }
        public static void CreateFile(File File)
        {
            try
            {
                bool IfExists = false;
                foreach (File us in list)
                {
                    if (us.suffix == File.suffix) { IfExists = true; break; }
                }
                if (!IfExists)
                {
                    if (File.suffix.Contains(';'))
                    {
                        throw new Exception("Filename cannot contain ;");
                    }
                    if (File.suffix.Contains(','))
                    {
                        throw new Exception("Filename cannot contain ,");
                    }
                    if (File.suffix.Contains(';'))
                    {
                        throw new Exception("Password cannot contain ;");
                    }
                    if (File.suffix.Contains(','))
                    {
                        throw new Exception("Password cannot contain ,");
                    }
                    string datafile = Files.ReadText("0:\\list.reg");
                    if (datafile == "")
                    {
                        datafile = datafile + File.suffix;
                        Files.WriteAllText("0:\\list.reg", datafile);
                        Reload();
                    }
                    else
                    {
                        datafile = datafile + ";" + File.suffix;
                        Files.WriteAllText("0:\\list.reg", datafile);
                        Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                Kernel.DrawStatus("UAS Exception: " + ex.Message, Color.Red);
                Kernel.DelayCode(1000);
            }
        }
        public static void DeleteFile(string FileName)
        {
            bool IfExists = false;
            foreach (File us in list)
            {
                if (us.suffix == FileName) { IfExists = true; break; }
            }
            if (IfExists)
            {
                string data = "";
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].suffix != FileName)
                    {
                        if (i == 1)
                        {
                            data = data + list[i - 1].suffix;
                        }
                        else
                        {
                            data = data + ";" + list[i - 1].suffix;
                        }
                    }
                }
                Files.WriteAllText("0:\\list.reg", data);
                Reload();
            }
        }
    }
    public class File
    {
        public string suffix;
        public File(string suffix)
        {
            this.suffix = suffix;
        }
    }
}
