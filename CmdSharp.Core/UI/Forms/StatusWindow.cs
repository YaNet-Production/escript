using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CmdSharp
{
    public partial class StatusWindow : Form
    {
        public TextBox ibox = new TextBox();
        public StatusWindow()
        {
            InitializeComponent();
            ibox.Visible = false;
            ibox.TabStop = false;
            Controls.Add(ibox);
            Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);

            Version cVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            labelVersion.Text = "ESCRIPT " + cVer.Major + "." + cVer.Minor;
        }


        public void SetProgress(int v)
        {
            if (v == -1) { progress1.Visible = false; return; }
            if (v == -2) { progress1.Visible = true; progress1.Style = ProgressBarStyle.Marquee; return; }
            progress1.Visible = true;
            progress1.Style = ProgressBarStyle.Continuous;
            progress1.Value = v;
        }

        public void SetText(string caption, string text)
        {
            //if (text.Length >= 200) Size = new Size((Screen.PrimaryScreen.WorkingArea.Width/2), (Screen.PrimaryScreen.WorkingArea.Height - 80));

            if (InvokeRequired) { Invoke(new Action(delegate { SetText(caption, text); })); return; }

            Version cVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Text = caption;// + " | " + "ESCRIPT " + cVer.Major + "." + cVer.Minor;
            labelCaption.Text = caption;
            label1.Text = text;
            label1.Select(0, 0);
            ActiveControl = labelCaption;
        }

        private void StatusWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            hideBtn_Click(null, null);
        }

        private void hideBtn_Click(object sender, EventArgs e)
        {
            Hide();
            //Cmd.Process("HideStatus");
        }

        private void label1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }
    }
}
