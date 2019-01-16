using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CmdSharp
{

    public class ImportedLibInfo
    {
        public Assembly assembly;
        public Type[] types;
        public Type funcType = null;
        public object classInstance = null;

        public ImportedLibInfo(string fileName, Functions functions, string text = "EFuncInvoke")
        {
            assembly = Assembly.LoadFile(new FileInfo(fileName).FullName);
            types = assembly.GetTypes();
            foreach (var type in types)
            {
                Program.Debug("[" + type.Module.Name + "] Type: " + type.FullName);
                if (type.Name.Contains(text))
                {
                    Program.Debug("[" + type.Module.Name + "] Found: " + type.FullName, ConsoleColor.DarkGreen);
                    funcType = type;
                    classInstance = Activator.CreateInstance(funcType);

                    foreach (var m in type.GetMethods())
                    {
                        if (m.Name.Contains("Initialize"))
                        {
                            Program.Debug("[" + type.Module.Name + "] Calling: " + m.Name, ConsoleColor.DarkYellow);
                            m.Invoke(classInstance, new object[] { functions });
                        }
                    }

                }
            }
        }
    }
}
