using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace escript
{
    public class Functions
    {
        public Dictionary<string, int> ConsoleColors = new Dictionary<string, int>();

        #region imports
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        const int SW_SHOWMINIMIZED = 2;

        [DllImport("winmm.dll")]
        public static extern uint mciSendString(
            string lpstrCommand, string lpstrReturnString, uint uReturnLength, uint hWndCallback);
        #endregion

        public List<EMethod> Methods;
        public Dictionary<string, int> Labels;


        //public Functions(List<EMethod> methods, Dictionary<string, int> labels)
        //{
        //    Methods = methods;
        //    Labels = labels;
        //}

        public object Compare(string one, string p, string two, string ifOkDoThis, string el = "null")
        {
            return ifvar(one, p, two, ifOkDoThis, el);
        }

        public object ifvar(string one, string condition, string two, string ifOkDoThis, string elseDoThis = "null")
        {
            try
            {
                int first = int.Parse(one.ToString()), second = int.Parse(two.ToString());
                if (condition.ToString() == "<") if (first < second) { Cmd.Process(ifOkDoThis.ToString(), Methods, Labels); return 1; } 
                if (condition.ToString() == ">") if (first > second) { Cmd.Process(ifOkDoThis.ToString(), Methods, Labels); return 1; }
                if (condition.ToString() == "<=") if (first <= second) { Cmd.Process(ifOkDoThis.ToString(), Methods, Labels); return 1; }
                if (condition.ToString() == ">=") if (first >= second) { Cmd.Process(ifOkDoThis.ToString(), Methods, Labels); return 1; }
                if (condition.ToString() == "==") if (first == second) { Cmd.Process(ifOkDoThis.ToString(), Methods, Labels); return 1; }
            }
            catch
            {
                if (condition.ToString() == "==")
                {
                    if (one.ToString() == two.ToString()) { Cmd.Process(ifOkDoThis.ToString(), Methods, Labels); return 1; }
                }
                else return -1;
            }
            if(!elseDoThis.ToString().ToLower().StartsWith("null")) Cmd.Process(elseDoThis.ToString(), Methods, Labels);
            return 0;
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
            try
            {
                uint rLen = uint.Parse(returnLength);
                uint hw = uint.Parse(HwndCallback);
                string rStr = null;
                if (returnString.ToLower() != "null") rStr = returnString;
                uint result = mciSendString(command, rStr, rLen, hw);
                if (result == 0) return "1";
                else return result.ToString();
            }
            catch (Exception ex) {
                Program.ConWrLine("ERROR: " + ex.Message);
                return "0"; }
        }

        public string web_get(object Url, object Data)
        {
            Program.ConWrLine("WARNING: web_get is WebGet now");
            return WebGet(Url, Data);
        }
        public string web_post(object Url, object Data)
        {
            Program.ConWrLine("WARNING: web_post is WebPost now");
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
        
        public string WebPost(object Url, object Data)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(Url.ToString());
            req.Method = "POST";
            req.Timeout = 100000;
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] sentData = Encoding.GetEncoding(1251).GetBytes(Data.ToString());
            req.ContentLength = sentData.Length;
            System.IO.Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
            sendStream.Close();
            System.Net.WebResponse res = req.GetResponse();
            System.IO.Stream ReceiveStream = res.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(ReceiveStream, Encoding.UTF8);
            //Кодировка указывается в зависимости от кодировки ответа сервера
            Char[] read = new Char[256];
            int count = sr.Read(read, 0, 256);
            string Out = String.Empty;
            while (count > 0)
            {
                String str = new String(read, 0, count);
                Out += str;
                count = sr.Read(read, 0, 256);
            }
            return Out;
        }

        public object TCP_SetTriggers(object triggerMsg, object triggerDisconnected, object triggerConnected)
        {
            Cmd.CmdParams["TCP_triggerMsg"] = triggerMsg.ToString();
            Cmd.CmdParams["TCP_triggerDisconnected"] = triggerDisconnected.ToString();
            Cmd.CmdParams["TCP_triggerConnected"] = triggerConnected.ToString();
            return 1;
        }

        public object TCP_Disconnect()
        {
            TCPConnection.Disconnect();
            return 1;
        }
        public object TCP_Connect(object ip, object port)
        {
            return TCPConnection.Connect(ip.ToString(), int.Parse(port.ToString()), Methods, Labels);
        }

        public object TCP_Send(object text)
        {
            TCPConnection.Send(text.ToString());
            return 1;
        }

        public object echo(object text)
        {
            return WriteLine(text);
        }

        public object Beep()
        {
            Console.Beep();
            return 1;
        }
        

        public object CheckUpdates()
        {
            Program.CheckUpdates();
            return "1";
        }

        public object InstallESCRIPT()
        {
            Process p = new Process();
            string me = System.Reflection.Assembly.GetExecutingAssembly().Location;

            p.StartInfo.FileName = me;
            p.StartInfo.Arguments = "-installNext";
            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                p.StartInfo.Verb = "runas";
            }
            p.Start();
            Environment.Exit(0);
            return "1";
        }

        public object BeepEx(object frequency, object duration)
        {
            Console.Beep(int.Parse(frequency.ToString()), int.Parse(duration.ToString()));
            return 1;
        }

        public object WriteLine(object line)
        {
            Program.ConWrLine(line.ToString());
            return 1;
        }

        public object Exit()
        {
            Environment.Exit(0);
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

        public object Times(string howManyTimes, string command, string infoVariable = "for")
        {
            for (int a = 0; a < int.Parse(howManyTimes.ToString()); a++)
            {
                Cmd.CmdParams[infoVariable.ToString()] = a.ToString();
                Cmd.Process(Cmd.Str(command.ToString()), Methods, Labels);
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

        public object UseTextTest(string method)
        {
            System.Reflection.MethodInfo mth = this.GetType().GetMethod(method);
            if (mth == null) return "null";
            var Args = mth.GetParameters();

            ConsoleColor ca = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Program.ConWrite("USE: ");
            Console.ForegroundColor = ConsoleColor.White;
            if (method == "ifvar") method = "if";
            if (method == "Times") method = "for";
            Program.ConWrite(method + " ");
            for (int i = 0;i<Args.Length;i++)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                string arg = "[" + Args[i].Name;
                if (Args[i].DefaultValue.ToString().Length >= 1) arg += " = " + Args[i].DefaultValue;
                Program.ConWrite(arg + "]");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                if (i != (Args.Length-1)) Program.ConWrite(Cmd.CmdParams["splitArgs"]);
            }
            Program.ConWrLine("");
            Console.ForegroundColor = ca;
            return "1";
        }

        public object GetMethod(string methodName)
        {
            //if(methodName.Length == 0)
            //{
            //    Program.ConWrLine(UseTextTest("GetMethod"));
            //    return "0";
            //}
            System.Reflection.MethodInfo mth = this.GetType().GetMethod(methodName);
            if (mth == null) return "0";
            var Args = mth.GetParameters();
            Program.ConWrLine("Method: " + mth.Name);
            for (int i = 0; i < Args.Length; i++)
            {
                Program.ConWrLine("[Argument " + i + "] " + Args[i].Name + " = " + Args[i].DefaultValue);
            }
            return "1";
        }
        public object dir(string directory = "null")
        {
            ConsoleColor co = Console.ForegroundColor;
            string c = directory;
            if(c.ToLower().StartsWith("null")) c = ReturnCurrentDirectory().ToString();
            Console.ForegroundColor = ConsoleColor.White;
            Program.ConWrLine(" ");
            Program.ConWrLine("   Directory: " + c);
            try
            {
                var dirs = Directory.EnumerateDirectories(c);
                var files = Directory.EnumerateFiles(c);
                Console.ForegroundColor = ConsoleColor.Gray;
                Program.ConWrLine("   " + files.Count<string>() + " files, " + dirs.Count<string>() + " directories");
                Program.ConWrLine(" ");
                DirectoryInfo i = null;
                foreach (var dir in dirs)
                {
                    i = new DirectoryInfo(dir);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Program.ConWrLine("<DIR>\t\t" + i.Name);
                }

                FileInfo fi = null;
                foreach (var f in files)
                {
                    fi = new FileInfo(f);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    if (fi.Extension.ToLower() == ".es") Console.ForegroundColor = ConsoleColor.Green;
                    if (fi.Extension.ToLower() == ".exe") Console.ForegroundColor = ConsoleColor.DarkCyan;
                    if (fi.Extension.ToLower() == ".bat" || fi.Extension.ToLower() == ".cmd") Console.ForegroundColor = ConsoleColor.Gray;

                    string size = (fi.Length / 1000) + " KB";
                    if ((fi.Length / 1000) <= 0)
                        size = fi.Length + " B";
                    Program.ConWrLine("<" + fi.Extension + ">\t" + size + "\t" + fi.Name);
                }
            }
            catch (Exception ex)
            {
                Program.ConWrLine("ERROR: " + ex.Message);
                return "0";
            }
            Console.ForegroundColor = co;
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
            switch(n)
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

        public object doc(string topic = "FirstHelp")
        {
            return GetDocumentation(topic);
        }

        public object man(string topic = "FirstHelp")
        {
            return GetDocumentation(topic);
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
                foreach(var v in ConsoleColors)
                {
                    if(foregroundColor.ToLower() == v.Key.ToLower())
                    {
                        cc = GetConsoleColor(v.Value);
                        break;
                    }
                }
            }
            Console.ForegroundColor = cc;
            Program.ScriptColor = cc;
            return "1";
        }

        public object color(string foregroundColor = "15") { return SetColor(foregroundColor); }

        public object Help()
        {
            Program.ConWrLine("To get help type: GetDocumentation");
            Program.ConWrLine("Internet connection is required.");
            return "1";
        }

        public object update()
        {
            return UpdateProgram();
        }



        public object cat(string filename) { return ReadFile(filename); }

        public object ReadFile(string filename)
        {
            StreamReader r = new StreamReader(filename);
            string a = r.ReadToEnd();
            r.Dispose();
            return a;
        }
        public object UpdateProgram()
        {
            echo("Downloading latest ESCRIPT...");
            WebClient w = new WebClient();
            w.DownloadFile("https://raw.githubusercontent.com/feel-the-dz3n/escript/master/UpdateFiles/escript-latest.exe", "escript-update.exe");
            echo("Starting escript.exe -install");
            Process.Start(".\\escript-update.exe", "-close -install");
            return "1";
        }
        public object WriteFile(string filename, string data, string notClearFile)
        {
            string a = "";
            if(notClearFile.StartsWith("1"))
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

        public object StartProgram(string fileName, string arguments = "", string waitForExit = "0")
        {
            Process a = new Process();
            a.StartInfo.FileName = fileName;
            if (!arguments.ToLower().StartsWith("null")) a.StartInfo.Arguments = arguments;

            try
            {
                a.Start();
                if (waitForExit.ToLower().StartsWith("1")) a.WaitForExit();
            }
            catch (Exception ex)
            {
                Program.ConWrLine(ex.Message);
                return 0;
            }

            return "1";
        }

        public object start(string fileName, string arguments = "", string waitForExit = "0")
        {
            return StartProgram(fileName, arguments, waitForExit);
        }


        public object GetDocumentation(string topic = "FirstHelp")
        {
            WebClient client = new WebClient();
            string url = "https://raw.githubusercontent.com/feel-the-dz3n/escript/master/documentation/" + topic + ".txt";
            string text = client.DownloadString(url); 
            if(text.StartsWith("REDIRECT:"))
            {
                return GetDocumentation(text.Replace("REDIRECT:", ""));
            }
            echo(text);
            return "1";
        }

        public object ver() { return Version(); }
        public object winver() { return OSVersion(); }
        public object msg(string caption, string text, string icon, string type) { return ShowMessageBox(caption, text, icon, type); }

        public object cls() { return Clear(); }

        public object Clear()
        {
            Console.Clear();
            return "1";
        }

        public object TestMethod(string a1 = "Default Argument 1", string a2 = "Default Argument 2", string a3 = "Default Argument 3")
        {
            Program.ConWrLine("a1 : " + a1);
            Program.ConWrLine("a2 : " + a2);
            Program.ConWrLine("a3 : " + a3);
            return "1";
        }

        public object ShowMessageBox(string caption, string text, string icon, string type)
        {
            var r = MessageBox.Show(text.ToString(), caption.ToString(), GetMsgBoxBtns(int.Parse(type.ToString())), GetMsgBoxIcon(int.Parse(icon.ToString())));
            return r.ToString();
        }
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

        public object HideConsole()
        {
            var handle = GetConsoleWindow();
            return ShowWindow(handle, SW_HIDE).ToString();
        }

        public object ShowConsole()
        {
            var handle = GetConsoleWindow();
            return ShowWindow(handle, SW_SHOW).ToString();
        }

        public object GetConsoleWindowHandle()
        {
            return GetConsoleWindow().ToString();
        }

        public object MinimizeConsole()
        {
            var handle = GetConsoleWindow();
            return ShowWindow(handle, SW_SHOWMINIMIZED).ToString();
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

        public object Version()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public object OSVersion()
        {
            return Environment.OSVersion.VersionString;
        }

        public object ReturnOSVersion()
        {
            return OSVersion();
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

            // foreach (var v in ConsoleColors) Program.ConWrLine(String.Format("[{0}] {1}", v.Value, v.Key));
        }
    }
}
