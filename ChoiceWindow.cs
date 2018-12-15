using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace escript
{
    public partial class ChoiceWindow : Form
    {
        public ChoiceWindow()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Version cVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            labelVersion.Text = "ESCRIPT " + cVer.Major + "." + cVer.Minor;
        }
        
        public void SetButt(string[] bts, string Default = "null")
        {
            foreach(var btn in bts)
            {
                if (btn == "null") continue;
                RadioButton r = new RadioButton();
                r.Text = btn;
                r.Margin = new Padding(0);
                flowLayoutPanel1.Controls.Add(r);
                if (Default == r.Text)
                {
                    r.Checked = true;
                    flowLayoutPanel1.ScrollControlIntoView(r);
                }
            }
        }

        public void SetText(string text)
        {
            
            //if (text.Length >= 200) Size = new Size((Screen.PrimaryScreen.WorkingArea.Width/2), (Screen.PrimaryScreen.WorkingArea.Height - 80));
            label1.Text = text;
        }

        public string Get()
        {
            foreach(RadioButton btn in flowLayoutPanel1.Controls)
            {
                if (btn.Checked) return btn.Text;
            }
            return "";
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
