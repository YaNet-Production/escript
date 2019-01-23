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

        public ImportedLibInfo(Assembly a)
        {
            assembly = a;
        }

        public ImportedLibInfo(string fileName, object[] Arguments = null)
        {
            if (Arguments == null)
                Arguments = new object[] { };

            assembly = Assembly.LoadFile(new FileInfo(fileName).FullName);
        }
    }
}
