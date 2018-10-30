using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace escript
{
    public class API
    {
        public string output = "";
        public delegate void MethodContainer();
        public event MethodContainer onNewOutput;

        //public Dictionary<string, string> Variables = new Dictionary<string, string>();
        public int Initialize()
        {
            Program.Init(new string[] { }, false, this);
            return 1;
        }

        public void trigger(string text)
        {
            output = text;
            onNewOutput();
        }

        public object Process(string command)
        {
            return Cmd.Process(Cmd.Str(command), null, null);
        }
    }
}
