using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CmdSharp.Stuff.Extensions
{
    public static class Extensions
    {
        public static string JoinArray(this string[] array, string separator = "")
        {
            string result = "";
            foreach (var a in array)
                result += separator + array;
            return result;
        }
        public static char ReadChar(this Stream stream)
        {
            var rb = stream.ReadByte();
            //Program.Debug("[ReadChar] Byte: " + rb);

            if (rb == -1)
                return '\0';


            var result = Convert.ToChar(rb);
            //Debugger.Log(0, "Debug", result.ToString());
            return result;
        }
    }
}
