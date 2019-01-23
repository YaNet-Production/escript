#if IsCore
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms : IDisposable
{
    public class Control
    {
        public int Left = 0, Top = 0;
        public string Text = "";

        public Control()
        {

        }
    }

    public class Form
    {
        public int Left = 0, Top = 0;
        public string Text = "";
        public List<Control> Controls = new List<Control>();

        public Form()
        {

        }
    }
}
#endif
