namespace escript_api_test
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.output = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.scriptbox = new System.Windows.Forms.TextBox();
            this.processstring = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageInstance = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button7 = new System.Windows.Forms.Button();
            this.argTb = new System.Windows.Forms.TextBox();
            this.listBoxArgs = new System.Windows.Forms.ListBox();
            this.button6 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonSet = new System.Windows.Forms.Button();
            this.buttonUnset = new System.Windows.Forms.Button();
            this.tbVarValue = new System.Windows.Forms.TextBox();
            this.tbVarName = new System.Windows.Forms.TextBox();
            this.listBoxVars = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageInstance.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // output
            // 
            this.output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.output.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.output.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.output.ForeColor = System.Drawing.Color.Gainsboro;
            this.output.Location = new System.Drawing.Point(6, 12);
            this.output.Multiline = true;
            this.output.Name = "output";
            this.output.Size = new System.Drawing.Size(320, 354);
            this.output.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.output);
            this.groupBox1.Location = new System.Drawing.Point(302, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(332, 372);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Black;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.LawnGreen;
            this.button2.Location = new System.Drawing.Point(24, 40);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(231, 78);
            this.button2.TabIndex = 1;
            this.button2.Text = "create escript instance";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 320);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(278, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "new ESCode(...).RunScript();";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // scriptbox
            // 
            this.scriptbox.Location = new System.Drawing.Point(0, 0);
            this.scriptbox.Multiline = true;
            this.scriptbox.Name = "scriptbox";
            this.scriptbox.Size = new System.Drawing.Size(278, 316);
            this.scriptbox.TabIndex = 1;
            this.scriptbox.WordWrap = false;
            // 
            // processstring
            // 
            this.processstring.Location = new System.Drawing.Point(2, 150);
            this.processstring.Multiline = true;
            this.processstring.Name = "processstring";
            this.processstring.Size = new System.Drawing.Size(277, 55);
            this.processstring.TabIndex = 3;
            this.processstring.Text = "echo Cmd.Process() works fine";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(2, 224);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(274, 30);
            this.button3.TabIndex = 4;
            this.button3.Text = "do";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageInstance);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(288, 372);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPageInstance
            // 
            this.tabPageInstance.Controls.Add(this.groupBox3);
            this.tabPageInstance.Controls.Add(this.label3);
            this.tabPageInstance.Controls.Add(this.label1);
            this.tabPageInstance.Controls.Add(this.button2);
            this.tabPageInstance.Location = new System.Drawing.Point(4, 22);
            this.tabPageInstance.Name = "tabPageInstance";
            this.tabPageInstance.Size = new System.Drawing.Size(280, 346);
            this.tabPageInstance.TabIndex = 2;
            this.tabPageInstance.Text = "Create ESCRIPT instance";
            this.tabPageInstance.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button7);
            this.groupBox3.Controls.Add(this.argTb);
            this.groupBox3.Controls.Add(this.listBoxArgs);
            this.groupBox3.Controls.Add(this.button6);
            this.groupBox3.Location = new System.Drawing.Point(3, 157);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(274, 186);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Arguments";
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(6, 130);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(262, 23);
            this.button7.TabIndex = 7;
            this.button7.Text = "Remove Selected";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // argTb
            // 
            this.argTb.Location = new System.Drawing.Point(6, 159);
            this.argTb.Name = "argTb";
            this.argTb.Size = new System.Drawing.Size(226, 20);
            this.argTb.TabIndex = 6;
            this.argTb.Text = "-console";
            // 
            // listBoxArgs
            // 
            this.listBoxArgs.FormattingEnabled = true;
            this.listBoxArgs.Location = new System.Drawing.Point(6, 19);
            this.listBoxArgs.Name = "listBoxArgs";
            this.listBoxArgs.Size = new System.Drawing.Size(262, 108);
            this.listBoxArgs.TabIndex = 4;
            this.listBoxArgs.SelectedIndexChanged += new System.EventHandler(this.listBoxArgs_SelectedIndexChanged);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(238, 157);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(30, 23);
            this.button6.TabIndex = 5;
            this.button6.Text = "+";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "First of all";
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label1.Location = new System.Drawing.Point(-1, 121);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(280, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "Hold down Left Shift Key to run in Debug Mode";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.scriptbox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(280, 346);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Run Script";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.button3);
            this.tabPage2.Controls.Add(this.processstring);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(280, 346);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Cmd.Process";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label2.Location = new System.Drawing.Point(73, 208);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(146, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Use it only for ONE command";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.groupBox2);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.buttonSet);
            this.tabPage3.Controls.Add(this.buttonUnset);
            this.tabPage3.Controls.Add(this.tbVarValue);
            this.tabPage3.Controls.Add(this.tbVarName);
            this.tabPage3.Controls.Add(this.listBoxVars);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(280, 346);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "Variables";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(125, 236);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(13, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "=";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 236);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "$";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button5);
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Location = new System.Drawing.Point(3, 288);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(274, 56);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(140, 19);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(128, 23);
            this.button5.TabIndex = 1;
            this.button5.Text = "Remove all!";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(6, 19);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(128, 23);
            this.button4.TabIndex = 0;
            this.button4.Text = "VarReinit";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label4.Location = new System.Drawing.Point(67, 217);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(131, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Updating every 5 seconds";
            // 
            // buttonSet
            // 
            this.buttonSet.Location = new System.Drawing.Point(138, 259);
            this.buttonSet.Name = "buttonSet";
            this.buttonSet.Size = new System.Drawing.Size(139, 23);
            this.buttonSet.TabIndex = 4;
            this.buttonSet.Text = "set ...";
            this.buttonSet.UseVisualStyleBackColor = true;
            this.buttonSet.Click += new System.EventHandler(this.buttonSet_Click);
            // 
            // buttonUnset
            // 
            this.buttonUnset.Location = new System.Drawing.Point(3, 259);
            this.buttonUnset.Name = "buttonUnset";
            this.buttonUnset.Size = new System.Drawing.Size(134, 23);
            this.buttonUnset.TabIndex = 3;
            this.buttonUnset.Text = "unset ...";
            this.buttonUnset.UseVisualStyleBackColor = true;
            this.buttonUnset.Click += new System.EventHandler(this.buttonUnset_Click);
            // 
            // tbVarValue
            // 
            this.tbVarValue.Location = new System.Drawing.Point(143, 233);
            this.tbVarValue.Name = "tbVarValue";
            this.tbVarValue.Size = new System.Drawing.Size(134, 20);
            this.tbVarValue.TabIndex = 2;
            this.tbVarValue.TextChanged += new System.EventHandler(this.tbVarValue_TextChanged);
            // 
            // tbVarName
            // 
            this.tbVarName.Location = new System.Drawing.Point(18, 233);
            this.tbVarName.Name = "tbVarName";
            this.tbVarName.Size = new System.Drawing.Size(102, 20);
            this.tbVarName.TabIndex = 1;
            this.tbVarName.TextChanged += new System.EventHandler(this.tbVarName_TextChanged);
            // 
            // listBoxVars
            // 
            this.listBoxVars.Dock = System.Windows.Forms.DockStyle.Top;
            this.listBoxVars.FormattingEnabled = true;
            this.listBoxVars.Location = new System.Drawing.Point(0, 0);
            this.listBoxVars.Name = "listBoxVars";
            this.listBoxVars.Size = new System.Drawing.Size(280, 212);
            this.listBoxVars.TabIndex = 0;
            this.listBoxVars.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 399);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "ESCRIPT API TEST";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPageInstance.ResumeLayout(false);
            this.tabPageInstance.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox output;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox scriptbox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox processstring;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageInstance;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button buttonSet;
        private System.Windows.Forms.Button buttonUnset;
        private System.Windows.Forms.TextBox tbVarValue;
        private System.Windows.Forms.TextBox tbVarName;
        private System.Windows.Forms.ListBox listBoxVars;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox argTb;
        private System.Windows.Forms.ListBox listBoxArgs;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
    }
}

