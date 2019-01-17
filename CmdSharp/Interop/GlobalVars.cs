using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
#if !IsCore
using System.Windows.Forms;
#endif

namespace CmdSharp
{
    public static class GlobalVars
    {
        
#if IsCore
        public static bool IsCore = true;
#else
        public static bool IsCore = false;
#endif
        private static APICore _api = null;
        public static APICore API { get { return _api; } set { _api = value; if (value == null) UsingAPI = false; else UsingAPI = true; } }
        public static bool UsingAPI = false;
        public static bool DebugWhenFormingEnv = false;
        public static bool IgnoreRunAsAdmin = false;
        public static bool StopProgram = false;
        public static bool IsCompiledScript = false;
        public const string StuffServer = "https://raw.githubusercontent.com/feel-the-dz3n/escript-stuff/master/";
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static bool StringToBool(string text)
        {
            if (text == "1" || text.ToLower() == "true") return true;
            return false;
        }

        public static string[] RemoveEntry(string[] source, string entry)
        {
            List<string> list = source.ToList();
            list.Remove(entry);
            return list.ToArray();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr BeginUpdateResource(string pFileName,
            [MarshalAs(UnmanagedType.Bool)]bool bDeleteExistingResources);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UpdateResource(IntPtr hUpdate, IntPtr lpType, string lpName, ushort wLanguage,
            IntPtr lpData, uint cbData);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr FindResource(IntPtr hModule, string lpName, IntPtr lpType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LockResource(IntPtr hResData);
        [DllImport("user32.dll")]
        public static extern int LookupIconIdFromDirectoryEx(byte[] presbits, bool fIcon, int cxDesired, int cyDesired, uint Flags);
        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconFromResourceEx(byte[] pbIconBits, uint cbIconBits, bool fIcon, uint dwVersion, int cxDesired, int cyDesired, uint uFlags);

        public static List<ImportedLibInfo> LoadedLibs = new List<ImportedLibInfo>();
        public static List<Thread> UserThreads = new List<Thread>();

#if !IsCore


        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        public const int SW_SHOWMINIMIZED = 2;

        [DllImport("winmm.dll")]
        public static extern uint mciSendString(
            string lpstrCommand, string lpstrReturnString, uint uReturnLength, uint hWndCallback);


        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey); // Keys enumeration

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        public const int WM_VSCROLL = 277;
        public const int SB_PAGEBOTTOM = 7;



        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(System.Int32 vKey);

#endif
        public static FileInfo GetAboutMe()
        {
            string me = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return new FileInfo(me);

        }

        public static string[] ObjectListToStringArray(List<object> list)
        {
            return ObjectListToStringList(list).ToArray();
        }

        public static List<string> ObjectListToStringList(List<object> list)
        {
            List<string> a = new List<string>();
            foreach (var i in list) a.Add(i.ToString());
            return a;
        }

        public static string[] RemoveEntries(string[] source, int start, int end)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < source.Length; i++)
            {
                if (i >= start && i <= end) continue;

                list.Add(source[i]);
            }
            return list.ToArray();
        }


        public static string RemoveDirtFromString(string code)
        {
            string result = code;
            //for (int i = 0; i < code.Length; i++) EConsole.Write(code[i].ToString());
            try
            {
                if (result[0] == ' ') result = RemoveDirtFromString(result.Remove(0, 1));
                else if (result[0] == '\t') result = RemoveDirtFromString(result.Remove(0, 1));
                else if (result[0] == '	') result = RemoveDirtFromString(result.Remove(0, 1));
            }
            catch (Exception ex) { }
            return result;
        }


        public static string[] RemoveDirtFromScript(string[] script)
        {
            for (int i = 0; i < script.Length; i++)
            {
                script[i] = RemoveDirtFromString(script[i]);
            }
            return script;
        }

        public static string RemoveDirtFromCode(string code)
        {
            string[] script = code.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);//ESCode.SplitCode(code);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < script.Length; i++)
            {
                result.AppendLine(RemoveDirtFromString(script[i]));
            }
            return result.ToString();
        }

        // https://stackoverflow.com/questions/11743160/how-do-i-encode-and-decode-a-base64-string

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

#if !IsCore
    public static class Keyboard
    {
        [Flags]
        public enum KeyStates
        {
            None = 0,
            Down = 1,
            Toggled = 2
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern short GetKeyState(int keyCode);

    

        public static KeyStates GetKeyStateX(Keys key)
        {
            KeyStates state = KeyStates.None;

            short retVal = GetKeyState((int)key);

            //If the high-order bit is 1, the key is down
            //otherwise, it is up.
            if ((retVal & 0x8000) == 0x8000)
                state |= KeyStates.Down;

            //If the low-order bit is 1, the key is toggled.
            if ((retVal & 1) == 1)
                state |= KeyStates.Toggled;

            return state;
        }

        public static KeyStates GetKeyStateFromCode(int key)
        {
            return GetKeyStateX((Keys)key);
        }

        public static Keys KeyCodeToKey(int key)
        {
            //foreach (System.Int32 i in Enum.GetValues(typeof(Keys)))
            //{
            //    if (i == key) return (Keys)i;
            //}
            //return Keys.None;
            return (Keys)key;
        }

        public static bool IsKeyDown(Keys key)
        {
            return KeyStates.Down == (GetKeyStateX(key) & KeyStates.Down);
        }

        public static bool IsKeyToggled(Keys key)
        {
            return KeyStates.Toggled == (GetKeyStateX(key) & KeyStates.Toggled);
        }
    }
#endif


}
