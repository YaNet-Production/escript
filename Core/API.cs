using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace escript
{
    public static class API
    {
        public delegate void WriteEventMethod(object text);
        public static event WriteEventMethod WriteEvent;

        public delegate void WriteLineEventMethod(object text);
        public static event WriteLineEventMethod WriteLineEvent;

        public static void Start()
        {
            Start(new List<string>());
        }

        public static void Start(List<string> args)
        {
            GlobalVars.UsingAPI = true;
            Program.Main(args.ToArray());
        }
        
        internal static void WriteLine(object text) { WriteLineEvent(text); }
        internal static void Write(object text) { WriteEvent(text); }
    }
}
