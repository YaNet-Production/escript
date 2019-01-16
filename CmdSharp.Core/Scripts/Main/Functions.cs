using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Reflection;
#if !IsCore
using System.Windows.Forms;
#endif
using System.Runtime.InteropServices;

namespace CmdSharp
{
    public class Functions
    {

#if !IsCore
        private StatusWindow statusWnd = null;
#else
        private object statusWnd = null;
#endif

        public Dictionary<string, int> ConsoleColors = new Dictionary<string, int>();
        public Dictionary<string, int> MsgBoxIcons = new Dictionary<string, int>();
        public Dictionary<string, int> MsgBoxButtons = new Dictionary<string, int>();

        public Dictionary<string, int> Labels;

        public void Import(string fileName, string className, object[] arguments = null)
        {
#if ESCODE_IMPLEMENTED
                if (fileName.Contains(".es") || fileName.Contains(".esh"))
                {
                    new ESCode(fileName);
                    return null;//.Cmd.Done;
                }
                else
#endif

            ImportedLibInfo i = new ImportedLibInfo(fileName, className, arguments);
            GlobalVars.LoadedLibs.Add(i);

        }

        //public object Compare(string one, string p, string two, string ifOkDoThis, string el = "null")
        //{
        //    return If(one, p, two, ifOkDoThis, el);
        //}

        public void Logo()
        {
            EConsole.WriteLine("         `-://:-.                          ");
            EConsole.WriteLine("    `.-////////////:-`                     ");
            EConsole.WriteLine("   -::///////:::///////.                   ");
            EConsole.WriteLine("   .....-::////////////.                   ");
            EConsole.WriteLine("   ..........://///////.                   ");
            EConsole.WriteLine("   ```.......://////:-.`                   ");
            EConsole.WriteLine("       ```...:/:--.`       ``              ");
            EConsole.WriteLine("   ``````````..```````.`   `.:--.`         ");
            EConsole.WriteLine("   ....`````````..-:://.      `--::.       ");
            EConsole.WriteLine("   .........`.::///////.    ``.-::.`       ");
            EConsole.WriteLine("   ..........:///////:-`  `..::.`          ");
            EConsole.WriteLine("   ..........:///:-.`      ```       `````.");
            EConsole.WriteLine("   .....-------.`                ```.://oo+");
            EConsole.WriteLine("   .....-..````````.-::.         `.-o+/-.  ");
            EConsole.WriteLine("   .......````.-:://///.                   ");
            EConsole.WriteLine("   ..........://///////.                   ");
            EConsole.WriteLine("     ```.....:////:-.``                    ");
            EConsole.WriteLine("          ```--..`                         ");
        }

        public string ToString(string Long, string Format)
        {
            return long.Parse(Long).ToString(Format);
        }

        //public object If(string one, string condition, string two, string ifOkDoThis, string elseDoThis = "null")
        //{
        //    try
        //    {
        //        int first = int.Parse(one.ToString()), second = int.Parse(two.ToString());
        //        if (condition.ToString() == "<") if (first < second) { Cmd.Process(ifOkDoThis.ToString()/* FIX ME, Labels*/); return null;//.Cmd.Done; }
        //        if (condition.ToString() == ">") if (first > second) { Cmd.Process(ifOkDoThis.ToString()/* FIX ME, Labels*/); return null;//.Cmd.Done; }
        //        if (condition.ToString() == "<=") if (first <= second) { Cmd.Process(ifOkDoThis.ToString()/* FIX ME, Labels*/); return null;//.Cmd.Done; }
        //        if (condition.ToString() == ">=") if (first >= second) { Cmd.Process(ifOkDoThis.ToString()/* FIX ME, Labels*/); return null;//.Cmd.Done; }
        //        if (condition.ToString() == "==") if (first == second) { Cmd.Process(ifOkDoThis.ToString()/* FIX ME, Labels*/); return null;//.Cmd.Done; }
        //        if (condition.ToString() == "!=") if (first != second) { Cmd.Process(ifOkDoThis.ToString()/* FIX ME, Labels*/); return null;//.Cmd.Done; }
        //    }
        //    catch
        //    {
        //        if (condition.ToString() == "==")
        //        {
        //            if (one.ToString() == two.ToString()) { Cmd.Process(ifOkDoThis.ToString()/* FIX ME, Labels*/); return null;//.Cmd.Done; }
        //        }
        //        else if (condition.ToString() == "!=")
        //        {
        //            if (one.ToString() != two.ToString()) { Cmd.Process(ifOkDoThis.ToString()/* FIX ME, Labels*/); return null;//.Cmd.Done; }
        //        }
        //        else return -1;
        //    }
        //    if (!elseDoThis.ToString().ToLower().StartsWith("null")) Cmd.Process(elseDoThis.ToString()/* FIX ME, Labels*/);
        //    return null;//.Cmd.Fail;
        //}

        public object cd(string directory = "null") { return ChangeDir(directory); }

        public string ChangeDir(string directory = "null")
        {
            if (directory.ToLower() == "null")
                return ReturnCurrentDirectory();

            Directory.SetCurrentDirectory(directory);

            return null;
        }

        public object mciOpenAndPlay(string fileName, string alias = "escript")
        {
            mciClose(alias);
            mciOpen(fileName, alias);
            return mciPlay(alias);
        }

        public object mciPlay(string alias = "escript")
        {
            return mciSendCommand("play " + alias);
        }

        public object mciOpen(string fileName, string alias = "escript")
        {
            return mciSendCommand("open \"" + fileName + "\" alias " + alias);
        }

        public object mciClose(string alias = "escript")
        {
            return mciSendCommand("close " + alias);
        }



        public object mciSendCommand(string command, string returnString = "null", string returnLength = "0", string HwndCallback = "0")
        {
#if !IsCore
            try
            {
                uint rLen = uint.Parse(returnLength);
                uint hw = uint.Parse(HwndCallback);
                string rStr = null;
                if (returnString.ToLower() != "null") rStr = returnString;
                uint result = GlobalVars.mciSendString(command, rStr, rLen, hw);
                if (result == 0) return null;//.Cmd.Done;
                else return result.ToString();
            }
            catch (Exception ex)
            {
                EConsole.WriteLine("ERROR: " + ex.Message);
                return null;//.Cmd.Fail;
            }
#else
            return null;//.Cmd.Fail;
#endif
        }

        public string count(string one, string action, string two) { return Count(one, action, two); }

        public string Count(string one, string action, string two)
        {
            long q = long.Parse(one);
            long w = long.Parse(two);
            long result = 0;
            if (action == "+") result = q + w;
            else if (action == "-") result = q - w;
            else if (action == "/" || action == ":") result = q / w;
            else if (action == "*") result = q * w;
            else if (action == "%") result = q % w;
            else return "null";
            return result.ToString();
        }

        public object ShowWindow(string handle, string command = "5")
        {
#if !IsCore
            if (GlobalVars.ShowWindow((IntPtr)int.Parse(handle), int.Parse(command)))
#endif
                return null;//.Cmd.Done;
            return null;//.Cmd.Fail;
        }

        public string web_get(object Url, object Data)
        {
            return WebGet(Url, Data);
        }
        public string web_post(string Url, string Data)
        {
            return WebPost(Url, Data);
        }
        /*
         * From https://blog.foolsoft.ru/c-funkcii-post-i-get-zaprosov-gotovye-k-primen/
         */
        public string WebGet(object Url, object Data)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(Url + "?" + Data);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.Stream stream = resp.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();
            return Out;
        }


        public string WebPost(string Url, string postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(Url);

            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString;
        }

        public object IsConsoleCreated()
        {
            if (EConsole.IsConsoleCreated) return null;//.Cmd.Done;
            else return null;//.Cmd.Fail;
        }

        public object CreateConsole()
        {
            EConsole.CreateConsole();
            return null;//.Cmd.Done;
        }

        public object ConsoleSetFont(string fontFamily = "Consolas", string clearScreen = "1")
        {
#if !IsCore
            if (EConsole.cWnd != null)
            {
                EConsole.cWnd.SetFont(fontFamily);
                if (StringToBool(clearScreen))
                {
                    Clear();
                    PrintIntro();
                }
                return null;//.Cmd.Done;
            }
            return null;//.Cmd.Fail;
#else
            return "-1";
#endif
        }

        //public object PrintResult(string result, string prefix = "")
        //{
        //    Program.PrintResult(result, prefix);
        //    return null;//.Cmd.Done;
        //}

        public object ConsoleSetFontSize(string size = "10,35", string clearScreen = "1")
        {
#if !IsCore
            if (EConsole.cWnd != null)
            {
                EConsole.cWnd.SetFontSize(float.Parse(size));
                if (StringToBool(clearScreen))
                {
                    Clear();
                    PrintIntro();
                }
                return null;//.Cmd.Done;
            }
            return null;//.Cmd.Fail;
#else
            return "-1";
#endif
        }

        public object CommandInterpreter()
        {
            CoreMain.CommandLine();
            return null;//.Cmd.Done;
        }

        public object TCP_SetTriggers(string triggerMsg, string triggerDisconnected, string triggerConnected)
        {
            Variables.Set("TCP_triggerMsg", triggerMsg);
            Variables.Set("TCP_triggerDisconnected", triggerDisconnected);
            Variables.Set("TCP_triggerConnected", triggerConnected);
            return null;//.Cmd.Done;
        }

        public object TCP_Disconnect()
        {
            TCPConnection.Disconnect();
            return null;//.Cmd.Done;
        }
        public object TCP_Connect(object ip, object port)
        {
            return TCPConnection.Connect(ip.ToString(), int.Parse(port.ToString())/* FIX ME, Labels*/);
        }

        public object TCP_Send(object text)
        {
            TCPConnection.Send(text.ToString());
            return null;//.Cmd.Done;
        }

        public object Write(string text = "null", string color = "null")
        {
            if (text.ToLower() == "null") text = "";
            string oldColor = GetForegroundColor().ToString();
            if (color != "null") SetColor(color);
            EConsole.Write(text);
            if (color != "null") SetColor(oldColor);
            return null;//.Cmd.Done;
        }

        public object echo(object text = null, string color = "null")
        {
            return WriteLine(text, color);
        }

        public object Beep()
        {
            Console.Beep();
            return null;//.Cmd.Done;
        }

        public object ConsoleSetBuffer(int width, int height)
        {
            Console.SetBufferSize(width, height);
            return null;//.Cmd.Done;
        }

        public object ConsoleSetWindowSize(int width, int height)
        {
            Console.SetWindowSize(width, height);
            return null;//.Cmd.Done;
        }

        public object ConsoleSetWindowPosition(int x, int y)
        {
            Console.SetWindowPosition(x, y);
            return null;//.Cmd.Done;
        }

        public object ConsoleSetCursorPosition(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            return null;//.Cmd.Done;
        }

        public object StringStartsWith(string text, string value)
        {
            if (text.StartsWith(value)) return null;//.Cmd.Done;
            else return null;//.Cmd.Fail;
        }

        public object StringLength(string text)
        {
            return text.Length;
        }

        public object StringContains(string text, string contains)
        {
            if (text.Contains(contains)) return true;
            return false;
        }

        public object StringTrim(string text, string symbol = "null")
        {
            if (symbol == "null") return text.Trim();
            else return text.Trim(symbol[0]);
        }


        public object StringTrimStart(string text, string symbol = "null")
        {
            if (symbol == "null") return text.TrimStart();
            else return text.TrimStart(symbol[0]);
        }

        public object StringTrimEnd(string text, string symbol = "null")
        {
            if (symbol == "null") return text.TrimEnd();
            else return text.TrimEnd(symbol[0]);
        }

        public object StringSetLength(string text, string length)
        {
            try
            {
                string result = "";
                int len = int.Parse(length) - 1;
                for (int i = 0; i < text.Length; i++)
                {
                    result += text[i];
                    if (i == len) break;
                }
                return result;
            }
            catch (Exception ex)
            {
                CmdSharp.Debug.DebugText(ex.ToString(), ConsoleColor.DarkRed);
                throw ex;
            }
        }

        public object IsInt(string text)
        {
            try
            {
                int.Parse(text);
                return true;
            }
            catch { }
            return false;
        }

        public object IsFileLocked(string path)
        {
            return CoreMain.IsFileLocked(new FileInfo(path));
        }

        public string StringSplit(string text, string splitSymbol, string variable = "null")
        {
            if (variable == "null") variable = RandomString(20);
            ListCreate(variable);
            foreach (var a in text.Split(new string[] { splitSymbol }, StringSplitOptions.RemoveEmptyEntries))
            {
                ListAdd(variable, a);
            }
            return variable;
        }


        public string StringRemoveToChar(string text, string toChar, string startIndex = "0")
        {
            int idx = int.Parse(startIndex);
            for (int i = idx; i < text.Length; i++)
            {
                if (text[i] == Convert.ToChar(toChar))
                {
                    return text.Remove(idx, i);
                }
            }
            return text;
        }

        public string StringReplace(string text, string oldValue, string newValue)
        {
            return text.Replace(oldValue, newValue);
        }

        public string StringRemove(string text, string startIndex, string endIndex)
        {
            return text.Remove(int.Parse(startIndex), int.Parse(endIndex));
        }

        public object GetTempPath()
        {
            return System.IO.Path.GetTempPath();
        }

        public object GetTempFile()
        {
            return System.IO.Path.GetTempFileName();
        }

        public object GetPath(string specialFolderNameOrCode)
        {
            try
            {
                return Environment.GetFolderPath((Environment.SpecialFolder)int.Parse(specialFolderNameOrCode));
            }
            catch
            {
                return Environment.GetFolderPath((Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), specialFolderNameOrCode, true));

            }
        }

        public object GetProgramPath()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location;
        }


        public object CheckUpdates()
        {
            CoreMain.CheckUpdates();
            return null;//.Cmd.Done;
        }

        public object SetStatusProgress(string value = "-2")
        {
            // -2 = Marquee
            // -1 = Inivisible
            // etc = Value
#if !IsCore
            if (statusWnd != null)
            {
                statusWnd.Invoke(new Action(delegate
                {
                    statusWnd.SetProgress(int.Parse(value));
                }));
                return null;//.Cmd.Done;
            }
            return null;//.Cmd.Fail;
#else
            return null;//.Cmd.Fail;
#endif
        }

        public object ProcessGetPath(string nameOrPid)
        {
            try
            {
                int id = int.Parse(nameOrPid);
                return Process.GetProcessById(id).MainModule.FileName;
            }
            catch
            {
                return Process.GetProcessesByName(nameOrPid).FirstOrDefault().MainModule.FileName;
            }
        }

        public object ProcessFindByName(string name, string variable)
        {
            EList list = new EList();
            foreach (var p in Process.GetProcessesByName(name))
            {
                list.Content.Add(p.Id.ToString());
            }
            Variables.SetVariableObject(variable, list);
            return null;//.Cmd.Done;
        }

        public object ProcessGetPid(string name)
        {
            return Process.GetProcessesByName(name).FirstOrDefault().Id;
        }

        public object ProcessGetName(string pid)
        {
            return Process.GetProcessById(int.Parse(pid)).ProcessName;
        }

        public object ProcessKill(string nameOrPid)
        {
            try
            {
                int id = int.Parse(nameOrPid);
                Process.GetProcessById(id).Kill();
            }
            catch
            {
                Process.GetProcessesByName(nameOrPid).FirstOrDefault().Kill();
            }
            return null;//.Cmd.Done;
        }

        public object SetStatus(string caption, string status = "null", string value = "-2", string createNew = "0")
        {
#if !IsCore
            if (status == "null")
            {
                status = caption;
                caption = "Status";
            }
            EConsole.WriteLine(status);
            if (Variables.GetValue("forceConsole") != "1")
            {
                new Thread(delegate ()
                {
                    if (statusWnd == null)
                    {
                        statusWnd = new StatusWindow();
                        statusWnd.Activate();
                    }

                    if (createNew == "1")
                    {
                        statusWnd.Dispose();
                        statusWnd = new StatusWindow();
                        statusWnd.Activate();
                    }

                    statusWnd.SetText(caption, status);
                    try
                    {
                        statusWnd.ShowDialog();
                    }
                    catch { }

                }).Start();
                Thread.Sleep(300);
                SetStatusProgress(value);
            }
#else
            EConsole.WriteLine(status);
#endif
            return null;//.Cmd.Done;
        }

        public object HideStatus() // Todo
        {
#if !IsCore
            if (statusWnd != null)
            {
                if (statusWnd.InvokeRequired) statusWnd.Invoke(new Action(() => { statusWnd.Dispose(); }));
                else statusWnd.Dispose();
                statusWnd = null;
                return null;//.Cmd.Done;
            }
#endif
            return null;//.Cmd.Fail;
        }

        public object DirExists(string dirName)
        {
            if (Directory.Exists(dirName)) return null;//.Cmd.Done;
            return null;//.Cmd.Fail;
        }

        public object FileExists(string fileName)
        {
            if (File.Exists(fileName)) return null;//.Cmd.Done;
            return null;//.Cmd.Fail;
        }

        public object DeleteDir(string dirName)
        {
            if (!Directory.Exists(dirName)) return "-2";
            Directory.Delete(dirName);
            return null;//.Cmd.Fail;
        }

        public object MakeDir(string dirName)
        {
            if (Directory.Exists(dirName)) return "-2";
            try
            {
                Directory.CreateDirectory(dirName);
                return null;//.Cmd.Done;
            }
            catch
            {
            }
            return null;//.Cmd.Fail;
        }

        //        public object InstallESCRIPT()
        //        {
        //#if !IsCore
        //            Process p = new Process();
        //            string me = System.Reflection.Assembly.GetExecutingAssembly().Location;

        //            p.StartInfo.FileName = me;
        //            p.StartInfo.Arguments = "-installNext";
        //            if (System.Environment.OSVersion.Version.Major >= 6)
        //            {
        //                if(Environment.OSVersion.Version.Major > 5) p.StartInfo.Verb = "runas";
        //            }
        //            p.Start();
        //            Environment.Exit(0);
        //            return null;//.Cmd.Done;
        //#else
        //            return null;//.Cmd.Fail;
        //#endif
        //        }

        public object BeepEx(object frequency, object duration)
        {
            Console.Beep(int.Parse(frequency.ToString()), int.Parse(duration.ToString()));
            return null;//.Cmd.Done;
        }

        public object WriteLine(object line = null, string color = "null")
        {
            if (line == null)
                line = "";

            string oldColor = GetForegroundColor().ToString();

            if (color != "null")
                SetColor(color);

            EConsole.WriteLine(line);

            if (color != "null")
                SetColor(oldColor);

            return null;
        }

        public object Exit(string code = "0")
        {
            Environment.Exit(int.Parse(code));
            return null;//.Cmd.Done;
        }

        public object ReturnUsername()
        {
            return Environment.UserName;
        }


        public object ReturnUserDomainName()
        {
            return Environment.UserDomainName;
        }

        public object While(string a, string condition, string b, string task)
        {
            // todo
            return "-1";
        }

        public object ListEnum(string listName, string command, string enumIdxVar = "enumIdx", string enumValueVar = "enumValue")
        {
            EList list = Variables.GetValueObject(listName) as EList;

            for (int i = 0; i < list.Content.Count; i++)
            {
                Variables.SetVariable(enumIdxVar, i.ToString());
                Variables.SetVariable(enumValueVar, list.Content[i].ToString());
                Cmd.Process(command/* FIX ME, Labels*/);
            }
            Variables.Remove(enumIdxVar);
            Variables.Remove(enumValueVar);
            return null;//.Cmd.Done;
        }

        public void DebugText(string text, string foregroundColor = "white")
        {
            ConsoleColor cc = ConsoleColor.White;
            try
            {
                cc = GetConsoleColor(int.Parse(foregroundColor));
            }
            catch
            {
                foreach (var v in ConsoleColors)
                {
                    if (foregroundColor.ToLower() == v.Key.ToLower())
                    {
                        cc = GetConsoleColor(v.Value);
                        break;
                    }
                }
            }
            CmdSharp.Debug.DebugText("[SCRIPT] " + text, cc);
        }

        //public object For(string start, string stop, string command, string infoVariable = "for")
        //{
        //    for (int a = int.Parse(start); a < int.Parse(stop.ToString()); a++)
        //    {
        //        if (GlobalVars.StopProgram) break;
        //        Variables.Set(infoVariable.ToString(), a.ToString());
        //        Cmd.Process(command.ToString()/* FIX ME, Labels*/);
        //    }
        //    return null;//.Cmd.Done;
        //}

        public object Exception(object text)
        {
            throw new Exception(text.ToString());
        }

        public object ReturnStackTrace()
        {
            return Environment.StackTrace;
        }

        public string ReturnCurrentDirectory()
        {
            return Environment.CurrentDirectory;
        }

        public object StringEmpty()
        {
            return String.Empty;
        }

        public object ReturnMachineName()
        {
            return Environment.MachineName;
        }

        public object CopyEx(string Source, string Destination, string Pattern = "*", string Overwrite = "0", string IncludeSubFolders = "1")
        {

            if (File.Exists(Source))
            {
                FileInfo s = new FileInfo(Source);
                // file
                CmdSharp.Debug.DebugText($"{s.FullName} -> {Destination}");
                //s.CopyTo(Destination, StringToBool(Overwrite));
                return null;//.Cmd.Done;
            }
            else
            {
                if (Directory.Exists(Source))
                {
                    DirectoryInfo d = new DirectoryInfo(Source);
                    // directory
                    var files = d.EnumerateFiles(Pattern);
                    foreach (var file in files)
                    {
                        CmdSharp.Debug.DebugText($"{file.FullName} -> {Destination}");
                        //file.CopyTo(Destination, StringToBool(Overwrite));
                    }

                    if (StringToBool(IncludeSubFolders))
                    {
                        var subfiles = EEnumerateSubFolders(d.FullName, Pattern);
                        foreach (var subfile in subfiles)
                        {
                            var dest = Path.Combine(Destination, subfile.FullName.Replace(d.FullName, ""));
                            CmdSharp.Debug.DebugText($"{subfile.FullName} -> {dest}");
                        }
                    }

                    return null;//.Cmd.Done;
                }
                else return null;//.Cmd.GetError("Source not found");
            }
        }

        private List<FileInfo> EEnumerateSubFolders(string folder, string pattern = "*")
        {
            List<FileInfo> list = new List<FileInfo>();
            var dirs = new DirectoryInfo(folder).EnumerateDirectories();
            foreach (var dir in dirs)
            {
                var files = dir.EnumerateFiles(pattern);
                foreach (var file in files)
                    list.Add(file);

                var dirs2 = dir.EnumerateDirectories(pattern);
                foreach (var d2 in dirs2)
                {
                    var t = EEnumerateSubFolders(d2.FullName, pattern);
                    foreach (var item in t)
                        list.Add(item);
                }
            }
            return list;
        }

        public object FileCopy(string fileName, string destination, string overwriteFile = "0")
        {
            bool ov = false;
            if (overwriteFile.StartsWith("1")) ov = true;
            try
            {
                File.Copy(fileName, destination, ov);
                return null;//.Cmd.Done;
            }
            catch (Exception ex) { throw ex; }
        }

        public void f(string contains = null) { GetMethods(contains); }

        public void GetMethods(string contains = null)
        {
            EMethodNew[] m = EnvironmentManager.AllMethods;


            if (contains != null)
            {
                EConsole.ForegroundColor = ConsoleColor.Magenta;
                EConsole.WriteLine("Searching for: " + contains);
            }

            foreach (var method in m)
            {
                if (contains == null)
                    UseTextTest(method.Name, false);
                else
                {
                    if (method.Name.ToLower().Contains(contains.ToLower()))
                        UseTextTest(method.Name, false);
                }
            }
        }

        public Dictionary<MethodInfo, object> WhereIsMethod(string name)
        {
            MethodInfo mth = null;
            object target = this;
            try
            {
                mth = GetType().GetMethod(name); // split command by space, used to split first argument. If there are no spaces then we will get just only command, without errors
            }
            catch { }


            for (int i = 0; i < GlobalVars.LoadedLibs.Count; i++)
            {
                ImportedLibInfo imp = GlobalVars.LoadedLibs[i];
                if (imp.classInstance != null)
                {
                    if (imp.funcType.GetMethod(name) != null)
                    {
                        mth = imp.funcType.GetMethod(name);
                        target = imp.classInstance;
                    }
                }
            }
            var d = new Dictionary<MethodInfo, object>();
            d.Add(mth, target);
            return d;
        }

        public object DictCreate(string variable)
        {
            EDictionary list = new EDictionary();
            Variables.SetVariableObject(variable, list);
            return null;//.Cmd.Done;
        }

        public object DictEdit(string variable, string key, object value)
        {
            EDictionary list = Variables.GetValueObject(variable) as EDictionary;
            list.Content[key] = value;
            return null;//.Cmd.Done;
        }

        public object DictAdd(string variable, object key, object value)
        {
            EDictionary list = Variables.GetValueObject(variable) as EDictionary;
            list.Content.Add(key, value);
            return null;//.Cmd.Done;
        }

        public object DictRemove(string variable, object key)
        {
            EDictionary list = Variables.GetValueObject(variable) as EDictionary;
            list.Content.Remove(key);
            return null;//.Cmd.Done;
        }

        public object DictReverse(string variable)
        {
            EDictionary list = Variables.GetValueObject(variable) as EDictionary;
            list.Content.Reverse();
            return null;//.Cmd.Done;
        }

        public object DictClear(string variable)
        {
            EDictionary list = Variables.GetValueObject(variable) as EDictionary;
            list.Content.Clear();
            return null;//.Cmd.Done;
        }


        public object DictContainsKey(string variable, object key)
        {
            var list = (Variables.GetValueObject(variable) as EDictionary);
            if (list.Content.ContainsKey(key)) return true;
            return false;
        }
        public object DictContainsValue(string variable, object value)
        {
            var list = (Variables.GetValueObject(variable) as EDictionary);
            if (list.Content.ContainsValue(value)) return true;
            return false;
        }

        public object DictInfo(string variable)
        {
            var list = (Variables.GetValueObject(variable) as EDictionary);
            string result = list.ToString();
            return result;
        }

        public object DictGetValue(string variable, object key)
        {
            return (Variables.GetValueObject(variable) as EDictionary).Content[key];
        }

        public object DictCount(string variable)
        {
            return (Variables.GetValueObject(variable) as EDictionary).Content.Count;
        }


        public object ListCreate(string variable)
        {
            EList list = new EList();
            Variables.SetVariableObject(variable, list);
            return null;//.Cmd.Done;
        }

        public object ListEdit(string variable, string index, object value)
        {
            EList list = Variables.GetValueObject(variable) as EList;
            list.Content[int.Parse(index)] = value;
            return null;//.Cmd.Done;
        }

        public object ListAdd(string variable, object value)
        {
            EList list = Variables.GetValueObject(variable) as EList;
            list.Content.Add(value);
            return ListGetIndex(variable, value);
        }

        public object ListRemove(string variable, object item)
        {
            EList list = Variables.GetValueObject(variable) as EList;
            list.Content.Remove(item);
            return null;//.Cmd.Done;
        }

        public object ListReverse(string variable)
        {
            EList list = Variables.GetValueObject(variable) as EList;
            list.Content.Reverse();
            return null;//.Cmd.Done;
        }

        public object ListClear(string variable)
        {
            EList list = Variables.GetValueObject(variable) as EList;
            list.Content.Clear();
            return null;//.Cmd.Done;
        }


        public object ListGetIndex(string variable, object value)
        {
            EList list = Variables.GetValueObject(variable) as EList;

            for (int i = 0; i < list.Content.Count; i++)
            {
                if (list.Content[i].ToString() == value.ToString()) return i;
            }

            return -1;
        }

        public object ListContains(string variable, object item)
        {
            if ((int)ListGetIndex(variable, item) == -1) return false;
            return true;
        }

        public object ListInfo(string variable)
        {
            var list = (Variables.GetValueObject(variable) as EList);
            int idx = list.DefaultIndex;
            list.DefaultIndex = -1;
            string result = list.ToString();
            list.DefaultIndex = idx;
            return result;
        }

        public object ListGetValue(string variable, string index)
        {
            return (Variables.GetValueObject(variable) as EList).Content[int.Parse(index)];
        }

        public int ListDefaultIndex(string variable, string value = "null")
        {
            if (value == "null") return (Variables.GetValueObject(variable) as EList).DefaultIndex;

            (Variables.GetValueObject(variable) as EList).DefaultIndex = int.Parse(value);

            return int.MaxValue;
        }


        public int ListCount(string variable)
        {
            return (Variables.GetValueObject(variable) as EList).Content.Count;
        }


        public string cmd(string command)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe");
            startInfo.Arguments = "/C " + command;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            Process p = Process.Start(startInfo);
            string output = p.StandardOutput.ReadToEnd();
            string error = p.StandardError.ReadToEnd();
            p.WaitForExit();
            if (output.Length != 0)
                return output;
            else if (error.Length != 0)
                return error;
            return output;
        }

        public string FileFullName(string file)
        {
            return new FileInfo(file).FullName;
        }

        public string FileName(string file)
        {
            return new FileInfo(file).Name;
        }

        public object AssemblyVersion(string file, string a = "null")
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(new FileInfo(file).FullName);

            switch (a.ToLower())
            {
                case "major": return assembly.GetName().Version.Major;
                case "minor": return assembly.GetName().Version.Minor;
                case "build": return assembly.GetName().Version.Build;
                case "revision": return assembly.GetName().Version.Revision;
                case "builddate": return new DateTime(2000, 1, 1).AddDays(assembly.GetName().Version.Build).AddSeconds(assembly.GetName().Version.Revision * 2).ToString();
                default: return assembly.GetName().Version.ToString();
            }
        }

        public object FileVersion(string file, string a = "null")
        {
            switch (a.ToLower())
            {
                case "major": return FileVersionInfo.GetVersionInfo(file).FileMajorPart;
                case "minor": return FileVersionInfo.GetVersionInfo(file).FileMinorPart;
                case "build": return FileVersionInfo.GetVersionInfo(file).FileBuildPart;
                case "private": return FileVersionInfo.GetVersionInfo(file).FilePrivatePart;
                default: return FileVersionInfo.GetVersionInfo(file).FileVersion;
            }

        }

        public object ProductVersion(string file, string a = "null")
        {
            switch (a.ToLower())
            {
                case "major": return FileVersionInfo.GetVersionInfo(file).ProductMajorPart;
                case "minor": return FileVersionInfo.GetVersionInfo(file).ProductMinorPart;
                case "build": return FileVersionInfo.GetVersionInfo(file).ProductBuildPart;
                case "private": return FileVersionInfo.GetVersionInfo(file).ProductPrivatePart;
                default: return FileVersionInfo.GetVersionInfo(file).ProductVersion;
            }

        }

        public object FileMove(string moveFrom, string moveTo)
        {
            try
            {
                File.Move(moveFrom, moveTo);
                return null;//.Cmd.Done;
            }
            catch (Exception ex) { throw ex; }
        }

        public object FileDelete(string fileName)
        {
            try
            {
                File.Delete(fileName);
                return null;//.Cmd.Done;
            }
            catch (Exception ex) { throw ex; }
        }

        public object ConsoleTextBox(string title, string text)
        {
            return CmdSharp.ConsoleTextBox.Show(title, text);
        }

        public object TextBox(string title, string text = "null", string multiline = "0")
        {
            if (Variables.GetValue("forceConsole") == "1") return ConsoleTextBox(title, text);
#if !IsCore
            TextBoxWindow w = new TextBoxWindow();
            //w.Text = title + " | ESCRIPT " + ver("major") + "." + ver("minor");
            if (text == "null") text = "";
            w.SetText(text);
            w.SetCaption(title);
            if (multiline == "0") w.NoLines();
            if (w.ShowDialog() == DialogResult.OK)
            {
                return w.GetInput().Replace("||", "^split^").Replace("{", "^(^").Replace("}", "^)^");
            }
            else return false;
#else
            return ConsoleTextBox(title, text);
#endif
        }

        //public object ListCreateSplit(string buttons, string split = ";")
        //{
        //    string name = RandomString(20);
        //    ListCreate(name);
        //    foreach(var a in buttons.Split(split[0]))
        //    {
        //        ListAdd(name, a);
        //    }
        //    return name;
        //}

        public string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public object ChoiceWindow(string title, string text, string buttons, string defButton = "null")
        {
            return ChoiceWindowEx(title, text, StringSplit(buttons, ";"), defButton, "1");
        }

        public object ChoiceWindowEx(string title, string text, string vListButtons, string defButton = "null", string removeVariable = "1")
        {

            if (Variables.GetValue("forceConsole") == "1") return -1;
#if !IsCore
            ChoiceWindow w = new ChoiceWindow();
            //w.Text = title + " | ESCRIPT " + ver("major") + "." + ver("minor");
            if (text == "null") text = "";
            w.SetText(text);
            w.SetCaption(title);

            EList a = (Variables.GetValueObject(vListButtons) as EList);
            w.SetButt(GlobalVars.ObjectListToStringArray(a.Content), defButton);

            string result = w.ShowDialog().ToString();

            if (StringToBool(removeVariable)) Variables.Remove(vListButtons);

            //Variables.Set(vResult, w.Get());
            return w.Get();
#else
            return -1;
#endif
        }

        public object title(string title = "null") { return SetTitle(title); }

        public object SetTitle(string title = "null")
        {
            string version = "ESCRIPT " + Cmd.Process("ver major") + "." + Cmd.Process("ver minor");
#if IsCore
            version += " Core";
#else
            version += " Standard";
#endif
            if (title != "null") version = title + " | " + version;
            EConsole.Title = version;
            return null;//.Cmd.Done;
        }

        public void TestHighlight(IntPtr Address, string Name = "Nikita Ivanov", bool Force = false, int Age = int.MaxValue)
        {
            EConsole.WriteLine("Invoking f(\"TestHighlight\");");
            f("TestHighlight");
        }

        public void UseTextTest(string method, bool printUse = true)
        {
            EMethodNew[] m = EnvironmentManager.AllMethods;
            EMethodNew mth = null;
            
            foreach (var me in m)
            {
                if (me.Name == method)
                {
                    mth = me;
                    break;
                }
            }

            if (mth == null)
                throw new Exception("Method not found");

            var Args = mth.Pamareters;

            if (printUse)
            {
                EConsole.Write("USE: ", ConsoleColor.Yellow);
            }

            EConsole.Write(method + "(", ConsoleColor.White);
            for (int i = 0; i < Args.Length; i++)
            {
                EConsole.Write($"{Args[i].ParameterType.Name} ", ConsoleColor.Blue);
                EConsole.Write($"{Args[i].Name}", ConsoleColor.Gray);
                if (Args[i].DefaultValue != null && Args[i].DefaultValue.GetType() != typeof(DBNull))
                {
                    EConsole.Write(" = ", ConsoleColor.DarkGray);

                    if (Args[i].DefaultValue.GetType() == typeof(string) || Args[i].DefaultValue.GetType() == typeof(char))
                        EConsole.Write(Parser.CommandNormalizer.ConvertObjectToStringCode(Args[i].DefaultValue), ConsoleColor.Yellow);
                    else if (Args[i].DefaultValue.GetType() == typeof(bool))
                        EConsole.Write(Parser.CommandNormalizer.ConvertObjectToStringCode(Args[i].DefaultValue), ConsoleColor.Blue);
                    else
                        EConsole.Write(Parser.CommandNormalizer.ConvertObjectToStringCode(Args[i].DefaultValue), ConsoleColor.Green);
                }
                else
                {
                }

                if (i != Args.Length - 1) EConsole.Write(", ", ConsoleColor.DarkGray);
            }
            EConsole.WriteLine(")", ConsoleColor.White);
            return;
        }
    

        public object GetMethod(string methodName)
        {
            //if(methodName.Length == 0)
            //{
            //    EConsole.WriteLine(UseTextTest("GetMethod"));
            //    return null;//.Cmd.Fail;
            //}
            System.Reflection.MethodInfo mth = null;

            List<MethodInfo> m = this.GetType().GetMethods().ToList();


            for (int i = 0; i < GlobalVars.LoadedLibs.Count; i++)
            {
                ImportedLibInfo imp = GlobalVars.LoadedLibs[i];
                if (imp.classInstance != null)
                {
                    m.AddRange(imp.funcType.GetMethods());
                }
            }

            foreach (var me in m)
            {
                if (me.Name == methodName) mth = me;
            }


            if (mth == null) return null;//.Cmd.Fail;
            var Args = mth.GetParameters();
            EConsole.WriteLine("Method: " + mth.Name);
            for (int i = 0; i < Args.Length; i++)
            {
                EConsole.WriteLine("[Argument " + i + "] " + Args[i].Name + " = " + Args[i].DefaultValue);
            }
            return null;//.Cmd.Done;
        }

        public object ls(string directory = "null") { return dir(directory); }
        public object dir(string directory = "null")
        {
            ConsoleColor co = EConsole.ForegroundColor;
            string c = directory;
            if (c.ToLower().StartsWith("null")) c = ReturnCurrentDirectory().ToString();
            EConsole.ForegroundColor = ConsoleColor.White;
            EConsole.WriteLine(" ");
            EConsole.WriteLine("   Directory: " + c);
            try
            {
                var dirs = Directory.EnumerateDirectories(c);
                var files = Directory.EnumerateFiles(c);
                EConsole.ForegroundColor = ConsoleColor.Gray;
                EConsole.WriteLine("   " + files.Count<string>() + " files, " + dirs.Count<string>() + " directories");
                EConsole.WriteLine(" ");
                DirectoryInfo i = null;
                foreach (var dir in dirs)
                {
                    i = new DirectoryInfo(dir);
                    EConsole.ForegroundColor = ConsoleColor.Yellow;
                    EConsole.WriteLine("<DIR>\t\t" + i.Name);
                }

                FileInfo fi = null;
                foreach (var f in files)
                {
                    fi = new FileInfo(f);
                    EConsole.ForegroundColor = ConsoleColor.Cyan;
                    if (fi.Extension.ToLower() == ".es") EConsole.ForegroundColor = ConsoleColor.Green;
                    if (fi.Extension.ToLower() == ".exe") EConsole.ForegroundColor = ConsoleColor.DarkCyan;
                    if (fi.Extension.ToLower() == ".bat" || fi.Extension.ToLower() == ".cmd") EConsole.ForegroundColor = ConsoleColor.Gray;

                    string size = (fi.Length / 1000) + " KB";
                    if ((fi.Length / 1000) <= 0)
                        size = fi.Length + " B";
                    EConsole.WriteLine("<" + fi.Extension + ">\t" + size + "\t" + fi.Name);
                }
            }
            catch (Exception ex)
            {
                EConsole.WriteLine("ERROR: " + ex.Message);
                return null;//.Cmd.Fail;
            }
            EConsole.ForegroundColor = co;
            return null;//.Cmd.Done;
        }

        public object FileRename(string renameFrom, string renameTo)
        {
            return FileMove(renameFrom, renameTo);
        }
        public object rename(string renameFrom, string renameTo) { return FileMove(renameFrom, renameTo); }
        public object ren(string renameFrom, string renameTo) { return FileMove(renameFrom, renameTo); }
        public object move(string moveFrom, string moveTo) { return FileMove(moveFrom, moveTo); }
        public object delete(string fileName) { return FileDelete(fileName); }

        public object copy(string fileName, string destination, string overwrite = "0") { return FileCopy(fileName, destination, overwrite); }
        public object exit() { return Exit(); }

        private ConsoleColor GetConsoleColor(int n)
        {
            switch (n)
            {
                default: return ConsoleColor.Black;
                case 0: return ConsoleColor.Black;
                case 1: return ConsoleColor.DarkBlue;
                case 2: return ConsoleColor.DarkGreen;
                case 3: return ConsoleColor.DarkCyan;
                case 4: return ConsoleColor.DarkRed;
                case 5: return ConsoleColor.DarkMagenta;
                case 6: return ConsoleColor.DarkYellow;
                case 7: return ConsoleColor.Gray;
                case 8: return ConsoleColor.DarkGray;
                case 9: return ConsoleColor.Blue;
                case 10: return ConsoleColor.Green;
                case 11: return ConsoleColor.Cyan;
                case 12: return ConsoleColor.Red;
                case 13: return ConsoleColor.Magenta;
                case 14: return ConsoleColor.Yellow;
                case 15: return ConsoleColor.White;
            }
        }

        public object doc(string topic = "FirstHelp", string version = "null")
        {
            return GetDocumentation(topic);
        }

        public object man(string topic = "FirstHelp", string version = "null")
        {
            return GetDocumentation(topic);
        }


        //        public object Splash()
        //        {
        //#if !IsCore
        //            CoreMain.verText = Variables.GetValue("workingScriptText");
        //            //Thread mThread = new Thread(CoreMain.FormThread);
        //            mThread.SetApartmentState(ApartmentState.STA);
        //            mThread.Start();
        //            Thread.Sleep(2000);
        //            return null;//.Cmd.Done;
        //#else
        //            return null;//.Cmd.Fail;
        //#endif
        //        }

        public object GetForegroundColor()
        {
            return EConsole.ForegroundColor.ToString();
        }

        public object SetColor(string foregroundColor = "15")
        {
            ConsoleColor cc = ConsoleColor.White;
            try
            {
                cc = GetConsoleColor(int.Parse(foregroundColor));
            }
            catch
            {
                foreach (var v in ConsoleColors)
                {
                    if (foregroundColor.ToLower() == v.Key.ToLower())
                    {
                        cc = GetConsoleColor(v.Value);
                        break;
                    }
                }
            }
            EConsole.ForegroundColor = cc;
            CoreMain.ScriptColor = cc;
            return null;//.Cmd.Done;
        }


        public object Break()
        {
            return Stop();
        }

        public object CountCode(string pattern)
        {
            string[] f = Directory.EnumerateFiles(ReturnCurrentDirectory().ToString(), pattern).ToArray<string>();
            int count = 0;
            EConsole.WriteLine(f.Length + " files");
            foreach (var file in f)
            {
                using (StreamReader w = new StreamReader(file))
                {
                    FileInfo fi = new FileInfo(file);
                    int len = w.ReadToEnd().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Length;
                    count += len;
                    EConsole.WriteLine(fi.Name + ": " + len);
                }
            }
            EConsole.WriteLine("Total: " + count);
            return null;//.Cmd.Done;
        }

        public object ConvertScript(string fileName, string outName = "null", string iconPath = "null", string anykey = "0")
        {
            if (iconPath != "null" && outName == "null") return null;//.Cmd.Fail;

            Process source = Process.GetCurrentProcess();
            Process target = new Process();
            target.StartInfo = source.StartInfo;
            target.StartInfo.FileName = source.MainModule.FileName;
            target.StartInfo.WorkingDirectory = Path.GetDirectoryName(source.MainModule.FileName);
            target.StartInfo.UseShellExecute = true;
            string o = "/convert";
            if (outName.ToLower() != "null") o = String.Format("/convert \"{0}\"", outName);
            if (iconPath != "null") o += " \"" + iconPath + "\"";
            target.StartInfo.Arguments = String.Format("\"{0}\" {1}", fileName, o);
            if (anykey == "1") target.StartInfo.Arguments += " -anykey";
            target.Start();
            target.WaitForExit();
            Variables.Set("convertResult", target.ExitCode.ToString());
            return "CMD_DONE_EXIT_CODE_" + target.ExitCode.ToString();
        }



        public object Stop()
        {
            GlobalVars.StopProgram = true;
            return null;//.Cmd.Done;
        }

        public object color(string foregroundColor = "15") { return SetColor(foregroundColor); }

        public object Help()
        {
            EConsole.WriteLine("To get help topics type: doc");
            EConsole.WriteLine("(internet connection is required)");
            EConsole.WriteLine("");
            EConsole.WriteLine("[Offline Help]");
            EConsole.WriteLine("Get all available methods: GetMethods (f)");
            EConsole.WriteLine("Search for some methods: f [text]");
            EConsole.WriteLine("Get/set current directory: cd [directory]");
            EConsole.WriteLine("Update program: UpdateProgram");
            EConsole.WriteLine("Create script using template: CreateScript [fileName]");
            return null;//.Cmd.Done;
        }

        public object update(string r = "latest")
        {
            return UpdateProgram(r);
        }

        public object cat(string filename) { return ReadFile(filename); }

        public object ReadFile(string filename)
        {
            StreamReader r = new StreamReader(filename);
            string a = r.ReadToEnd();
            r.Dispose();
            return a;
        }

        //public object DownloadUpdate()
        //{
        //    string path = Path.Combine(GetTempPath().ToString(), "escript-update.exe");
        //    WebClient w = new WebClient();
        //    w.DownloadFile(GlobalVars.StuffServer + "UpdateFiles/escript-latest.exe", path);
        //    return path;
        //}

        //public object StartUpdate(string path)
        //{
        //    Process.Start(path, "-install");
        //    Environment.Exit(0);
        //    return null;//.Cmd.Done;
        //}

        public object UpdateProgram(string r = "latest")
        {
            string tempPath = System.IO.Path.GetTempPath();
            string uScriptPath = Path.Combine(tempPath, "UpdateScript.es");

            CoreMain.WriteResourceToFile("escript.Stuff.UpdateScript.es", uScriptPath);

            Process eInstance = new Process();
            eInstance.StartInfo.FileName = GlobalVars.GetAboutMe().FullName;
            eInstance.StartInfo.Arguments = String.Format("\"{0}\" \"{1}\"", uScriptPath, r);
            eInstance.Start();
            return null;//.Cmd.Done;
        }
        public object WriteFile(string filename, string data, string notClearFile)
        {
            string a = "";
            if (notClearFile.StartsWith("1"))
            {
                using (StreamReader r = new StreamReader(filename))
                {
                    a = r.ReadToEnd();
                }
            }
            a += data;
            StreamWriter w = new StreamWriter(filename);
            w.Write(a);
            w.Dispose();
            return a;
        }

        public object DownloadText(string url)
        {
            WebClient w = new WebClient();
            return w.DownloadString(url);
        }

        public object DownloadFile(string url, string filename)
        {
            WebClient w = new WebClient();
            w.DownloadFile(url, filename);
            return null;//.Cmd.Done;
        }

        public object StartProgram(string fileName, string arguments = "null", string waitForExit = "0", string asAdmin = "0")
        {
            Process a = new Process();
            a.StartInfo.FileName = fileName;
            a.StartInfo.UseShellExecute = true;
            if (asAdmin == "1") { if (Environment.OSVersion.Version.Major > 5) a.StartInfo.Verb = "runas"; }

            if (arguments != "null") a.StartInfo.Arguments = arguments;

            a.Start();
            if (waitForExit.ToLower().StartsWith("1"))
            {
                a.WaitForExit();
                return a.ExitCode;
            }

            return null;//.Cmd.Done;
        }

        public object Return(object obj, bool breakMethod = true)
        {
            if (Variables.GetValue("workingMethod") != "")
            {
                Variables.SetVariableObject("workingMethodResult", obj);
                if (breakMethod) Variables.SetVariable("workingMethodBreak", "1");
            }
            return obj;
        }

        public object PathCombine(string path, string path2)
        {
            return Path.Combine(path, path2);
        }
        
        public object GetAsyncKeyState(string vKey)
        {
#if IsCore
            return "-1";
#else

            return GlobalVars.GetAsyncKeyState(int.Parse(vKey));
#endif
        }

        public object GetKeyState(string keyCodeOrName)
        {
#if !IsCore
            try
            {
                int keyCode = int.Parse(keyCodeOrName);
                return Keyboard.GetKeyStateFromCode(keyCode).ToString();
            }
            catch
            {
                foreach (System.Int32 i in Enum.GetValues(typeof(Keys)))
                {
                    if (Enum.GetName(typeof(Keys), i).ToLower() == keyCodeOrName.ToLower()) return Keyboard.GetKeyStateX((Keys)i);
                }
                return null;//.Cmd.GetError("KEY_NOT_FOUND");
            }
#else
            return -1;
#endif
        }

        public bool IsKeyDown(string key)
        {
#if !IsCore
            try
            {
                int keyCode = int.Parse(key);
                bool result = Keyboard.IsKeyDown(Keyboard.KeyCodeToKey(keyCode));
                if (result) return true;
                else return false;
            }
            catch
            {
                foreach (System.Int32 i in Enum.GetValues(typeof(Keys)))
                {
                    if (Enum.GetName(typeof(Keys), i).ToLower() == key.ToLower())
                    {
                        bool result = Keyboard.IsKeyDown((Keys)i);
                        if (result) return true;
                        else return false;
                    }
                }
                throw new Exception("Key not found");
            }
#else
            throw new NotImplementedException();
#endif
        }

        public object Base64Encode(string text)
        {
            return GlobalVars.Base64Encode(text);
        }

        public object Base64Decode(string text)
        {
            return GlobalVars.Base64Decode(text);
        }


        public object start(string fileName, string arguments = "null", string waitForExit = "0", string asAdmin = "0")
        {
            return StartProgram(fileName, arguments, waitForExit, asAdmin);
        }
        
        public object GetDocumentation(string topic = "FirstHelp", string version = "null")
        {
            string url = GlobalVars.StuffServer + "documentation/$ProgramVersion/" + topic + ".txt";
            if (version == "null") version = String.Format("{0}.{1}", Cmd.Process("ver major"), Cmd.Process("ver minor"));
            url = url.Replace("$ProgramVersion", version);
            CmdSharp.Debug.DebugText("Downloading: " + url);
            WebClient client = new WebClient();
            string text = client.DownloadString(url);
            if (text.StartsWith("REDIRECT:"))
            {
                return GetDocumentation(text.Replace("REDIRECT:", ""));
            }
            echo(text);
            return null;//.Cmd.Done;
        }

        //public object import(string fileName, string log = "0")
        //{
        //    FileInfo f = new FileInfo(fileName);
        //    System.Reflection.Assembly asm = System.Reflection.Assembly.LoadFile(f.FullName);
        //    string a = asm.GetName().Name;
        //    EConsole.WriteLine(a);
        //    System.Reflection.MethodInfo[] b = asm.GetType(asm.GetName().Name).GetMethods();
        //    foreach(var m in b)
        //    {
        //        EConsole.WriteLine(m.Name);
        //    }
        //    return null;//.Cmd.Fail;
        //}



        public object cls() { return Clear(); }

        public object Clear()
        {
            EConsole.Clear();
            return null;//.Cmd.Done;
        }

        public void set(string name = "null", object value = null) { SetVar(name, value); }

        public void SetVar(string name = "null", object value = null)
        {
            if (name.ToLower() == "null") { VarList(); return; }

            string workingMethod = Variables.GetValue("workingMethod");

            Variables.SetVariableObject(name, value, new List<string> { "Method=" + workingMethod });
        }

        public void unset(string name) { RemoveVar(name); return; }

        public void RemoveVar(string name)
        {
            Variables.Remove(name);
        }

        public object VarReinit(string addSystem = "1")
        {
            Variables.Initialize(StringToBool(addSystem));
            return null;//.Cmd.Done;
        }

        public object BoolToString(string b)
        {
            if (b.ToLower() == "true" || b == "1") return "1";
            else return "0";
        }

        public void EnvInit()
        {
            EnvironmentManager.Initialize();
        }

        public void EnvMethods()
        {
            foreach (var a in EnvironmentManager.AllMethods)
            {
                EConsole.WriteLine($"({a.GetType().Name}): {a.ToString()}");
            }
        }

        public string SyntaxCheck(string command)
        {
            return Cmd.SyntaxCheck(command);
        }

        public bool StringToBool(string text)
        {
            if (text == "1" || text.ToLower() == "true") return true;
            return false;
        }

        public object VarCount()
        {
            return Variables.VarList.Count;
        }

        public object VarOptions(string variable)
        {
            var l = Variables.GetVariable(variable).Options;
            StringBuilder b = new StringBuilder();
            foreach (var a in l)
            {
                b.AppendLine(a);
            }
            return b.ToString();
        }
        
        public object VarList(bool showSystem = false, bool returnIt = false, bool ignoreOptions = false)
        {
            StringBuilder r = new StringBuilder();
            if (!returnIt) EConsole.WriteLine(String.Format("Variables ({0}):", Variables.VarList.Count), ConsoleColor.Gray);
            else r.AppendLine(String.Format("Variables ({0}):", Variables.VarList.Count));
            for (int idx = 0; idx < Variables.VarList.Count; idx++)
            {
                if (!ignoreOptions)
                {
                    if (!showSystem && Variables.VarList[idx].Options.Contains("System")) continue;
                    if (Variables.VarList[idx].Options.Contains("Hidden"))
                    {
                        if (Variables.VarList[idx].Options.Contains("Debug") && Variables.GetBool("programDebug"))
                            goto ignoreThis;

                        continue;
                    }
                    ignoreThis:;
                }

                string add = "";

                if (Variables.VarList[idx].Value == null)
                    add = "(null) ";
                /*else if (Variables.VarList[idx].Value.GetType() == typeof(string))
                    add = "(string) ";// "(default) ";
                else if (Variables.VarList[idx].Value.GetType() == typeof(EList))
                    add = "(list) ";
                else if (Variables.VarList[idx].Value.GetType() == typeof(EDictionary))
                    add = "(dict) ";*/
                else
                    add = $"({Variables.VarList[idx].Value.GetType().Name}) ";

                if (!returnIt)
                {
                    EConsole.Write("[" + idx + "] ", ConsoleColor.DarkGray);
                    EConsole.Write(Variables.VarList[idx].Name, ConsoleColor.Yellow);
                    EConsole.Write(" = ", ConsoleColor.White);
                    EConsole.Write(add, ConsoleColor.DarkGray);
                    EConsole.Write(Variables.VarList[idx].ValueSafe, ConsoleColor.Green);
                    EConsole.WriteLine("");
                }
                else
                    r.AppendLine($"[{idx}] {Variables.VarList[idx].Name} = {add}{Variables.VarList[idx].ValueSafe}");
            }

            if (returnIt)
                return r.ToString();
            else
                return null;//.Cmd.Done;
        }

        public void Type(object t)
        {
            TestMethod(t, null, null, null, null, null, true);
        }

        public void TestMethod(object a1 = null, object a2 = null, object a3 = null, object a4 = null, object a5 = null, object a6 = null, bool onlyOne = false)
        {
            string arg1 = "null", arg2 = "null", arg3 = "null", arg4 = "null", arg5 = "null", arg6 = "null";

            if (a1 != null) arg1 = $"({a1.GetType().Name}) {a1.ToString()}";
            if (a2 != null) arg2 = $"({a2.GetType().Name}) {a2.ToString()}";
            if (a3 != null) arg3 = $"({a3.GetType().Name}) {a3.ToString()}";
            if (a4 != null) arg4 = $"({a4.GetType().Name}) {a4.ToString()}";
            if (a5 != null) arg5 = $"({a5.GetType().Name}) {a5.ToString()}";
            if (a6 != null) arg6 = $"({a6.GetType().Name}) {a6.ToString()}";

            EConsole.WriteLine($"[1] {arg1}", ConsoleColor.Red);

            if (onlyOne)
                return;

            EConsole.WriteLine($"[2] {arg2}", ConsoleColor.Green);
            EConsole.WriteLine($"[3] {arg3}", ConsoleColor.Blue);
            EConsole.WriteLine($"[4] {arg4}", ConsoleColor.DarkRed);
            EConsole.WriteLine($"[5] {arg5}", ConsoleColor.DarkGreen);
            EConsole.WriteLine($"[6] {arg6}", ConsoleColor.DarkBlue);
        }


        public object ReadLine()
        {
            return EConsole.ReadLine();
        }

        public object ReadKey(string intercept = "0")
        {
            return EConsole.ReadKey(StringToBool(intercept)).KeyChar.ToString();
        }

        // 
        // Real variables test
        //
        //public static string Gex(string variable, string message)
        //{
        //    Exception e = new Exception(message);
        //    Variables.SetVariableObject(variable, e);
        //    return null;//.Cmd.Done;
        //}

        //public static string Throw(string exceptionVariable)
        //{
        //    Exception e = (Exception)Variables.GetValueObject(exceptionVariable);
        //    throw e;
        //}

            

        public void DebugEx(string what = null, bool newState = true)
        {
            EConsole.WriteLine("   Debug variables");
            if (what == null)
            {
                for (int idx = 0; idx < Variables.VarList.Count; idx++)
                {
                    if (Variables.VarList[idx].Name.EndsWith("Debug") || Variables.VarList[idx].Options.Contains("Debug"))
                        EConsole.WriteLine("     " + Variables.VarList[idx].Name + ": " + Variables.VarList[idx].Value);
                }
            }
            else
            {
                Variables.SetVariableObject(what + "Debug", newState);
                EConsole.WriteLine("     " + what + "Debug: " + newState);
            }

            if (Debugger.IsAttached)
                EConsole.WriteLine("    The debugger is attached.");

            EConsole.WriteLine("   Environment Manager");
            EConsole.WriteLine($"     Classes ({ EnvironmentManager.Classes.Count})");

            for (int i = 0; i < EnvironmentManager.Classes.Count; i++)
            {
                EConsole.WriteLine($"       {EnvironmentManager.Classes[i].Name} - IsUser: {EnvironmentManager.Classes[i].IsUser} - Methods: {EnvironmentManager.Classes[i].Methods.Length}");
            }
            EConsole.WriteLine($"     All Methods ({ EnvironmentManager.AllMethods.Length})");
        }

        public void Debug(bool attachDebugger = false, bool showCmd = true, bool showResult = true, bool showConsole = true)
        {
            GlobalVars.DebugWhenFormingEnv = true;

            if (showConsole)
                ShowConsole();

            EConsole.WriteLine(" -> DEBUG MODE", ConsoleColor.Magenta);

            Variables.Set("programDebug", true);

            Variables.Set("showCommands", showCmd);
            Variables.Set("showResult", showResult);

            if (attachDebugger)
                Debugger.Launch();

            DebugEx();
        }

        public void NoDebug()
        {
            for (int idx = 0; idx < Variables.VarList.Count; idx++)
            {
                if (Variables.VarList[idx].Name.EndsWith("Debug"))
                    Variables.VarList[idx].Edit(false);
            }
            DebugEx();
            EConsole.WriteLine("Debug is offline");
        }

        public object msg(string caption, string text = "null", string icon = "none", string type = "ok") { return ShowMessageBox(caption, text, icon, type); }

        public object ConsoleBox(string caption, string text, string icon, string type)
        {
            foreach (var icn in MsgBoxIcons)
            {
                if (icn.Key.ToLower() == icon)
                {
                    icon = icn.Value.ToString();
                    break;
                }
            }
            foreach (var btn in MsgBoxButtons)
            {
                if (btn.Key.ToLower() == type)
                {
                    type = btn.Value.ToString();
                    break;
                }
            }
            ShowConsole();
            var r = ConsoleMessageBox.Show(caption, text, int.Parse(icon), int.Parse(type));
            return r.ToString();
        }

        public object IsAssoc(string extension)
        {
            FileAssociation.FILE_EXTENSION = extension;

            if (FileAssociation.IsAssociated) return null;//.Cmd.Done;
            else return null;//.Cmd.Fail;
        }

        public object assoc(string appPath, string extension, string productName, string description, string icon = "")
        {
            FileAssociation.FILE_EXTENSION = extension;
            FileAssociation.ProductName = productName;
            FileAssociation.Associate(appPath, description, icon);
            return null;//.Cmd.Done;
        }

        //public object RunCode(string code)
        //{
        //    ESCode script = new ESCode(ESCode.SplitCode(code, new string[] { "~n~" }));
        //    script.RunScript();
        //    return null;//.Cmd.Done;
        //}

        //public object RunScript(string fileName)
        //{
        //    ESCode script = new ESCode(fileName);
        //    script.RunScript();
        //    return null;//.Cmd.Done;
        //}

        public object Restart(string runAs = "0", string addArgs = "null")
        {
            Process source = Process.GetCurrentProcess();
            Process target = new Process();
            target.StartInfo = source.StartInfo;
            target.StartInfo.FileName = source.MainModule.FileName;
            target.StartInfo.WorkingDirectory = Path.GetDirectoryName(source.MainModule.FileName);

            //Required for UAC to work
            target.StartInfo.UseShellExecute = true;

            for (int i = 0; i < (Variables.GetValueObject("args") as EList).Content.Count; i++)
            {
                var b = (Variables.GetValueObject("args") as EList).Content;
                target.StartInfo.Arguments += "\"" + b[i].ToString() + "\" ";
            }


            target.StartInfo.Arguments += addArgs + " ";

            target.StartInfo.Arguments += "-wait ";
            if (Variables.GetValue("forceConsole") == "1") target.StartInfo.Arguments += "-console ";
            if (runAs == "1")
            {
                if (GlobalVars.IgnoreRunAsAdmin) return null;//.Cmd.Done;
                if (Environment.OSVersion.Version.Major > 5) target.StartInfo.Verb = "runas";
                target.StartInfo.Arguments += "-ignoreRunasRestart";
            }
            //EConsole.WriteLine("Final args: " + target.StartInfo.Arguments);
            //return null;//.Cmd.Done;
            try
            {
                if (!target.Start())
                    return null;//.Cmd.Fail;
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                //Cancelled
                if (e.NativeErrorCode == 1223)
                    return null;//.Cmd.Fail;
            }
            Environment.Exit(0);
            return null;//.Cmd.Done;
        }

        public object CreateScript(string fileName, string overwrite = "0")
        {
            FileInfo f = new FileInfo(fileName);
            if (File.Exists(f.FullName) && overwrite != "1") return null;//.Cmd.Fail;

            string fName = f.FullName;
            if (!f.Name.Contains(".es")) fName = fName + ".es";

            CoreMain.WriteResourceToFile("escript.TemplateScript.es", fName);

            return null;//.Cmd.Done;
        }

        public object UpdateScript(string fileName, string oldVersion)
        {

            return "-1";
        }

        public void PrintCompatibleTypes()
        {
            EConsole.WriteLine("These types are compatible with ESCRIPT: ", ConsoleColor.Yellow);
            foreach (var c in Parser.CompatibleTypesClass.CompatibleTypesNames)
            {
                EConsole.WriteLine(c, ConsoleColor.White);
            }
        }

        public string InvokeTest(string a, string b)
        {
            return a + " " + b;
        }

        public object FileDialog(string title = "null", string filter = "null", string multiselect = "0")
        {
#if !IsCore
            OpenFileDialog of = new OpenFileDialog();
            if (title != "null") of.Title = title;
            if (filter != "null") of.Filter = filter;
            of.Multiselect = StringToBool(multiselect);
            if (of.ShowDialog() == DialogResult.OK)
            {
                if (of.Multiselect)
                {
                    string variable = RandomString(20);
                    EList list = new EList();
                    list.Content = of.FileNames.ToList<object>();
                    Variables.SetVariableObject(variable, list);
                    return variable;
                }
                else return of.FileName;
            }
#endif
            return "null";
        }

        public object FolderDialog(string description = "null")
        {
#if !IsCore
            FolderBrowserDialog of = new FolderBrowserDialog();
            if (description != "null") of.Description = description;
            if (of.ShowDialog() == DialogResult.OK)
            {
                return of.SelectedPath;
            }
#endif
            return "null";
        }


        public object ShowMessageBox(string caption, string text = null, string icon = "none", string type = "ok")
        {
#if !IsCore
            if (text == null)
            {
                text = caption;
                caption = "Message";
            }
            //if (Variables.GetValue("forceConsole") == "1") return ConsoleBox(caption, text, icon, type);
            foreach (var icn in MsgBoxIcons)
            {
                if (icn.Key.ToLower() == icon)
                {
                    icon = icn.Value.ToString();
                    break;
                }
            }
            foreach (var btn in MsgBoxButtons)
            {
                if (btn.Key.ToLower() == type.ToLower())
                {
                    type = btn.Value.ToString();
                    break;
                }
            }
            var r = MessageBox.Show(text, caption, (MessageBoxButtons)int.Parse(type), (MessageBoxIcon)int.Parse(icon));
            return r.ToString();
#else
            return ConsoleBox(caption, text, icon, type);
#endif
        }
#if !IsCore
#endif
        public object HideConsole()
        {
            if (!EConsole.IsConsoleCreated) return null;//.Cmd.Fail;
            if (Variables.GetValue("forceConsole") == "1") return null;//.Cmd.Fail;
#if !IsCore
            var handle = EConsole.Handle;
            return GlobalVars.ShowWindow(handle, GlobalVars.SW_HIDE).ToString();
#else
            return null;//.Cmd.Fail;
#endif
        }

        public object ShowConsole()
        {
            EConsole.CreateConsole();
#if !IsCore
            var handle = EConsole.Handle;
            return GlobalVars.ShowWindow(handle, GlobalVars.SW_SHOW).ToString();
#else
            return null;//.Cmd.Fail;
#endif
        }


        public object GetConsoleWindowHandle()
        {
            if (!EConsole.IsConsoleCreated) return null;//.Cmd.Fail;
#if !IsCore
            return EConsole.Handle;
#else
            return null;//.Cmd.Fail;
#endif
        }

        public object MinimizeConsole()
        {
            if (!EConsole.IsConsoleCreated) return null;//.Cmd.Fail;
#if !IsCore
            var handle = EConsole.Handle;
            return GlobalVars.ShowWindow(handle, GlobalVars.SW_SHOWMINIMIZED).ToString();
#else
            return null;//.Cmd.Fail;
#endif
        }

        public object ReturnProcessorCount()
        {
            return Environment.ProcessorCount.ToString();
        }


        public object ThreadSleep(string time)
        {
            Thread.Sleep(int.Parse(time));
            return null;//.Cmd.Done;
        }

        public object sleep(string time) { return ThreadSleep(time); }
        public object wait(string time) { return ThreadSleep(time); }

        public object async(string method, string returnVariable = "resultAsync", string returnMethod = "null")
        {
            Thread newThread = new Thread(delegate ()
            {
                object result = Cmd.Process(method/* FIX ME, Labels*/);
                Variables.SetVariableObject(returnVariable, result);
                if (Variables.GetValue("showResult") == "1") CoreMain.PrintResult(result, "[ASYNC] ");
                if (returnMethod != "null") Cmd.Process(returnMethod/* FIX ME, Labels*/);
                GlobalVars.UserThreads.Remove(Thread.CurrentThread);
            });
            GlobalVars.UserThreads.Add(newThread);
            newThread.Start();

            return null;//.Cmd.Done;
        }

        public object ThreadList()
        {
            StringBuilder x = new StringBuilder();
            for (int i = 0; i < GlobalVars.UserThreads.Count; i++)
            {
                x.AppendLine(i + "-" + GlobalVars.UserThreads[i].ManagedThreadId);
            }
            return x.ToString();
        }

        public object ThreadAbort(string id)
        {
            GlobalVars.UserThreads[int.Parse(id)].Abort();
            return null;//.Cmd.Done;
        }


        public object ThreadAbortAll()
        {
            for (int i = 0; i < GlobalVars.UserThreads.Count; i++)
            {
                GlobalVars.UserThreads[i].Abort();
            }
            return null;//.Cmd.Done;
        }

        //public object MethodGetCode(string name)
        //{
        //    for (int i = 0; i < GlobalVars.Methods.Count; i++)
        //    {
        //        if (GlobalVars.Methods[i].Name == name) { return GlobalVars.Methods[i].GetCode(); }
        //    }
        //    return null;//.Cmd.Fail;
        //} 

        //public object MethodRemove(string name)
        //{
        //    for (int i = 0; i < GlobalVars.Methods.Count; i++)
        //    {
        //        if (GlobalVars.Methods[i].Name == name)
        //        {
        //            GlobalVars.Methods.Remove(GlobalVars.Methods[i]); return null;//.Cmd.Done; }
        //        }
        //        return null;//.Cmd.Fail;
        //    }
        //    return null;
        //}

#if ESCODE_IMPLEMENTED
        public object MethodAdd(string name, string code)
        {
            try {
                MethodRemove(name);
            }
            catch { }
            try
            {
                string c = "func " + name + "\r\n{\r\n" + code + "\r\n}";
                CmdSharp.Debug.DebugText(c);
                new ESCode(c.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries));
            }
            catch (Exception ex)
            {
                EConsole.WriteLine(ex.ToString());
                return null;//.Cmd.Fail;
            }
            return null;//.Cmd.Done;
        }
#endif
        /*
        public object EnumerateGetPath()
        {
            foreach (System.Int32 i in Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                string n = Enum.GetName(typeof(Environment.SpecialFolder), i);
                EConsole.WriteLine("SpecialFolder.Add(\"" + n + "\", " + i + ");");
            }
            return null;//.Cmd.Done;
        }*/

        public void PrintIntro()
        {
            EConsole.PrintIntro();
        }


        public object Version(string a = "all")
        {
            return AssemblyVersion(GetProgramPath().ToString(), a);
        }

        public string DateTimeNow(string format = "null")
        {
            if(format == "null") return DateTime.Now.ToString();
            return DateTime.Now.ToString(format);
        }

        public string OSVersion()
        {
            return Environment.OSVersion.VersionString;
        }

        public string ReturnOSVersion()
        {
            return OSVersion();
        }

        public string ToUpper(string text)
        {
            return text.ToUpper();
        }

        public string ToLower(string text)
        {
            return text.ToLower();
        }

        //public object OpenFileDialog(string title, string filter)
        //{
        //    System.Windows.Forms.OpenFileDialog o = new OpenFileDialog
        //    {
        //        Title = title,
        //        Filter = filter
        //    };
        //    if (o.ShowDialog() == DialogResult.OK) return o.FileName;
        //    return null;//.Cmd.Fail;
        //}

        public Functions()
        {
            ConsoleColors.Add("Black", 0);
            ConsoleColors.Add("DarkBlue", 1);
            ConsoleColors.Add("DarkGreen", 2);
            ConsoleColors.Add("DarkCyan", 3);
            ConsoleColors.Add("DarkRed", 4);
            ConsoleColors.Add("DarkMagenta", 5);
            ConsoleColors.Add("DarkYellow", 6);
            ConsoleColors.Add("Gray", 7);
            ConsoleColors.Add("DarkGray", 8);
            ConsoleColors.Add("Blue", 9);
            ConsoleColors.Add("Green", 10);
            ConsoleColors.Add("Cyan", 11);
            ConsoleColors.Add("Red", 12);
            ConsoleColors.Add("Magenta", 13);
            ConsoleColors.Add("Yellow", 14);
            ConsoleColors.Add("White", 15);

            MsgBoxIcons.Add("None", 0);
            MsgBoxIcons.Add("Information", 1);
            MsgBoxIcons.Add("Warning", 2);
            MsgBoxIcons.Add("Error", 3);
            MsgBoxIcons.Add("Exclamation", 4);
            MsgBoxIcons.Add("Question", 5);

            MsgBoxButtons.Add("OK", 0);
            MsgBoxButtons.Add("OKCancel", 1);
            MsgBoxButtons.Add("YesNo", 2);
            MsgBoxButtons.Add("YesNoCancel", 3);
            MsgBoxButtons.Add("RetryCancel", 4);
            MsgBoxButtons.Add("AbortRetryIgnore", 5);

        }
    }
}
