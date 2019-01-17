#if ESCRIPT
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CmdSharp
{
    public class ESCode
    {
        public static int a = 0;
        public Dictionary<string, int> Labels = new Dictionary<string, int>();
        public FileInfo fileInfo = null;
        public string[] ScriptContent = { };

        public string FullName
        {
            get { if (fileInfo == null) return ""; else return fileInfo.FullName; }
            set { fileInfo = new FileInfo(value); }
        }


        public ESCode(string fileName)
        {
            fileInfo = new FileInfo(fileName);
            StreamReader reader = new StreamReader(fileInfo.FullName);
            string[] code = reader.ReadToEnd().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            reader.Close();
            reader.Dispose();
            NewFromCode(code);
        }
        

        public ESCode(string[] code)
        {
            fileInfo = GlobalVars.GetAboutMe();
            NewFromCode(code);
        }


        public static string[] SplitCode(string code, string[] seperator = null)
        {
            if(code.StartsWith("BASE64:"))
            {
                code = GlobalVars.Base64Decode(code.Remove(0, "BASE64:".Length));
            }
            List<string> s = new List<string>() { "\r\n", "\n" };
            if (seperator != null)
            {
                foreach(var str in seperator)
                {
                    s.Add(str);
                }
            }
            return code.Split(s.ToArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        private void GenerateBlocks()
        {
            return; // it can brake scripts so fak it
            for (int m = 0; m < ScriptContent.Length; m++)
            {
                try
                {
                    if (ScriptContent[m] == "{" && !ScriptContent[m - 1].Contains("func"))
                    {
                        int methodStartIdx = m + 1;
                        int end = 0;
                        EMethod blockMethod = new EMethod();
                        blockMethod.Name = "Block_" + GlobalVars.RandomString(10);
                        blockMethod.Options = new List<string>() { "Hidden", "ESCRIPT" };

                        for (int c = methodStartIdx; c < ScriptContent.Length; c++)
                        {
                            if (!ScriptContent[c].StartsWith("}"))
                            {

                                if (ScriptContent[c].Length <= 0)
                                    continue;
                                if (ScriptContent[c].StartsWith("//"))
                                    continue;

                                blockMethod.Code.Add(ScriptContent[c]);
                            }
                            else
                            {
                                end = c;
                                break;
                            }
                        }

                        GlobalVars.Methods.Add(blockMethod);
                        ScriptContent = GlobalVars.RemoveEntries(ScriptContent, m + 1, end);
                        ScriptContent[m] = ScriptContent[m].Replace("{", blockMethod.Name);
                        Program.Debug($"Added block: {blockMethod.Name} at {m} line");
                    }
                }
                catch { }
            }
        }

        private void CleanCode()
        {
            for (int m = 0; m < ScriptContent.Length; m++)
            {
                ScriptContent[m] = GlobalVars.RemoveDirtFromString(ScriptContent[m]);

                if (ScriptContent[m].StartsWith("//"))
                    GlobalVars.RemoveEntry(ScriptContent, ScriptContent[m]);
            }
        }

        private void GenerateLabels()
        {
            for (int m = 0; m < ScriptContent.Length; m++)
            {
                if (ScriptContent[m].Length <= 1) continue;

                if (ScriptContent[m][0] == ':')
                {
                    Labels.Add(ScriptContent[m].Remove(0, 1), m);
                    Program.Debug("Added label: " + ScriptContent[m].Remove(0, 1) + ", line: " + m + 1, ConsoleColor.DarkGreen);
                }
            }
        }

        private void GenerateMethods()
        {

            for (int m = 0; m < ScriptContent.Length; m++)//methods & labels & dirtycode
            {

                if (ScriptContent[m].Length <= 1) continue;
                if (ScriptContent[m].StartsWith("//")) continue;
                if (ScriptContent[m].StartsWith("func "))
                {

                    int methodStartIdx = 0;
                    if (ScriptContent[m + 1].StartsWith("{")) methodStartIdx = m + 2;
                    EMethod thisMethod = new EMethod();
                    thisMethod.Name = ScriptContent[m].Remove(0, "func ".Length).Split(' ')[0];
                    try
                    {
                        var argList1 = ScriptContent[m].Remove(0, "func ".Length + thisMethod.Name.Length + 1);
                        var mth = Cmd.GetMethodsInsideOfString(argList1);
                        var argListNoDefault = argList1;

                        foreach (var argStr in mth)
                        {
                            string fstr = "=" + argStr.text.ToString();
                            argListNoDefault = argListNoDefault.Replace(fstr, "");
                        }

                        var argList = argListNoDefault.Split(' ').ToList();

                        foreach (var arg in argList)
                        {
                            if (arg.StartsWith("(") && arg.EndsWith(")"))
                            {
                                thisMethod.Options.Add(arg.Remove(0, 1).Remove(arg.Length - 2, 1));
                                continue;
                            }
                            EMethodArgument argObj = new EMethodArgument();
                            argObj.Name = arg;
                            foreach (var argStr in mth)
                            {
                                string fstr = arg + "=" + argStr.text.ToString();
                                if (argList1.Contains(fstr))
                                {
                                    argObj.DefaultValue = argStr.text.ToString().Remove(0, 1).Remove(argStr.text.ToString().Length - 2, 1);
                                }
                                else
                                {
                                }
                            }
                            thisMethod.Arguments.Add(argObj);
                        }
                    }
                    catch
                    {
                        //EConsole.WriteLine($"ERROR PROCESSING {thisMethod.Name}!{Environment.NewLine}{ex.ToString()}", true, ConsoleColor.Red);
                    }

                    int end = 0;
                    for (int c = methodStartIdx; c < ScriptContent.Length; c++)
                    {
                        if (!ScriptContent[c].StartsWith("}") && !ScriptContent[m].StartsWith("//"))
                        {
                            ScriptContent[c] = GlobalVars.RemoveDirtFromString(ScriptContent[c]);

                            if (ScriptContent[c].Length <= 0)
                                continue;
                            if (ScriptContent[c].StartsWith("//"))
                                continue;

                            thisMethod.Code.Add(ScriptContent[c]);
                        }
                        else
                        {
                            end = c;
                            break;
                        }
                    }
                    m = end;

                    for (int x = 0; x < GlobalVars.Methods.Count; x++)
                    {
                        if (GlobalVars.Methods[x].Name == thisMethod.Name)
                        {
                            Program.Debug($"Overwriting exiting method ({thisMethod.Name})");
                            GlobalVars.Methods.Remove(GlobalVars.Methods[x]);
                        }
                    }

                    GlobalVars.Methods.Add(thisMethod);

                    Program.Debug("Added method: " + thisMethod.Name + ", line: " + m + 1, ConsoleColor.DarkGreen);


                    if (thisMethod.Arguments.Count >= 1)
                    {
                        Program.Debug("Arguments:");
                        foreach (var argument in thisMethod.Arguments)
                        {
                            Program.Debug($"\t{argument.Name} = {argument.DefaultValue}");
                        }
                    }

                    if (thisMethod.Options.Count >= 1)
                    {
                        Program.Debug("Options:");
                        foreach (var option in thisMethod.Options)
                        {
                            Program.Debug($"\t{option}");
                        }
                    }
                }


            }
        }

        private void NewFromCode(string[] code)
        {
            ScriptContent = code;
            Program.Debug($"Processing {fileInfo.Name}...", ConsoleColor.DarkCyan);

            CleanCode();

            GenerateMethods();

            GenerateBlocks();

            GenerateLabels();

            Program.Debug($"Processing {fileInfo.Name}: completed", ConsoleColor.DarkGreen); 
        }

        public bool ThereIsMainMethod(List<EMethod> methods)
        {
            foreach (var method in methods)
                if (method.Name == "Main")
                    return true;

            return false;
        }

        public void RunScript()
        {
            Program.Debug(" - script start -");

            StringBuilder wCode = new StringBuilder();
            for (int i = 0; i < ScriptContent.Length; i++)
            {
                wCode.AppendLine(ScriptContent[i]);
            }

            Variables.Add("workingScriptText", fileInfo.Name);
            Variables.Add("workingScriptFileName", fileInfo.Name);
            Variables.Add("workingScriptFullFileName", fileInfo.FullName);
            Variables.Add("workingScriptCode", wCode.ToString());
            Variables.Add("workingMethodBreak", "0");
            Variables.Add("workingMethodResult", "0");

            Cmd.Process("title " + fileInfo.Name);
            
            if (ThereIsMainMethod(GlobalVars.Methods))
            {
                if (Variables.GetValue("showCommands") == "1")
                    EConsole.WriteLine(Variables.GetValue("invitation") + "Main");

                object result = Cmd.Process("Main"/*, FIX ME Labels*/);

                Program.SetResult(result);

                if (Variables.GetValue("showResult") == "1")
                {
                    Program.PrintResult(Variables.GetValueObject("result"));
                }
            }
            else
            {
                Program.Debug("'Main' NOT found!");
                for (a = 0; a < ScriptContent.Length; a++)//code processing
                {
                    string line = ScriptContent[a];
                    if (line.Length <= 0) continue;
                    if (line.StartsWith("//")) continue;

                    if (line.StartsWith("func ")) // skip code if it's a method
                    {
                        foreach (var m in GlobalVars.Methods)
                        {
                            if (m.Name == line.Remove(0, "func ".Length))
                            {
                                a += (m.Code.Count + 2);
                            }
                        }
                    }
                    else
                    {
                        if (Variables.GetValue("showCommands") == "1") EConsole.WriteLine(Variables.GetValue("invitation") + line);

                        object result = Cmd.Process(line/*, FIX ME Labels*/);
                        Program.SetResult(result);
                        if (Variables.GetValue("showResult") == "1")
                        {
                            Program.PrintResult(Variables.GetValueObject("result"));
                        }

                        if (GlobalVars.StopProgram) break;
                    }
                    if (Variables.GetValue("forceGC") == "1")
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
            }
            Program.Debug(" - script end -");
        }
    }
}
#endif