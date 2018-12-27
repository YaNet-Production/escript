using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace escript
{
    public class EResult
    {
        public class Default
        {
            public static object Name = "DEFAULT";
            public static object Error = "DEFAULT_ERROR";

            public static string Get(string text = "")
            {
                if (text.Length <= 0) return Name.ToString();

                return Name + "_" + Convert(text);
            }

            public static string GetError(string text = "")
            {
                if (text.Length <= 0) return Error.ToString();

                return Error + "_" + Convert(text);
            }

            public static string Convert(string source)
            {
                return source.ToUpper().Replace(" ", "_").Replace(",", "").Replace(".", "");
            }
        }

        public class Cmd : Default
        {
            public new static object Name = "CMD";
            public static object Done = "CMD_DONE";
            public static object Fail = "CMD_FAIL";
            public new static object Error = "CMD_ERROR";
            public static object NotFound = "CMD_NOT_FOUND";

            public new static string Get(string text = "")
            {
                if (text.Length <= 0) return Name.ToString();

                return Name + "_" + Convert(text);
            }

            public new static string GetError(string text = "")
            {
                if (text.Length <= 0) return Error.ToString();

                return Error + "_" + Convert(text);
            }


        }
        public class ESCRIPT : Default
        {
            public new static object Name = "ESCRIPT";
            public new static object Error = "ESCRIPT_ERROR";
            public new static string Get(string text = "")
            {
                if (text.Length <= 0) return Name.ToString();

                return Name + "_" + Convert(text);
            }

            public new static string GetError(string text = "")
            {
                if (text.Length <= 0) return Error.ToString();

                return Error + "_" + Convert(text);
            }
        }
        public class Syntax : Default
        {
            public new static object Name = "SYNTAX";
            public new static object Error = "SYNTAX_ERROR";
            public new static string Get(string text = "")
            {
                if (text.Length <= 0) return Name.ToString();

                return Name + "_" + Convert(text);
            }

            public new static string GetError(string text = "")
            {
                if (text.Length <= 0) return Error.ToString();

                return Error + "_" + Convert(text);
            }
        }

    }
}
