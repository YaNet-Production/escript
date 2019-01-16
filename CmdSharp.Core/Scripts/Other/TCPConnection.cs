using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace CmdSharp
{
    class TCPConnection
    {
        public static bool IsConnected = false;
        private static StreamWriter swSender;
        private static StreamReader srReceiver;
        private static TcpClient tcpServer;
        private static Dictionary<string, int> currentLabels;

        public static int Connect(string IP, int PORT, Dictionary<string, int> lbls = null)
        {
            currentLabels = lbls;
            try
            {
                if (IsConnected) return 0;

                tcpServer = new TcpClient();
                tcpServer.Connect(IP, PORT);
                IsConnected = true;
                new Thread(msg).Start();

                throw new NotImplementedException();

                //for (int i = 0; i < GlobalVars.Methods.Count; i++)
                //{
                //    var m = GlobalVars.Methods[i];
                //    if (m.Name == Variables.GetValue("TCP_triggerConnected"))
                //    {
                //        Cmd.Process(m.Name);
                //    }
                //}
                //return 1;
            }
            catch { return -1; }
        }


        public static void Send(object text)
        {
            swSender = new StreamWriter(tcpServer.GetStream());
            swSender.WriteLine(text.ToString());
            swSender.Flush();
            //Llog("MW-Online", "To server: " + text.ToString());
        }
        public static void Disconnect()
        {
            IsConnected = false;
            throw new NotImplementedException();
            //for (int i = 0; i < GlobalVars.Methods.Count; i++)
            //{
            //    var m = GlobalVars.Methods[i];
            //    if (m.Name == Variables.GetValue("TCP_triggerDisconnected"))
            //    {
            //        Cmd.Process(m.Name);
            //    }
            //}
            tcpServer.Close();
            try
            {
                swSender.Dispose();
            }
            catch { }
            try
            {
                srReceiver.Dispose();
            }
            catch { }
        }
        private static void msg()
        {
            try
            {
                srReceiver = new StreamReader(tcpServer.GetStream());
                while (true)
                {
                    string msg = srReceiver.ReadLine();
                    Variables.Set("tcpMsg", msg);
                    if (msg == null) throw new Exception("TCP says null");
                    throw new NotImplementedException();
                    //for (int i = 0; i < GlobalVars.Methods.Count; i++)
                    //{
                    //    var m = GlobalVars.Methods[i];
                    //    if (m.Name == Variables.GetValue("TCP_triggerMsg"))
                    //    {
                    //        Cmd.Process(m.Name);
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                EConsole.WriteLine("TCP ERROR: " + ex.Message); Disconnect();
            }
        }

    }
}
