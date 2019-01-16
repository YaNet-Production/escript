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
        }
    }
}
