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

namespace escript
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


        //public Functions(List<EMethod> methods, Dictionary<string, int> labels)
        //{
        //    Methods = methods;
        //    Labels = labels;
        //}

        public object Import(string fileName, string typeName = "EFuncInvoke")
        {
            try
            {
                if (fileName.Contains(".es") || fileName.Contains(".esh"))
                {
                    new ESCode(fileName);
                    return "1";
                }
                else
                {
                    ImportLibClass i = new ImportLibClass(fileName, this, typeName);
                    GlobalVars.LoadedLibs.Add(i);
                    return "1";
                }
            }
            catch (Exception ex)
            {
                if (fileName.ToLower().StartsWith("import"))
                {
#if !IsCore
                    IntPtr resourceInfo = GlobalVars.FindResource(IntPtr.Zero, fileName, (IntPtr)10);
                    Program.Debug(fileName + " Import-Resource: 0x" + resourceInfo.ToString("X4"));

                    if (resourceInfo != IntPtr.Zero)
                    {
                        uint size = GlobalVars.SizeofResource(IntPtr.Zero, resourceInfo);
                        IntPtr pt = GlobalVars.LoadResource(IntPtr.Zero, resourceInfo);
                        byte[] bPtr = new byte[size];
                        Marshal.Copy(pt, bPtr, 0, (int)size);
                        string code = Encoding.ASCII.GetString(bPtr);
                        Program.Debug(fileName + ":\r\n" + code);

                        new ESCode(ESCode.SplitCode(code));
                        return "1";
                    }
                    return "0";
#else
                    return "-1";
#endif
                }
                
                EConsole.WriteLine(ex.ToString());
                return "-1";
            }
        }

        public object Compare(string one, string p, string two, string ifOkDoThis, string el = "null")
        {
            return If(one, p, two, ifOkDoThis, el);
        }

        public object Logo()
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
            return "1";
        }

        public object If(string one, string condition, string two, string ifOkDoThis, string elseDoThis = "null")
        {
            try
            {
                int first = int.Parse(one.ToString()), second = int.Parse(two.ToString());
                if (condition.ToString() == "<") if (first < second) { Cmd.Process(ifOkDoThis.ToString(), Labels); return 1; }
                if (condition.ToString() == ">") if (first > second) { Cmd.Process(ifOkDoThis.ToString(), Labels); return 1; }
                if (condition.ToString() == "<=") if (first <= second) { Cmd.Process(ifOkDoThis.ToString(), Labels); return 1; }
                if (condition.ToString() == ">=") if (first >= second) { Cmd.Process(ifOkDoThis.ToString(), Labels); return 1; }
                if (condition.ToString() == "==") if (first == second) { Cmd.Process(ifOkDoThis.ToString(), Labels); return 1; }
                if (condition.ToString() == "!=") if (first != second) { Cmd.Process(ifOkDoThis.ToString(), Labels); return 1; }
            }
            catch
            {
                if (condition.ToString() == "==")
                {
                    if (one.ToString() == two.ToString()) { Cmd.Process(ifOkDoThis.ToString(), Labels); return 1; }
                }
                else if (condition.ToString() == "!=")
                {
                    if (one.ToString() != two.ToString()) { Cmd.Process(ifOkDoThis.ToString(), Labels); return 1; }
                }
                else return -1;
            }
            if (!elseDoThis.ToString().ToLower().StartsWith("null")) Cmd.Process(elseDoThis.ToString(), Labels);
            return 0;
        }

        public object cd(string directory = "null") { return ChangeDir(directory); }

        public object ChangeDir(string directory = "null")
        {
            if (directory.ToLower() == "null") return ReturnCurrentDirectory();
            Directory.SetCurrentDirectory(directory);
            return "1";
        }

        public string mciOpenAndPlay(string fileName, string alias = "escript")
        {
            mciClose(alias);
            mciOpen(fileName, alias);
            return mciPlay(alias);
        }

        public string mciPlay(string alias = "escript")
        {
            return mciSendCommand("play " + alias);
        }

        public string mciOpen(string fileName, string alias = "escript")
        {
            return mciSendCommand("open \"" + fileName + "\" alias " + alias);
        }

        public string mciClose(string alias = "escript")
        {
            return mciSendCommand("close " + alias);
        }



        public string mciSendCommand(string command, string returnString = "null", string returnLength = "0", string HwndCallback = "0")
        {
#if !IsCore
            try
            {
                uint rLen = uint.Parse(returnLength);
                uint hw = uint.Parse(HwndCallback);
                string rStr = null;
                if (returnString.ToLower() != "null") rStr = returnString;
                uint result = GlobalVars.mciSendString(command, rStr, rLen, hw);
                if (result == 0) return "1";
                else return result.ToString();
            }
            catch (Exception ex)
            {
                EConsole.WriteLine("ERROR: " + ex.Message);
                return "0";
            }
#else
            return "0";
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

        public string ShowWindow(string handle, string command = "5")
        {
#if !IsCore
            if (GlobalVars.ShowWindow((IntPtr)int.Parse(handle), int.Parse(command)))
#endif
                return "1";
            return "0";
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
            if (EConsole.IsConsoleOk) return "1";
            else return "0";
        }

        public object CreateConsole()
        {
            EConsole.CreateConsole();
            return "1";
        }

        public object ConsoleSetFont(string fontFamily = "Consolas", string clearScreen = "1")
        {
#if !IsCore
            if(EConsole.cWnd != null)
            {
                EConsole.cWnd.SetFont(fontFamily);
                if (StringToBool(clearScreen))
                {
                    Clear();
                    PrintIntro();
                }
                return "1";
            }
            return "0";
#else
            return "-1";
#endif
        }

        //public object PrintResult(string result, string prefix = "")
        //{
        //    Program.PrintResult(result, prefix);
        //    return "1";
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
                return "1";
            }
            return "0";
#else
            return "-1";
#endif
        }

        public object CommandInterpreter()
        {
            Program.CommandLine();
            return "1";
        }

        public object TCP_SetTriggers(string triggerMsg, string triggerDisconnected, string triggerConnected)
        {
            Variables.Set("TCP_triggerMsg", triggerMsg);
            Variables.Set("TCP_triggerDisconnected", triggerDisconnected);
            Variables.Set("TCP_triggerConnected", triggerConnected);
            return 1;
        }

        public object TCP_Disconnect()
        {
            TCPConnection.Disconnect();
            return 1;
        }
        public object TCP_Connect(object ip, object port)
        {
            return TCPConnection.Connect(ip.ToString(), int.Parse(port.ToString()), Labels);
        }

        public object TCP_Send(object text)
        {
            TCPConnection.Send(text.ToString());
            return 1;
        }

        public object Write(string text = "null", string color = "null")
        {
            if (text.ToLower() == "null") text = "";
            string oldColor = GetForegroundColor().ToString();
            if (color != "null") SetColor(color);
            EConsole.Write(text);
            if (color != "null") SetColor(oldColor);
            return 1;
        }

        public object echo(string text = "null", string color = "null")
        {
            return WriteLine(text, color);
        }

        public object Beep()
        {
            Console.Beep();
            return 1;
        }

        public object ConsoleSetBuffer(int width, int height)
        {
            Console.SetBufferSize(width, height);
            return 1;
        }

        public object ConsoleSetWindowSize(int width, int height)
        {
            Console.SetWindowSize(width, height);
            return 1;
        }

        public object ConsoleSetWindowPosition(int x, int y)
        {
            Console.SetWindowPosition(x, y);
            return 1;
        }

        public object ConsoleSetCursorPosition(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            return 1;
        }

        public object StringStartsWith(string text, string value)
        {
            if (text.StartsWith(value)) return "1";
            else return "0";
        }

        public object StringLength(string text)
        {
            return text.Length;
        }

        public object StringContains(string text, string contains)
        {
            if (text.Contains(contains)) return "1";
            return "0";
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
                Program.Debug(ex.ToString(), ConsoleColor.DarkRed);
                throw ex;
            }
        }

        public string IsInt(string text)
        {
            try
            {
                int.Parse(text);
                return "1";
            }
            catch { }
            return "0";
        }

        public object IsFileLocked(string path)
        {
            return Program.IsFileLocked(new FileInfo(path));
        }

        public string StringSplit(string text, string splitSymbol, string variable = "null")
        {
            if (variable == "null") variable = RandomString("20");
            ListCreate(variable);
            foreach (var a in text.Split(new string[] { splitSymbol }, StringSplitOptions.RemoveEmptyEntries))
            {
                ListAdd(variable, a);
            }
            return variable;
        }

        public void deb()
        {
            Debug();
            string code =
@"$a=a
$b=b
$c=c
$d=d
$something=test
$i=love
$e=script
$lol=kek
$cheburek=null";

            MethodAdd("debx (IgnoreCleanup)", code);
            Cmd.Process("debx");
        }

        //        public void deb()
        //        {
        //            string code = 
        //@"$a=#b||c
        //echo {StringSetLength $a||2}";
        //            MethodRemove("debx");
        //            MethodAdd("debx", code);
        //            Debug();
        //            EConsole.WriteLine("Code:\r\n" + MethodGetCode("debx"));
        //            Cmd.Process("debx");
        //        }

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
            Program.CheckUpdates();
            return "1";
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
                return "1";
            }
            return "0";
#else
            return "0";
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
            return "1";
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
            return "1";
        }

        public object SetStatus(string caption, string status = "null", string value = "-2", string createNew = "0")
        {
#if !IsCore
            if(status == "null")
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
            return "1";
        }

        public object HideStatus() // Todo
        {
#if !IsCore
            if (statusWnd != null)
            {
                statusWnd.Dispose();
                statusWnd = null;
                return "1";
            }
#endif
            return "0";
        }

        public object DirExists(string dirName)
        {
            if (Directory.Exists(dirName)) return "1";
            return "0";
        }

        public object FileExists(string fileName)
        {
            if (File.Exists(fileName)) return "1";
            return "0";
        }

        public object DeleteDir(string dirName)
        {
            if (!Directory.Exists(dirName)) return "-2";
            Directory.Delete(dirName);
            return "0";
        }

        public object MakeDir(string dirName)
        {
            if (Directory.Exists(dirName)) return "-2";
            try
            {
                Directory.CreateDirectory(dirName);
                return "1";
            }
            catch
            {
            }
            return "0";
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
        //            return "1";
        //#else
        //            return "0";
        //#endif
        //        }

        public object BeepEx(object frequency, object duration)
        {
            Console.Beep(int.Parse(frequency.ToString()), int.Parse(duration.ToString()));
            return 1;
        }

        public object WriteLine(string line = "null", string color = "null")
        {
            if (line.ToLower() == "null") line = "";
            string oldColor = GetForegroundColor().ToString();
            if (color != "null") SetColor(color);
            EConsole.WriteLine(line);
            if (color != "null") SetColor(oldColor);
            return 1;
        }

        public object Exit(string code = "0")
        {
            Environment.Exit(int.Parse(code));
            return 1;
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
                Cmd.Process(command, Labels);
            }
            Variables.Remove(enumIdxVar);
            Variables.Remove(enumValueVar);
            return "1";
        }

        public object DebugText(string text, string foregroundColor = "white")
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
            Program.Debug("[SCRIPT] " + text, cc);
            return "1";
        }

        public object For(string start, string stop, string command, string infoVariable = "for")
        {
            for (int a = int.Parse(start); a < int.Parse(stop.ToString()); a++)
            {
                if (GlobalVars.StopProgram) break;
                Variables.Set(infoVariable.ToString(), a.ToString());
                Cmd.Process(command.ToString(), Labels);
            }
            return "1";
        }

        public object Exception(object text)
        {
            throw new Exception(text.ToString());
        }

        public object ReturnStackTrace()
        {
            return Environment.StackTrace;
        }

        public object ReturnCurrentDirectory()
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

        public object FileCopy(string fileName, string destination, string overwriteFile = "0")
        {
            bool ov = false;
            if (overwriteFile.StartsWith("1")) ov = true;
            try
            {
                File.Copy(fileName, destination, ov);
                return "1";
            }
            catch (Exception ex) { throw ex; }
        }

        public object f(string contains = "null") { return GetMethods(contains); }

        public object GetMethods(string contains = "null")
        {
            List<MethodInfo> m = this.GetType().GetMethods().ToList();

            for (int i = 0; i < GlobalVars.LoadedLibs.Count; i++)
            {
                ImportLibClass imp = GlobalVars.LoadedLibs[i];
                if (imp.obj != null)
                {
                    m.AddRange(imp.funcType.GetMethods());
                }
            }



            if (contains != "null")
            {
                EConsole.ForegroundColor = ConsoleColor.Magenta;
                EConsole.WriteLine("Searching for: " + contains);
            }
            foreach (var method in m)
            {
                if (contains == "null") UseTextTest(method.Name, "0");
                else
                {
                    if (method.Name.ToLower().Contains(contains.ToLower())) UseTextTest(method.Name, "0");

                }
            }
            for (int i = 0; i < GlobalVars.Methods.Count; i++)
            {
                if (contains == "null") UseTextTest(GlobalVars.Methods[i].Name, "0", "1");
                else
                {
                    if (GlobalVars.Methods[i].Name.ToLower().Contains(contains.ToLower())) UseTextTest(GlobalVars.Methods[i].Name, "0", "1");

                }
            }

            return "1";
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
                ImportLibClass imp = GlobalVars.LoadedLibs[i];
                if (imp.obj != null)
                {
                    if (imp.funcType.GetMethod(name) != null)
                    {
                        mth = imp.funcType.GetMethod(name);
                        target = imp.obj;
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
            return 1;
        }

        public object DictEdit(string variable, string key, object value)
        {
            EDictionary list = Variables.GetValueObject(variable) as EDictionary;
            list.Content[key] = value;
            return "1";
        }

        public object DictAdd(string variable, object key, object value)
        {
            EDictionary list = Variables.GetValueObject(variable) as EDictionary;
            list.Content.Add(key, value);
            return "1";
        }

        public object DictRemove(string variable, object key)
        {
            EDictionary list = Variables.GetValueObject(variable) as EDictionary;
            list.Content.Remove(key);
            return "1";
        }

        public object DictReverse(string variable)
        {
            EDictionary list = Variables.GetValueObject(variable) as EDictionary;
            list.Content.Reverse();
            return "1";
        }

        public object DictClear(string variable)
        {
            EDictionary list = Variables.GetValueObject(variable) as EDictionary;
            list.Content.Clear();
            return "1";
        }


        public object DictContainsKey(string variable, object key)
        {
            var list = (Variables.GetValueObject(variable) as EDictionary);
            if (list.Content.ContainsKey(key)) return 1;
            return 0;
        }
        public object DictContainsValue(string variable, object value)
        {
            var list = (Variables.GetValueObject(variable) as EDictionary);
            if (list.Content.ContainsValue(value)) return 1;
            return 0;
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
            return 1;
        }

        public object ListEdit(string variable, string index, object value)
        {
            EList list = Variables.GetValueObject(variable) as EList;
            list.Content[int.Parse(index)] = value;
            return "1";
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
            return "1";
        }

        public object ListReverse(string variable)
        {
            EList list = Variables.GetValueObject(variable) as EList;
            list.Content.Reverse();
            return "1";
        }

        public object ListClear(string variable)
        {
            EList list = Variables.GetValueObject(variable) as EList;
            list.Content.Clear();
            return "1";
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
            if ((int)ListGetIndex(variable, item) == -1) return 0;
            return 1;
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

        public object ListDefaultIndex(string variable, string value = "null")
        {
            if (value == "null") return (Variables.GetValueObject(variable) as EList).DefaultIndex.ToString();

            (Variables.GetValueObject(variable) as EList).DefaultIndex = int.Parse(value);

            return "1";
        }


        public object ListCount(string variable)
        {
            return (Variables.GetValueObject(variable) as EList).Content.Count;
        }


        public object cmd(string command)
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

        public object FileFullName(string file)
        {
            return new FileInfo(file).FullName;
        }

        public object FileName(string file)
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
                return "1";
            }
            catch (Exception ex) { throw ex; }
        }

        public object FileDelete(string fileName)
        {
            try
            {
                File.Delete(fileName);
                return "1";
            }
            catch (Exception ex) { throw ex; }
        }

        public object ConsoleTextBox(string title, string text)
        {
            return escript.ConsoleTextBox.Show(title, text);
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
            string result = w.ShowDialog().ToString();
            Variables.Set("dialogText", Cmd.ProcessString(w.GetInput().Replace("||", "^split^").Replace("{", "^(^").Replace("}", "^)^")));
            return result;
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

        public string RandomString(string length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, int.Parse(length))
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public object ChoiceWindow(string title, string text, string vListButtons, string defButton = "null", string removeVariable = "1")
        {
            string vResult = "dialogText";

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

            if (removeVariable == "1") Variables.Remove(vListButtons);

            Variables.Set(vResult, w.Get());
            return result;
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
            return 1;
        }

        public object UseTextTest(string method, string printUse = "1", string customPrint = "0")
        {
            if (customPrint == "0")
            {
                System.Reflection.MethodInfo mth = null;

                List<MethodInfo> m = this.GetType().GetMethods().ToList();


                for (int i = 0; i < GlobalVars.LoadedLibs.Count; i++)
                {
                    ImportLibClass imp = GlobalVars.LoadedLibs[i];
                    if (imp.obj != null)
                    {
                        m.AddRange(imp.funcType.GetMethods());
                    }
                }

                foreach (var me in m)
                {
                    if (me.Name == method) mth = me;
                }


                if (mth == null) return "null";
                var Args = mth.GetParameters();

                ConsoleColor ca = EConsole.ForegroundColor;
                EConsole.ForegroundColor = ConsoleColor.Yellow;
                if (printUse == "1")
                {
                    EConsole.Write("USE: ");
                }
                EConsole.ForegroundColor = ConsoleColor.White;
                if (method == "ifvar") method = "if";
                if (method == "Times") method = "for";
                EConsole.Write(method + " ");
                for (int i = 0; i < Args.Length; i++)
                {
                    EConsole.ForegroundColor = ConsoleColor.Gray;
                    string arg = "[" + Args[i].Name;
                    if (Args[i].DefaultValue.ToString().Length >= 1) arg += " = " + Args[i].DefaultValue;
                    EConsole.Write(arg + "]");
                    EConsole.ForegroundColor = ConsoleColor.DarkGray;
                    if (i != (Args.Length - 1)) EConsole.Write(Variables.GetValue("splitArgs"));
                }
                EConsole.WriteLine("");
                EConsole.ForegroundColor = ca;
                return "1";

            }
            else
            {
                for (int x = 0; x < GlobalVars.Methods.Count; x++) if (GlobalVars.Methods[x].Name == method)
                    {
                        ConsoleColor ca = EConsole.ForegroundColor;
                        EConsole.ForegroundColor = ConsoleColor.Yellow;
                        if (printUse == "1")
                        {
                            EConsole.Write("USE: ");
                        }
                        EConsole.ForegroundColor = ConsoleColor.DarkGray;
                        EConsole.Write("(escript) ");
                        EConsole.ForegroundColor = ConsoleColor.White;
                        EConsole.Write(method + " ");
                        if (GlobalVars.Methods[x].Arguments.Count >= 1)
                            for (int i = 0; i < GlobalVars.Methods[x].Arguments.Count; i++)
                            {
                                EConsole.ForegroundColor = ConsoleColor.Gray;
                                string arg = "[" + GlobalVars.Methods[x].Arguments[i].Name;
                                if(GlobalVars.Methods[x].Arguments[i].DefaultValue != "null")
                                {
                                    arg += " = " + GlobalVars.Methods[x].Arguments[i].DefaultValue;
                                }
                                EConsole.Write(arg + "]");
                                EConsole.ForegroundColor = ConsoleColor.DarkGray;
                                if (i != (GlobalVars.Methods[x].Arguments.Count - 1)) EConsole.Write(Variables.GetValue("splitArgs"));
                            }
                        EConsole.WriteLine("");
                        EConsole.ForegroundColor = ca;
                        return "1";
                    }
            }
            return "null";
        }

        public object GetMethod(string methodName)
        {
            //if(methodName.Length == 0)
            //{
            //    EConsole.WriteLine(UseTextTest("GetMethod"));
            //    return "0";
            //}
            System.Reflection.MethodInfo mth = null;

            List<MethodInfo> m = this.GetType().GetMethods().ToList();


            for (int i = 0; i < GlobalVars.LoadedLibs.Count; i++)
            {
                ImportLibClass imp = GlobalVars.LoadedLibs[i];
                if (imp.obj != null)
                {
                    m.AddRange(imp.funcType.GetMethods());
                }
            }

            foreach (var me in m)
            {
                if (me.Name == methodName) mth = me;
            }


            if (mth == null) return "0";
            var Args = mth.GetParameters();
            EConsole.WriteLine("Method: " + mth.Name);
            for (int i = 0; i < Args.Length; i++)
            {
                EConsole.WriteLine("[Argument " + i + "] " + Args[i].Name + " = " + Args[i].DefaultValue);
            }
            return "1";
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
                return "0";
            }
            EConsole.ForegroundColor = co;
            return "1";
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
        //            Program.verText = Variables.GetValue("workingScriptText");
        //            //Thread mThread = new Thread(Program.FormThread);
        //            mThread.SetApartmentState(ApartmentState.STA);
        //            mThread.Start();
        //            Thread.Sleep(2000);
        //            return "1";
        //#else
        //            return "0";
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
            Program.ScriptColor = cc;
            return "1";
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
            return "1";
        }

        public object ConvertScript(string fileName, string outName = "null", string iconPath = "null", string anykey = "0")
        {
            if (iconPath != "null" && outName == "null") return "0";

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
            return "1;" + target.ExitCode.ToString();
        }



        public object Stop()
        {
            GlobalVars.StopProgram = true;
            return 1;
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
            return "1";
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
        //    return "1";
        //}

        public object UpdateProgram(string r = "latest")
        {
            string tempPath = System.IO.Path.GetTempPath();
            string uScriptPath = Path.Combine(tempPath, "UpdateScript.es");

            Program.WriteResourceToFile("escript.UpdateScript.es", uScriptPath);

            Process eInstance = new Process();
            eInstance.StartInfo.FileName = GlobalVars.GetAboutMe().FullName;
            eInstance.StartInfo.Arguments = String.Format("\"{0}\" \"{1}\"", uScriptPath, r);
            eInstance.Start();
            return "1";
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
            return "1";
        }

        public object StartProgram(string fileName, string arguments = "null", string waitForExit = "0", string asAdmin = "0")
        {
            Process a = new Process();
            a.StartInfo.FileName = fileName;
            a.StartInfo.UseShellExecute = true;
            if (asAdmin == "1") { if (Environment.OSVersion.Version.Major > 5) a.StartInfo.Verb = "runas"; }

            if (arguments != "null") a.StartInfo.Arguments = arguments;

            try
            {
                a.Start();
                if (waitForExit.ToLower().StartsWith("1"))
                {
                    a.WaitForExit();
                    return a.ExitCode;
                }
            }
            catch (Exception ex)
            {
                EConsole.WriteLine(ex.Message);
                return -1;
            }

            return "1";
        }

        public object Return(string text, string breakMethod = "1")
        {
            if (Variables.GetValue("workingMethod") != "")
            {
                Variables.SetVariable("workingMethodResult", text);
                if (breakMethod == "1") Variables.SetVariable("workingMethodBreak", "1");
            }
            return text;
        }

        public object PathCombine(string path, string path2)
        {
            return Path.Combine(path, path2);
        }

        public object DwmExtendFrameIntoClientArea(string windowHandle, int left = -1, int right = -1, int top = -1, int bottom = -1)
        {
#if !IsCore
            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                if (Aero.DwmIsCompositionEnabled())
                {
                    IntPtr h = GlobalVars.GetConsoleWindowHandle();
                    if (windowHandle.ToLower() != "null") h = (IntPtr)int.Parse(windowHandle);
                    Aero.Glass(h, left, right, top, bottom);
                    return 1;
                }
            }
#endif
            return 0;
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
                return -1;
            }
#else
            return -1;
#endif
        }

        public object IsKeyDown(string key)
        {
#if !IsCore
            try
            {
                int keyCode = int.Parse(key);
                bool result = Keyboard.IsKeyDown(Keyboard.KeyCodeToKey(keyCode));
                if (result) return 1;
                else return 0;
            }
            catch
            {
                foreach (System.Int32 i in Enum.GetValues(typeof(Keys)))
                {
                    if (Enum.GetName(typeof(Keys), i).ToLower() == key.ToLower())
                    {
                        bool result = Keyboard.IsKeyDown((Keys)i);
                        if (result) return 1;
                        else return 0;
                    }
                }
                return -1;
            }
#else
            return -1;
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

        public object GetMethodOptions(string method)
        {
            for (int x = 0; x < GlobalVars.Methods.Count; x++) if (GlobalVars.Methods[x].Name == method)
                {
                    StringBuilder b = new StringBuilder();
                    foreach(var o in GlobalVars.Methods[x].Options)
                    {
                        b.AppendLine(o);
                    }
                    return b.ToString();
                }
            return "0";
        }

        public object GetDocumentation(string topic = "FirstHelp", string version = "null")
        {
            string url = GlobalVars.StuffServer + "documentation/$ProgramVersion/" + topic + ".txt";
            if (version == "null") version = String.Format("{0}.{1}", Cmd.Process("ver major"), Cmd.Process("ver minor"));
            url = url.Replace("$ProgramVersion", version);
            Program.Debug("Downloading: " + url);
            WebClient client = new WebClient();
            string text = client.DownloadString(url);
            if (text.StartsWith("REDIRECT:"))
            {
                return GetDocumentation(text.Replace("REDIRECT:", ""));
            }
            echo(text);
            return "1";
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
        //    return 0;
        //}
        


        public object cls() { return Clear(); }

        public object Clear()
        {
            EConsole.Clear();
            return "1";
        }

        public object set(string name = "null", string value = "null") { return SetVar(name, value); }

        public object SetVar(string name = "null", string value = "null")
        {
            if (name.ToLower() == "null") return VarList();

            string workingMethod = Variables.GetValue("workingMethod");

            Variables.Set(name, Cmd.RemoveHot(value), new List<string> { "Method=" + workingMethod });
            return "1";
        }

        public object unset(string name) { return RemoveVar(name); }

        public object RemoveVar(string name)
        {
            return Variables.Remove(name);
        }

        public object VarCreated(string name)
        {
            EVariable v = Variables.GetVariable(name);
            if (v == null) return 0;
            else return v.Created.ToString();
        }
        public object VarEdited(string name)
        {
            EVariable v = Variables.GetVariable(name);
            if (v == null) return 0;
            else return v.Edited.ToString();
        }

        public object VarReinit(string addSystem = "1")
        {
            Variables.Initialize(StringToBool(addSystem));
            return "1";
        }

        public string BoolToString(string b)
        {
            if (b.ToLower() == "true" || b == "1") return "1";
            else return "0";
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
            foreach(var a in l)
            {
                b.AppendLine(a);
            }
            return b.ToString();
        }

        public object MethodCleanup(string MethodName)
        {
            return Cmd.MethodCleanup(MethodName);
        }

        public object VarList(string showSystem = "0", string returnIt = "0", string ignoreOptions = "0")
        {
            StringBuilder r = new StringBuilder();
            ConsoleColor cccc = EConsole.ForegroundColor;
            EConsole.ForegroundColor = ConsoleColor.Gray;
            if (returnIt == "0") EConsole.WriteLine(String.Format("Variables ({0}):", Variables.VarList.Count));
            if (returnIt == "1") r.AppendLine(String.Format("Variables ({0}):", Variables.VarList.Count));
            for (int idx = 0; idx < Variables.VarList.Count; idx++)
            {
                if (!StringToBool(ignoreOptions))
                {
                    if (!StringToBool(showSystem) && Variables.VarList[idx].Options.Contains("System")) continue;
                    if (Variables.VarList[idx].Options.Contains("Hidden")) continue;
                }

                if (returnIt == "0") EConsole.ForegroundColor = ConsoleColor.DarkGray;
                if (returnIt == "0") EConsole.Write("[" + idx + "] ");
                if (returnIt == "0") EConsole.ForegroundColor = ConsoleColor.Yellow;
                if (returnIt == "0") EConsole.Write(Variables.VarList[idx].Name);
                if (returnIt == "0") EConsole.ForegroundColor = ConsoleColor.White;
                if (returnIt == "0") EConsole.Write(" = ");
                if (returnIt == "0") EConsole.ForegroundColor = ConsoleColor.Green;
                if (returnIt == "0") EConsole.Write(Variables.VarList[idx].Value);
                if (returnIt == "0") EConsole.WriteLine("");
                if (returnIt == "1") r.AppendLine("[" + idx + "] " + Variables.VarList[idx].Name + " = " + Variables.VarList[idx].Value);
            }
            EConsole.ForegroundColor = cccc;
            if (returnIt == "1") return r.ToString();
            return "1";
        }

        public object TestMethod(string a1 = "Default Argument 1", string a2 = "Default Argument 2", string a3 = "Default Argument 3")
        {
            EConsole.WriteLine("(.NET method)", true, ConsoleColor.DarkGray);
            EConsole.WriteLine("a1 : " + a1, true, ConsoleColor.Red);
            EConsole.WriteLine("a2 : " + a2, true, ConsoleColor.Green);
            EConsole.WriteLine("a3 : " + a3, true, ConsoleColor.Blue);
            return "1";
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
        //    return "1";
        //}

        //public static string Throw(string exceptionVariable)
        //{
        //    Exception e = (Exception)Variables.GetValueObject(exceptionVariable);
        //    throw e;
        //}


        public object ForceSystemConsole()
        {
            Variables.Set("useCustomConsole", "0");
            EConsole.Kill();
            EConsole.CreateConsole();
            return "1";
        }

        public object ForceGUIConsole()
        {
            Variables.Set("useCustomConsole", "1");
            EConsole.Kill();
            EConsole.CreateConsole();
            return "1";
        }

        public object Debug(string debugger = "0", string showCmd = "1", string showResult = "1", string showConsole = "1")
        {
            GlobalVars.DebugWhenFormingEnv = true;
            if (showConsole == "1") ShowConsole();
            EConsole.ForegroundColor = ConsoleColor.Magenta;
            EConsole.WriteLine(" -> DEBUG MODE");
            EConsole.ForegroundColor = ConsoleColor.White;
            Variables.Set("programDebug", "1");
            Variables.Set("showCommands", showCmd);
            Variables.Set("showResult", showResult);
            if (debugger == "1") Debugger.Launch();
            return 1;
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

            if (FileAssociation.IsAssociated) return "1";
            else return "0";
        }

        public object assoc(string appPath, string extension, string productName, string description, string icon = "")
        {
            FileAssociation.FILE_EXTENSION = extension;
            FileAssociation.ProductName = productName;
            FileAssociation.Associate(appPath, description, icon);
            return "1";
        }

        public object RunScript(string fileName, string code = "null")
        {
            if (code.ToLower() != "1")
            {
                ESCode script = new ESCode(ESCode.SplitCode(code, new string[] { "~n~" }));
                script.RunScript();
            }
            else
            {
                ESCode script = new ESCode(fileName);
                script.RunScript();
            }
            return "1";
        }

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
                if (GlobalVars.IgnoreRunAsAdmin) return "1";
                if(Environment.OSVersion.Version.Major > 5) target.StartInfo.Verb = "runas";
                target.StartInfo.Arguments += "-ignoreRunasRestart";
            }
            //EConsole.WriteLine("Final args: " + target.StartInfo.Arguments);
            //return "1";
            try
            {
                if (!target.Start())
                    return "0";
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                //Cancelled
                if (e.NativeErrorCode == 1223)
                    return "0";
            }
            Environment.Exit(0);
            return "1";
        }

        public object CreateScript(string fileName, string overwrite = "0")
        {
            FileInfo f = new FileInfo(fileName);
            if (File.Exists(f.FullName) && overwrite != "1") return "0";

            string fName = f.FullName;
            if (!f.Name.Contains(".es")) fName = fName + ".es";

            Program.WriteResourceToFile("escript.TemplateScript.es", fName);

            return "1";
        }

        public object UpdateScript(string fileName, string oldVersion)
        {

            return "-1";
        }

        public object ShowMessageBox(string caption, string text = "null", string icon = "none", string type = "ok")
        {
#if !IsCore
            if (text == "null")
            {
                text = caption;
                caption = "Message";
            }
            if (Variables.GetValue("forceConsole") == "1") return ConsoleBox(caption, text, icon, type);
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
            var r = MessageBox.Show(new Form() { }, text, caption, GetMsgBoxBtns(int.Parse(type)), GetMsgBoxIcon(int.Parse(icon)));
            return r.ToString();
#else
            return ConsoleBox(caption, text, icon, type);
#endif
        }
#if !IsCore
        private MessageBoxButtons GetMsgBoxBtns(int type)
        {

            switch (type)
            {
                case 1: return MessageBoxButtons.OKCancel;
                case 2: return MessageBoxButtons.YesNo;
                case 3: return MessageBoxButtons.YesNoCancel;
                case 4: return MessageBoxButtons.RetryCancel;
                case 5: return MessageBoxButtons.AbortRetryIgnore;
            }
            return MessageBoxButtons.OK;
        }
        private MessageBoxIcon GetMsgBoxIcon(int type)
        {
            switch (type)
            {
                case 1:
                    {
                        return MessageBoxIcon.Information;
                    }
                case 2:
                    {
                        return MessageBoxIcon.Warning;
                    }
                case 3:
                    {
                        return MessageBoxIcon.Error;
                    }
                case 4:
                    {
                        return MessageBoxIcon.Exclamation;
                    }
                case 5:
                    {
                        return MessageBoxIcon.Question;
                    }
            }
            return MessageBoxIcon.None;
        }
#endif
        public object HideConsole()
        {
            if (!EConsole.IsConsoleOk) return "0";
            if (Variables.GetValue("forceConsole") == "1") return "0";
#if !IsCore
            var handle = GlobalVars.GetConsoleWindowHandle();
            return GlobalVars.ShowWindow(handle, GlobalVars.SW_HIDE).ToString();
#else
            return 0;
#endif
        }

        public object ShowConsole()
        {
            EConsole.CreateConsole();
#if !IsCore
            var handle = GlobalVars.GetConsoleWindowHandle();
            return GlobalVars.ShowWindow(handle, GlobalVars.SW_SHOW).ToString();
#else
            return 0;
#endif
        }
        

        public object GetConsoleWindowHandle()
        {
            if (!EConsole.IsConsoleOk) return "0";
#if !IsCore
            return GlobalVars.GetConsoleWindowHandle().ToString();
#else
            return 0;
#endif
        }

        public object MinimizeConsole()
        {
            if (!EConsole.IsConsoleOk) return "0";
#if !IsCore
            var handle = GlobalVars.GetConsoleWindowHandle(); 
            return GlobalVars.ShowWindow(handle, GlobalVars.SW_SHOWMINIMIZED).ToString();
#else
            return 0;
#endif
        }

        public object ReturnProcessorCount()
        {
            return Environment.ProcessorCount.ToString();
        }


        public object ThreadSleep(string time)
        {
            Thread.Sleep(int.Parse(time));
            return "1";
        }

        public object sleep(string time) { return ThreadSleep(time); }
        public object wait(string time) { return ThreadSleep(time); }

        public object async(string method, string returnVariable = "resultAsync", string returnMethod = "null")
        {
            Thread newThread = new Thread(delegate ()
            {
                string result = Cmd.Process(method, Labels);
                Variables.Set(returnVariable, result);
                if (Variables.GetValue("showResult") == "1") Program.PrintResult(result, "[ASYNC] ");
                if (returnMethod != "null") Cmd.Process(returnMethod, Labels);
                GlobalVars.UserThreads.Remove(Thread.CurrentThread);
            });
            GlobalVars.UserThreads.Add(newThread);
            newThread.Start();

            return 1;
        }
        
        public object ThreadList()
        {
            StringBuilder x = new StringBuilder();
            for(int i = 0; i < GlobalVars.UserThreads.Count; i++)
            {
                x.AppendLine(i + "-" + GlobalVars.UserThreads[i].ManagedThreadId);
            }
            return x.ToString();
        }

        public object ThreadAbort(string id)
        {
            GlobalVars.UserThreads[int.Parse(id)].Abort();
            return "1";
        }


        public object ThreadAbortAll()
        {
            for (int i = 0; i < GlobalVars.UserThreads.Count; i++)
            {
                GlobalVars.UserThreads[i].Abort();
            }
            return "1";
        }

        public object MethodGetCode(string name)
        {
            for (int i = 0; i < GlobalVars.Methods.Count; i++)
            {
                if (GlobalVars.Methods[i].Name == name) { return GlobalVars.Methods[i].GetCode(); }
            }
            return 0;
        }

        public object MethodRemove(string name)
        {
            for(int i =0;i<GlobalVars.Methods.Count;i++)
            {
                if (GlobalVars.Methods[i].Name == name) { GlobalVars.Methods.Remove(GlobalVars.Methods[i]); return 1; }
            }
            return 0;
        }

        public object MethodAdd(string name, string code)
        {
            try {
                MethodRemove(name);
            }
            catch { }
            try
            {
                string c = "func " + name + "\r\n{\r\n" + code + "\r\n}";
                Program.Debug(c);
                new ESCode(c.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries));
            }
            catch (Exception ex)
            {
                EConsole.WriteLine(ex.ToString());
                return "0";
            }
            return "1";
        }
        /*
        public object EnumerateGetPath()
        {
            foreach (System.Int32 i in Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                string n = Enum.GetName(typeof(Environment.SpecialFolder), i);
                EConsole.WriteLine("SpecialFolder.Add(\"" + n + "\", " + i + ");");
            }
            return "1";
        }*/

        public object PrintIntro()
        {
            EConsole.PrintIntro();
            return 1;
        }


        public object Version(string a = "all")
        {
            return AssemblyVersion(GetProgramPath().ToString(), a);
        }

        public object DateTimeNow(string format = "null")
        {
            if(format == "null") return DateTime.Now.ToString();
            return DateTime.Now.ToString(format);
        }

        public object OSVersion()
        {
            return Environment.OSVersion.VersionString;
        }

        public object ReturnOSVersion()
        {
            return OSVersion();
        }

        public object ToUpper(string text)
        {
            return text.ToUpper();
        }

        public object ToLower(string text)
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
        //    return "0";
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
