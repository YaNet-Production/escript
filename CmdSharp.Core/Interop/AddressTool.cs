using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CmdSharp
{
    public static class AddressTool
    {
        public static IntPtr Get(object obj)
        {
            GCHandle handle = GCHandle.Alloc(obj, GCHandleType.Weak);
            IntPtr pointer = GCHandle.ToIntPtr(handle);
            handle.Free();
            return pointer;
        }
    }
}
