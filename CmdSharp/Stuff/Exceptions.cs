using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmdSharp.Exceptions
{
    public class InvokeMethodNotFoundException : Exception
    {
        public InvokeMethodNotFoundException(string message) : base(message)
        {
        }
    }

    public class VariableInitializerException : Exception
    {
        public VariableInitializerException(string message) : base(message)
        {
        }
    }

    public class VariableSetException : Exception
    {
        public VariableSetException(string message) : base(message)
        {
        }
    }

    public class ParserException : Exception
    {
        public ParserException(string message) : base(message)
        {
        }
    }
}
