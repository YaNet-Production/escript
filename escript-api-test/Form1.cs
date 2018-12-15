using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace escript_api_test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            API_WriteLineEvent("First of all, you should create ESCRIPT instance. P.S.: Hold down Left Shift key to enter Debug Mode");
            escript.API.WriteEvent += API_WriteEvent;
            escript.API.WriteLineEvent += API_WriteLineEvent;
            scriptbox.Text =
@"//
// ESCRIPT TEST
//

func Main
{
    Method
}

func Method
{
    echo
    echo ESCRIPT is working!
    echo {DateTimeNow}
    echo
}";
            button2.Focus();
            new Thread(UpdateVars).Start();
        }

        private void API_WriteLineEvent(object text)
        {
            output.AppendText(text.ToString() + Environment.NewLine);
        }

        private void API_WriteEvent(object text)
        {
            output.AppendText(text.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string code = scriptbox.Text;
            new escript.ESCode(escript.ESCode.SplitCode(code)).RunScript();
        }

        public List<string> NormList(ListBox.ObjectCollection a)
        {
            List<string> r = new List<string>();
            foreach(var i in a)
            {
                r.Add(i.ToString());
            }
            return r;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            API_WriteLineEvent(" ");
            API_WriteLineEvent("Starting ESCRIPT...");
            button2.Visible = false;
            escript.API.Start(NormList(listBoxArgs.Items));
            output.Clear();
            escript.Cmd.Process("PrintIntro");
            tabControl1.SelectTab(tabPage1);
            tabPageInstance.Dispose();
        }

        public void UpdateVars()
        {
            while(true)
            {
                try
                {
                    listBoxVars.BeginInvoke(new Action(delegate
                    {

                        try
                        {
                            listBoxVars.Items.Clear();
                            for (int i = 0; i < escript.Variables.VarList.Count; i++)
                            {
                                listBoxVars.Items.Add(escript.Variables.VarList[i].Name + "\t" + escript.Variables.VarList[i].Value.ToString());
                            }
                        }
                        catch { }

                    }));
                }
                catch { }
                Thread.Sleep(5000);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            escript.Cmd.Process(processstring.Text);
        }

        private void tbVarName_TextChanged(object sender, EventArgs e)
        {
        //    buttonUnset.Text = "unset " + tbVarName.Text;
        //    buttonSet.Text = "$" + tbVarName.Text + "=" + tbVarValue.Text;
        }

        private void tbVarValue_TextChanged(object sender, EventArgs e)
        {
            //tbVarName_TextChanged(sender, e);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string[] a = listBoxVars.SelectedItem.ToString().Split('\t');
                tbVarName.Text = a[0];
                tbVarValue.Text = "#" + a[1];
            }
            catch
            {

            }
        }

        private void buttonUnset_Click(object sender, EventArgs e)
        {
            escript.Cmd.Process("unset " + tbVarName.Text);
        }

        private void buttonSet_Click(object sender, EventArgs e)
        {
            escript.Cmd.Process("$" + tbVarName.Text + "=" + tbVarValue.Text);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach(var i in listBoxVars.Items)
            {
                string[] a = i.ToString().Split('\t');
                escript.Cmd.Process("unset " + a[0]);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            escript.Cmd.Process("VarReinit");
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void listBoxArgs_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                listBoxArgs.Items.Remove(listBoxArgs.SelectedItem);
            }
            catch { }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            listBoxArgs.Items.Add(argTb.Text);
            argTb.SelectAll();
        }
    }
}
