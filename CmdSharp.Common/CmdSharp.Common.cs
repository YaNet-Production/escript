using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmdSharp
{
    public class Common
    {
        public static object Process(string Command, bool HandleExceptions = true)
        {
            return Cmd.Process(Command, HandleExceptions);
        }

        public static object ProcessNative(string Method, params object[] Arguments)
        {
            List<object> args = new List<object>();

            foreach (var o in Arguments)
                args.Add(o);

            return Cmd.InvokeMethod(Method, args.ToArray());
        }

        public static void DebugText(string text, ConsoleColor color = ConsoleColor.DarkGray, bool print = true)
        {
            Debug.DebugText(text, color, print);
        }

        public static List<EVariable> Variables { get { return CmdSharp.Variables.VarList; } }

        public static void VariableSet(string name, object value, List<string> options = null)
        {
            CmdSharp.Variables.Set(name, value, options);
        }

        public static void VariableRemove(string name, bool force = false)
        {
            CmdSharp.Variables.Remove(name, force);
        }

        public static object VariableGet(string name)
        {
            return CmdSharp.Variables.Get(name);
        }

        public static bool VariableGetBool(string name)
        {
            return CmdSharp.Variables.GetBool(name);
        }
    }
}
