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
    public partial class ChoiceWindow : Form
    {
        private DialogResult result = DialogResult.Cancel;

        public ChoiceWindow()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
            
            labelVersion.Text = "CmdSharp";
        }
        
        public void SetButtons(ChoiceWndButton[] ButtonsArray, ChoiceWndButton Default = null)
        {
            foreach(var btn in ButtonsArray)
            {
                if (btn.Text == null)
                    continue;

                RadioButton r = new RadioButton();
                r.Text = btn.Text;
                r.Tag = new KeyValuePair<ChoiceWndButton, object>(btn, btn.Tag);

                r.Margin = new Padding(0);

                flowLayoutPanel1.Controls.Add(r);

                if (btn == Default)
                {
                    r.Checked = true;
                    flowLayoutPanel1.ScrollControlIntoView(r);
                }
            }
        }

        public void SetText(string text)
        {
            label1.Text = text;
        }

        public UI.Forms.DialogAnswer GetAnswer()
        {
            string value = "";
            object tag = null;
            ChoiceWndButton button = null;
            
            foreach (RadioButton btn in flowLayoutPanel1.Controls)
            {
                if (btn.Checked)
                {
                    value = btn.Text;
                    tag = ((KeyValuePair<ChoiceWndButton, object>)btn.Tag).Value;
                    button = ((KeyValuePair<ChoiceWndButton, object>)btn.Tag).Key;
                    break;
                }
            }
            return new UI.Forms.DialogAnswer(value, result, tag) { ChoiceWndButton = button };
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
            result = DialogResult.OK;
        }

        private void TextBoxWindow_Load(object sender, EventArgs e)
        {

        }
    }
    public class ChoiceWndButton
    {
        public string Text = "";
        public object Tag = null;

        public ChoiceWndButton() { }

        public ChoiceWndButton(string text, object tag = null)
        {
            Text = text;
            Tag = tag;
        }
    }
}
