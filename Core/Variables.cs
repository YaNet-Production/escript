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

        public static object Remove(string name, bool force = false)
        {
            EVariable v = GetVariable(name);
            return Remove(v, force);
        }

        public static object Remove(EVariable v, bool force = false)
        {
            if (v != null)
            {
                if (!force)
                {
                    if (v.Options.Contains("ESCRIPT")) return "ESCRIPT_ERROR_REMOVE_PROTECTED_VARIABLE";
                }
                VarList.Remove(v);
                return 1;
            }
            return "ESCRIPT_ERROR_VARIABLE_NOT_FOUND";
        }

        public static object GetValueObject(string name)
        {
            EVariable a = GetVariable(name);
            if (a == null) return "";
            else
            {
                return a.Value;
            }
        }

        public static bool GetBool(string name)
        {
            return GlobalVars.StringToBool(GetValue(name));
        }

        public static string GetValue(string name, bool replace = true)
        {
            EVariable a = GetVariable(name);
            if (a == null) return "";
            else
            {
                if (a.Value.ToString().Contains("{") && a.Value.ToString().Contains("}") && replace)
                {
                    if (GetValue("varCanBeMethod") == "1")
                    {

                        List<iString> list = new List<iString>();

                        for (int i = 0; i < a.Value.ToString().Length; i++)
                        {
                            if (a.Value.ToString()[i] == '{')
                            {
                                iString test = new iString() { startIdx = i };
                                for (int c = test.startIdx; c < a.Value.ToString().Length; c++)
                                {
                                    if (a.Value.ToString()[c] == '}')
                                    {
                                        test.text.Append("}");
                                        test.endIdx = c + 1;
                                        break;
                                    }
                                    test.text.Append(a.Value.ToString()[c]);
                                }
                                i = test.endIdx;
                                list.Add(test);
                            }
                        }

                        foreach (var r in list)
                        {
                            return a.Value.ToString().Replace(r.text.ToString(), Cmd.Process(r.text.ToString().TrimStart('{').TrimEnd('}')));
                        }
                    }
                }
                return a.Value.ToString(); }
        }

        public static void Add(string name, string value, List<string> options = null) { SetVariable(name, value, options); }



        public static void SetVariableObject(string name, object value, List<string> options = null)
        {
            if (value == null) value = "";
            EVariable e = GetVariable(name);
            if (e == null)
            {
                EVariable variable = new EVariable(name, value, options);
                if (options != null) variable.Options = options;
                VarList.Add(variable);
            }
            else
                e.Edit(value);
        }

        public static void Set(string name, string value, List<string> options = null) { SetVariable(name, value, options); }
        public static void SetVariable(string name, string value, List<string> options = null)
        {
            if (value == null) value = "null";
            if (value.Length == 0) value = "null";
            EVariable e = GetVariable(name);
            if (e == null)
            {
                EVariable variable = new EVariable(name, value, options);
                VarList.Add(variable);
            }
            else
            {
                if (e.Options.Contains("ReadOnly")) throw new Exception("ESCRIPT_ERROR_EDIT_READONLY_VARIABLE");
                e.Edit(value, options);
            }
            
        }

        public static void Initialize(bool addSystem = true, string[] args = null)
        {
            VarList.Clear();

            if (addSystem)
            {
                foreach (System.Collections.DictionaryEntry env in Environment.GetEnvironmentVariables())
                {
                    string name = (string)env.Key;
                    string value = (string)env.Value;
                    Add(name, value, new List<string>() { "System" });
                }
            }

            Add("cmdDone", EResult.Cmd.Done.ToString(), new List<string>() { "ESCRIPT", "ReadOnly", "Results" });
            Add("cmdFail", EResult.Cmd.Fail.ToString(), new List<string>() { "ESCRIPT", "ReadOnly", "Results" });

            SetVariableObject("args", new EList(), new List<string>() { "ESCRIPT" });
            if (args != null)
            {
                SetVariableObject("args", new EList() { Content = args.ToList<object>() }, new List<string>() { "ESCRIPT" });
            }

            //SetVariableObject("result", new EList() { DefaultIndex = -2, Content = { "null" } });
            Add("result", "null", new List<string>() { "ESCRIPT" });
            Add("splitArgs", "||", new List<string>() { "ESCRIPT" });
            Add("inputText", "ReadLine: ", new List<string>() { "ESCRIPT", "Hidden" });
            Add("invitation", "ESCRIPT> ", new List<string>() { "ESCRIPT" });
            Add("showResult", "0", new List<string>() { "ESCRIPT" });
            Add("showCommands", "0", new List<string>() { "ESCRIPT" });
            Add("programDebug", "0", new List<string>() { "ESCRIPT" });
            if (GlobalVars.DebugWhenFormingEnv) Set("programDebug", "1", new List<string>() { "ESCRIPT" });
            Add("startTime", DateTime.Now.ToString(), new List<string>() { "ESCRIPT" });
#if IsCore
            Add("edition", "core", new List<string>() { "ESCRIPT" });
#else
            Add("edition", "standard", new List<string>() { "ESCRIPT" });
#endif
            Add("stuffServer", GlobalVars.StuffServer, new List<string>() { "ESCRIPT", "Hidden", "ReadOnly" });
            Add("logFile", GlobalVars.LogFile, new List<string>() { "ESCRIPT", "Hidden" });
            Add("forceConsole", "0", new List<string>() { "ESCRIPT" });
            Add("checkUpdates", "1", new List<string>() { "ESCRIPT", "Hidden" });
            Add("isCompiledScript", "0", new List<string>() { "ESCRIPT", "Hidden" });
            Add("varCanBeMethod", "0", new List<string>() { "ESCRIPT" }); // example: $varCanBeMethod=#1
                                        // $something=#{#{DateTimeNow}}
                                        // echo $something
                                        // #{#{DateTimeNow}} -> {DateTimeNow} -> xx.xx.xx xx:xx:xx
                                        // [SCREEN] xx.xx.xx xx:xx:xx
           
            Add("invokeIgnoreBadCmd", "1", new List<string>() { "ESCRIPT" }); // example: 
                                            // $kek=Something
                                            //
                                            // If method "Something" not found and dollarIgnoreBadCmd = 1
                                            // it will return Something as result
                                            //
                                            // echo $kek
                                            // [SCREEN] Something

            Add("displayNoCmd", "1", new List<string>() { "ESCRIPT" });
            Add("useCustomConsole", "1", new List<string>() { "ESCRIPT", "Hidden" });
            Add("forceGC", "0", new List<string>() { "ESCRIPT" });
            Add("checkSyntax", "1", new List<string>() { "ESCRIPT" });
            Add("taskTimeout", "3000", new List<string>() { "ESCRIPT", "Hidden" });
            Add("varParseWithEnd", "1", new List<string>() { "ESCRIPT", "Hidden" }); // if 1, variables will be parsed like "$variable$". if 0, like "$variable"
            Add("abortAfterBreak", "1", new List<string>() { "ESCRIPT" });
            Add("exitAfterBreak", "0", new List<string>() { "ESCRIPT" });

            Program.Debug("Time: " + DateTime.Now.ToString(), ConsoleColor.DarkGreen);
        }
        
    }
}
