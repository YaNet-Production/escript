using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace escript
{
    public static class ConsoleTextBox
    {

        public static string Show(string caption, string text)
        {
            string result = "OK";
            ConsoleColor s = EConsole.ForegroundColor;
            //Console.Clear();
            EConsole.ForegroundColor = ConsoleColor.White;
            EConsole.WriteLine("");
            EConsole.ForegroundColor = ConsoleColor.Cyan;
            EConsole.WriteLine(caption);
            EConsole.Write(ConsoleMessageBox.GetTest(caption));
            EConsole.WriteLine("");
            EConsole.ForegroundColor = ConsoleColor.White;
            EConsole.WriteLine(text);

            EConsole.ForegroundColor = ConsoleColor.Gray;
            EConsole.WriteLine("");

            Variables.Set("dialogInput", Console.ReadLine());

            EConsole.WriteLine("");
            EConsole.WriteLine("");
            EConsole.ForegroundColor = s;
            return result;
        }

    }

    public static class ConsoleMessageBox
    {
        public static string GetTest(string text)
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < text.Length; i++) b.Append("=");
            return b.ToString();
        }

        public static string Show(string caption, string text, int icon, int buttons)
        {
            string result = "0";
            ConsoleColor s = EConsole.ForegroundColor;
            //Console.Clear();
            EConsole.ForegroundColor = ConsoleColor.White;
            EConsole.WriteLine("");
            EConsole.ForegroundColor = ConsoleColor.Cyan;
            if (icon == 3) EConsole.ForegroundColor = ConsoleColor.Red;
            if (icon == 2 || icon == 4) EConsole.ForegroundColor = ConsoleColor.Yellow;
            EConsole.WriteLine(caption);
            EConsole.Write(GetTest(caption));
            EConsole.WriteLine("");
            EConsole.ForegroundColor = ConsoleColor.White;
            EConsole.WriteLine(text);

            EConsole.ForegroundColor = ConsoleColor.Gray;
            EConsole.WriteLine("");
            if (buttons == 0)
            {
                EConsole.Write("   OK [Enter] ");
                while (true)
                {
                    var key = EConsole.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        result = "OK";
                        break;
                    }
                }
            }
            else if (buttons == 1)
            {
                EConsole.Write("   OK [Enter] | Cancel [C] ");
                while (true)
                {
                    var key = EConsole.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        result = "OK";
                        break;
                    }
                    else if (key.Key == ConsoleKey.C)
                    {
                        result = "Cancel";
                        break;
                    }
                }
            }
            else if (buttons == 2)
            {
                EConsole.Write("   Yes [Y] | No [N] ");
                while (true)
                {
                    var key = EConsole.ReadKey(true);
                    if (key.Key == ConsoleKey.Y)
                    {
                        result = "Yes";
                        break;
                    }
                    else if (key.Key == ConsoleKey.N)
                    {
                        result = "No";
                        break;
                    }
                }
            }
            else if (buttons == 3)
            {
                EConsole.Write("   Yes [Y] | No [N] | Cancel [C] ");
                while (true)
                {
                    var key = EConsole.ReadKey(true);
                    if (key.Key == ConsoleKey.Y)
                    {
                        result = "Yes";
                        break;
                    }
                    else if (key.Key == ConsoleKey.N)
                    {
                        result = "No";
                        break;
                    }
                    else if (key.Key == ConsoleKey.C)
                    {
                        result = "Cancel";
                        break;
                    }
                }
            }
            else if (buttons == 4)
            {
                EConsole.Write("   Retry [R] | Cancel [C] ");
                while (true)
                {
                    var key = EConsole.ReadKey(true);
                    if (key.Key == ConsoleKey.R)
                    {
                        result = "Retry";
                        break;
                    }
                    else if (key.Key == ConsoleKey.C)
                    {
                        result = "Cancel";
                        break;
                    }
                }
            }
            else if (buttons == 5)
            {
                EConsole.Write("   Abort [A] | Retry [R] | Ignore [I] ");
                while (true)
                {
                    var key = EConsole.ReadKey(true);
                    if (key.Key == ConsoleKey.R)
                    {
                        result = "Retry";
                        break;
                    }
                    else if (key.Key == ConsoleKey.I)
                    {
                        result = "Ignore";
                        break;
                    }
                    else if (key.Key == ConsoleKey.A)
                    {
                        result = "Abort";
                        break;
                    }
                }
            }
            else
            {
                EConsole.WriteLine("");
                EConsole.Write("   ERROR: Unknown keys");
            }
            EConsole.WriteLine("");
            EConsole.WriteLine("");
            EConsole.ForegroundColor = s;
            return result;
        }
    }
}
