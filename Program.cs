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
        public static bool StopProgram = false;
        public static ConsoleColor ScriptColor = ConsoleColor.White;
        static API api = null;
        public static string Out = "";
        public static int a = 0;
        public static void RunScript(string file)
        {
            new Thread(CheckUpdates).Start();
            StreamReader reader = new StreamReader(file);
            string[] fromfile = reader.ReadToEnd().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            reader.Close();

            reader.Dispose();

            Dictionary<string, int> Labels = new Dictionary<string, int>();
            List<EMethod> Methods = new List<EMethod>();

            FileInfo f = new FileInfo(file);
            Variables.Add("workingScriptName", f.Name, false);
            Variables.Add("workingScriptFullName", f.FullName, false);
            Cmd.Process("title " + f.Name, null, null);

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
                    if (Variables.GetValue("showCommands") == "1") Program.ConWrLine(Variables.GetValue("invitation") + line);
                    
                    string result = Cmd.Process(Cmd.Str(line), Methods, Labels).ToString();
                    SetResult(result);
                    if (Variables.GetValue("showResult") == "1") 
                    {
                        PrintResult(Variables.GetValue("result"));
                    }

                    if (StopProgram) break;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public static FileInfo GetAboutMe()
        {
            string me = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return new FileInfo(me);

        }
        public static void ConWrLine(object text)
        {
            Console.WriteLine(text);
            if (api != null) api.trigger(text.ToString());
        }

        public static void ConWrite(object text)
        {
            Console.Write(text);
            if (api != null) api.trigger(text.ToString());
        }

        public static void SetResult(string result)
        {
            Variables.SetVariable("result", result);
        }
        public static void CheckUpdates()
        {
            try
            {
                string currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                string latestVersion = new System.Net.WebClient().DownloadString("https://raw.githubusercontent.com/feel-the-dz3n/escript/master/UpdateFiles/latest-version.txt");
                if(currentVersion != latestVersion)
                {
                    ConsoleColor c = Console.ForegroundColor;
                    int x = Console.CursorLeft;
                    int y = Console.CursorTop;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.CursorLeft = 0;
                    Console.CursorTop = 0;
                    Program.ConWrLine("   ESCRIPT " + latestVersion + " is available. You can update using UpdateProgram command.");
                    Console.ForegroundColor = c;
                    Console.CursorLeft = x;
                    Console.CursorTop = y;
                }
            }
            catch (Exception ex)
            {

            }
        }
        public static void Debug(string text, ConsoleColor col = ConsoleColor.DarkGray)
        {
            if(Variables.GetValue("programDebug") == "1")
            {
                Console.ForegroundColor = col;
                ConWrLine("[DEBUG] " + text);
            }
        }

        public static void PrintResult(string result)
        {
            ConsoleColor aaaaa = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            ConWrite("Result: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            ConWrite(result);
            if(result == "-1")
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                ConWrite(" (error)");
            }
            if (result == "0")
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                ConWrite(" (not completed/error)");
            }
            if (result == "1")
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                ConWrite(" (ok)");
            }
            ConWrLine("");
            Console.ForegroundColor = aaaaa;
        }

        public static void Init(string[] args, bool overwrite = true, API ap = null)
        {
            api = ap;
            if (overwrite)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Title = "ESCRIPT " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                //Program.ConWrLine(Console.Title);

            }
            
            Program.ConWrLine(" ");
            if (overwrite) Console.ForegroundColor = ConsoleColor.Green;
            Program.ConWrLine(" = ESCRIPT by Dz3n | version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            if (overwrite) Console.ForegroundColor = ConsoleColor.Gray;
            Program.ConWrLine(" = https://github.com/feel-the-dz3n/escript");
            Program.ConWrLine(" = https://vk.com/dz3n.escript");
            Program.ConWrLine(" = https://discord.gg/jXcjuqv");
            Program.ConWrLine(" ");
            
            
            if (overwrite)
            {
                Console.ForegroundColor = ScriptColor;
                Console.BackgroundColor = ConsoleColor.Black;

            }
#if DEBUG
            Debugger.Launch();
#endif
            Variables.Initialize();

#if !DEBUG
            if(args.Contains<string>("-debug"))
#endif
            {
                Variables.Set("programDebug", "1");
            }
            Debug("Environment formed", ConsoleColor.DarkGreen);
            

            if (overwrite)
            {
                try
                {
                    if (args.Length <= 0)
                    {
                        CommandLine();
                    }
                    else if (File.Exists(args[0]))
                    // if (File.Exists("D:\\vsprojects\\escript-master\\bin\\tcp_test.es"))
                    {
                        RunScript(args[0]);
                       // RunScript("D:\\vsprojects\\escript-master\\bin\\tcp_test.es"); 
                    }
                    else
                    {
                        if (args.Contains<string>("-close"))
                        {
                            foreach (var p in Process.GetProcesses())
                            {
                                try
                                {
                                    if (p.ProcessName.ToLower() == "escript" && p.Id != Process.GetCurrentProcess().Id) p.Kill();
                                    if (p.ProcessName.ToLower() == "escript-update" && p.Id != Process.GetCurrentProcess().Id) p.Kill();
                                }
                                catch { }
                            }
                        }
                        if(args.Contains<string>("-install"))
                        {
                            string InstallScript = "install.es";
                            using (StreamWriter w = new StreamWriter(InstallScript))
                            {
                                w.WriteLine("func InstallEscriptYes");
                                w.WriteLine("{");
                                w.WriteLine("InstallESCRIPT");
                                w.WriteLine("}");


                                w.WriteLine("func Canceled");
                                w.WriteLine("{");
                                w.WriteLine("Clear");
                                w.WriteLine("echo ");
                                w.WriteLine("SetColor 12");
                                w.WriteLine("echo  = ESCRIPT $version Installation");
                                w.WriteLine("echo ");
                                w.WriteLine("echo ");
                                w.WriteLine("echo ");
                                w.WriteLine("echo ");
                                w.WriteLine("echo ");
                                w.WriteLine("echo ");
                                w.WriteLine("echo ");
                                w.WriteLine("echo ");
                                w.WriteLine("SetColor 15");
                                w.WriteLine("echo                       The installation was canceled by user");
                                w.WriteLine("set inputText||null");
                                w.WriteLine("echo ^ReadKey");
                                w.WriteLine("}");

                                w.WriteLine("Version");
                                w.WriteLine("set version||$result");

                                w.WriteLine("Clear");
                                w.WriteLine("echo ");
                                w.WriteLine("SetColor 10");
                                w.WriteLine("echo  = ESCRIPT $version Installation");
                                w.WriteLine("SetColor DarkGray");
                                w.WriteLine("echo    this installation is also script, but generated (install.es)");
                                
                                w.WriteLine("echo ");
                                w.WriteLine("echo ");
                                w.WriteLine("echo ");
                                w.WriteLine("echo ");
                                w.WriteLine("echo ");
                                w.WriteLine("SetColor 15");
                                w.WriteLine("echo                       Welcome to ESCRIPT installation!");
                                w.WriteLine("echo ");
                                w.WriteLine("echo                       Do you want to install ESCRIPT?");
                                w.WriteLine("SetColor 7");
                                w.WriteLine("echo                       (admin rights will be requested)");
                                w.WriteLine("echo ");
                                w.WriteLine("SetColor 14");
                     w.WriteLine("set inputText||                                                  Y/N: ");
                                w.WriteLine("set install||^ReadLine");
                                w.WriteLine("ToLower $install");
                                w.WriteLine("set install||$result");
                                w.WriteLine("if $install||==||y||InstallEscriptYes||Canceled");
                                
                            }
                            RunScript(InstallScript);
                            Environment.Exit(0);
                        }
                        if (args.Contains<string>("-assoc"))
                        {
                            string me = System.Reflection.Assembly.GetExecutingAssembly().Location;
                            Program.ConWrLine("Associating *.es files...");
                            FileAssociation.Associate("ESCRIPT file", me + ",1");
                            Program.ConWrLine("Creating a script on the desktop...");
                            try
                            {
                                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Command Interpreter.es";
                                //ConWrLine(desktop);
                                using (StreamWriter w = new StreamWriter(desktop))
                                {
                                    w.WriteLine(":ESCRIPT 4.0");
                                    w.WriteLine("echo P.S.: This is script, you can add something here");
                                    w.WriteLine(":set invitation||EPOOP> ");
                                    w.WriteLine(":title I Love Dz3n");
                                    w.WriteLine("CommandInterpreter");
                                }
                            }
                            catch (Exception ex) { Program.ConWrLine("ERROR: " + ex.Message); }
                            Console.ForegroundColor = ConsoleColor.Green;
                            Program.ConWrLine("ESCRIPT was installed!");
                            Thread.Sleep(2000);
                            Program.ConWrLine("");
                            CommandLine();
                        }
                        if (args.Contains<string>("-installNext"))
                        {
                            foreach (var p in Process.GetProcesses())
                            {
                                if (p.ProcessName.ToLower() == "escript" && p.Id != Process.GetCurrentProcess().Id) p.Kill();
                            }

                            string me = System.Reflection.Assembly.GetExecutingAssembly().Location;
                            string destination = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\ESCRIPT";
                            FileInfo aboutme = GetAboutMe();

                            if (aboutme.DirectoryName == destination && aboutme.Name.ToLower().StartsWith("escript.exe"))
                            {
                                Program.ConWrLine("You can't do this. Use escript-install.exe or UpdateProgram method.");
                            }
                            else
                            {

                                Program.ConWrLine("Installing ESCRIPT...");
                                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\ESCRIPT")) Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\ESCRIPT");
                                File.Copy(me, destination + "\\escript.exe", true);
                                new Process() { StartInfo = { FileName = destination + "\\escript.exe", Arguments = "-close -assoc" } }.Start();
                                Cmd.Process("HideConsole", null, null);
                                Environment.Exit(0);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Program.ConWrLine(ex.ToString());
                    //if(args.Contains<string>("-install") && !args.Contains<string>("-close"))
                    //{
                    //    Program.ConWrLine("If you have some troubles with installation");
                    //    Program.ConWrLine("You should try to run ESCRIPT with -createInstallation argument");
                    //}
                }

                while (true)
                {
                    Cmd.Process("ShowConsole", null, null);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write("\nSTOP. ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("R - restart; D - set; C - command interpreter; another key - exit ");
                    var key = Console.ReadKey().Key;

                    Program.ConWrLine("\n");
                    if (key == ConsoleKey.R)
                    {
                        Cmd.Process("HideConsole", null, null);
                        Application.Restart();
                        Environment.Exit(0);
                    }
                    else if (key == ConsoleKey.D) Cmd.Process("set", null, null);
                    else if (key == ConsoleKey.C)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Program.ConWrLine("Use: \"Break\" to back to stop menu");
                        CommandLine();
                    }
                    else Environment.Exit(0);
                }
            }
        }
        public static void CommandLine()
        {
            new Thread(CheckUpdates).Start();
            Variables.Set("showResult", "1");
            StopProgram = false;
            Console.ForegroundColor = ConsoleColor.White;
            Program.ConWrLine("Need help? Type: help");
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(Variables.GetValue("invitation"));
                Console.ForegroundColor = ConsoleColor.White;
                string line = Console.ReadLine();
                Console.ForegroundColor = ScriptColor;
                SetResult(Cmd.Process(Cmd.Str(line), null, null));
                if (Variables.GetValue("showResult") == "1")
                {
                    PrintResult(Variables.GetValue("result"));
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (StopProgram) break;
            }
        }
        static void Main(string[] args)
        {
            Init(args);
        }
    }
}
