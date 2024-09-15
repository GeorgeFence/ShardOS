using Cosmos.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShardOS
{
    public static class KeyboardEx
    {
        private static readonly List<Action<ConsoleKeyInfo>> Callbacks;
        public static ConsoleKeyInfo k;
        public static bool IsKeyPressed = false;


        public static void RegisterCallback(Action<ConsoleKeyInfo> CallBack)
        {
            Callbacks.Add(CallBack);
        }

        public static bool TryReadKey(out ConsoleKeyInfo Key)
        {
            if (KeyboardManager.TryReadKey(out var key))
            {
                Key = new ConsoleKeyInfo(key.KeyChar, key.Key.ToConsoleKey(), key.Modifiers == ConsoleModifiers.Shift, key.Modifiers == ConsoleModifiers.Alt, key.Modifiers == ConsoleModifiers.Control);
                return true;
            }

            Key = default(ConsoleKeyInfo);
            return false;
        }

        public static ConsoleKeyInfo? ReadKey()
        {
            if (TryReadKey(out var Key))
            {
                return Key;
            }

            return null;
        }
        public static String GetInput()
        {
            string s = null;
            if(System.Console.KeyAvailable)
            {
                ConsoleKeyInfo key = System.Console.ReadKey();
                
            }
            return s;
        }


    }
}
