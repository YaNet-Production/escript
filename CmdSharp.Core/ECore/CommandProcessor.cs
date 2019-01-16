using CmdSharp.Parser;
using CmdSharp.Stuff.Extensions;
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

namespace CmdSharp
{
    public class TestClass
    {
        object t;
        string a, b;
        public TestClass(object toStringText, string s = "default test", string c = "default test 2")
        {
            t = toStringText;
            a = s;
            b = c;
        }
        public override string ToString()
        {
            return "escript.TestClass(string toStringText) { " + t.ToString() + ", a: " + a + ", b: " + b + " }";
        }
    }
    public class SuperClass
    {
        object t;
        public SuperClass(object toStringText)
        {
            t = toStringText;
        }
        public override string ToString()
        {
            return "escript.SuperClass(string toStringText) { " + t.ToString() + " }";
        }
    }

    public class CommandLineClass
    {
        public string OrignalCommand;
        public string Command;
        public List<object> Arguments;

        public CommandLineClass(string c)
        {
            OrignalCommand = c;
        }

        public CommandLineClass() { }
    }

    public class Cmd
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
        public static object Process(string command, bool HandleExceptions = true)
        {
            // Dev Example:
            // int kek = 25;
            // Test(125, "K;ek", kek, &kek /* ptr */, Test, FFF(TTT())); AAA();
            object ProcessExResult = null;
            
            try
            {
                var Normalized = CommandNormalizer.NormalizeInvokeLines(command);

                if (Normalized.Length == 0)
                    throw new Exception("The commands array is empty.");
                else if (Normalized.Length == 1)
                {
                    ProcessExResult = EParserDz3n.Process(Normalized[0]);
                }
                else if (Normalized.Length >= 2)
                {
                    List<object> Result = new List<object>();
                    foreach(var cmd in Normalized)
                    {
                        Result.Add(EParserDz3n.Process(cmd));
                    }
                    ProcessExResult = Result.ToArray();
                }
            }
            catch (Exception ex)
            {
                if (!HandleExceptions)
                    throw ex;

                EConsole.WriteLine(ex.GetType().Name + ": " + ex.Message, ConsoleColor.Red);
                Debug.DebugText(ex.StackTrace, ConsoleColor.DarkRed);

                return null; 
            }

            return ProcessExResult;
        }

        public static object CreateInstance(string TypeName, object[] Arguments = null)
        {
            if (Arguments == null)
                Arguments = new object[] { };

            Type type = null;
            object ResultInstance = null;

            var SearchResult = EnvironmentManager.SearchForType(TypeName);
            if (SearchResult == null)
                throw new Exception($"Type '{TypeName}' not found.");

            ResultInstance = SearchResult.CreateInstance(Arguments);
            
            return ResultInstance;
        }

        public static object InvokeMethod(string Name, object[] Arguments = null)
        {
            foreach(var m in EnvironmentManager.AllMethods)
            {
                if(m.Name == Name)
                {

                    try
                    {
                        return m.Invoke(Arguments);

                    }
                    catch (ArgumentException ex)
                    {
                        EConsole.WriteLine("ERROR: Invalid arguments", ConsoleColor.Red);
                        Debug.DebugText(ex.ToString(), ConsoleColor.DarkRed);
                        Process("UseTextTest(\"" + Name + "\");");// Call UseTextTest method to show arguments of command

                        return null;
                    }
                    catch (TargetInvocationException ex)
                    {
                        EConsole.WriteLine("ERROR: Can't invoke \"" + Name + "\" method because of exception:", ConsoleColor.Red);
                        EConsole.WriteLine(ex.InnerException.ToString(), ConsoleColor.DarkRed);
                        Process("UseTextTest(\"" + Name + "\");");// Call UseTextTest method to show arguments of command

                        return null;// EResult.ESCRIPT.GetError("Exception");
                    }
                    catch (Exception ex)
                    {
                        EConsole.WriteLine(ex.GetType().Name + ": " + ex.Message, ConsoleColor.Red);
                        Debug.DebugText(ex.StackTrace, ConsoleColor.DarkRed);
                        Process("UseTextTest(\"" + Name + "\");"); // Call UseTextTest method to show arguments of command

                        return null;//EResult.Cmd.GetError(ex.Message);
                    }
                }
            }
            throw new Exceptions.InvokeMethodNotFoundException($"Method '{Name}' is not found");


#if ESCRIPT
            object methodClassInstance = fnc;
            MethodInfo method = null;

            if (Arguments == null)
                Arguments = new object[] { };

            try
            {
                method = fnc.GetType().GetMethod(Name);
            }
            catch { }


            for (int i = 0; i < GlobalVars.LoadedLibs.Count; i++)
            {
                ImportedLibInfo imp = GlobalVars.LoadedLibs[i];
                if (imp.classInstance != null)
                {
                    var ImportedMethod = imp.funcType.GetMethod(Name);
                    if (ImportedMethod != null)
                    {
                        Program.Debug("Method " + Name + " found in " + imp.funcType.FullName);
                        method = ImportedMethod;
                        methodClassInstance = imp.classInstance;
                        break;
                    }
                }
            }

            if (method == null)
                throw new Exceptions.InvokeMethodNotFoundException("Method not found");

            try
            {
                var OriginalArguments = method.GetParameters();

                if (Arguments.Length < OriginalArguments.Length)
                {
                    for (int i = 0; i < OriginalArguments.Length; i++)
                    {
                        try // If argument is empty/broken, set it's value to method's default value.
                        {
                            object aaa = Arguments[i];
                            if (aaa == null) Arguments[i] = OriginalArguments[i].DefaultValue;
                        }
                        catch // If argument not found, append default value to the array
                        {
                            var TempList = Arguments.ToList();
                            TempList.Add(OriginalArguments[i].DefaultValue);
                            Arguments = TempList.ToArray();
                        }
                    }
                }

                object theResult = null;

                theResult = method.Invoke(methodClassInstance, Arguments);

                return theResult;

            }
            catch (ArgumentException ex)
            {
                EConsole.WriteLine("ERROR: Invalid arguments", ConsoleColor.Red);
                Program.Debug(ex.ToString(), ConsoleColor.DarkRed);
                Process("UseTextTest(\"" + Name + "\");");// Call UseTextTest method to show arguments of command

                return null;
            }
            catch (TargetInvocationException ex)
            {
                EConsole.WriteLine("ERROR: Can't invoke \"" + Name + "\" method because of exception:", ConsoleColor.Red);
                EConsole.WriteLine(ex.InnerException.ToString(), ConsoleColor.DarkRed);
                Process("UseTextTest(\"" + Name + "\");");// Call UseTextTest method to show arguments of command

                return null;// EResult.ESCRIPT.GetError("Exception");
            }
            catch (Exception ex)
            {
                EConsole.WriteLine(ex.GetType().Name + ": " + ex.Message, ConsoleColor.Red);
                Program.Debug(ex.StackTrace, ConsoleColor.DarkRed);
                Process("UseTextTest(\"" + Name + "\");"); // Call UseTextTest method to show arguments of command

                return null;//EResult.Cmd.GetError(ex.Message);
            }
#endif
        }

        //public string ProcessOld(string command, Dictionary<string, int> Labels = null, bool ignoreLog = false, bool displayNoCmd = true)
        //{
        //    if (Program.log != null && !ignoreLog && Variables.GetValue("ignoreLog") != "1") Program.log.WriteLine("[" + DateTime.Now.ToString() + "] [COMMAND] " + command); // write to the log
        //    if (ignoreLog) Variables.Set("ignoreLog", "1");

        //    if (Variables.GetValue("checkSyntax") == "1")
        //    {
        //        string SyntaxCheckResult = SyntaxCheck(command);
        //        if (SyntaxCheckResult != "") return SyntaxCheckResult;
        //    }

        //    string returnIn = ""; // will be  used for command like: lol<echo 123
        //    if (command.StartsWith("$") && command.Contains("=")) // check for $
        //    {
        //        string[] c = command.Split('=');
        //        command = command.Remove(0, c[0].Length + 1);
        //        returnIn = c[0].Remove(0, 1);
        //        if (returnIn.EndsWith("$")) returnIn = returnIn.Remove(returnIn.Length - 1, 1);
        //        //command = "set " + returnIn + Variables.GetValue("splitArgs") + command;
        //    }

        //    if (returnIn != "") displayNoCmd = false;

        //    command = ProcessString(command); // Parse command for some garbage and replace $variables... See Str for details

        //    string result = ProcessEx(command, Labels, displayNoCmd); // call ProcessEx and get returned value
        //    if (ignoreLog) Variables.Set("ignoreLog", "1");

        //    if (returnIn != "")
        //    {
        //        if (result == "CMD_NOT_FOUND")
        //        {
        //            if (Variables.GetValue("invokeIgnoreBadCmd") == "1")
        //            {
        //                result = ProcessEx("#" + command, Labels);
        //            }
        //        }

        //        Variables.Set(returnIn, result, new List<string> { "Method=" + Variables.GetValue("workingMethod") }); // if was used something like "lol<echo 123" write returned value in variable ("lol" in example)
        //    }

        //    if (Program.log != null && !ignoreLog && Variables.GetValue("ignoreLog") != "1") Program.log.WriteLine("[" + DateTime.Now.ToString() + "] [COMMAND] Result: " + result); // write to the log
        //    if (ignoreLog) Variables.Remove("ignoreLog");
        //    return result; // return the result from ProcessEx
        //}

        ////
        //// This method processes command
        //// Methods, escript.Functions Processing
        ////
        //private string ProcessExOld(string command, Dictionary<string, int> Labels = null, bool displayNoCmd = true)
        //{
        //    if (String.IsNullOrWhiteSpace(command)) return "";
        //    Variables.Set("previousCommand", command);
        //    if (command.StartsWith("help") || command == "?") return ProcessEx("Help", Labels);
        //    // we can't use "if" and "for" in escript.Functions class as method name, so let's replace "if" to "If" and "for" to "For" lol
        //    else if (command.StartsWith("if ")) return ProcessEx(command.Replace("if ", "If "), Labels);
        //    else if (command.StartsWith("for ")) return ProcessEx(command.Replace("for ", "For "), Labels);
        //    else if (command.StartsWith("return ")) return ProcessEx(command.Replace("return ", "Return "), Labels);
        //    else if (command.StartsWith("#")) return command.Remove(0, 1); // remove first symbol
        //    else if (command.StartsWith(":")) return "0"; // for labels
        //    else if (command.StartsWith("-")) return "1"; //

        //    else
        //    {
        //        string eAp = EverythingAfter(command, ' ', 1);
        //        foreach (var m in GlobalVars.Methods)
        //        {
        //            if (command.Split(' ')[0] == m.Name)
        //            {
        //                //Program.Debug("Running " + m.Name + " method!", ConsoleColor.DarkCyan);
        //                Variables.SetVariable("workingMethodResult", "1");

        //                object[] objs = eAp.Split(new string[] { Variables.GetValue("splitArgs") }, StringSplitOptions.RemoveEmptyEntries).ToArray<object>(); // split by "||" (default)


        //                for (int aIdx = 0; aIdx < m.Arguments.Count; aIdx++)
        //                {
        //                    var argObj = m.Arguments[aIdx];
        //                    var varName = argObj.Name;
        //                    var varValue = "";
        //                    try
        //                    {
        //                        varValue = objs[aIdx].ToString();
        //                    }
        //                    catch
        //                    {
        //                        varValue = argObj.DefaultValue;
        //                    }
        //                    varValue = ProcessArgument(varValue, Labels).ToString();
        //                    Variables.SetVariable(argObj.Name, varValue, new List<string>() { "Method=" + m.Name });
        //                }

        //                for (int idx = 0; idx < m.Code.Count; idx++)
        //                {
        //                    Variables.SetVariable("workingMethod", m.Name);
        //                    if (Variables.GetValue("showCommands") == "1") EConsole.WriteLine(Variables.GetValue("invitation") + m.Name + "> " + m.Code[idx]);
        //                    bool ignoreLogOption = false;
        //                    if (m.Options.Contains("IgnoreLog")) ignoreLogOption = true;

        //                    string res = Cmd.Process(m.Code[idx], Labels, ignoreLogOption).ToString();

        //                    Program.SetResult(res);
        //                    if (Variables.GetValue("showResult") == "1" && !m.Code[idx].ToLower().StartsWith("return") && !m.Options.Contains("IgnorePrintResult"))
        //                    {
        //                        Program.PrintResult(Variables.GetValue("result"));
        //                    }



        //                    if (GlobalVars.StopProgram) break;
        //                    if (Variables.GetValue("workingMethodBreak") == "1")
        //                    {
        //                        Variables.Set("workingMethodBreak", "0");
        //                        break;
        //                    }
        //                }



        //                //for (int aIdx = 0; aIdx < m.Arguments.Count; aIdx++)
        //                //{
        //                //    var argObj = m.Arguments[aIdx];
        //                //    Variables.Remove(Variables.GetVariable(argObj.Name));
        //                //}

        //                if (m.Options.Contains("Cleanup")) MethodCleanup(m.Name);

        //                return Variables.GetValue("workingMethodResult");
        //            }
        //            if (GlobalVars.StopProgram) break;
        //        }

        //        if (Labels != null)
        //        {
        //            if (command.StartsWith("goto")) // for labels, PLEASE REWRITE
        //            {
        //                string label = Cmd.EverythingAfter(command, ' ', 1);
        //                ESCode.a = Labels[label];
        //                return "1";
        //            }

        //        }
        //        // 
        //        // Here we invoking methods in escript.Functions class
        //        //
        //        try
        //        {
        //            fnc.Labels = Labels;
        //            MethodInfo mth = null;
        //            object target = fnc;

        //            try
        //            {
        //                mth = fnc.GetType().GetMethod(command.Split(' ')[0]); // split command by space, used to split first argument. If there are no spaces then we will get just only command, without errors
        //            }
        //            catch { }


        //            for (int i = 0; i < GlobalVars.LoadedLibs.Count; i++)
        //            {
        //                ImportLibClass imp = GlobalVars.LoadedLibs[i];
        //                if (imp.obj != null)
        //                {
        //                    if (imp.funcType.GetMethod(command.Split(' ')[0]) != null)
        //                    {
        //                        Program.Debug("Method " + command.Split(' ')[0] + " found in " + imp.funcType.FullName);
        //                        mth = imp.funcType.GetMethod(command.Split(' ')[0]);
        //                        target = imp.obj;
        //                    }
        //                }
        //            }

        //            if (mth == null) throw new Exception("Method not found");

        //            try
        //            {
        //                string p = EverythingAfter(command, ' ', 1); // Get everything after space
        //                object[] objs = p.Split(new string[] { Variables.GetValue("splitArgs") }, StringSplitOptions.RemoveEmptyEntries).ToArray<object>(); // split by "||" (default)
        //                for (int i = 0; i < objs.Length; i++)
        //                {
        //                    objs[i] = ProcessArgument(objs[i], Labels);
        //                }
        //                var Args = mth.GetParameters();

        //                if (objs.Length < Args.Length) // what the fuck is this idk 
        //                {
        //                    for (int i = 0; i < Args.Length; i++)
        //                    {
        //                        try
        //                        {
        //                            object aaa = objs[i];
        //                            if (aaa == null) objs[i] = Args[i].DefaultValue;
        //                        }
        //                        catch
        //                        {
        //                            var ox = objs.ToList();
        //                            ox.Add(Args[i].DefaultValue);
        //                            objs = ox.ToArray();
        //                        }
        //                    }
        //                }

        //                object theResult = "";

        //                // INVOKE THE METHOD!!!!!!
        //                theResult = mth.Invoke(target, objs);
        //                if (theResult == null) theResult = "null";

        //                if(theResult.GetType() == typeof(bool))
        //                {
        //                    if ((bool)theResult) return "1";
        //                    else return "0";
        //                }

        //                return theResult.ToString();

        //            }
        //            catch (System.NullReferenceException)
        //            {
        //            }
        //            catch (ArgumentException ex)
        //            {
        //                EConsole.WriteLine("ERROR: Invalid arguments", ConsoleColor.Red);
        //                Program.Debug(ex.ToString(), ConsoleColor.DarkRed);
        //                Process("UseTextTest " + command.Split(' ')[0], Labels); // Call UseTextTest method to show arguments of command

        //                return EResult.Syntax.GetError("Invalid arguments");
        //            }
        //            catch (TargetInvocationException ex)
        //            {
        //                EConsole.WriteLine("ERROR: Can't invoke \"" + command.Split(' ')[0] + "\" method because of exception:", ConsoleColor.DarkRed);
        //                EConsole.WriteLine(ex.InnerException.ToString(), ConsoleColor.Red);
        //                Process("UseTextTest " + command.Split(' ')[0], Labels); // Call UseTextTest method to show arguments of command

        //                return EResult.ESCRIPT.GetError("Exception");
        //            }
        //            catch (Exception ex)
        //            {
        //                try
        //                {
        //                    var Args = mth.GetParameters();
        //                    List<object> defaultParams = new List<object>();
        //                    for (int i = 0; i < Args.Length; i++)
        //                    {
        //                        defaultParams.Add(Args[i].DefaultValue);
        //                    }
        //                    return mth.Invoke(target, defaultParams.ToArray()).ToString();
        //                }
        //                catch
        //                {
        //                    EConsole.WriteLine(ex.GetType().Name + ": " + ex.Message, ConsoleColor.Red);
        //                    Program.Debug(ex.StackTrace, ConsoleColor.DarkRed);
        //                    Process("UseTextTest " + command.Split(' ')[0], Labels, true); // Call UseTextTest method to show arguments of command

        //                    return EResult.Cmd.GetError(ex.Message);
        //                }

        //            }
        //        }
        //        catch { }
        //    }
        //    string lt = command;
        //    if (command.Contains(' ')) lt = command.Split(' ')[0];
        //    if (lt == "if") Process("UseTextTest If", Labels, true);
        //    else if (lt == "for") Process("UseTextTest For", Labels, true);
        //    else if (lt == "return") Process("UseTextTest Return", Labels);
        //    else
        //    {
        //        if (displayNoCmd && Variables.GetValue("displayNoCmd") == "1")
        //        {
        //            EConsole.WriteLine("No such method/command: " + lt);
        //            EConsole.WriteLine("Need help? Type: help");
        //        }
        //    }

        //    return EResult.Cmd.Get("Not found").ToString();
        //}

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
                Debug.DebugText("Removing " + var.Name + " because of " + MethodName + " cleanup");
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
                    Debug.DebugText(ex.ToString(), ConsoleColor.DarkRed);
                }

            }
            return list;
        }

        //public object ProcessArgument(object arg, Dictionary<string, int> Labels = null)
        //{
        //    arg = arg.ToString().Replace("^split^", Variables.GetValue("splitArgs")); // replace ^split^ to || (default) 
        //    List<iString> inside = GetMethodsInsideOfString(arg.ToString());
        //    foreach (var a in inside)
        //    {
        //        string icode = a.text.ToString();

        //        if (icode.StartsWith("{") && icode.EndsWith("}"))
        //        {
        //            string clean = icode.Remove(0, 1).Remove(icode.Length - 2, 1);
        //            string result = Cmd.Process(clean, null, false, false);
        //            if (result == "CMD_NOT_FOUND")
        //            {
        //                if (Variables.GetValue("invokeIgnoreBadCmd") == "1")
        //                {
        //                    result = Process("#" + clean, null);
        //                }
        //            }
        //            //Program.Debug("{PARSE} " + arg + " -> " + result);

        //            arg = arg.ToString().Replace(icode, result);
        //        }
        //    }

        //    arg = arg.ToString().Replace("^(^", "{").Replace("^)^", "}");

        //    //if (objs[i].ToString().Contains("{") && objs[i].ToString().Contains("}")) // check for {{}} lol
        //    //{
        //    //    if (objs[i].ToString().Contains("{{") && objs[i].ToString().Contains("}}")) // {{Text}}
        //    //    {

        //    //    }
        //    //    else// {Text}
        //    //    {

        //    //    }
        //    //}

        //    if (arg.ToString().StartsWith("#{")) arg = arg.ToString().TrimStart('#'); // #{Method}
        //    return arg;
        //}

        public string ReadConsoleLine()
        {
            if (Variables.GetValue("inputText") != "null") Console.Write(Variables.GetValue("inputText"));
            return EConsole.ReadLine();
        }
        public string ReadConsoleKey()
        {
            if (Variables.GetValue("inputText") != "null") Console.Write(Variables.GetValue("inputText"));
            string k = EConsole.ReadKey().KeyChar.ToString();
            EConsole.WriteLine("");
            return k;
        }
        public void HideConsoleTest()
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
