using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmdSharpProgram
{
    class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            CmdSharp.CoreMain.Init(args);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            CmdSharp.CoreMain.BachokMessage((Exception)e.ExceptionObject, true);
        }
    }
}
