using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmdSharp.UI.Forms
{
    public class DialogAnswer
    {
        public string Value;
        public object ValueTag = null;
        public ChoiceWndButton ChoiceWndButton = null;
        public System.Windows.Forms.DialogResult Result;

        public DialogAnswer(string value, System.Windows.Forms.DialogResult result, object tag = null)
        {
            Value = value;
            ValueTag = tag;
            Result = result;
        }
    }
}
