using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CmdSharp
{
    public static class EConsole
    {
        // TODO: Cross-platform things
        private const UInt32 StdOutputHandle = 0xFFFFFFF5;
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(UInt32 nStdHandle);
        [DllImport("kernel32.dll")]
        private static extern void SetStdHandle(UInt32 nStdHandle, IntPtr handle);
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        public static bool IsGUI
        {
            get
            {
                if (cWnd == null)
                    return false;
                else
                    return true;
            }
        }

        public static IntPtr Handle
        {
            get
            {
                if (cWnd != null)
                    return cWnd.Handle;
                else
                    return GetConsoleWindow();
            }
        }

        public static CustomConsoleWindow cWnd = null;
        private static bool WaitingForForm = false;
        private static string _title = "ESCRIPT";
        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            internal short X;
            internal short Y;

            internal COORD(short x, short y)
            {
                X = x;
                Y = y;
            }
        }
        private const int STD_OUTPUT_HANDLE = -11;
        private const int TMPF_TRUETYPE = 4;
        private const int LF_FACESIZE = 32;
        private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        public static IntPtr cWndHandle = IntPtr.Zero;
        static IntPtr currentStdout = IntPtr.Zero;
        private const int MY_CODE_PAGE = 866;

        public static bool IsConsoleCreated = false;

        public static void CreateConsole()
        {
            if (IsConsoleCreated)
                return;

            bool UseSystem = !Variables.GetBool("useCustomConsole");

            if (!UseSystem)
            {
                WaitingForForm = true;

                new Thread(delegate ()
                {
                    if (cWnd == null)
                    {
                        cWnd = new CustomConsoleWindow();
                        cWnd.ShowDialog();
                    }
                }).Start();

                while (cWnd == null)
                {
                    Thread.Sleep(10);
                }

                IsConsoleCreated = true;
                WaitingForForm = false;
            }
            else
            {
                AllocConsole();
                Console.CancelKeyPress += Console_CancelKeyPress;

                // stdout's handle seems to always be equal to 7
                IntPtr defaultStdout = new IntPtr(7);
                currentStdout = GetStdHandle(StdOutputHandle);

                if (currentStdout != defaultStdout)
                    // reset stdout
                    SetStdHandle(StdOutputHandle, defaultStdout);

                IsConsoleCreated = true;

                try
                {
                    // reopen stdout
                    TextWriter writer = new StreamWriter(Console.OpenStandardOutput())
                    { AutoFlush = true };
                    Console.SetOut(writer);
                    Console.OutputEncoding = System.Text.Encoding.UTF8;
                }
                catch (Exception ex)
                {
                    EConsole.WriteLine("Can't set Console Output Encoding: " + ex.ToString(), true, ConsoleColor.Red);
                }

                try
                {
                    TextReader reader = new StreamReader(Console.OpenStandardInput());
                    Console.SetIn(reader);
                    Console.InputEncoding = System.Text.Encoding.Unicode;
                }
                catch (Exception ex)
                {
                    EConsole.WriteLine("Can't set Console Input Encoding: " + ex.ToString(), true, ConsoleColor.Red);
                }

            }

            PrintIntro();
            Version cVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            Title = String.Format("CmdSharp {0}.{1}", cVer.Major, cVer.Minor);

        }

        public static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (!GlobalVars.StopProgram)
            {
                EConsole.WriteLine(" ");
                EConsole.WriteLine(" [!] Cancel Key Pressed, performing last action before Break");
                EConsole.WriteLine(" ");
                GlobalVars.StopProgram = true;
                if (e != null) e.Cancel = true;
            }
            else
            {
                EConsole.WriteLine(" [!] Cancel Key Pressed, StopProgram is true, performing default action");
                if (e == null) Environment.Exit(0);
            }
        }

        public static void PrintIntro()
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string eVer = version.ToString();
            string buildInfo = "";
            if (version.Revision != 0)
            {
                switch (version.Revision)
                {
                    default: { buildInfo = "UNSTABLE VERSION"; break; }
                    case 1: { buildInfo = "BETA VERSION (only for testing)"; break; }
                }
            }

            EConsole.ForegroundColor = ConsoleColor.Green;




            Version cVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            EConsole.WriteLine(" ");

            EConsole.Write($"   cmd# {cVer.Major}.{cVer.Minor}", ConsoleColor.Green);

            switch (EnvironmentManager.Edition)
            {
                case EnvironmentManager.ESEditions.Core:
                    {
                        EConsole.WriteLine(" Core", ConsoleColor.Magenta);
                        break;
                    }
                case EnvironmentManager.ESEditions.Standard:
                    {
                        EConsole.WriteLine(" Standard", ConsoleColor.White);
                        break;
                    }
            }
            
            EConsole.WriteLine("   Copyright (C) Dz3n 2017-2019", ConsoleColor.Gray);
            EConsole.Write("   Build " + version.Build + " ", ConsoleColor.DarkGray);

            EConsole.WriteLine(buildInfo, ConsoleColor.Yellow);

            if (cVer.Revision != 0)
                EConsole.WriteLine("   Type \"update\" to install the latest stable version", true, ConsoleColor.DarkYellow);

            EConsole.WriteLine("");
            EConsole.WriteLine("   OS: " + Environment.OSVersion.VersionString, ConsoleColor.DarkGray);
            EConsole.WriteLine("   https://github.com/feel-the-dz3n/escript | https://discord.gg/jXcjuqv", ConsoleColor.DarkGray);
            EConsole.WriteLine("   https://vk.com/dz3n.escript", ConsoleColor.DarkGray);
            EConsole.WriteLine("");

        }

        public static ConsoleColor ForegroundColor
        {
            get { return Console.ForegroundColor; }
            set
            {
                Console.ForegroundColor = value;

                if (cWnd != null)
                {
                    if (WaitingForForm)
                        while (WaitingForForm == true)
                        {
                            Thread.Sleep(10);
                        }

                    cWnd.ForegroundColor = ColorFromConsoleColor(value);
                }
            }
        }

        public static ConsoleColor BackgroundColor
        {
            get
            {
                if (IsConsoleCreated)
                {
                    if (cWnd == null)
                        return Console.BackgroundColor;
                }

                return ConsoleColor.Black;
            }
            set
            {
                if (IsConsoleCreated)
                {
                    if (cWnd == null)
                        Console.BackgroundColor = value;
                    else
                    {
                        if (WaitingForForm)
                            while (WaitingForForm == true)
                            {
                                Thread.Sleep(10);
                            }
                        cWnd.BackgroundColor = ColorFromConsoleColor(value, true);
                    }
                }
            }
        }

        private static System.Drawing.Color ColorFromConsoleColor(ConsoleColor console, bool isBack = false)
        {
            switch (console)
            {
                case ConsoleColor.DarkBlue: return System.Drawing.Color.FromArgb(0, 64, 128);
                case ConsoleColor.DarkCyan: return System.Drawing.Color.FromArgb(0, 151, 151);
                case ConsoleColor.DarkGray: return System.Drawing.Color.FromArgb(132, 132, 132);
                case ConsoleColor.DarkGreen: return System.Drawing.Color.FromArgb(49, 175, 12);
                case ConsoleColor.DarkMagenta: return System.Drawing.Color.FromArgb(147, 0, 73);
                case ConsoleColor.DarkRed: return System.Drawing.Color.FromArgb(182, 73, 73);
                case ConsoleColor.DarkYellow: return System.Drawing.Color.FromArgb(197, 172, 75);

                case ConsoleColor.Blue: return System.Drawing.Color.FromArgb(60, 157, 255);
                case ConsoleColor.Cyan: return System.Drawing.Color.FromArgb(0, 214, 244);
                case ConsoleColor.Gray: return System.Drawing.Color.FromArgb(200, 200, 200);
                case ConsoleColor.Green: return System.Drawing.Color.FromArgb(92, 241, 61);
                case ConsoleColor.Magenta: return System.Drawing.Color.FromArgb(225, 0, 113);
                case ConsoleColor.Red: return System.Drawing.Color.FromArgb(255, 45, 45);
                case ConsoleColor.Yellow: return System.Drawing.Color.FromArgb(255, 255, 17);

                case ConsoleColor.Black: return System.Drawing.Color.Black;
                case ConsoleColor.White: return System.Drawing.Color.White;

                default: return System.Drawing.Color.FromName(console.ToString());
            }
        }

        public static void WriteLine(object text, ConsoleColor color)
        {
            WriteLine(text, true, color);
        }

        public static void WriteLine(object text, bool log = true, ConsoleColor color = ConsoleColor.Black)
        {
            if (CoreMain.log != null && log)
                CoreMain.log.WriteLine("[" + DateTime.Now.ToString() + "] " + text);

            if (GlobalVars.UsingAPI)
                GlobalVars.API.InvokeWriteLine(text);

            if (IsConsoleCreated)
            {
                ConsoleColor old = EConsole.ForegroundColor;

                if (color != ConsoleColor.Black)
                    EConsole.ForegroundColor = color;

                if (cWnd != null)
                {
                    if (WaitingForForm) while (WaitingForForm == true) { Thread.Sleep(10); }
                    cWnd.Invoke(new Action(delegate
                    {
                        cWnd.WriteLine(text, ColorFromConsoleColor(color));
                    }));
                }
                else
                    Console.WriteLine(text);

                if (color != ConsoleColor.Black) EConsole.ForegroundColor = old;

            }
        }

        public static void Write(object text, ConsoleColor color = ConsoleColor.Black)
        {
            if (CoreMain.log != null)
                CoreMain.log.Write(text);

            if (GlobalVars.UsingAPI)
                GlobalVars.API.InvokeWrite(text);

            if (IsConsoleCreated)
            {
                ConsoleColor old = EConsole.ForegroundColor;

                if (color != ConsoleColor.Black) EConsole.ForegroundColor = color;

                if (cWnd != null)
                {
                    if (WaitingForForm) while (WaitingForForm == true) { Thread.Sleep(10); }
                    cWnd.Invoke(new Action(delegate
                    {
                        cWnd.Write(text);
                    }));
                }
                else
                    Console.Write(text);

                if (color != ConsoleColor.Black)
                    EConsole.ForegroundColor = old;
            }
        }

        public static string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;

                if (IsConsoleCreated && cWnd == null)
                    Console.Title = value;
                else if (IsConsoleCreated && cWnd != null)
                    cWnd.Text = value;
            }
        }

        public static void Clear()
        {
            if (!IsConsoleCreated)
                CreateConsole();

            if (cWnd != null)
            {
                cWnd.Clear();
            }
            else
                Console.Clear();
        }

        public static string ReadLine()
        {
            if (!IsConsoleCreated)
                CreateConsole();

            if (cWnd != null)
            {
                if (WaitingForForm) while (WaitingForForm == true) { Thread.Sleep(10); }
                string cr = cWnd.ReadLine();
                return cr;
            }
            else
                return Console.ReadLine();
        }
        
        public static string ReadKey(bool hideKey = false)
        {
            if (!IsConsoleCreated)
                CreateConsole();

            if (IsGUI)
            {
                if (WaitingForForm) while (WaitingForForm == true) { Thread.Sleep(10); }
                char result = '-';
                try
                {
                    result = cWnd.ReadKey()[0];
                }
                catch { }

                System.Windows.Forms.Keys oneKeyKey = cWnd.oneKeyKey;
                return oneKeyKey.ToString();
            }
            else
                return Console.ReadKey(hideKey).Key.ToString();
        }
    }
}
