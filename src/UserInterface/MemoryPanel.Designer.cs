namespace Rca301Emulator.UserInterface
{
    partial class MemoryPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.numColumns = new System.Windows.Forms.NumericUpDown();
            this.lNumColumns = new System.Windows.Forms.Label();
            this.cbViewType = new System.Windows.Forms.ComboBox();
            this.lViewType = new System.Windows.Forms.Label();
            this.cbAddressType = new System.Windows.Forms.ComboBox();
            this.lAddressType = new System.Windows.Forms.Label();
            this.numAddress = new System.Windows.Forms.NumericUpDown();
            this.lAddress = new System.Windows.Forms.Label();
            this.btnGoToAddress = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.fpTools = new System.Windows.Forms.FlowLayoutPanel();
            this.mMemory = new Rca301Emulator.UserInterface.MemoryControl();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setNextInstructionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.numColumns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAddress)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.fpTools.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // numColumns
            // 
            this.numColumns.Location = new System.Drawing.Point(59, 13);
            this.numColumns.Margin = new System.Windows.Forms.Padding(3, 13, 3, 3);
            this.numColumns.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numColumns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numColumns.Name = "numColumns";
            this.numColumns.Size = new System.Drawing.Size(40, 20);
            this.numColumns.TabIndex = 5;
            this.numColumns.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numColumns.ValueChanged += new System.EventHandler(this.OnColumnsChanged);
            // 
            // lNumColumns
            // 
            this.lNumColumns.AutoSize = true;
            this.lNumColumns.Location = new System.Drawing.Point(3, 13);
            this.lNumColumns.Margin = new System.Windows.Forms.Padding(3, 13, 3, 0);
            this.lNumColumns.Name = "lNumColumns";
            this.lNumColumns.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lNumColumns.Size = new System.Drawing.Size(50, 15);
            this.lNumColumns.TabIndex = 6;
            this.lNumColumns.Text = "Columns:";
            // 
            // cbViewType
            // 
            this.cbViewType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbViewType.FormattingEnabled = true;
            this.cbViewType.Location = new System.Drawing.Point(333, 13);
            this.cbViewType.Margin = new System.Windows.Forms.Padding(3, 13, 3, 3);
            this.cbViewType.Name = "cbViewType";
            this.cbViewType.Size = new System.Drawing.Size(90, 21);
            this.cbViewType.TabIndex = 7;
            this.cbViewType.SelectedValueChanged += new System.EventHandler(this.OnViewTypeChanged);
            // 
            // lViewType
            // 
            this.lViewType.AutoSize = true;
            this.lViewType.Location = new System.Drawing.Point(280, 13);
            this.lViewType.Margin = new System.Windows.Forms.Padding(3, 13, 3, 0);
            this.lViewType.Name = "lViewType";
            this.lViewType.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lViewType.Size = new System.Drawing.Size(47, 15);
            this.lViewType.TabIndex = 8;
            this.lViewType.Text = "View as:";
            // 
            // cbAddressType
            // 
            this.cbAddressType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAddressType.FormattingEnabled = true;
            this.cbAddressType.Location = new System.Drawing.Point(184, 13);
            this.cbAddressType.Margin = new System.Windows.Forms.Padding(3, 13, 3, 3);
            this.cbAddressType.Name = "cbAddressType";
            this.cbAddressType.Size = new System.Drawing.Size(90, 21);
            this.cbAddressType.TabIndex = 9;
            this.cbAddressType.SelectedValueChanged += new System.EventHandler(this.OnAddressTypeChanged);
            // 
            // lAddressType
            // 
            this.lAddressType.AutoSize = true;
            this.lAddressType.Location = new System.Drawing.Point(105, 13);
            this.lAddressType.Margin = new System.Windows.Forms.Padding(3, 13, 3, 0);
            this.lAddressType.Name = "lAddressType";
            this.lAddressType.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lAddressType.Size = new System.Drawing.Size(73, 15);
            this.lAddressType.TabIndex = 10;
            this.lAddressType.Text = "Addresses as:";
            // 
            // numAddress
            // 
            this.numAddress.Location = new System.Drawing.Point(62, 11);
            this.numAddress.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numAddress.Name = "numAddress";
            this.numAddress.Size = new System.Drawing.Size(60, 20);
            this.numAddress.TabIndex = 11;
            // 
            // lAddress
            // 
            this.lAddress.AutoSize = true;
            this.lAddress.Location = new System.Drawing.Point(8, 13);
            this.lAddress.Name = "lAddress";
            this.lAddress.Size = new System.Drawing.Size(48, 13);
            this.lAddress.TabIndex = 12;
            this.lAddress.Text = "Address:";
            // 
            // btnGoToAddress
            // 
            this.btnGoToAddress.Location = new System.Drawing.Point(131, 11);
            this.btnGoToAddress.Name = "btnGoToAddress";
            this.btnGoToAddress.Size = new System.Drawing.Size(91, 23);
            this.btnGoToAddress.TabIndex = 13;
            this.btnGoToAddress.Text = "Go to Address";
            this.btnGoToAddress.UseVisualStyleBackColor = true;
            this.btnGoToAddress.Click += new System.EventHandler(this.OnGoToAddressClicked);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnGoToAddress);
            this.groupBox1.Controls.Add(this.lAddress);
            this.groupBox1.Controls.Add(this.numAddress);
            this.groupBox1.Location = new System.Drawing.Point(429, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(229, 42);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            // 
            // fpTools
            // 
            this.fpTools.AutoSize = true;
            this.fpTools.Controls.Add(this.lNumColumns);
            this.fpTools.Controls.Add(this.numColumns);
            this.fpTools.Controls.Add(this.lAddressType);
            this.fpTools.Controls.Add(this.cbAddressType);
            this.fpTools.Controls.Add(this.lViewType);
            this.fpTools.Controls.Add(this.cbViewType);
            this.fpTools.Controls.Add(this.groupBox1);
            this.fpTools.Dock = System.Windows.Forms.DockStyle.Top;
            this.fpTools.Location = new System.Drawing.Point(0, 0);
            this.fpTools.Name = "fpTools";
            this.fpTools.Size = new System.Drawing.Size(680, 48);
            this.fpTools.TabIndex = 15;
            this.fpTools.SizeChanged += new System.EventHandler(this.OnToolsPanelSizeChanged);
            // 
            // mMemory
            // 
            this.mMemory.CellPadding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.mMemory.ContextMenuStrip = this.contextMenuStrip;
            this.mMemory.Location = new System.Drawing.Point(41, 90);
            this.mMemory.Margin = new System.Windows.Forms.Padding(10);
            this.mMemory.Name = "mMemory";
            this.mMemory.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.mMemory.Size = new System.Drawing.Size(617, 310);
            this.mMemory.TabIndex = 0;
            this.mMemory.Scroll += new System.Windows.Forms.ScrollEventHandler(this.OnMemoryScroll);
            this.mMemory.CurrentAddressPositionChanged += new System.EventHandler(this.OnCurrentAddressChanged);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setNextInstructionToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(178, 26);
            // 
            // setNextInstructionToolStripMenuItem
            // 
            this.setNextInstructionToolStripMenuItem.Name = "setNextInstructionToolStripMenuItem";
            this.setNextInstructionToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.setNextInstructionToolStripMenuItem.Text = "Set Next Instruction";
            this.setNextInstructionToolStripMenuItem.Click += new System.EventHandler(this.OnSetNextInstructionClick);
            // 
            // MemoryPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fpTools);
            this.Controls.Add(this.mMemory);
            this.Name = "MemoryPanel";
            this.Size = new System.Drawing.Size(680, 419);
            ((System.ComponentModel.ISupportInitialize)(this.numColumns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAddress)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.fpTools.ResumeLayout(false);
            this.fpTools.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Rca301Emulator.UserInterface.MemoryControl mMemory;
        private System.Windows.Forms.NumericUpDown numColumns;
        private System.Windows.Forms.Label lNumColumns;
        private System.Windows.Forms.ComboBox cbViewType;
        private System.Windows.Forms.Label lViewType;
        private System.Windows.Forms.ComboBox cbAddressType;
        private System.Windows.Forms.Label lAddressType;
        private System.Windows.Forms.NumericUpDown numAddress;
        private System.Windows.Forms.Label lAddress;
        private System.Windows.Forms.Button btnGoToAddress;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FlowLayoutPanel fpTools;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem setNextInstructionToolStripMenuItem;
    }
}
