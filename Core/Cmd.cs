using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace escript
{
    public class ImportLibClass
    {

        public Assembly assembly;
        public Type[] types;
        public Type funcType = null;
        public object obj = null;

        public ImportLibClass(string fileName, Functions functions, string text = "EFuncInvoke")
        {
            assembly = Assembly.LoadFile(new FileInfo(fileName).FullName);
            types = assembly.GetTypes();
            foreach(var type in types)
            {
                Program.Debug("["+ type.Module.Name+"] Type: " + type.FullName);
                if(type.Name.Contains(text))
                {
                    Program.Debug("[" + type.Module.Name + "] Found: " + type.FullName, ConsoleColor.DarkGreen);
                    funcType = type;
                    obj = Activator.CreateInstance(funcType); 

                    foreach(var m in type.GetMethods())
                    {
                        if(m.Name.Contains("Initialize"))
                        {
                            Program.Debug("[" + type.Module.Name + "] Calling: " + m.Name, ConsoleColor.DarkYellow);
                            m.Invoke(obj, new object[] { functions });
                        }
                    }
                    
                }
            }
        }
    }
    public static class Cmd
    {

        private static Functions fnc = new Functions();


        public static string SyntaxCheck(string command)
        {
            return "";
        }

        //
        // This method just calls ProcessEx with the same arguments and writes everything to the log
        // We simply can call ProcessEx directly, but then we need to make some garbage in code. 
        //
        // Example:
        // Cmd.Process("if {msg caption||text||question||yesno}||==||Yes||Exit");
        //
        public static string Process(string command, Dictionary<string, int> Labels = null, bool ignoreLog = false, bool displayNoCmd = true)
        {
            if (Program.log != null && !ignoreLog && Variables.GetValue("ignoreLog") != "1") Program.log.WriteLine("[" + DateTime.Now.ToString() + "] [COMMAND] " + command); // write to the log
            if (ignoreLog) Variables.Set("ignoreLog", "1");

            if (Variables.GetValue("checkSyntax") == "1")
            {
                string SyntaxCheckResult = SyntaxCheck(command);
                if (SyntaxCheckResult != "") return SyntaxCheckResult;
            }

            string returnIn = ""; // will be  used for command like: lol<echo 123
            if (command.StartsWith("$") && command.Contains("=")) // check for $
            {
                string[] c = command.Split('=');
                command = command.Remove(0, c[0].Length + 1);
                returnIn = c[0].Remove(0, 1);
                if (returnIn.EndsWith("$")) returnIn = returnIn.Remove(returnIn.Length - 1, 1);
                //command = "set " + returnIn + Variables.GetValue("splitArgs") + command;
            }
            
            if (returnIn != "") displayNoCmd = false;

            command = ProcessString(command); // Parse command for some garbage and replace $variables... See Str for details

            string result = ProcessEx(command, Labels, displayNoCmd); // call ProcessEx and get returned value
            if (ignoreLog) Variables.Set("ignoreLog", "1");

            if (returnIn != "")
            {
                if (result == "CMD_NOT_FOUND")
                {
                    if (Variables.GetValue("invokeIgnoreBadCmd") == "1")
                    {
                        result = ProcessEx("#" + command, Labels);
                    }
                }

                Variables.Set(returnIn, result, new List<string> { "Method=" + Variables.GetValue("workingMethod") }); // if was used something like "lol<echo 123" write returned value in variable ("lol" in example)
            }

            if (Program.log != null && !ignoreLog && Variables.GetValue("ignoreLog") != "1") Program.log.WriteLine("[" + DateTime.Now.ToString() + "] [COMMAND] Result: " + result); // write to the log
            if (ignoreLog) Variables.Remove("ignoreLog");
            return result; // return the result from ProcessEx
        }

        //
        // This method processes command
        // Methods, escript.Functions Processing
        //
        private static string ProcessEx(string command, Dictionary<string, int> Labels = null, bool displayNoCmd = true)
        {
            if (String.IsNullOrWhiteSpace(command)) return "";
            Variables.Set("previousCommand", command);
            if (command.StartsWith("help") || command == "?") return ProcessEx("Help", Labels);
            // we can't use "if" and "for" in escript.Functions class as method name, so let's replace "if" to "If" and "for" to "For" lol
            else if (command.StartsWith("if ")) return ProcessEx(command.Replace("if ", "If "), Labels);
            else if (command.StartsWith("for ")) return ProcessEx(command.Replace("for ", "For "), Labels);
            else if (command.StartsWith("return ")) return ProcessEx(command.Replace("return ", "Return "), Labels);
            else if (command.StartsWith("#")) return command.Remove(0, 1); // remove first symbol
            else if (command.StartsWith(":")) return "0"; // for labels
            else if (command.StartsWith("-")) return "1"; //

            else
            {
                string eAp = EverythingAfter(command, ' ', 1);
                foreach (var m in GlobalVars.Methods)
                {
                    if (command.Split(' ')[0] == m.Name)
                    {
                        //Program.Debug("Running " + m.Name + " method!", ConsoleColor.DarkCyan);
                        Variables.SetVariable("workingMethodResult", "1");

                        object[] objs = eAp.Split(new string[] { Variables.GetValue("splitArgs") }, StringSplitOptions.RemoveEmptyEntries).ToArray<object>(); // split by "||" (default)


                        for (int aIdx = 0; aIdx < m.Arguments.Count; aIdx++)
                        {
                            var argObj = m.Arguments[aIdx];
                            var varName = argObj.Name;
                            var varValue = "";
                            try
                            {
                                varValue = objs[aIdx].ToString();
                            }
                            catch
                            {
                                varValue = argObj.DefaultValue;
                            }
                            varValue = ProcessArgument(varValue, Labels).ToString();
                            Variables.SetVariable(argObj.Name, varValue, new List<string>() { "Method=" + m.Name });
                        }

                        for (int idx = 0; idx < m.Code.Count; idx++)
                        {
                            Variables.SetVariable("workingMethod", m.Name);
                            if (Variables.GetValue("showCommands") == "1") EConsole.WriteLine(Variables.GetValue("invitation") + m.Name + "> " + m.Code[idx]);
                            bool ignoreLogOption = false;
                            if (m.Options.Contains("IgnoreLog")) ignoreLogOption = true;

                            string res = Cmd.Process(m.Code[idx], Labels, ignoreLogOption).ToString();

                            Program.SetResult(res);
                            if (Variables.GetValue("showResult") == "1" && !m.Code[idx].ToLower().StartsWith("return") && !m.Options.Contains("IgnorePrintResult"))
                            {
                                Program.PrintResult(Variables.GetValue("result"));
                            }



                            if (GlobalVars.StopProgram) break;
                            if (Variables.GetValue("workingMethodBreak") == "1")
                            {
                                Variables.Set("workingMethodBreak", "0");
                                break;
                            }
                        }



                        //for (int aIdx = 0; aIdx < m.Arguments.Count; aIdx++)
                        //{
                        //    var argObj = m.Arguments[aIdx];
                        //    Variables.Remove(Variables.GetVariable(argObj.Name));
                        //}

                        if (m.Options.Contains("Cleanup")) MethodCleanup(m.Name);

                        return Variables.GetValue("workingMethodResult");
                    }
                    if (GlobalVars.StopProgram) break;
                }

                if (Labels != null)
                {
                    if (command.StartsWith("goto")) // for labels, PLEASE REWRITE
                    {
                        string label = Cmd.EverythingAfter(command, ' ', 1);
                        ESCode.a = Labels[label];
                        return "1";
                    }

                }
                // 
                // Here we invoking methods in escript.Functions class
                //
                try
                {
                    fnc.Labels = Labels;
                    MethodInfo mth = null;
                    object target = fnc;

                    try
                    {
                        mth = fnc.GetType().GetMethod(command.Split(' ')[0]); // split command by space, used to split first argument. If there are no spaces then we will get just only command, without errors
                    }
                    catch { }


                    for (int i = 0; i < GlobalVars.LoadedLibs.Count; i++)
                    {
                        ImportLibClass imp = GlobalVars.LoadedLibs[i];
                        if (imp.obj != null)
                        {
                            if (imp.funcType.GetMethod(command.Split(' ')[0]) != null)
                            {
                                Program.Debug("Method " + command.Split(' ')[0] + " found in " + imp.funcType.FullName);
                                mth = imp.funcType.GetMethod(command.Split(' ')[0]);
                                target = imp.obj;
                            }
                        }
                    }

                    if (mth == null) throw new Exception("Method not found");

                    try
                    {
                        string p = EverythingAfter(command, ' ', 1); // Get everything after space
                        object[] objs = p.Split(new string[] { Variables.GetValue("splitArgs") }, StringSplitOptions.RemoveEmptyEntries).ToArray<object>(); // split by "||" (default)
                        for (int i = 0; i < objs.Length; i++)
                        {
                            objs[i] = ProcessArgument(objs[i], Labels);
                        }
                        var Args = mth.GetParameters();

                        if (objs.Length < Args.Length) // what the fuck is this idk 
                        {
                            for (int i = 0; i < Args.Length; i++)
                            {
                                try
                                {
                                    object aaa = objs[i];
                                    if (aaa == null) objs[i] = Args[i].DefaultValue;
                                }
                                catch
                                {
                                    var ox = objs.ToList();
                                    ox.Add(Args[i].DefaultValue);
                                    objs = ox.ToArray();
                                }
                            }
                        }

                        object theResult = "";

                        // INVOKE THE METHOD!!!!!!
                        theResult = mth.Invoke(target, objs);
                        if (theResult == null) theResult = "null";

                        if(theResult.GetType() == typeof(bool))
                        {
                            if ((bool)theResult) return "1";
                            else return "0";
                        }

                        return theResult.ToString();

                    }
                    catch (System.NullReferenceException)
                    {
                    }
                    catch (ArgumentException ex)
                    {
                        EConsole.WriteLine("ERROR: Invalid arguments", ConsoleColor.Red);
                        Program.Debug(ex.ToString(), ConsoleColor.DarkRed);
                        Process("UseTextTest " + command.Split(' ')[0], Labels); // Call UseTextTest method to show arguments of command

                        return EResult.Syntax.GetError("Invalid arguments");
                    }
                    catch (TargetInvocationException ex)
                    {
                        EConsole.WriteLine("ERROR: Can't invoke \"" + command.Split(' ')[0] + "\" method because of exception:", ConsoleColor.DarkRed);
                        EConsole.WriteLine(ex.InnerException.ToString(), ConsoleColor.Red);
                        Process("UseTextTest " + command.Split(' ')[0], Labels); // Call UseTextTest method to show arguments of command

                        return EResult.ESCRIPT.GetError("Exception");
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            var Args = mth.GetParameters();
                            List<object> defaultParams = new List<object>();
                            for (int i = 0; i < Args.Length; i++)
                            {
                                defaultParams.Add(Args[i].DefaultValue);
                            }
                            return mth.Invoke(target, defaultParams.ToArray()).ToString();
                        }
                        catch
                        {
                            EConsole.WriteLine(ex.GetType().Name + ": " + ex.Message, ConsoleColor.Red);
                            Program.Debug(ex.StackTrace, ConsoleColor.DarkRed);
                            Process("UseTextTest " + command.Split(' ')[0], Labels, true); // Call UseTextTest method to show arguments of command

                            return EResult.Cmd.GetError(ex.Message);
                        }

                    }
                }
                catch { }
            }
            string lt = command;
            if (command.Contains(' ')) lt = command.Split(' ')[0];
            if (lt == "if") Process("UseTextTest If", Labels, true);
            else if (lt == "for") Process("UseTextTest For", Labels, true);
            else if (lt == "return") Process("UseTextTest Return", Labels);
            else
            {
                if (displayNoCmd && Variables.GetValue("displayNoCmd") == "1")
                {
                    EConsole.WriteLine("No such method/command: " + lt);
                    EConsole.WriteLine("Need help? Type: help");
                }
            }

            return EResult.Cmd.Get("Not found").ToString();
        }

        public static object MethodCleanup(string MethodName)
        {
            List<EVariable> removeList = new List<EVariable>();

            for (int idx = 0; idx < Variables.VarList.Count; idx++)
            {
                EVariable var = Variables.VarList[idx];

                for (int optionIdx = 0; optionIdx < var.Options.Count; optionIdx++)
                {
                    string option = var.Options[optionIdx];
                    if (GlobalVars.RemoveDirtFromString(option) == "Method=" + GlobalVars.RemoveDirtFromString(MethodName))
                    {
                        removeList.Add(var);
                    }
                }
            }

            foreach(var var in removeList)
            {
                Program.Debug("Removing " + var.Name + " because of " + MethodName + " cleanup");
                Variables.Remove(var);
            }

            return 1;
        }

        public static string EverythingAfter(string text, char split = ' ', int startIdx = 0)
        {
            string[] r = text.Split(split);
            string result = "";
            for (int a = startIdx; a < r.Length; a++)
            {
                if (a == startIdx) result += r[a];
                else result += " " + r[a];
            }
            return result;
        }


        public static string RemoveHot(string k)
        {

            return k
                .Replace(Variables.GetValue("splitArgs"), "^split^")
                .Replace("{", "^(^")
                .Replace("}", "^)^")
                .Replace("$", "&#dollar;");
        }


        public static string ReturnHot(string k)
        {

            return k
                .Replace("^split^", Variables.GetValue("splitArgs"))
                .Replace("^(^", "{")
                .Replace("^)^", "}")
                .Replace("&#dollar;", "$");
        }

        public static bool IsInMethod(string code, string val)
        {
            var m=  GetMethodsInsideOfString(code);
            foreach(var a in m)
            {
                if (a.text.ToString().Contains(val)) return true;
            }
            return false;
        }

        private delegate string ProcessStringHandler(string str);
        public static string ProcessString(string str)
        {
            return ProcessStringEx(str);

            // todo
            ProcessStringHandler handler = new ProcessStringHandler(ProcessStringEx);

            int timeout = 3000;
            try
            {
                int e = int.Parse(Variables.GetValue("taskTimeout"));
                if (e >= 500) timeout = e;
            }
            catch { }

            IAsyncResult result = handler.BeginInvoke(str, null, null);

            if (result.AsyncWaitHandle.WaitOne(timeout))
            {
                return handler.EndInvoke(result);
            }
            else
                return str;

        }

        public static string ProcessStringVariables(string result, string open, string close)
        {
            for (int i = 0; i < Variables.VarList.Count; i++)
            {
                try
                {
                    string vName = Variables.VarList[i].Name;
                    string vValue = Variables.VarList[i].Value.ToString();
                    string eValueX = RemoveHot(vValue);
                    //Program.Debug("$" + vName + "$" + " -> " + eValueX, ConsoleColor.DarkMagenta);
                    result = result.Replace(open + vName + close, eValueX);


                }
                catch { }
            }
            return result;
        }

        private static string ProcessStringEx(string str)
        {
            if (str == null) return "";

            string result = str;


            //
            // $var$=value
            // Return $var$ -> value
            //
            if (Variables.GetValue("varParseWithEnd") == "1") result = ProcessStringVariables(result, "$", "$");

            //
            // $var=value
            // Return $var -> value
            //
            result = ProcessStringVariables(result, "$", "");

            //
            // {||} -> {^split^} 
            //
            // test: msg {{||}}||{{||}}||{Return 0}||{Return 0}
            List<iString> list = GetMethodsInsideOfString(result);
            foreach (var t in list)
            {
                // We can't process varible which contains $splitArgs (||)
                // so let's replace || with ^split^
                // then we will replace ^split^ back to ||
                result = result.Replace(t.text.ToString(), t.text.ToString().Replace(Variables.GetValue("splitArgs"), "^split^"));
            }




            //
            // &#dollar; -> $
            // ~n~ -> \r\n
            //
            result = result.Replace("&#dollar;", "$").Replace("~n~", Environment.NewLine);
            
            return result;
        }

        private delegate List<iString> GetMethodsInsideOfStringHandler(string result);
        public static List<iString> GetMethodsInsideOfString(string result)
        {
            return GetMethodsInsideOfStringEx(result);

            // todo
            GetMethodsInsideOfStringHandler handler = new GetMethodsInsideOfStringHandler(GetMethodsInsideOfStringEx);

            IAsyncResult res = handler.BeginInvoke(result, null, null);


            int timeout = 3000;
            try
            {
                int e = int.Parse(Variables.GetValue("taskTimeout"));
                if (e >= 500) timeout = e;
            }
            catch { }

            if (res.AsyncWaitHandle.WaitOne(timeout))
            {
                return handler.EndInvoke(res);
            }
            else
                return new List<iString>();

        }
        private static List<iString> GetMethodsInsideOfStringEx(string result)
        {
            List<iString> list = new List<iString>();
            if (result.Contains("{") && result.Contains("}"))
            {
                for (int i = 0; i < result.Length; i++)
                {
                    //try
                    //{
                        // not using {{ }} anymore, use {# } instead
                        // example: 
                        // async {#msg 1||2||3||4}

                        if (result[i] == '{' && result[i + 1] != '{') // for {}
                        {
                            iString test = new iString() { startIdx = i, endIdx = -1 };
                            for (int c = test.startIdx; c < result.Length; c++)
                            {
                                if (result[c] == '}')
                                {
                                    test.text.Append("}");
                                    test.endIdx = c + 1;
                                    break;
                                }
                                test.text.Append(result[c]);
                            }
                            if (test.endIdx == -1) throw new Exception("ESCRIPT_ERROR_INVALID_INVOKE");
                            i = test.endIdx;
                            list.Add(test);
                        }
                    //}
                    //catch (Exception ex)
                    //{
                    //    Program.Debug(ex.ToString(), ConsoleColor.DarkRed);
                    //}
                }
            }
            return list;
        }

        public static List<vString> GetVariablesInsideOfString(string result)
        {
            List<vString> list = new List<vString>();

            for (int i = 0; i < result.Length; i++)
            {
                try
                {
                    if (result[i] == '$')
                    {
                        vString vs = new vString() { startIdx = i };
                        for (int c = vs.startIdx; c < result.Length; c++)
                        {
                            if (result[c] == '|') break;
                            vs.endIdx = c;
                            vs.full.Append(result[c]);
                        }
                        i = vs.endIdx;
                        list.Add(vs);
                    }
                }
                catch (Exception ex)
                {
                    Program.Debug(ex.ToString(), ConsoleColor.DarkRed);
                }

            }
            return list;
        }

        public static object ProcessArgument(object arg, Dictionary<string, int> Labels = null)
        {
            arg = arg.ToString().Replace("^split^", Variables.GetValue("splitArgs")); // replace ^split^ to || (default) 
            List<iString> inside = GetMethodsInsideOfString(arg.ToString());
            foreach (var a in inside)
            {
                string icode = a.text.ToString();

                if (icode.StartsWith("{") && icode.EndsWith("}"))
                {
                    string clean = icode.Remove(0, 1).Remove(icode.Length - 2, 1);
                    string result = Cmd.Process(clean, Labels, false, false);
                    if (result == "CMD_NOT_FOUND")
                    {
                        if (Variables.GetValue("invokeIgnoreBadCmd") == "1")
                        {
                            result = Process("#" + clean, Labels);
                        }
                    }
                    //Program.Debug("{PARSE} " + arg + " -> " + result);

                    arg = arg.ToString().Replace(icode, result);
                }
            }

            arg = arg.ToString().Replace("^(^", "{").Replace("^)^", "}");

            //if (objs[i].ToString().Contains("{") && objs[i].ToString().Contains("}")) // check for {{}} lol
            //{
            //    if (objs[i].ToString().Contains("{{") && objs[i].ToString().Contains("}}")) // {{Text}}
            //    {

            //    }
            //    else// {Text}
            //    {

            //    }
            //}

            if (arg.ToString().StartsWith("#{")) arg = arg.ToString().TrimStart('#'); // #{Method}
            return arg;
        }

        public static string ReadConsoleLine()
        {
            if (Variables.GetValue("inputText") != "null") Console.Write(Variables.GetValue("inputText"));
            return EConsole.ReadLine();
        }
        public static string ReadConsoleKey()
        {
            if (Variables.GetValue("inputText") != "null") Console.Write(Variables.GetValue("inputText"));
            string k = EConsole.ReadKey().KeyChar.ToString();
            EConsole.WriteLine("");
            return k;
        }
        public static void HideConsoleTest()
        {
#if !IsCore
            var handle = GlobalVars.GetConsoleWindow();
            GlobalVars.ShowWindow(handle, GlobalVars.SW_HIDE);
#endif
        }
        
    }
    public class iString
    {
        public int startIdx;
        public int endIdx;
        public StringBuilder text = new StringBuilder();
    }

    public class vString
    {
        public int startIdx;
        public int endIdx;
    public StringBuilder full = new StringBuilder();
        
    }

}
