using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CmdSharp
{
    public class APICore
    {
        private object Instance = null;
        private MethodInfo ConsoleWriteLineEvent;
        private MethodInfo ConsoleWriteEvent;

        public APICore(object thisInstance, MethodInfo WriteLine, MethodInfo Write)
        {
            Instance = thisInstance;
            ConsoleWriteLineEvent = WriteLine;
            ConsoleWriteEvent = Write;
        }

        public void InvokeWrite(object data)
        {
            ConsoleWriteEvent.Invoke(Instance, new object[] { data });
        }

        public void InvokeWriteLine(object data)
        {
            ConsoleWriteLineEvent.Invoke(Instance, new object[] { data });
        }
    }
}
