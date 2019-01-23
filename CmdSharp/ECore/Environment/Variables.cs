using CmdSharp.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmdSharp
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
                    if (v.Options.Contains("CmdSharp")) return "ESCRIPT_ERROR_REMOVE_PROTECTED_VARIABLE";
                }
                VarList.Remove(v);
                return 1;
            }
            return "ESCRIPT_ERROR_VARIABLE_NOT_FOUND";
        }

        public static object Get(string name, bool NoResolve = false) { return GetValueObject(name, NoResolve); }

        public static object GetValueObject(string name, bool NoResolve = false)
        {
            if (!NoResolve)
            {
                object dots = DotsResolver.Resolve(name);
                if (dots != null)
                {
                    if (dots.GetType() == typeof(EProperty))
                    {
                        return ((EProperty)dots).GetValue();
                    }
                    else if (dots.GetType() == typeof(EField))
                    {
                        return ((EField)dots).GetValue();
                    }
                    else if (dots.GetType() == typeof(EMethodNew))
                    {
                        return ((EMethodNew)dots).Invoke();
                    }
                    else
                        throw new Exception($"Excpeted field, but '{dots.GetType().Name}' received.");
                }
            }
            
            EVariable a = GetVariable(name);

            if (a == null)
                return null;

            return a.Value;
        }

        public static bool GetBool(string name)
        {
            var v = Get(name);
            
            if (v.GetType() == typeof(bool))
                return (bool)v;
            else if (v.GetType() == typeof(string))
                return GlobalVars.StringToBool((string)v);
            else
                throw new Exception($"'{v.GetType().Name}' can't be converted into 'Boolean'");
        }

        public static string GetValue(string name, bool replace = true)
        {
            EVariable a = GetVariable(name);
            if (a == null) return "";
            else
            {/*
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
                }*/
                return a.Value.ToString(); }
        }

        public static void Add(string name, object value, List<string> options = null) { SetVariable(name, value, options); }



        public static void SetVariableObject(string name, object value, List<string> options = null)
        {
            SetVariable(name, value, options);
        }

        public static void Set(string name, object value, List<string> options = null) { SetVariable(name, value, options); }

        public static void SetVariable(string name, object value, List<string> options = null)
        {
            object dots = DotsResolver.Resolve(name);
            if(dots != null)
            {
                if (dots.GetType() == typeof(EProperty))
                {
                    ((EProperty)dots).SetValue(value);
                }
                else if (dots.GetType() == typeof(EField))
                {
                    ((EField)dots).SetValue(value);
                }
                else
                    throw new Exception($"Excpeted field or method, but '{dots.GetType().Name}' received.");
            }

            EVariable e = GetVariable(name);

            if (e == null)
            {
                EVariable variable = new EVariable(name, value, options);
                VarList.Add(variable);
            }
            else
            {
                if (e.Options.Contains("ReadOnly"))
                    throw new Exception("ESCRIPT_ERROR_EDIT_READONLY_VARIABLE");

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

            SetVariableObject("args", new EList(), new List<string>() { "CmdSharp" });
            if (args != null)
            {
                SetVariableObject("args", new EList() { Content = args.ToList<object>() }, new List<string>() { "CmdSharp" });
            }

            //SetVariableObject("result", new EList() { DefaultIndex = -2, Content = { "null" } });
            SetVariableObject("result", null, new List<string>() { "CmdSharp" });
            Add("invitation", "cmd#> ", new List<string>() { "CmdSharp" });
            Add("showResult", false, new List<string>() { "CmdSharp" });
            Add("showCommands", false, new List<string>() { "CmdSharp" });
            Add("programDebug", false, new List<string>() { "CmdSharp" });

            SetVariableObject("parserDebug", true, new List<string>() { "CmdSharp", "Hidden" });
            SetVariableObject("envDebug", true, new List<string>() { "CmdSharp", "Hidden" });

            if (GlobalVars.DebugWhenFormingEnv)
                Set("programDebug", true, new List<string>() { "CmdSharp" });

            Add("startTime", DateTime.Now, new List<string>() { "CmdSharp" });

            Add("edition", EnvironmentManager.Edition, new List<string>() { "CmdSharp", "ReadOnly" });
            Add("stuffServer", GlobalVars.StuffServer, new List<string>() { "CmdSharp", "Hidden", "ReadOnly" });

            SetVariableObject("forceConsole", false, new List<string>() { "CmdSharp" });
            SetVariableObject("checkUpdates", true, new List<string>() { "CmdSharp", "Hidden" });

            Add("isCompiledScript", false, new List<string>() { "CmdSharp", "Hidden" });

            //Add("varCanBeMethod", "0", new List<string>() { "CmdSharp", "Hidden", "ReadOnly" }); // example: $varCanBeMethod=#1
                                        // $something=#{#{DateTimeNow}}
                                        // echo $something
                                        // #{#{DateTimeNow}} -> {DateTimeNow} -> xx.xx.xx xx:xx:xx
                                        // [SCREEN] xx.xx.xx xx:xx:xx
           
            //Add("invokeIgnoreBadCmd", "1", new List<string>() { "CmdSharp" }); // example: 
                                            // $kek=Something
                                            //
                                            // If method "Something" not found and dollarIgnoreBadCmd = 1
                                            // it will return Something as result
                                            //
                                            // echo $kek
                                            // [SCREEN] Something

           // Add("displayNoCmd", "1", new List<string>() { "CmdSharp" });

            Add("forceGC", false, new List<string>() { "CmdSharp" });
            //Add("checkSyntax", tru, new List<string>() { "CmdSharp" });
            //Add("taskTimeout", "3000", new List<string>() { "CmdSharp", "Hidden" });
            //Add("varParseWithEnd", "1", new List<string>() { "CmdSharp", "Hidden" }); // if 1, variables will be parsed like "$variable$". if 0, like "$variable"
            Add("abortAfterBreak", true, new List<string>() { "CmdSharp" });
            Add("exitAfterBreak", false, new List<string>() { "CmdSharp" });
        }
        
    }
}
