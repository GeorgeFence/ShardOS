using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ShardOS
{
    public static class RegistryManager
    {
        public static List<Register> registers = new List<Register>();
        public static void Initialize()
        {
            if (!Files.FileExists("0:\\registers.reg"))
            {
                Files.CreateFile("0:\\registers.reg");
                Kernel.Shutdown(2);
            }
            Reload();
        }
        public static void Reload()
        {
            try
            {
                string datafile = Files.ReadText("0:\\registers.reg");
                //Kernel.DrawStatus(datafile);
                //Kernel.DelayCode(2000);
                if(datafile != "")
                {
                    string[] s = datafile.Split(';');

                    for (int i = 0; i < s.Count(); i++)
                    {
                        RegisterType t;
                        string[] s3 = s[i].Split(",");
                        if (s3[0] == "REG_COLOR")
                        {
                            t = RegisterType.REG_COLOR;
                        }
                        else if (s3[0] == "REG_STR")
                        {
                            t = RegisterType.REG_STR;
                        }
                        else
                        {
                            t = RegisterType.Unknown;
                        }

                        registers.Add(new Register(s3[1], s3[2], t));
                    }
                }
            }
            catch (Exception ex) { Kernel.DrawStatus(ex.Message, Color.Red); Kernel.DelayCode(2000); }
           
        }
        public static void AddRegistry(RegisterType type, string Name, string Value)
        {
            string typereg = "";
            if (type == RegisterType.REG_COLOR)
            {
                typereg = "REG_COLOR";
            }
            else if (type == RegisterType.REG_STR)
            {
                typereg = "REG_STR";
            }
            else
            {
                typereg = "Unknown";
            }
            try
            {
                bool IfExists = false;
                foreach (Register reg in registers)
                {
                    if (reg.name == Name) { IfExists = true; break; }
                }
                if (!IfExists)
                {
                    if (Value.Contains(';'))
                    {
                        throw new Exception("Value cannot contain ;");
                    }
                    if (Value.Contains(','))
                    {
                        throw new Exception("Value cannot contain ,");
                    }
                    if (Name.Contains(';'))
                    {
                        throw new Exception("Name cannot contain ;");
                    }
                    if (Name.Contains(','))
                    {
                        throw new Exception("Name cannot contain ,");
                    }
                    string datafile = Files.ReadText("0:\\registers.reg");
                    if(datafile == "")
                    {
                        datafile = datafile + typereg + "," + Name + "," + Value;
                        Files.WriteAllText("0:\\registers.reg", datafile);
                        Reload();
                    }
                    else
                    {
                        datafile = datafile + ";" + typereg + "," + Name + "," + Value;
                        Files.WriteAllText("0:\\registers.reg", datafile);
                        Reload();
                    }
                }
            }
            catch(Exception ex)
            {
                Kernel.DrawStatus("Cannot create registry! N: " + Name + " V: " + Value + " T: " + typereg + ". Exception: " + ex.Message, Color.Red); 
                Kernel.DelayCode(1000);
            }
        }
        public static void DeleteRegistry(string Name)
        {
            bool IfExists = false;
            foreach (Register reg in registers)
            {
                if (reg.name == Name) { IfExists = true; break; }
            }
            if (IfExists)
            {
                string data = "";
                for (int i = 0; i < registers.Count; i++)
                {
                    if (registers[i].name != Name)
                    {
                        if (i == 1)
                        {
                            data = data + registers[i - 1].type.ToString() + "," + registers[i - 1].name + "," + registers[i].value;
                        }
                        else
                        {
                            data = data + ";" + registers[i - 1].type.ToString() + "," + registers[i - 1].name + "," + registers[i].value;
                        }
                    }
                }
                Files.WriteAllText("0:\\registers.reg", data);
                Reload();
            }
        }

        public static string GetValue(string Name)
        {
            string s = "";
            foreach (Register reg in registers)
            {
                if (reg.name == Name) { s = reg.value; break; }
            }
            return s;
        }
        public static string GetType(string Name)
        {
            string s = "";
            foreach (Register reg in registers)
            {
                if (reg.name == Name) { s = reg.type.ToString(); break; }
            }
            return s;
        }
    }

    public class Register
    {
        public string name;
        public string value;
        public RegisterType type;
        public Register(string name, string value, RegisterType type)
        {
            this.name = name;
            this.value = value;
            this.type = type;
        }
    }
    public enum RegisterType
    {
        REG_COLOR,
        REG_STR,
        Unknown
    };
}
