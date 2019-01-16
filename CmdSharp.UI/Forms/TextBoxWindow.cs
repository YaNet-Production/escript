using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CmdSharp
{
    public partial class TextBoxWindow : Form
    {
        public TextBoxWindow()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Version cVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            labelVersion.Text = "ESCRIPT " + cVer.Major + "." + cVer.Minor;
            MinimumSize = this.Size;
            MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        public void SetText(string text)
        {

            string[] n = text.Split('\n');
            int maximumSymbol = 0;
            int maximumLines = n.Length;
            for (int i = 0; i < n.Length; i++)
            {
                if (n[i].Length > maximumSymbol) maximumSymbol = n[i].Length;
            }

            int height = (maximumLines * 18) + 150;
            int width = (maximumSymbol * 8) + 100;

            Program.Debug("maximumSymbol = " + maximumSymbol + ", width = " + width);
            Program.Debug("maximumLines = " + maximumLines + ", height = " + height);

            //if (height > Size.Height) Size = new Size(Size.Width, height);
            //if (width > Size.Width) Size = new Size(width, Size.Height);
            
            //if (text.Length >= 200) Size = new Size((Screen.PrimaryScreen.WorkingArea.Width/2), (Screen.PrimaryScreen.WorkingArea.Height - 80));
            label1.Text = text;
        }

        public string GetInput()
        {
            return WinBox.Text;
        }

        public void NoLines()
        {
            AcceptButton = null;
            WinBox.Multiline = false;
        }

        public void SetCaption(string text)
        {
            Text = text;
            labelCaption.Text = text;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void labelCaption_Click(object sender, EventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
        }

        private void TextBoxWindow_Load(object sender, EventArgs e)
        {
        }
    }
}
