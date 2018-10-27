using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace escript
{
    class Cmd
    {
        
        private static Functions fnc = new Functions();
        public static Dictionary<string, string> CmdParams = new Dictionary<string, string>();
        public static string Process(string command, List<EMethod> Methods, Dictionary<string, int> Labels)
        {
            if (String.IsNullOrWhiteSpace(command)) return "0";
            //Program.ConWrLine(command);
            CmdParams["previousCommand"] = command;
            if (command.StartsWith("help") || command == "?")
            {
                return Process("Help", Methods, Labels);
            }
            else if (command.StartsWith("if ")) return Process(command.Replace("if ", "ifvar "), Methods, Labels);
            else if (command.StartsWith("for ")) return Process(command.Replace("for ", "Times "), Methods, Labels);
            else if (command.StartsWith(":")) { return "0"; }
            else if (command.StartsWith("-"))
            {
                return "1";
            }
            else if (command.StartsWith("set"))
            {
                if (command.Split(' ').Length <= 1)
                {
                    ConsoleColor cccc = Console.ForegroundColor;
                    int idx = 0;
                    foreach (var item in CmdParams)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Program.ConWrite("[" + idx + "] ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Program.ConWrite(item.Key);
                        Console.ForegroundColor = ConsoleColor.White;
                        Program.ConWrite(" = ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Program.ConWrite(item.Value);
                        Program.ConWrLine("");
                        idx++;
                    }
                    Console.ForegroundColor = cccc;
                }
                else if (command.Split(' ').Length >= 3)
                {
                    string key = command.Remove(0, "set ".Length).Split(' ')[0];
                    string l = StrSp(command.Remove(0, "set ".Length), ' ', 1);
                    if (String.IsNullOrWhiteSpace(key) || String.IsNullOrWhiteSpace(l))
                    {
                        Program.ConWrLine("Error");
                        return "0";
                    }
                    if (CmdParams.ContainsKey(key)) CmdParams.Remove(key);
                    CmdParams.Add(key, l);
                }
                else
                {
                    string key = command.Remove(0, "set ".Length).Split(' ')[0];
                    Program.ConWrLine(key);
                    if (!CmdParams.ContainsKey(key)) return "0";
                    CmdParams.Remove(key);
                }
                return "1";
            }
            else
            {
                if (Methods != null && Labels != null)
                {
                    if (command.StartsWith("goto"))
                    {
                        string label = Cmd.StrSp(command, ' ', 1);
                        // Program.ConWrLine(label);
                        Program.a = Labels[label];
                        return "1";
                    }
                    foreach (var m in Methods)
                    {
                        if (Cmd.Str(command).StartsWith(m.Name))
                        {
                            //Program.ConWrLine("Running " + m.Name + " method!");
                            for (int idx = 0; idx < m.Code.Count; idx++)
                            {
                                CmdParams["workingMethod"] = m.Name;
                                if (Cmd.CmdParams["showCommands"] == "1") Program.ConWrLine(Cmd.CmdParams["invitation"] + m.Name + "> " + m.Code[idx]);
                                string res = Cmd.Process(Cmd.Str(m.Code[idx]), Methods, Labels).ToString();
                                Program.SetResult(res);
                                if (Cmd.CmdParams["showResult"] == "1")
                                {
                                    Program.PrintResult(Cmd.CmdParams["result"]);
                                }

                                CmdParams.Remove("workingMethod");
                            }

                            return "1";
                        }
                    }
                }
                try
                {
                    fnc.Methods = Methods;
                    fnc.Labels = Labels;
                    MethodInfo mth = fnc.GetType().GetMethod(command.Split(' ')[0]);
                    try
                    {
                        //var Args = mth.GetParameters();
                        //Program.ConWrLine("Method: " + mth.Name + ", arguments:");
                        //for (int i = 0; i < Args.Length; i++)
                        //{
                        //    Program.ConWrLine(Args[i].Name + " = " + Args[i].DefaultValue);
                        //}

                        string p = StrSp(command, ' ', 1);
                        if (p.Contains(CmdParams["splitArgs"]))
                        {
                            object[] objs = p.Split(new string[] { CmdParams["splitArgs"] }, StringSplitOptions.RemoveEmptyEntries).ToArray<object>();
                            var Args = mth.GetParameters();

                            if(objs.Length < Args.Length)
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
                           

                            return mth.Invoke(fnc, objs).ToString();

                        }
                        else
                        {
                            if (mth == null) throw new System.NullReferenceException();
                            //Program.ConWrLine("no splitArgs");
                            if (p.Length == 0)
                            {
                                throw new Exception("ERROR: Wrong arguments");
                            }

                            var Args = mth.GetParameters();
                            List<object> oneArgParams = new List<object>();
                            oneArgParams.Add(p);
                            for (int i = 1; i < Args.Length; i++)
                            {
                                oneArgParams.Add(Args[i].DefaultValue);
                            }

                            return mth.Invoke(fnc, oneArgParams.ToArray()).ToString();

                        }
                    }
                    catch (System.NullReferenceException)
                    {
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            var Args = mth.GetParameters();
                            List<object> defaultParams = new List<object>();
                            for(int i = 0; i < Args.Length; i++)
                            {
                                defaultParams.Add(Args[i].DefaultValue);
                            }
                            return mth.Invoke(fnc, defaultParams.ToArray()).ToString();
                        }
                        catch {
                            Program.ConWrLine(ex.Message);
                            Process("UseTextTest " + command.Split(' ')[0], Methods, Labels);
                            return "-1";
                        }

                    }
                }
                catch { }
            }
            string lt = command;
            if (command.Contains(' ')) lt = command.Split(' ')[0];
            if (lt == "if") Process("UseTextTest ifvar", Methods, Labels);
            else if (lt == "for") Process("UseTextTest Times", Methods, Labels);
            else
            {
                Program.ConWrLine("No such method/command: " + lt);
                Program.ConWrLine("Need help? Type: help");
            }
            
            return "0";
        }
        public static string StrSp(string text, char split = ' ', int startIdx = 0)
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
        public static string Str(string str)
        {
            string[] tmp = str.Split(' ');
            for (int c = 0; c < tmp.Length; c++)
            {
                if (tmp[c].Contains("^ReadLine")) tmp[c] = tmp[c].Replace("^ReadLine", ReadConsoleLine());
                if (tmp[c].Contains("^ReadKey")) tmp[c] = tmp[c].Replace("^ReadKey", ReadConsoleKey());

            }

            string result = "";
            for (int a = 0; a < tmp.Length; a++)
            {
                if (a == 0) result += tmp[a];
                else result += " " + tmp[a];
            }
            foreach (var a in CmdParams)
            {
                result = result.Replace("$" + a.Key, a.Value);
            }
            result = result.Replace("&#dollar;", "$");
            return result;
        }
        public static string ReadConsoleLine()
        {
            if (CmdParams["inputText"] != "null") Console.Write(CmdParams["inputText"]);
            return Console.ReadLine();
        }
        public static string ReadConsoleKey()
        {
            if (CmdParams["inputText"] != "null") Console.Write(CmdParams["inputText"]);
            string k = Console.ReadKey().KeyChar.ToString();
            Program.ConWrLine("");
            return k;
        }
    }
}
