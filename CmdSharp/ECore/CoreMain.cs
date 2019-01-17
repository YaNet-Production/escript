using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
namespace CmdSharp
{
    public class ConverterImportResources
    {
        public string OriginalName;
        public string FullName;
    }

    public class CoreMain
    {
        public static StreamWriter log = null;
        public static ConsoleColor ScriptColor = ConsoleColor.White;
        public static string Out = "";

        public static void WriteResourceToFile(string resourceName, string fileName)
        {
            Debug.DebugText("Write resource to file: " + resourceName + " : " + fileName);
            using (var resource = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                }
            }
        }

        public static string ReadResource(string resourceName)
        {
            using (var resource = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(resource))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        
        public static string CheckForImports(IntPtr resourceUpdateHandle, string scriptContent, string mname, string mainPath)
        {
            string result = scriptContent;
            var h = resourceUpdateHandle;
            string[] scriptLines = null;// ESCode.SplitCode(scriptContent);
            foreach (var line in scriptLines)
            {
                if (line.StartsWith("Import "))
                {
                    FileInfo importScriptFile = new FileInfo(line.Remove(0, "Import ".Length));
                    if (!importScriptFile.Exists) importScriptFile = new FileInfo(Path.Combine(mainPath, line.Remove(0, "Import ".Length)));

                    if (!importScriptFile.Exists)
                    {
                        EConsole.WriteLine($"[ERROR] File {importScriptFile.FullName} not found ({line})", true, ConsoleColor.Red);
                        continue;
                    }
                    if(importScriptFile.Extension.ToLower() != ".es" && importScriptFile.Extension.ToLower() != ".escript" && importScriptFile.Extension.ToLower() != ".esh")
                    {
                        EConsole.WriteLine($"Import: Unsupported extension: {importScriptFile.Extension}", true, ConsoleColor.Yellow);
                        continue;
                    }

                    string importName = "import" + Cmd.Process("RandomString(16);");

                    result = result.Replace(line, "Import " + importName);

                    EConsole.WriteLine("Processing Import " + importScriptFile.Name + "...");

                    string importScriptContent;
                    using (StreamReader r = new StreamReader(importScriptFile.FullName))
                    {
                        importScriptContent = r.ReadToEnd();
                    }

                    
                    importScriptContent = GlobalVars.RemoveDirtFromCode(importScriptContent);
                    importScriptContent = CheckForImports(h, importScriptContent, mname, mainPath);
                    //importScriptContent = "BASE64:" + GlobalVars.Base64Encode(importScriptContent);


                    
                    byte[] bytes = Encoding.ASCII.GetBytes(importScriptContent);
                    
                    

                    // Get language identifier
                    System.Globalization.CultureInfo currentCulture = System.Globalization.CultureInfo.CurrentCulture;
                    int pid = ((ushort)currentCulture.LCID) & 0x3ff;
                    int sid = ((ushort)currentCulture.LCID) >> 10;
                    ushort languageID = (ushort)((((ushort)pid) << 10) | ((ushort)sid));
                    


                    // Get pointer to data
                    GCHandle scriptHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);



                    if (GlobalVars.UpdateResource(h, (IntPtr)10, importName, languageID, scriptHandle.AddrOfPinnedObject(), (uint)bytes.Length))
                    {
                        EConsole.WriteLine("[" + importName + "(" + importScriptFile.Name + ")] 0x" + scriptHandle.AddrOfPinnedObject().ToString("X4") + " (" + bytes.Length + ") -> " + mname);
                    }
                    else throw new Exception($"Can't insert resource ({importScriptFile.Name}): " + Marshal.GetLastWin32Error());

                }
            }
            return result;
        }

        //
        // Source: https://stackoverflow.com/questions/4127785/using-updateresource-in-c
        //
        public static void CompileScript(string fileName, string outName, string iconPath = "", bool anyKey = false)
        {
            string mname = new FileInfo(outName).Name;
            if (!GlobalVars.IgnoreRunAsAdmin)
            {
                Process source = Process.GetCurrentProcess();
                Process target = new Process();
                target.StartInfo = source.StartInfo;
                target.StartInfo.FileName = source.MainModule.FileName;
                target.StartInfo.WorkingDirectory = Path.GetDirectoryName(source.MainModule.FileName);
                target.StartInfo.UseShellExecute = true;

                if(Environment.OSVersion.Version.Major > 5) target.StartInfo.Verb = "runas";
                string iconInfo = "";
                if (iconPath != "") iconInfo = " \"" + iconPath + "\"";

                target.StartInfo.Arguments = String.Format("\"{0}\" /convert \"{1}\"{2} -ignoreRunasRestart", fileName, outName, iconInfo);
                if (anyKey) target.StartInfo.Arguments += " -anykey";
                target.Start();
                return;
            }


            Cmd.Process("ShowConsole();");
            Cmd.Process("title(\"Compile " + new FileInfo(fileName).Name + "\");");


            EConsole.ForegroundColor = ConsoleColor.White;
            EConsole.WriteLine("   Script: " + fileName);
            EConsole.ForegroundColor = ConsoleColor.Yellow;
            EConsole.WriteLine(" = STARTING COMPILATION");
            EConsole.WriteLine("");
            EConsole.ForegroundColor = ConsoleColor.Gray;
            try
            {

                EConsole.WriteLine("Reading " + new FileInfo(fileName).Name + "...");
                string scriptContent;
                using (StreamReader r = new StreamReader(fileName))
                {
                    scriptContent = r.ReadToEnd();
                }

                scriptContent = GlobalVars.RemoveDirtFromCode(scriptContent);




                File.Copy(System.Reflection.Assembly.GetExecutingAssembly().Location, outName, true);
                EConsole.WriteLine("Created copy of current executable");
                var h = GlobalVars.BeginUpdateResource(outName, false);
                EConsole.WriteLine("BeginUpdateResource (" + mname + ")");
                if (h == null) throw new Exception("Handle = null");

                // Get language identifier
                System.Globalization.CultureInfo currentCulture = System.Globalization.CultureInfo.CurrentCulture;
                int pid = ((ushort)currentCulture.LCID) & 0x3ff;
                int sid = ((ushort)currentCulture.LCID) >> 10;
                ushort languageID = (ushort)((((ushort)pid) << 10) | ((ushort)sid));

                EConsole.WriteLine("Language: " + languageID);

                byte[] iconData = { };



                // Get pointer to data

                //for (int i = 1; i <= 12; i++)
                //{
                //    if (GlobalVars.UpdateResource(h, (IntPtr)3, "#"+i.ToString(), 0, IntPtr.Zero, (uint)0))
                //    {
                //        EConsole.WriteLine("   Icon #" + i + " removed from final executable");
                //    }
                //    else EConsole.WriteLine(" ! Icon #" + i + " NOT removed: " + Marshal.GetLastWin32Error());
                //}

                //for (int i = 1; i <= 6; i++)
                //{
                //    if (GlobalVars.UpdateResource(h, (IntPtr)3, i.ToString(), languageID, iconHandle.AddrOfPinnedObject(), (uint)iconData.Length))
                //    {
                //        EConsole.WriteLine("   Icon #" + i + " updated");
                //    }
                //    else EConsole.WriteLine(" ! Icon #" + i + " NOT updated: " + Marshal.GetLastWin32Error());
                //}

                //if (GlobalVars.UpdateResource(h, (IntPtr)3, "1", languageID, iconHandle.AddrOfPinnedObject(), (uint)iconData.Length))
                //{
                //    EConsole.WriteLine("   Icon data inserted");
                //}
                //else throw new Exception("Can't insert resource: icon data: " + Marshal.GetLastWin32Error());

                //EConsole.WriteLine(" = EndUpdateResource");
                //if (!GlobalVars.EndUpdateResource(h, false)) throw new Exception("Can't finish resource updating");
                //h = GlobalVars.BeginUpdateResource(outName, false);
                //EConsole.WriteLine(" = BeginUpdateResource");


                scriptContent = CheckForImports(h, scriptContent, mname, new FileInfo(fileName).Directory.FullName);
                //scriptContent = "BASE64:" + GlobalVars.Base64Encode(scriptContent);


                byte[] bytes = Encoding.ASCII.GetBytes(scriptContent);

                GCHandle scriptHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

                if (GlobalVars.UpdateResource(h, (IntPtr)10, "script", languageID, scriptHandle.AddrOfPinnedObject(), (uint)bytes.Length))
                {
                    EConsole.WriteLine("[SCRIPT DATA] 0x" + scriptHandle.AddrOfPinnedObject().ToString("X4") + " (" + bytes.Length + ")" + mname);
                }
                else throw new Exception("Can't insert script resource: " + Marshal.GetLastWin32Error());

                EConsole.WriteLine("EndUpdateResource (" + mname + ")");
                if (!GlobalVars.EndUpdateResource(h, false)) throw new Exception("Can't finish resource updating: " + Marshal.GetLastWin32Error());

                if (Cmd.Process("winver();").ToString().ToLower().Contains("windows"))
                {
                    string iName = "";
                    try { iName = new FileInfo(iconPath).Name; } catch { }
                    EConsole.WriteLine("Extracting resources to insert icon " + iName);
                    WriteResourceToFile("escript.InsertIcons.exe", "InsertIconsX.exe");


                    if (iconPath == "") WriteResourceToFile("escript.FancyStuff.02.ico", "00.ico");
                    else File.Copy(iconPath, "00.ico");

                    EConsole.WriteLine("Running InsertIcons (https://github.com/einaregilsson/InsertIcons)");

                    Process ii = new Process();
                    ii.StartInfo.FileName = "InsertIconsX.exe";
                    ii.StartInfo.Arguments = "\"" + outName + "\" 00.ico";
                    ii.StartInfo.UseShellExecute = true;
                    if (Environment.OSVersion.Version.Major > 5) {ii.StartInfo.Verb = "runas"; }
                    ii.Start();
                    ii.WaitForExit();
                }
                else
                {
                    EConsole.WriteLine(" * Skipping Instert Icons (only for Windows + .NET Framework 4)");
                }

                EConsole.WriteLine("Removing temporary files...");
                if (File.Exists("InsertIconsX.exe")) File.Delete("InsertIconsX.exe");
                if (File.Exists("00.ico")) File.Delete("00.ico");
            }
            catch (Exception ex)
            {

                EConsole.WriteLine("Removing temporary files...");
                if (File.Exists("InsertIconsX.exe")) File.Delete("InsertIconsX.exe");
                if (File.Exists("00.ico")) File.Delete("00.ico");

                EConsole.WriteLine("");
                EConsole.ForegroundColor = ConsoleColor.Red;
                EConsole.WriteLine(" = ERROR: " + ex.ToString());
                EConsole.ForegroundColor = ConsoleColor.Gray;

                EConsole.WriteLine("Press any key to close this window...");
                try
                {
                    EConsole.ReadKey();
                }
                catch { }

#if DEBUG
                throw ex;
#else
                Environment.Exit(-1);
#endif
            }
            EConsole.WriteLine("");
            EConsole.ForegroundColor = ConsoleColor.Green;
            EConsole.WriteLine(" = COMPLETED");
            EConsole.ForegroundColor = ConsoleColor.White;
            EConsole.WriteLine("   Result: " + outName);
            EConsole.ForegroundColor = ConsoleColor.Gray;
            if (anyKey)
            {
                EConsole.WriteLine("Press any key to close this window...");
                EConsole.ReadKey();
            }
            Environment.Exit(0);
        }

        public static void SetResult(object result)
        {
            //EList list = Variables.GetValueObject("result") as EList;
            //list.Content.Add(result);
            Variables.SetVariableObject("result", result);
        }
        public static void CheckUpdates()
        {
            try
            {
                return; // TODO

                Version cVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string currentVersion = cVer.ToString();
                string verUrl = GlobalVars.StuffServer + "UpdateFiles/%vName%-version.txt";
                string verX = "latest";
                switch(cVer.Revision)
                {
                    case 0: { verX = "latest"; break; }
                    case 1: { verX = "beta"; break; }
                    default: { verX = "latest"; break; }
                }
                
                verUrl = verUrl.Replace("%vName%", verX);

                string latestVersion = new System.Net.WebClient().DownloadString(verUrl);
                Version uVer = new Version(latestVersion);
                if (cVer != uVer && (bool)Variables.GetValueObject("checkUpdates"))
                {
                    if (cVer >= uVer)
                    {
                        Debug.DebugText("CheckUpdates: current version is newer than on the server", ConsoleColor.DarkRed, false);
                        return;
                    }


                    string add = "";
                    if (cVer.Revision != 0) add = " " + verX;

                    if (GlobalVars.IsCompiledScript || Variables.GetValue("workingScriptFullFileName").Length > 1)
                    {
                        string uText = "cmd# " + latestVersion + " is available. You can install it using \"update" + add + "\" command.";
                        EConsole.WriteLine(uText);
                    }
                    else
                    {
                        Cmd.Process("update(\"" + add + "\");");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.DebugText("CheckUpdates: " + ex.ToString(), ConsoleColor.DarkRed);
            }
        }

        public static void PrintResult(object result, string prefix = "")
        {
            if (prefix != "") EConsole.Write(prefix, ConsoleColor.DarkGray);
            EConsole.Write("Result: ", ConsoleColor.DarkGray);

            if (result != null)
            {
                EConsole.Write($"({result.GetType().Name}) ", ConsoleColor.DarkGray);
                if (result.GetType() == typeof(Exception))
                {
                    EConsole.Write("invoke ToString() to get details", ConsoleColor.DarkGray);
                }
                else
                {
                    EConsole.Write(result.ToString(), ConsoleColor.Gray);
                }
            }
            else
                EConsole.Write("null", ConsoleColor.DarkGray);
            //result = result.ToString();
            ///*if (result == "-1")
            //{
            //    EConsole.ForegroundColor = ConsoleColor.DarkRed;
            //    EConsole.Write(" (error/-1)");
            //}
            //else if (result == "0" || result.ToLower() == "false")
            //{
            //    EConsole.ForegroundColor = ConsoleColor.DarkYellow;
            //    EConsole.Write(" (false/error/0)");
            //}
            //else if (result == "1" || result.ToLower() == "true")
            //{
            //    EConsole.ForegroundColor = ConsoleColor.DarkGreen;
            //    EConsole.Write(" (true/ok/1)");
            //}
            //else */if (result == "CMD_NOT_FOUND")
            //{
            //    EConsole.Write(" (command not found)", ConsoleColor.DarkGray);
            //}
            //else if (result.Length == 0)
            //{
            //    EConsole.Write(" (nothing or null)", ConsoleColor.DarkGray);
            //}
            //else if (result == "ESCRIPT_ERROR_EXCEPTION")
            //{
            //    EConsole.Write(" (invalid arguments or ESCRIPT issue)", ConsoleColor.DarkRed);
            //}
            //else if (result.StartsWith("SYNTAX_ERROR"))
            //{
            //    EConsole.Write(" (syntax error)", ConsoleColor.DarkRed);
            //}
            //else if (result.StartsWith("ESCRIPT_ERROR"))
            //{
            //    EConsole.Write(" (ESCRIPT error)", ConsoleColor.DarkRed);
            //}
            //else if (result.StartsWith("CMD_ERROR_NULL"))
            //{
            //    EConsole.Write(" (unknown error)", ConsoleColor.DarkRed);
            //}
            //else if (result.StartsWith("CMD_ERROR"))
            //{
            //    EConsole.Write(" (error)", ConsoleColor.DarkRed);
            //}
            //else if (result.StartsWith("CMD_DONE"))
            //{
            //    EConsole.Write(" (done)", ConsoleColor.DarkGreen);
            //}
            //else if (result.StartsWith("CMD_FAIL"))
            //{
            //    EConsole.Write(" (fail)", ConsoleColor.DarkYellow);
            //}
            EConsole.WriteLine("");
        }

        //public static void PrintResult(string result, string prefix = "null")
        //{
        //    Cmd.Process("PrintResult {#" + result + "}||" + prefix, null, true);
        //}

        public static void Init(string[] args, APICore api = null)
        {
            GlobalVars.API = api;

            Variables.Initialize(true, args);
            EnvironmentManager.Initialize();


            if (args.Contains<string>("-noUiConsole") || (bool)Cmd.Process("IsKeyDown(\"RShiftKey\");"))
            {
                Variables.Set("useCustomConsole", false, new List<string>() { "CmdSharp", "Hidden", "ReadOnly" });
            }
            else
            {
                Variables.Set("useCustomConsole", true, new List<string>() { "CmdSharp", "Hidden", "ReadOnly" });
            }

            try
            {
                string f = GetLogFile();
                if (f == null) throw new Exception();
                log = new StreamWriter(f);
                log.AutoFlush = true;

                Variables.Set("logFile", new FileInfo(f).FullName, new List<string>() { });

#if DEBUG
                Debugger.Launch();
#endif
            }
            catch
            {
                log = null;
            }

#if DEBUG
            Cmd.Process("Debug();");
#endif


            if (args.Contains<string>("-console") || (bool)Cmd.Process("IsKeyDown(\"LControlKey\");"))
            {
                Variables.SetVariableObject("forceConsole", true);
                Cmd.Process("ShowConsole();");
            }

            
            if (args.Contains<string>("-debug") || (bool)Cmd.Process("IsKeyDown(\"LShiftKey\");"))
            {
                Cmd.Process("Debug();");
            }

#if ESCODE_IMPLEMENTED
            //new ESCode(ESCode.SplitCode(ReadResource("escript.Functions.Functions.es")));
            //Debug.DebugText("escript.Functions.Functions.es imported");
#else
            Debug.DebugText("Functions.es is *not* imported: not implemented", ConsoleColor.DarkRed);
#endif

            if (args.Contains<string>("-ignoreRunasRestart")) GlobalVars.IgnoreRunAsAdmin = true;

            if (args.Contains<string>("/help") || args.Contains<string>("/?") || args.Contains<string>("--h") || args.Contains<string>("-help"))
            {
                Cmd.Process("ShowConsole();");
                EConsole.ForegroundColor = ConsoleColor.Gray;
                EConsole.WriteLine("CmdSharp <Script Path> [/help] [/convert] [-ignoreRunasRestart] [-console] [-debug] [-cmd] [-close] [-install]");
                EConsole.WriteLine("");
                EConsole.WriteLine("/help\t\t\tShow this article");
                EConsole.WriteLine("-ignoreRunAsRestart\tIgnore 'Restart 1'");
                EConsole.WriteLine("-console\t\tUse ConsoleMessageBox instead of Windows Forms");
                EConsole.WriteLine("-debug\t\t\tStart with debug mode");
                EConsole.WriteLine("-cmd\t\t\tStart command interpretator");
                EConsole.WriteLine("-install\t\tStart installation");
                EConsole.WriteLine(" ");
                EConsole.WriteLine("<Script Path> /convert <Out Path> <Icon Path>\tConvert .es to .exe");
                EConsole.WriteLine("Press any key...");
                EConsole.ReadKey();
                Environment.Exit(0);
            }

            if (args.Contains<string>("/convert"))
            {
                try
                {

                    FileInfo s = new FileInfo(args[0]);

                    if (!File.Exists(s.FullName)) throw new Exception("Script " + args[0] + " not found");
                    string outPath = "";

                    if (s.Name.Contains(".es")) outPath = s.Name.Replace(".es", ".exe");
                    else outPath = s.Name + ".exe";
                    try
                    {
                        if (args[2].Length >= 1)
                        {
                            outPath = args[2];
                        }
                    }
                    catch { }

                    string iconArg = "";
                    try
                    {
                        FileInfo ii = new FileInfo(args[3]);
                        if (File.Exists(ii.FullName)) iconArg = ii.FullName;
                    }
                    catch { }
                    FileInfo outFile = new FileInfo(outPath);

                    bool anykey = false;
                    if (args.Contains("-anykey")) anykey = true;

                    CompileScript(s.FullName, outFile.FullName, iconArg, anykey);
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    Cmd.Process("ShowConsole();");
                    EConsole.ForegroundColor = ConsoleColor.Gray;
                    EConsole.WriteLine("ERROR: " + ex.Message);
                    EConsole.WriteLine("");
                    EConsole.WriteLine("Use: CmdSharp <Script Path> /convert (<Compiled File Path>) (<Icon Path>)");
                    EConsole.WriteLine("Examples:");
                    EConsole.WriteLine("CmdSharp \"MyScript.es\" /convert\twill be converted in MyScript.exe");
                    EConsole.WriteLine("CmdSharp \"MyScript.es\" /convert \"C:\\Programs\\MyScript.exe\"");
                    EConsole.WriteLine("CmdSharp \"MyScript.es\" /convert \"C:\\Programs\\MyScript.exe\" \"myicon.ico\"");
                    EConsole.WriteLine("");
                    EConsole.WriteLine("If you want to insert icon, you must set <Compiled File Path>");

                    EConsole.ForegroundColor = ConsoleColor.Gray;
                    EConsole.WriteLine("Press any key to close this window...");
                    EConsole.ReadKey();
                    Environment.Exit(404);
                }
            }

            //if (!args.Contains<string>("-ignoreLib"))
            //{
            //    foreach (var f in GlobalVars.GetAboutMe().Directory.EnumerateFiles())
            //    {
            //        if (f.Name.Contains("ESLib"))
            //        {
            //            Cmd.Process("Import(\"" + f.FullName + "\");");
            //        }
            //    }
            //}
            
#if !IsCore
            IntPtr resourceInfo = GlobalVars.FindResource(IntPtr.Zero, "script", (IntPtr)10);

            if (resourceInfo != IntPtr.Zero)
            {
                Debug.DebugText("SCRIPT Resource: 0x" + resourceInfo.ToString("X4"));

                uint size = GlobalVars.SizeofResource(IntPtr.Zero, resourceInfo);
                IntPtr pt = GlobalVars.LoadResource(IntPtr.Zero, resourceInfo);
                GlobalVars.IsCompiledScript = true;
                Variables.Set("isCompiledScript", "1");
                byte[] bPtr = new byte[size];
                Marshal.Copy(pt, bPtr, 0, (int)size);
                string code = Encoding.ASCII.GetString(bPtr);
                Debug.DebugText("SCRIPT:\r\n" + code);

#if ESCODE_IMPLEMENTED
                ESCode script = new ESCode(ESCode.SplitCode(code));
                script.RunScript();

                Break();
#else
                throw new NotImplementedException("ESCode is not implemented");
#endif

            }
#endif
            if (args.Contains<string>("-install"))
            {
                if (GlobalVars.IsCompiledScript) throw new Exception("Can't install compiled version. Please, use clean ESCRIPT or remove script information from resources manually.");
                string InstallScript = "InstallScript.es";

                WriteResourceToFile("escript.Stuff.InstallScript.es", InstallScript);

#if ESCODE_IMPLEMENTED
                ESCode script = new ESCode(InstallScript);
                script.RunScript();
#else
                throw new NotImplementedException("ESCode is not implemented");
#endif

                Environment.Exit(0);
            }
            if (args.Contains<string>("-assoc"))
            {
                if (GlobalVars.IsCompiledScript) throw new Exception("Can't install compiled version. Please, use clean cmd# or remove script information from resources manually.");
                string me = System.Reflection.Assembly.GetExecutingAssembly().Location;
#if !IsCore
                //Cmd.Process("ShowConsole");

                EConsole.WriteLine("Associating cmd# files...");
                try
                {
                    FileAssociation.AssociateESCRIPT("cmd# Script", me, ".cmdsharp");
                    FileAssociation.AssociateESCRIPT("cmd# Script", me, ".cmdsharp");
                    //FileAssociation.AssociateESCRIPT("ESCRIPT Header File", me, ".cmdsharp", "escriptheader", false);
#if !IsCore
                    FileAssociation.SHChangeNotify(FileAssociation.SHCNE_ASSOCCHANGED, FileAssociation.SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
#endif
                }
                catch (Exception ex)
                {
                    EConsole.WriteLine("ERROR: " + ex.Message);
                    Environment.Exit(1);
                }
                EConsole.WriteLine("Creating desktop scripts...");
                try
                {
                    string desktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CmdSharp Console.cmdsharp");
                    WriteResourceToFile("CmdSharp.Core.Scripts.Scripts.DesktopShortcut.cmdsharp", desktop);

                    desktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CmdSharp Folder.cmdsharp");
                    WriteResourceToFile("CmdSharp.Core.Scripts.Scripts.ExampleFolder.cmdsharp", desktop);
                }
                catch (Exception ex)
                {
                    EConsole.WriteLine("ERROR: " + ex.Message);
                    Environment.Exit(21);
                }
#else
                        EConsole.ForegroundColor = ConsoleColor.Red;
                        EConsole.WriteLine("WARNING: ESCRIPT Core cannot be installed");
#endif
                EConsole.ForegroundColor = ConsoleColor.Green;
                EConsole.WriteLine("CmdSharp installed!");
                //Thread.Sleep(2000);
                EConsole.WriteLine("");
                Environment.Exit(0);
            }

            try
            {
                if (GlobalVars.UsingAPI) return;

                if ((args.Length <= 0 && !GlobalVars.UsingAPI) || args.Contains<string>("-cmd"))
                {
                    CommandLine();
                }
                else if (File.Exists(args[0]))
                {
#if ESCODE_IMPLEMENTED
                    ESCode script = new ESCode(args[0]);
                    script.RunScript();
#else
                    throw new NotImplementedException("ESCode is not implemented");
#endif
                }
                else if (!File.Exists(args[0]))
                {
                    CommandLine();
                }
            }
            catch (Exception ex)
            {
                BachokMessage(ex);
            }

            Break();

        }

        public static void BachokMessage(Exception ex, bool isUnhandled = false)
        {
            //
            // BACHOK POTIK ZONE
            //
            if (!GlobalVars.UsingAPI)
                Cmd.Process("ShowConsole();");

            if (!isUnhandled) EConsole.WriteLine("[HARD ERROR]", ConsoleColor.Red);
            else EConsole.WriteLine("[UNHANDLED EXCEPTION]", ConsoleColor.Red);

            EConsole.WriteLine(ex.ToString(), ConsoleColor.White);
#if DEBUG
            throw ex;
#endif
        }

        public static void Break()
        {
            if(GlobalVars.UsingAPI)
            {
                Cmd.Process("ThreadAbortAll();");
                EConsole.WriteLine("API_BREAK");
                return;
            }
            try
            {
                while (true)
                {
                    Cmd.Process("ShowConsole();");

                    if (Variables.GetBool("abortAfterBreak"))
                    {
                        Cmd.Process("ThreadAbortAll();");
                        Cmd.Process("HideStatus();");
                    }

                    if (Variables.GetBool("exitAfterBreak"))
                        Cmd.Process("exit(0);");

                    ChoiceWindow c = new ChoiceWindow();

                    c.SetText("Choose something or press cancel to exit");
                    c.SetCaption("CmdSharp - break mode");

                    ChoiceWndButton[] buttons =
                    {
                        new ChoiceWndButton("Restart CmdSharp"),
                        new ChoiceWndButton("Open log in a text editor"),
                        new ChoiceWndButton("Display variables list"),
                        new ChoiceWndButton("Enter debug mode and attach debugger"),
                        new ChoiceWndButton("Go to single-command interpreter mode"),
                        new ChoiceWndButton("Exit")
                    };
                    c.SetButtons(buttons);

                   

                    c.ShowDialog();
                    var result = c.GetAnswer();

                    if (result.ChoiceWndButton == null)
                        Cmd.Process("exit(0);");
                    else if (result.ChoiceWndButton == buttons[0]) // restart
                        Cmd.Process("Restart();");
                    else if (result.ChoiceWndButton == buttons[1]) // log
                        Cmd.Process($"start(\"notepad\", \"{(string)Variables.Get("logFile")}\");");
                    else if (result.ChoiceWndButton == buttons[2]) // set
                        Cmd.Process("VarList();");
                    else if (result.ChoiceWndButton == buttons[3]) // debug
                        Cmd.Process("Debug(true);");
                    else if (result.ChoiceWndButton == buttons[4]) // commandline
                    {
                        EConsole.WriteLine("Use: \"Break();\" to return back.", ConsoleColor.Gray);
                        CommandLine();
                    }
                    else if (result.ChoiceWndButton == buttons[5]) // exit
                        Cmd.Process("exit(0);");
                }
            }
            catch (Exception ex)
            {
                BachokMessage(ex);
            }
            Break();
        }
        public static void CommandLine()
        {
            ScriptColor = ConsoleColor.White;
            Cmd.Process("ShowConsole();");
            new Thread(CheckUpdates).Start();
            Variables.Set("showResult", "1");
            GlobalVars.StopProgram = false;
            EConsole.ForegroundColor = ConsoleColor.White;
            EConsole.WriteLine("Need help? Type: help();");

            if(!GlobalVars.IsCompiledScript && !System.Reflection.Assembly.GetExecutingAssembly().Location.Contains("\\CmdSharp\\CmdSharp.exe"))
                EConsole.WriteLine("To install this copy of CmdSharp type: install();");

            while (true)
            {
                EConsole.ForegroundColor = ConsoleColor.Green;
                EConsole.Write(Variables.GetValue("invitation"));
                EConsole.ForegroundColor = ConsoleColor.White;
                string line = EConsole.ReadLine();
                EConsole.ForegroundColor = ScriptColor;
                SetResult(Cmd.Process(GlobalVars.RemoveDirtFromString(line)));
                if (Variables.GetBool("showResult"))
                {
                    PrintResult(Variables.GetValueObject("result"));
                }
                if (Variables.GetValue("forceGC") == "1")
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                if (GlobalVars.StopProgram) break;
            }
        }

        public static string GetLogFile(string stamp = "")
        {
            string me = System.Reflection.Assembly.GetExecutingAssembly().Location;

            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CmdSharp")))
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ESCRIPT"));

            FileInfo f = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CmdSharp", String.Format("CmdSharp{0}.log", stamp)));

            if(File.Exists(f.FullName) && IsFileLocked(f))
            {
                return GetLogFile(DateTime.Now.ToString(" dd.MM.yyyy HH.mm.ss"));
            }

            return f.FullName;
        }

        // https://stackoverflow.com/questions/876473/is-there-a-way-to-check-if-a-file-is-in-use
        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}
