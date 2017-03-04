using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace escript
{
    class Program
    {
        static API api = null;
        public static string Out = "";
        public static int a = 0;
        public static void RunScript(string file)
        {

            StreamReader reader = new StreamReader(file);
            string[] fromfile = reader.ReadToEnd().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            reader.Close();

            reader.Dispose();

            Dictionary<string, int> Labels = new Dictionary<string, int>();
            List<EMethod> Methods = new List<EMethod>();

            for (int m = 0; m < fromfile.Length; m++)//methods
            {
                if (fromfile[m].StartsWith("func "))
                {

                    int methodStartIdx = 0;
                    if (fromfile[m + 1].StartsWith("{")) methodStartIdx = m + 2;
                    EMethod thisMethod = new EMethod();
                    thisMethod.Name = fromfile[m].Remove(0, "func ".Length);
                    int end = 0;
                    for (int c = methodStartIdx; c < fromfile.Length; c++)
                    {
                        if (!fromfile[c].StartsWith("}")) thisMethod.Code.Add(fromfile[c]);
                        else { end = c; break; }
                    }
                    m = end;
                    Methods.Add(thisMethod);
                }
            }

            for (int t = 0; t < fromfile.Length; t++)//labels
            {
                if (fromfile[t][0] == ':')
                {
                    Labels.Add(fromfile[t].Remove(0, 1), t);
                    //Program.ConWrLine("Added label: " + fromfile[t].Remove(0, 1) + ", line: " + t + 1);
                }

            }

            for (a = 0; a < fromfile.Length; a++)//code processing
            {
                string line = fromfile[a];
                if (line.StartsWith("func "))
                {
                    foreach (var m in Methods)
                    {
                        if (m.Name == line.Remove(0, "func ".Length))
                        {
                            a += (m.Code.Count + 2);
                        }
                    }
                }
                else
                {
                    if (Cmd.CmdParams["showCommands"] == "1") Program.ConWrLine(Cmd.CmdParams["invintation"] + line);
                    if (line.StartsWith("break")) break;

                    string result = Cmd.Process(Cmd.Str(line), Methods, Labels).ToString();
                    SetResult(result);
                    if (Cmd.CmdParams["showResult"] == "1") Program.ConWrLine(Cmd.CmdParams["result"]);
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        public static void ConWrLine(object text)
        {
            Console.WriteLine(text);
            if (api != null) api.trigger(text.ToString());
        }
        static void SetResult(string result)
        {
            Cmd.CmdParams["result"] = result;
        }
        public static void Init(string[] args, bool overwrite = true, API ap = null)
        {
            api = ap;
            if (overwrite)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Title = "ESCRIPT " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                //Program.ConWrLine(Console.Title);

            }
            
            Program.ConWrLine("    EEEEEE  SSSS   CCCCC RRRR  1 6PPP  TTTTTTT    ");
            Program.ConWrLine("    8      S      6      R   R 1 6   P    I       ");
            Program.ConWrLine("    EEEEEE  SSSS  6      RRRR  1 6PPP     I       ");
            Program.ConWrLine("    8           S 6      R R   1 6        I       ");
            Program.ConWrLine("    EEEEEE  SSSS   CCCCC R  R  1 6        I       ");
            if (overwrite)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Program.ConWrLine("    " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                Program.ConWrLine("\n");
                Console.ForegroundColor = ConsoleColor.White;
                Program.ConWrLine("To get documentation use: GetDocumentation FirstHelp||english");
                Program.ConWrLine("Where 'english' - it's your language");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.BackgroundColor = ConsoleColor.Black;

            }
#if DEBUG
            Debugger.Launch();
#endif
            Cmd.CmdParams.Clear();
            Cmd.CmdParams.Add("result", "-");
            Cmd.CmdParams.Add("readLine_Settings", "< ");
            Cmd.CmdParams.Add("showResult", "0");
            Cmd.CmdParams.Add("showCommands", "0");
            Cmd.CmdParams.Add("invintation", "> ");


            if (overwrite)
            {
                try
                {
                    if (args.Length <= 0)
                    {
                        while (true)
                        {
                            Console.Write(Cmd.CmdParams["invintation"]);
                            ConsoleColor old = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.White;
                            string line = Console.ReadLine();
                            if (line.StartsWith("break")) break;
                            SetResult(Cmd.Process(Cmd.Str(line), null, null));
                            if (Cmd.CmdParams["showResult"] == "1") Program.ConWrLine(Cmd.CmdParams["result"]);
                            Console.ForegroundColor = old;
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }
                    }
                    else if (File.Exists(args[0]))
                    {
                        RunScript(args[0]);
                    }
                    else
                    {
                        if (args.Contains<string>("-close"))
                        {
                            foreach (var p in Process.GetProcesses())
                            {
                                if (p.ProcessName.ToLower() == "escript" && p.Id != Process.GetCurrentProcess().Id) p.Kill();
                            }
                        }
                        if (args.Contains<string>("-assoc"))
                        {
                            Program.ConWrLine("Associating *.es files...");
                            FileAssociation.Associate("ESCRIPT file", "");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Program.ConWrLine("ESCRIPT was installed!");
                            Thread.Sleep(2000);
                            Cmd.CmdParams.Clear();
                            Main(new string[] { });
                        }
                        if (args.Contains<string>("-install"))
                        {
                            foreach (var p in Process.GetProcesses())
                            {
                                if (p.ProcessName.ToLower() == "escript" && p.Id != Process.GetCurrentProcess().Id) p.Kill();
                            }

                            string me = System.Reflection.Assembly.GetExecutingAssembly().Location;
                            string destination = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\ESCRIPT";
                            FileInfo aboutme = new FileInfo(me);

                            if (aboutme.DirectoryName == destination)
                            {
                                Program.ConWrLine("Download new version of ESCRIPT and start installer");
                            }
                            else
                            {

                                Program.ConWrLine("Installing ESCRIPT...");
                                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\ESCRIPT")) Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\ESCRIPT");
                                File.Copy(me, destination + "\\" + aboutme.Name, true);
                                new Process() { StartInfo = { FileName = destination + "\\" + aboutme.Name, Arguments = "-close -assoc" } }.Start();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Program.ConWrLine(ex.ToString());
                }

                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write("\nProgram stopped. Press 'R' to restart, 'D' for debug or any other key to exit ");
                    var key = Console.ReadKey().Key;

                    Program.ConWrLine("\n");
                    if (key == ConsoleKey.R) Main(args);
                    else if (key == ConsoleKey.D) Cmd.Process("set", null, null);
                    else Environment.Exit(0);
                }
            }
        }
        static void Main(string[] args)
        {
            Init(args);
        }
    }
}
