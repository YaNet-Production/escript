using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace escript
{
    public static class Variables
    {
        public static List<EVariable> VarList = new List<EVariable>();
        public static EVariable GetVariable(string name)
        {
            for(int i = 0; i < VarList.Count; i++)
            {
                if (VarList[i].Name == name) return VarList[i];
            }
            return null;
        }

        public static int Remove(string name)
        {
            EVariable v = GetVariable(name);
            if (v != null) if (v.CanBeRemoved) { VarList.Remove(v); return 1; }
            return 0;
        }

        public static int Remove(EVariable v)
        {
            if (v != null) if (v.CanBeRemoved) { VarList.Remove(v); return 1; }
            return 0;
        }

        public static string GetValue(string name)
        {
            EVariable a = GetVariable(name);
            if (a == null) return "";
            else return a.Value;
        }

        public static void Add(string name, string value, bool canBeRemoved = true) { SetVariable(name, value, canBeRemoved); }

        public static void Set(string name, string value, bool canBeRemoved = true) { SetVariable(name, value, canBeRemoved); }
        public static void SetVariable(string name, string value, bool canBeRemoved = true)
        {
            if (value == null) value = "null";
            EVariable e = GetVariable(name);
            if (e == null) VarList.Add(new EVariable(name, value, canBeRemoved));
            else 
                e.Edit(value);
            
        }

        public static void Initialize()
        {
            VarList.Clear();
            Add("result", "-", false);
            Add("splitArgs", "||", false);
            Add("inputText", "ReadLine: ", false);
            Add("invitation", "ESCRIPT> ", false);
            Add("showResult", "0", false);
            Add("showCommands", "0", false);
            Add("programDebug", "0");
            Add("startTime", DateTime.Now.ToString(), false);

            Program.Debug("Environment formed", ConsoleColor.DarkGreen);
            Program.Debug("Time: " + DateTime.Now.ToString(), ConsoleColor.DarkGreen);
        }
        
    }
}
