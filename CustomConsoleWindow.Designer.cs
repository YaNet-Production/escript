namespace escript
{
    partial class CustomConsoleWindow
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
            this.components = new System.ComponentModel.Container();
            this.textBoxOutput = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.gayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.colorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontSizeBox = new System.Windows.Forms.ToolStripMenuItem();
            this.cbFontSize = new System.Windows.Forms.ToolStripComboBox();
            this.fontFamilyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbFontFamily = new System.Windows.Forms.ToolStripComboBox();
            this.backColor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnChangeColor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnColorSolid = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.everythingVer = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.textBoxOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxOutput.ContextMenuStrip = this.contextMenuStrip1;
            this.textBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxOutput.EnableAutoDragDrop = true;
            this.textBoxOutput.Font = new System.Drawing.Font("Consolas", 10.35F);
            this.textBoxOutput.ForeColor = System.Drawing.Color.White;
            this.textBoxOutput.Location = new System.Drawing.Point(0, 0);
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.Size = new System.Drawing.Size(848, 481);
            this.textBoxOutput.TabIndex = 0;
            this.textBoxOutput.Text = "";
            this.textBoxOutput.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.textBoxOutput_LinkClicked);
            this.textBoxOutput.TextChanged += new System.EventHandler(this.textBoxOutput_TextChanged);
            this.textBoxOutput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxOutput_KeyDown);
            this.textBoxOutput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxOutput_KeyUp);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.gayToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator1,
            this.colorToolStripMenuItem,
            this.toolStripSeparator2,
            this.aboutToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(175, 126);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.ShortcutKeyDisplayString = "Ctrl+X";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(174, 22);
            this.toolStripMenuItem1.Text = "Cut";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // gayToolStripMenuItem
            // 
            this.gayToolStripMenuItem.Name = "gayToolStripMenuItem";
            this.gayToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.gayToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.gayToolStripMenuItem.Text = "Copy";
            this.gayToolStripMenuItem.Click += new System.EventHandler(this.gayToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(171, 6);
            // 
            // colorToolStripMenuItem
            // 
            this.colorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fontToolStripMenuItem,
            this.backColor,
            this.toolStripSeparator3,
            this.everythingVer});
            this.colorToolStripMenuItem.Name = "colorToolStripMenuItem";
            this.colorToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.colorToolStripMenuItem.Text = "Window properties";
            // 
            // fontToolStripMenuItem
            // 
            this.fontToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fontSizeBox,
            this.fontFamilyToolStripMenuItem});
            this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            this.fontToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fontToolStripMenuItem.Text = "Font";
            // 
            // fontSizeBox
            // 
            this.fontSizeBox.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbFontSize});
            this.fontSizeBox.Name = "fontSizeBox";
            this.fontSizeBox.ShortcutKeyDisplayString = "";
            this.fontSizeBox.ShowShortcutKeys = false;
            this.fontSizeBox.Size = new System.Drawing.Size(134, 22);
            this.fontSizeBox.Text = "Font size";
            // 
            // cbFontSize
            // 
            this.cbFontSize.Name = "cbFontSize";
            this.cbFontSize.Size = new System.Drawing.Size(121, 23);
            this.cbFontSize.TextChanged += new System.EventHandler(this.cbFontSize_TextChanged);
            // 
            // fontFamilyToolStripMenuItem
            // 
            this.fontFamilyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbFontFamily});
            this.fontFamilyToolStripMenuItem.Name = "fontFamilyToolStripMenuItem";
            this.fontFamilyToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.fontFamilyToolStripMenuItem.Text = "Font family";
            // 
            // cbFontFamily
            // 
            this.cbFontFamily.Name = "cbFontFamily";
            this.cbFontFamily.Size = new System.Drawing.Size(121, 23);
            this.cbFontFamily.TextChanged += new System.EventHandler(this.cbFontFamily_TextChanged);
            // 
            // backColor
            // 
            this.backColor.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnChangeColor,
            this.mnColorSolid});
            this.backColor.Name = "backColor";
            this.backColor.Size = new System.Drawing.Size(180, 22);
            this.backColor.Text = "Background Color";
            // 
            // mnChangeColor
            // 
            this.mnChangeColor.Name = "mnChangeColor";
            this.mnChangeColor.Size = new System.Drawing.Size(180, 22);
            this.mnChangeColor.Text = "Change";
            this.mnChangeColor.Click += new System.EventHandler(this.customToolStripMenuItem_Click);
            // 
            // mnColorSolid
            // 
            this.mnColorSolid.Enabled = false;
            this.mnColorSolid.Name = "mnColorSolid";
            this.mnColorSolid.Size = new System.Drawing.Size(180, 22);
            this.mnColorSolid.Text = "A";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(171, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(22, 24);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(34, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "SEND";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(177, 6);
            // 
            // everythingVer
            // 
            this.everythingVer.Enabled = false;
            this.everythingVer.Name = "everythingVer";
            this.everythingVer.Size = new System.Drawing.Size(180, 22);
            this.everythingVer.Text = ".NET Framework 4.0";
            // 
            // CustomConsoleWindow
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.ClientSize = new System.Drawing.Size(848, 481);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.textBoxOutput);
            this.Controls.Add(this.button1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "CustomConsoleWindow";
            this.Text = "ESCRIPT Console";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CustomConsoleWindow_FormClosing);
            this.Load += new System.EventHandler(this.CustomConsoleWindow_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CustomConsoleWindow_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CustomConsoleWindow_KeyUp);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox textBoxOutput;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem gayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem colorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontSizeBox;
        private System.Windows.Forms.ToolStripComboBox cbFontSize;
        private System.Windows.Forms.ToolStripMenuItem fontFamilyToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox cbFontFamily;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backColor;
        private System.Windows.Forms.ToolStripMenuItem mnChangeColor;
        private System.Windows.Forms.ToolStripMenuItem mnColorSolid;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem everythingVer;
    }
}