#if ESCRIPT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmdSharp
{
    public class EMethod
    {
        public List<string> Code = new List<string>();
        public string Name = "NULL";
        public List<EMethodArgument> Arguments = new List<EMethodArgument>();
        public List<string> Options = new List<string>();

        public string GetCode()
        {
            StringBuilder b = new StringBuilder();
            for(int i =0; i< Code.Count; i++)
            {
                b.AppendLine(Code[i]);
            }
            return b.ToString();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (var arg in Arguments)
            {
                builder.Append(arg.Name + " = " + arg.DefaultValue);
            }

            return builder.ToString();
        }
    }
    public class EMethodArgument
    {
        public string Name;
        public string DefaultValue = "null";

    }
}
#endif