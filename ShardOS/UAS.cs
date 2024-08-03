using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ShardOS
{
    public static class UAS
    {
        public static List<User> Users = new List<User>();
        public static User ActiveUser = new User("defaultUser","default","0:\\Users\\defaultUser");
        public static User defaultuser = new User("defaultUser", "default", "0:\\Users\\defaultUser");
        public static void Initialize()
        {
            if (!Files.FileExists("0:\\users.reg"))
            {
                Files.CreateFile("0:\\users.reg");
                Kernel.Shutdown(3);
            }
            Reload();
            if(Users.Count == 0)
            {
                Kernel.DrawStatus("Creating defaultUser");
                CreateUser(new User("defaultUser", "default", "0:\\Users\\defaultUser"));
                Kernel.DrawStatus("Creating user");
                Kernel.DelayCode(1000);
                Kernel.Shutdown(3);
            }
        }
        public static void Reload()
        {
            try
            {
                Users.Clear();
                string datafile = Files.ReadText("0:\\users.reg");
                string[] s = datafile.Split(';');

                for (int i = 0; i < s.Count(); i++)
                {
                    string[] s3 = s[i].Split(",");

                    Users.Add(new User(s3[0], s3[1], s3[2]));
                }
            }
            catch (Exception ex) { Kernel.DrawStatusForce("UAS Exception: " + ex.Message, Color.Red); Kernel.DelayCode(2000); }

        }
        public static void CreateUser(User user)
        {
            try
            {
                bool IfExists = false;
                foreach (User us in Users)
                {
                    if (us.Username == user.Username) { IfExists = true; break; }
                }
                if (!IfExists)
                {
                    if (user.Username.Contains(';'))
                    {
                        throw new Exception("Username cannot contain ;");
                    }
                    if (user.Username.Contains(','))
                    {
                        throw new Exception("Username cannot contain ,");
                    }
                    if (user.Password.Contains(';'))
                    {
                        throw new Exception("Password cannot contain ;");
                    }
                    if (user.Password.Contains(','))
                    {
                        throw new Exception("Password cannot contain ,");
                    }
                    string datafile = Files.ReadText("0:\\users.reg");
                    if (datafile == "")
                    {
                        datafile = datafile + user.Username + "," + user.Password + "," + user.UserFolderPath;
                        Files.WriteAllText("0:\\users.reg", datafile);
                        Reload();
                    }
                    else
                    {
                        datafile = datafile + ";" + user.Username + "," + user.Password + "," + user.UserFolderPath;
                        Files.WriteAllText("0:\\users.reg", datafile);
                        Reload();
                    }
                    if (!Files.FolderExists(user.UserFolderPath))
                    {
                        Files.CreateFolder(user.UserFolderPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Kernel.DrawStatus("UAS Exception: " + ex.Message, Color.Red);
                Kernel.DelayCode(1000);
            }
        }
        public static void DeleteUser(string UserName)
        {
            bool IfExists = false;
            foreach (User us in Users)
            {
                if (us.Username == UserName) { IfExists = true; break; }
            }
            if (IfExists)
            {
                string data = "";
                for (int i = 0; i < Users.Count; i++)
                {
                    if (Users[i].Username != UserName)
                    {
                        if (i == 1)
                        {
                            data = data + Users[i - 1].Username + "," + Users[i - 1].Password + "," + Users[i].UserFolderPath;
                        }
                        else
                        {
                            data = data + ";" + Users[i - 1].Username + "," + Users[i - 1].Password + "," + Users[i].UserFolderPath;
                        }
                    }
                }
                Files.WriteAllText("0:\\users.reg", data);
                Reload();
            }
        }
        public static string GetUserPassword(string username)
        {
            string s = "";
            foreach (User us in Users)
            {
                if (us.Username == username) { s = us.Password; break; }
            }
            return s;
        }
        public static void SetUserActive(User user)
        {
            ActiveUser = user;
        }
        public static void Logout()
        {
            ActiveUser = defaultuser;
            DesktopGrid.gridItems.Clear();
            Shell.Init(0, 0, (int)Kernel.Canvas.Mode.Width, (int)Kernel.Canvas.Mode.Height);
            Kernel.DrawStatusForce("Logout Successfull", Color.Green);
            bool once = true;
            while(ActiveUser == defaultuser)
            {
                Shell.Update();
                Shell.Draw(0,0);
                if (once)
                {
                    once = false;
                    ShellConsole.WriteLine("Logouting in 5");
                    Shell.Draw(0, 0);
                    Kernel.DelayCode(1000);
                    ShellConsole.WriteLine("4");
                    Shell.Draw(0, 0);
                    Kernel.DelayCode(1000);
                    ShellConsole.WriteLine("3", Color.Red);
                    Shell.Draw(0, 0);
                    Kernel.DelayCode(1000);
                    ShellConsole.WriteLine("2", Color.Red);
                    Shell.Draw(0, 0);
                    Kernel.DelayCode(1000);
                    ShellConsole.WriteLine("1", Color.Red);
                    Shell.Draw(0, 0);
                    Kernel.DelayCode(1000);
                    ShellConsole.WriteLine("0", Color.Red);
                    Shell.Draw(0, 0);
                    Kernel.DelayCode(200);
                }
            }
        }
    }

    public class User
    {
        public string Username;
        public string Password;
        public string UserFolderPath;
        public User(string username, string password, string UserFolderPath)
        {
            this.Username = username;
            this.Password = password;
            this.UserFolderPath = UserFolderPath;
        }
    }
}
