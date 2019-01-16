using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmdSharp
{
    public static class API
    {
        public delegate void WriteEventMethod(object text);
        public static event WriteEventMethod WriteEvent;

        public delegate void WriteLineEventMethod(object text);
        public static event WriteLineEventMethod WriteLineEvent;

        public static void Start()
        {
            Start(new string[] { });
        }

        public static void Start(string[] args)
        {
            GlobalVars.UsingAPI = true;
            CoreMain.Init(args);
        }
        
        internal static void WriteLine(object text) { WriteLineEvent(text); }
        internal static void Write(object text) { WriteEvent(text); }
    }
}
