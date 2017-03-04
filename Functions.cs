using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace escript
{
    class Functions
    {
        public List<EMethod> Methods;
        public Dictionary<string, int> Labels;
        public Functions(List<EMethod> methods, Dictionary<string, int> labels)
        {
            Methods = methods;
            Labels = labels;
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
            Cmd.Process(el.ToString(), Methods, Labels);
            return 0;
        }

        /*
         * From https://blog.foolsoft.ru/c-funkcii-post-i-get-zaprosov-gotovye-k-primen/
         */
        public string web_get(object Url, object Data)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(Url + "?" + Data);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.Stream stream = resp.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();
            return Out;
        }
        
        public string web_post(object Url, object Data)
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
            Console.Beep(int.Parse(p1.ToString()), int.Parse(p2.ToString()));
            return 1;
        }

        public object WriteLine(object line)
        {
            Program.ConWrLine(line.ToString());
            return 1;
        }

        public object Exit(object code)
        {
            Environment.Exit(int.Parse(code.ToString()));
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
            return "1";
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

        public object GetDocumentation(object method, object language)
        {
            WebClient client = new WebClient();
            string url = "https://raw.githubusercontent.com/YaNet-Production/escript/master/documentation/" + language + "/" + method + ".txt";
            string text = client.DownloadString(url); 
            echo(text);
            return text;
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
 
        public object ReturnProcessorCount()
        {
           return Environment.ProcessorCount.ToString();
        }

        public object ReturnOSVersion()
        {
            return Environment.OSVersion.VersionString;
        }
    }
}
