using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESLib
{
    public class EFuncInvoke
    {
        escript.Functions functions;
        
        public string TestLib(string text)
        {
            functions.color("cyan");
            functions.echo("Hello from ESLib.dll.EFuncInvoke!!!!!! Your argument is: " + text);
            return "1";
        }

        public void ESLib_Initialize(escript.Functions This)
        {
            functions = This;
            functions.ShowConsole();
            Console.ForegroundColor = ConsoleColor.Green;
            functions.echo("");
            functions.echo("ESCRIPT Example Library (ESLib.dll) loaded!");
            Console.ForegroundColor = ConsoleColor.Yellow;
            functions.echo("");
            functions.echo("Be sure, that method names are unique. Example:");
            functions.echo("ESLib1_Initialize");
            functions.echo("");
            Console.ForegroundColor = ConsoleColor.Blue;
            functions.echo("Available methods in this DLL: TestLib(string text)");
            functions.echo("");
        }
    }
}
