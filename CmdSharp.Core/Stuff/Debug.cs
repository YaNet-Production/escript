using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CmdSharp
{
    public class DebugStorage
    {
        public string Title = null;
        public List<object> Storage = new List<object>(); 

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();

            if (Title != null)
            {
                b.Append("[");
                b.Append(Title);
                b.Append("] ");
            }

            for(int i = 0; i < Storage.Count; i++)
            {
                b.Append(i + "=[");

                if (Storage[i] == null)
                    b.Append("null");
                else
                    b.Append($"({Storage[i].GetType().Name}) {Storage[i].ToString()}");

                b.Append("] ");
            }

            return b.ToString();
        }
    }

    public class Debug
    {
        public static DebugStorage GetSafeStorage(string type, params object[] values)
        {
            if (Variables.GetBool("programDebug") && Variables.GetBool(type.ToLower() + "Debug"))
            {
                DebugStorage storage = new DebugStorage();

                foreach(var a in values)
                {
                    storage.Storage.Add(a);
                }

                return storage;
            }

            return null;
        }
        public static string Store(string type, string Prefix, DebugStorage storage, bool Print = true, int randomLen = 5)
        {
            if (storage == null)
                return null;

            if (Variables.GetBool("programDebug") && Variables.GetBool(type.ToLower() + "Debug"))
            {
                var vName = "DebugStorage_" + Prefix + "_" + GlobalVars.RandomString(randomLen);
                //KeyValuePair<string, CommandTypes> k = new KeyValuePair<string, CommandTypes>(Command, result);
                Variables.SetVariableObject(vName, storage, new List<string>() { "ESCRIPT", "Debug", "Hidden" });

                if (Print)
                    Debug.Log("Parser", "[CmdType] Storage -> " + vName);

                return vName;
            }
            else
                return null;
        }
        public static void Log(string type, object text, ConsoleColor color = ConsoleColor.DarkYellow)
        {
            if (Variables.GetBool("programDebug") && Variables.GetBool(type.ToLower() + "Debug"))
            {
                DebugText("[" + type + "] " + text, color);
            }
        }

        public static void DebugText(string text, ConsoleColor col = ConsoleColor.DarkGray, bool PrintInConsole = true)
        {
            if (Variables.GetValue("ignoreLog") == "1") return;

            string result = "[DEBUG] " + text;

            if (CoreMain.log != null)
                CoreMain.log.WriteLine("[" + DateTime.Now.ToString() + "] " + result);

            if (Variables.GetBool("programDebug") && PrintInConsole)
            {
                ConsoleColor a = EConsole.ForegroundColor;
                EConsole.ForegroundColor = col;
                EConsole.WriteLine(result, false);
                EConsole.ForegroundColor = a;
            }

            Debugger.Log(0, "DEBUG", result + Environment.NewLine);
        }
    }
}
