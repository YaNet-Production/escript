using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CmdSharp.API
{
    public class Main
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
            if (WriteLineEvent == null || WriteEvent == null)
                throw new NullReferenceException();

            MethodInfo WriteLineMethod = null, WriteMethod = null;

            foreach(var m in typeof(Main).GetMethods())
            {
                if (m.Name == "WriteLine")
                    WriteLineMethod = m;
                else if (m.Name == "Write")
                    WriteMethod = m;
            }

            GlobalVars.API = new APICore(new Main(), WriteLineMethod, WriteMethod);
            CoreMain.Init(args);
        }
        
        internal static void WriteLine(object text) { WriteLineEvent(text); }
        internal static void Write(object text) { WriteEvent(text); }
    }
}
