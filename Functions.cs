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
    class Functions
    {
        #region imports
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        const int SW_SHOWMINIMIZED = 2;
        #endregion

        public List<EMethod> Methods;
        public Dictionary<string, int> Labels;


        //public Functions(List<EMethod> methods, Dictionary<string, int> labels)
        //{
        //    Methods = methods;
        //    Labels = labels;
        //}

        public object Compare(object one, object p, object two, object ifOkDoThis, object el)
        {
            return ifvar(one, p, two, ifOkDoThis, el);
        }

        public object ifvar(object one, object p, object two, object ifOkDoThis, object el)
        {
            try
            {
                int first = int.Parse(one.ToString()), second = int.Parse(two.ToString());
                if (p.ToString() == "<") if (first < second) { Cmd.Process(ifOkDoThis.ToString(), Methods, Labels); return 1; } 
                if (p.ToString() == ">") if (first > second) { Cmd.Process(ifOkDoThis.ToString(), Methods, Labels); return 1; }
                if (p.ToString() == "<=") if (first <= second) { Cmd.Process(ifOkDoThis.ToString(), Methods, Labels); return 1; }
                if (p.ToString() == ">=") if (first >= second) { Cmd.Process(ifOkDoThis.ToString(), Methods, Labels); return 1; }
                if (p.ToString() == "==") if (first == second) { Cmd.Process(ifOkDoThis.ToString(), Methods, Labels); return 1; }
            }
            catch
            {
                if (p.ToString() == "==")
                {
                    if (one.ToString() == two.ToString()) { Cmd.Process(ifOkDoThis.ToString(), Methods, Labels); return 1; }
                }
                else return -1;
            }
            if(!el.ToString().ToLower().StartsWith("null")) Cmd.Process(el.ToString(), Methods, Labels);
            return 0;
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

        public object Beep_Advanced(object p1, object p2)
        {
            Program.ConWrLine("WARNING: Beep_Advanced is BeepEx now");
            return BeepEx(p1, p2);
        }

        public object InstallESCRIPT()
        {
            Process p = new Process();
            p.StartInfo.FileName = ".\\escript.exe";
            p.StartInfo.Arguments = "-installNext";
            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                p.StartInfo.Verb = "runas";
            }
            p.Start();
            Environment.Exit(0);
            return "1";
        }

        public object BeepEx(object p1, object p2)
        {
            Console.Beep(int.Parse(p1.ToString()), int.Parse(p2.ToString()));
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

        public object Times(object t, object vr, object cmd)
        {
            for (int a = 0; a < int.Parse(t.ToString()); a++)
            {
                Cmd.CmdParams[vr.ToString()] = a.ToString();
                Cmd.Process(Cmd.Str(cmd.ToString()), Methods, Labels);
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

        public object FileCopy(string source, string destFname, string overwriteFile)
        {
            bool ov = false;
            if (overwriteFile.StartsWith("1")) ov = true;
            try
            {
                File.Copy(source, destFname, ov);
                return "1";
            }
            catch (Exception ex) { throw ex; }
        }

        public object FileMove(string source, string destFname)
        {
            try
            {
                File.Move(source, destFname);
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

        public object FileRename(string source, string result)
        {
            return FileMove(source, result);
        }

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

        public object SetColor(string fg)
        {
            if (!fg.ToLower().StartsWith("null"))
            {
                var cc = GetConsoleColor(int.Parse(fg));
                Console.ForegroundColor = cc;
                Program.ScriptColor = cc;
            }
            return "1";
        }

        public object Help()
        {
            Program.ConWrLine("To get documentation use: GetDocumentation FirstHelp");
            Program.ConWrLine("Internet connection is required.");
            return "1";
        }

        public object cat(string filename) { return ReadFile(filename); }

        public object ReadFile(string filename)
        {
            StreamReader r = new StreamReader(filename);
            string a = r.ReadToEnd();
            r.Dispose();
            return a;
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

        public object Start(string fileName, string arguments, string waitForExit)
        {
            Process a = new Process();
            a.StartInfo.FileName = fileName;
            if (!arguments.ToLower().StartsWith("null")) a.StartInfo.Arguments = arguments;

            a.Start();
            if(waitForExit.ToLower().StartsWith("1")) a.WaitForExit();

            return "1";
        }


        public object GetDocumentation(object method)
        {
            WebClient client = new WebClient();
            string url = "https://raw.githubusercontent.com/feel-the-dz3n/escript/master/documentation/" + method + ".txt";
            string text = client.DownloadString(url); 
            if(text.StartsWith("REDIRECT:"))
            {
                return GetDocumentation(text.Replace("REDIRECT:", ""));
            }
            echo(text);
            return text;
        }

        public object cls() { return Clear(); }

        public object Clear()
        {
            Console.Clear();
            return "1";
        }

        public object ShowMessageBox(object caption, object text, object icon, object type)
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
        

        public object Sleep(string time)
        {
            Thread.Sleep(int.Parse(time));
            return "1";
        }

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
    }
}
