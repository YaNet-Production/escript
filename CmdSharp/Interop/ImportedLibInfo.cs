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

        public ImportedLibInfo(string fileName, string ClassName, object[] Arguments = null)
        {
            if (Arguments == null)
                Arguments = new object[] { };

            assembly = Assembly.LoadFile(new FileInfo(fileName).FullName);
            types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.Name == ClassName)
                {
                    funcType = type;
                    classInstance = Activator.CreateInstance(funcType, Arguments);
                    break;
                    //foreach (var m in type.GetMethods())
                    //{
                    //    if (m.Name == MethodName)
                    //    {
                    //        m.Invoke(classInstance, Arguments);
                    //    }
                    //}

                }
            }
        }
    }
}
