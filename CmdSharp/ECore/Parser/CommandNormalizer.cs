using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CmdSharp.Parser
{
    public static class CommandNormalizer
    {
        public static string ConvertObjectToStringCode(object obj)
        {
            // Input: (string) a
            // Output: "a"
            if (obj == null)
                return "null";

            if (obj.GetType() == typeof(string))
                return "\"" + obj + "\"";

            if (obj.GetType() == typeof(char))
                return "'" + obj + "'";

            if (obj.GetType() == typeof(float))
                return obj.ToString().Replace(",", ".") + "f";

            if (obj.GetType() == typeof(bool))
                return obj.ToString().ToLower();

            return obj.ToString();
        }
        enum CodeCommentTypes
        {
            None,
            MultiLineComment,
            SingleLineComment,
        }
        public enum CodeIgnoreType
        {
            None,
            String,
            Char
        }

        private static void LogIgnoreType(CodeIgnoreType IgnoreType, char c = '\0', int i = -1)
        {
            if (!Variables.GetBool("parserDebug"))
                return;

            string idx = "";

            if (c != '\0')
                idx += ", char: " + c ;

            if (i != -1)
                idx += ", at " + i.ToString();

            Debug.Log("Parser", $"({new StackTrace().GetFrame(2).GetMethod().Name}) IgnoreType = {IgnoreType.ToString()}{idx}", ConsoleColor.DarkBlue); 
        }

        public static CodeIgnoreType CheckForIgnoreType(CodeIgnoreType current, char c, int i = -1)
        {
            CodeIgnoreType IgnoreType = current;

            if (c == '"')
            {
                if (IgnoreType == CodeIgnoreType.None)
                    IgnoreType = CodeIgnoreType.String;
                else if (IgnoreType == CodeIgnoreType.String)
                    IgnoreType = CodeIgnoreType.None;

                LogIgnoreType(IgnoreType, c, i);
            }


            if (c == "'"[0] && IgnoreType == CodeIgnoreType.None)
            {
                IgnoreType = CodeIgnoreType.Char;
                LogIgnoreType(IgnoreType, c, i);
            }
            else if (c == "'"[0] && IgnoreType == CodeIgnoreType.Char)
            {
                IgnoreType = CodeIgnoreType.None;
                LogIgnoreType(IgnoreType, c, i);
            }

            return IgnoreType;
        }

        public static string[] NormalizeInvokeLines(string Command)
        {
            // Input example:
            // string a = "BB;;\\n;;;\n; ;;;BB";int test =KEK(a);      FAK(test); if (a == b){
            // kek }else{}

            // Output example:
            // [0] string a = "BB;;\n;;;
            //                 ; ;;;BB";
            // [1] int test =KEK(a, ";");
            // [2]       FAK(test);
            // [3] if (a == b){kek}else{} (TODO)

            // TODO:
            // string formatting (\n,\r...)

            // RULES:
            // Only strings can contain ; char which will be ignored
            // Strings must be in " and always has startign " and ending "

            // How to avoid ; in strings? :thinking:
            // SPLIT IT MANUALLY

            List<string> result = new List<string>();

            string CurrentCmd = "";
            CodeCommentTypes CommentType = CodeCommentTypes.None;
            CodeIgnoreType IgnoreType = CodeIgnoreType.None;
            for (int i = 0; i < Command.Length; i++)
            {
                char c = Command[i];

                IgnoreType = CheckForIgnoreType(IgnoreType, c, i);

                // Code comment remover START
                if (IgnoreType == CodeIgnoreType.None && Command.Length >= 2)
                {
                    if (Command[i] == '/' && Command[i + 1] == '*' && CommentType == CodeCommentTypes.None)
                    {
                        CommentType = CodeCommentTypes.MultiLineComment;
                    }
                    if (Command[i] == '/' && Command[i + 1] == '/' && CommentType == CodeCommentTypes.None)
                    {
                        CommentType = CodeCommentTypes.SingleLineComment;
                    }
                }

                // Parsed string writer
                if (CommentType == CodeCommentTypes.None)
                    CurrentCmd += c;

                // Code comment remover END
                if (IgnoreType == CodeIgnoreType.None && Command.Length > i && i >= 1)
                {
                    if (Command[i] == '/' && Command[i - 1] == '*' && CommentType == CodeCommentTypes.MultiLineComment)
                    {
                        CommentType = CodeCommentTypes.None;
                    }
                }

                // Code line finalizer
                if (IgnoreType == CodeIgnoreType.None)
                {
                    if (c == '\r' || c == '\n')
                    { // replace this with spaces, they will be removed later
                        c = ' ';
                        // disable singleline comment at the end of the line
                        if (CommentType == CodeCommentTypes.SingleLineComment)
                            CommentType = CodeCommentTypes.None;
                    }

                    // disable single line comment at the end of the command
                    if (i == Command.Length && CommentType == CodeCommentTypes.SingleLineComment)
                        CommentType = CodeCommentTypes.None;

                    if (c == ';' && CommentType == CodeCommentTypes.None) // command end/split thing
                    {
                        result.Add(CurrentCmd);
                        CommentType = CodeCommentTypes.None;
                        IgnoreType = CodeIgnoreType.None;
                        CurrentCmd = ""; // new command;
                    }
                }
            }

            //if (result.Count == 0)
            //    throw new Exceptions.ParserException("Can't normalize commands");

            return result.ToArray();
        }
    }
}
