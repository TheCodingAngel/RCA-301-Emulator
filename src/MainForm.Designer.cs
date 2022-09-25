namespace Rca301Emulator
{
    partial class MainForm
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
            this.splitAll = new System.Windows.Forms.SplitContainer();
            this.assemblySourceContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assembleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToAssemblyLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assemblySourceLabel = new System.Windows.Forms.Label();
            this.splitRightFill = new System.Windows.Forms.SplitContainer();
            this.splitRightBottom = new System.Windows.Forms.SplitContainer();
            this.mVariablesLabel = new System.Windows.Forms.Label();
            this.mLabelsLabel = new System.Windows.Forms.Label();
            this.splitRightTop = new System.Windows.Forms.SplitContainer();
            this.lCPU = new System.Windows.Forms.Label();
            this.lOutput = new System.Windows.Forms.Label();
            this.toolsPanel = new System.Windows.Forms.Panel();
            this.btnImportMemory = new System.Windows.Forms.Button();
            this.btnExportMemory = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAssemble = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.tickPeriodEdit = new System.Windows.Forms.NumericUpDown();
            this.tickPeriodLabel = new System.Windows.Forms.Label();
            this.btnStep = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.status = new System.Windows.Forms.ToolStripStatusLabel();
            this.Reg_P = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reg_NOR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reg_N = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reg_A = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reg_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.importMemoryDialog = new System.Windows.Forms.OpenFileDialog();
            this.exportMemoryDialog = new System.Windows.Forms.SaveFileDialog();
            this.tbAssemblySource = new Rca301Emulator.UserInterface.RichTextBoxEx();
            this.mMemoryPanel = new Rca301Emulator.UserInterface.MemoryPanel();
            this.mWatchPanel = new Rca301Emulator.UserInterface.WatchPanel();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mLabelPanel = new Rca301Emulator.UserInterface.LabelsPanel();
            this.mLabelName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mLabelAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mCpuPanel = new Rca301Emulator.UserInterface.CpuPanel();
            this.regPDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.priDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.regNORDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.regNDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.regADataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.regBDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mOutputPanel = new Rca301Emulator.UserInterface.OutputPanel();
            this.cpuBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitAll)).BeginInit();
            this.splitAll.Panel1.SuspendLayout();
            this.splitAll.Panel2.SuspendLayout();
            this.splitAll.SuspendLayout();
            this.assemblySourceContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitRightFill)).BeginInit();
            this.splitRightFill.Panel1.SuspendLayout();
            this.splitRightFill.Panel2.SuspendLayout();
            this.splitRightFill.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitRightBottom)).BeginInit();
            this.splitRightBottom.Panel1.SuspendLayout();
            this.splitRightBottom.Panel2.SuspendLayout();
            this.splitRightBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitRightTop)).BeginInit();
            this.splitRightTop.Panel1.SuspendLayout();
            this.splitRightTop.Panel2.SuspendLayout();
            this.splitRightTop.SuspendLayout();
            this.toolsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tickPeriodEdit)).BeginInit();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mWatchPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mLabelPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mCpuPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cpuBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // splitAll
            // 
            this.splitAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitAll.Location = new System.Drawing.Point(0, 47);
            this.splitAll.Name = "splitAll";
            // 
            // splitAll.Panel1
            // 
            this.splitAll.Panel1.Controls.Add(this.tbAssemblySource);
            this.splitAll.Panel1.Controls.Add(this.assemblySourceLabel);
            // 
            // splitAll.Panel2
            // 
            this.splitAll.Panel2.Controls.Add(this.splitRightFill);
            this.splitAll.Panel2.Controls.Add(this.splitRightTop);
            this.splitAll.Size = new System.Drawing.Size(1245, 792);
            this.splitAll.SplitterDistance = 574;
            this.splitAll.TabIndex = 0;
            // 
            // assemblySourceContextMenu
            // 
            this.assemblySourceContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.assembleToolStripMenuItem,
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.goToAssemblyLineToolStripMenuItem});
            this.assemblySourceContextMenu.Name = "assemblySourceContextMenu";
            this.assemblySourceContextMenu.Size = new System.Drawing.Size(178, 136);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.loadToolStripMenuItem.Text = "Load (F3)";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.OnLoadClick);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.saveToolStripMenuItem.Text = "Save (F2)";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.OnSaveClick);
            // 
            // assembleToolStripMenuItem
            // 
            this.assembleToolStripMenuItem.Name = "assembleToolStripMenuItem";
            this.assembleToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.assembleToolStripMenuItem.Text = "Assemble (F9)";
            this.assembleToolStripMenuItem.Click += new System.EventHandler(this.OnAssembleClick);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.startToolStripMenuItem.Text = "Start (F5)";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.OnStartClick);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.stopToolStripMenuItem.Text = "Stop (F6)";
            this.stopToolStripMenuItem.Visible = false;
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.OnStopClick);
            // 
            // goToAssemblyLineToolStripMenuItem
            // 
            this.goToAssemblyLineToolStripMenuItem.Name = "goToAssemblyLineToolStripMenuItem";
            this.goToAssemblyLineToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.goToAssemblyLineToolStripMenuItem.Text = "Set Next Instruction";
            this.goToAssemblyLineToolStripMenuItem.Click += new System.EventHandler(this.OnGoToAssemblyLineClick);
            // 
            // assemblySourceLabel
            // 
            this.assemblySourceLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.assemblySourceLabel.Location = new System.Drawing.Point(0, 0);
            this.assemblySourceLabel.Name = "assemblySourceLabel";
            this.assemblySourceLabel.Size = new System.Drawing.Size(574, 23);
            this.assemblySourceLabel.TabIndex = 0;
            this.assemblySourceLabel.Text = "Assembly Source:";
            this.assemblySourceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitRightFill
            // 
            this.splitRightFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitRightFill.Location = new System.Drawing.Point(0, 100);
            this.splitRightFill.Name = "splitRightFill";
            this.splitRightFill.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitRightFill.Panel1
            // 
            this.splitRightFill.Panel1.Controls.Add(this.mMemoryPanel);
            // 
            // splitRightFill.Panel2
            // 
            this.splitRightFill.Panel2.Controls.Add(this.splitRightBottom);
            this.splitRightFill.Size = new System.Drawing.Size(667, 692);
            this.splitRightFill.SplitterDistance = 380;
            this.splitRightFill.TabIndex = 4;
            // 
            // splitRightBottom
            // 
            this.splitRightBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitRightBottom.Location = new System.Drawing.Point(0, 0);
            this.splitRightBottom.Name = "splitRightBottom";
            // 
            // splitRightBottom.Panel1
            // 
            this.splitRightBottom.Panel1.Controls.Add(this.mVariablesLabel);
            this.splitRightBottom.Panel1.Controls.Add(this.mWatchPanel);
            // 
            // splitRightBottom.Panel2
            // 
            this.splitRightBottom.Panel2.Controls.Add(this.mLabelsLabel);
            this.splitRightBottom.Panel2.Controls.Add(this.mLabelPanel);
            this.splitRightBottom.Size = new System.Drawing.Size(667, 308);
            this.splitRightBottom.SplitterDistance = 432;
            this.splitRightBottom.TabIndex = 3;
            // 
            // mVariablesLabel
            // 
            this.mVariablesLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.mVariablesLabel.AutoSize = true;
            this.mVariablesLabel.Location = new System.Drawing.Point(197, 9);
            this.mVariablesLabel.Name = "mVariablesLabel";
            this.mVariablesLabel.Size = new System.Drawing.Size(50, 13);
            this.mVariablesLabel.TabIndex = 2;
            this.mVariablesLabel.Text = "Variables";
            // 
            // mLabelsLabel
            // 
            this.mLabelsLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.mLabelsLabel.AutoSize = true;
            this.mLabelsLabel.Location = new System.Drawing.Point(96, 9);
            this.mLabelsLabel.Name = "mLabelsLabel";
            this.mLabelsLabel.Size = new System.Drawing.Size(38, 13);
            this.mLabelsLabel.TabIndex = 1;
            this.mLabelsLabel.Text = "Labels";
            // 
            // splitRightTop
            // 
            this.splitRightTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitRightTop.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitRightTop.Location = new System.Drawing.Point(0, 0);
            this.splitRightTop.Name = "splitRightTop";
            // 
            // splitRightTop.Panel1
            // 
            this.splitRightTop.Panel1.Controls.Add(this.lCPU);
            this.splitRightTop.Panel1.Controls.Add(this.mCpuPanel);
            // 
            // splitRightTop.Panel2
            // 
            this.splitRightTop.Panel2.Controls.Add(this.lOutput);
            this.splitRightTop.Panel2.Controls.Add(this.mOutputPanel);
            this.splitRightTop.Size = new System.Drawing.Size(667, 100);
            this.splitRightTop.SplitterDistance = 448;
            this.splitRightTop.TabIndex = 3;
            // 
            // lCPU
            // 
            this.lCPU.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lCPU.AutoSize = true;
            this.lCPU.Location = new System.Drawing.Point(220, 10);
            this.lCPU.Name = "lCPU";
            this.lCPU.Size = new System.Drawing.Size(29, 13);
            this.lCPU.TabIndex = 3;
            this.lCPU.Text = "CPU";
            // 
            // lOutput
            // 
            this.lOutput.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lOutput.AutoSize = true;
            this.lOutput.Location = new System.Drawing.Point(93, 10);
            this.lOutput.Name = "lOutput";
            this.lOutput.Size = new System.Drawing.Size(39, 13);
            this.lOutput.TabIndex = 5;
            this.lOutput.Text = "Output";
            // 
            // toolsPanel
            // 
            this.toolsPanel.Controls.Add(this.btnImportMemory);
            this.toolsPanel.Controls.Add(this.btnExportMemory);
            this.toolsPanel.Controls.Add(this.btnLoad);
            this.toolsPanel.Controls.Add(this.btnSave);
            this.toolsPanel.Controls.Add(this.btnAssemble);
            this.toolsPanel.Controls.Add(this.btnStart);
            this.toolsPanel.Controls.Add(this.btnStop);
            this.toolsPanel.Controls.Add(this.tickPeriodEdit);
            this.toolsPanel.Controls.Add(this.tickPeriodLabel);
            this.toolsPanel.Controls.Add(this.btnStep);
            this.toolsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolsPanel.Location = new System.Drawing.Point(0, 0);
            this.toolsPanel.Name = "toolsPanel";
            this.toolsPanel.Size = new System.Drawing.Size(1245, 47);
            this.toolsPanel.TabIndex = 2;
            // 
            // btnImportMemory
            // 
            this.btnImportMemory.Location = new System.Drawing.Point(614, 13);
            this.btnImportMemory.Name = "btnImportMemory";
            this.btnImportMemory.Size = new System.Drawing.Size(75, 23);
            this.btnImportMemory.TabIndex = 8;
            this.btnImportMemory.Text = "&Import (F7)";
            this.btnImportMemory.UseVisualStyleBackColor = true;
            this.btnImportMemory.Click += new System.EventHandler(this.OnImportMemoryClick);
            // 
            // btnExportMemory
            // 
            this.btnExportMemory.Location = new System.Drawing.Point(538, 13);
            this.btnExportMemory.Name = "btnExportMemory";
            this.btnExportMemory.Size = new System.Drawing.Size(75, 23);
            this.btnExportMemory.TabIndex = 7;
            this.btnExportMemory.Text = "&Export (F4)";
            this.btnExportMemory.UseVisualStyleBackColor = true;
            this.btnExportMemory.Click += new System.EventHandler(this.OnExportMemoryClick);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(13, 13);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "&Load (F3)";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.OnLoadClick);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(94, 13);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "&Save (F2)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.OnSaveClick);
            // 
            // btnAssemble
            // 
            this.btnAssemble.Location = new System.Drawing.Point(175, 13);
            this.btnAssemble.Name = "btnAssemble";
            this.btnAssemble.Size = new System.Drawing.Size(96, 23);
            this.btnAssemble.TabIndex = 2;
            this.btnAssemble.Text = "&Assemble (F9)";
            this.btnAssemble.UseVisualStyleBackColor = true;
            this.btnAssemble.Click += new System.EventHandler(this.OnAssembleClick);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(280, 13);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Sta&rt (F5)";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.OnStartClick);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(280, 13);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "Sto&p (F6)";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Visible = false;
            this.btnStop.Click += new System.EventHandler(this.OnStopClick);
            // 
            // tickPeriodEdit
            // 
            this.tickPeriodEdit.Location = new System.Drawing.Point(362, 14);
            this.tickPeriodEdit.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.tickPeriodEdit.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.tickPeriodEdit.Name = "tickPeriodEdit";
            this.tickPeriodEdit.Size = new System.Drawing.Size(58, 20);
            this.tickPeriodEdit.TabIndex = 5;
            this.tickPeriodEdit.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // tickPeriodLabel
            // 
            this.tickPeriodLabel.AutoSize = true;
            this.tickPeriodLabel.Location = new System.Drawing.Point(426, 18);
            this.tickPeriodLabel.Name = "tickPeriodLabel";
            this.tickPeriodLabel.Size = new System.Drawing.Size(20, 13);
            this.tickPeriodLabel.TabIndex = 5;
            this.tickPeriodLabel.Text = "ms";
            // 
            // btnStep
            // 
            this.btnStep.Location = new System.Drawing.Point(453, 13);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(75, 23);
            this.btnStep.TabIndex = 6;
            this.btnStep.Text = "S&tep (F8)";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.OnStepClick);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "Source 1.asm";
            this.openFileDialog.Filter = "Assembly files|*.asm|All files|*.*";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Assembly files|*.asm|All files|*.*";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 839);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1245, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(42, 17);
            this.statusLabel.Text = "Status:";
            // 
            // status
            // 
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(23, 17);
            this.status.Text = "OK";
            // 
            // Reg_P
            // 
            this.Reg_P.DataPropertyName = "Reg_P";
            this.Reg_P.HeaderText = "P";
            this.Reg_P.Name = "Reg_P";
            // 
            // Reg_NOR
            // 
            this.Reg_NOR.DataPropertyName = "Reg_NOR";
            this.Reg_NOR.HeaderText = "NOR";
            this.Reg_NOR.Name = "Reg_NOR";
            // 
            // Reg_N
            // 
            this.Reg_N.DataPropertyName = "Reg_N";
            this.Reg_N.HeaderText = "N";
            this.Reg_N.Name = "Reg_N";
            // 
            // Reg_A
            // 
            this.Reg_A.DataPropertyName = "Reg_A";
            this.Reg_A.HeaderText = "A";
            this.Reg_A.Name = "Reg_A";
            // 
            // Reg_B
            // 
            this.Reg_B.DataPropertyName = "Reg_B";
            this.Reg_B.HeaderText = "B";
            this.Reg_B.Name = "Reg_B";
            // 
            // importMemoryDialog
            // 
            this.importMemoryDialog.FileName = "Memory 1.mem";
            this.importMemoryDialog.Filter = "Export files|*.mem;*.dmp|All files|*.*";
            // 
            // exportMemoryDialog
            // 
            this.exportMemoryDialog.Filter = "Assembly files|*.mem|Dump files|*.dmp|All files|*.*";
            // 
            // tbAssemblySource
            // 
            this.tbAssemblySource.AcceptsTab = true;
            this.tbAssemblySource.ContextMenuStrip = this.assemblySourceContextMenu;
            this.tbAssemblySource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAssemblySource.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbAssemblySource.Location = new System.Drawing.Point(0, 23);
            this.tbAssemblySource.Name = "tbAssemblySource";
            this.tbAssemblySource.NumberAlignment = System.Drawing.StringAlignment.Center;
            this.tbAssemblySource.NumberBackground1 = System.Drawing.SystemColors.ControlLight;
            this.tbAssemblySource.NumberBackground2 = System.Drawing.SystemColors.Window;
            this.tbAssemblySource.NumberBorder = System.Drawing.SystemColors.ControlDark;
            this.tbAssemblySource.NumberBorderThickness = 1F;
            this.tbAssemblySource.NumberColor = System.Drawing.Color.DarkGray;
            this.tbAssemblySource.NumberFont = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbAssemblySource.NumberLeadingZeroes = false;
            this.tbAssemblySource.NumberLineCounting = Rca301Emulator.UserInterface.RichTextBoxEx.LineCounting.AsDisplayed;
            this.tbAssemblySource.NumberPadding = 2;
            this.tbAssemblySource.ShowLineNumbers = false;
            this.tbAssemblySource.Size = new System.Drawing.Size(574, 769);
            this.tbAssemblySource.TabIndex = 0;
            this.tbAssemblySource.Text = "";
            this.tbAssemblySource.WordWrap = false;
            this.tbAssemblySource.TextChanged += new System.EventHandler(this.OnAssemblySourceChanged);
            // 
            // mMemoryPanel
            // 
            this.mMemoryPanel.Columns = 16;
            this.mMemoryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mMemoryPanel.Location = new System.Drawing.Point(0, 0);
            this.mMemoryPanel.Name = "mMemoryPanel";
            this.mMemoryPanel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.mMemoryPanel.Size = new System.Drawing.Size(667, 380);
            this.mMemoryPanel.TabIndex = 0;
            this.mMemoryPanel.SetNextInstruction += new System.EventHandler<Rca301Emulator.UserInterface.EventArgsAddress>(this.OnSetNextInstruction);
            // 
            // mWatchPanel
            // 
            this.mWatchPanel.AllowUserToAddRows = false;
            this.mWatchPanel.AllowUserToDeleteRows = false;
            this.mWatchPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mWatchPanel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mWatchPanel.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this.mWatchPanel.Location = new System.Drawing.Point(3, 32);
            this.mWatchPanel.Name = "mWatchPanel";
            this.mWatchPanel.RowHeadersVisible = false;
            this.mWatchPanel.Size = new System.Drawing.Size(426, 273);
            this.mWatchPanel.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Name";
            this.dataGridViewTextBoxColumn1.HeaderText = "Variable";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Value";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "Offset";
            this.dataGridViewTextBoxColumn3.HeaderText = "Address";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "Size";
            this.dataGridViewTextBoxColumn4.HeaderText = "Size";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            // 
            // mLabelPanel
            // 
            this.mLabelPanel.AllowUserToAddRows = false;
            this.mLabelPanel.AllowUserToDeleteRows = false;
            this.mLabelPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mLabelPanel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mLabelPanel.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.mLabelName,
            this.mLabelAddress});
            this.mLabelPanel.Location = new System.Drawing.Point(3, 32);
            this.mLabelPanel.Name = "mLabelPanel";
            this.mLabelPanel.RowHeadersVisible = false;
            this.mLabelPanel.Size = new System.Drawing.Size(225, 273);
            this.mLabelPanel.TabIndex = 0;
            // 
            // mLabelName
            // 
            this.mLabelName.DataPropertyName = "Name";
            this.mLabelName.HeaderText = "Label";
            this.mLabelName.Name = "mLabelName";
            // 
            // mLabelAddress
            // 
            this.mLabelAddress.DataPropertyName = "Offset";
            this.mLabelAddress.HeaderText = "Address";
            this.mLabelAddress.Name = "mLabelAddress";
            // 
            // mCpuPanel
            // 
            this.mCpuPanel.AllowUserToAddRows = false;
            this.mCpuPanel.AllowUserToDeleteRows = false;
            this.mCpuPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mCpuPanel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mCpuPanel.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.regPDataGridViewTextBoxColumn,
            this.priDataGridViewTextBoxColumn,
            this.regNORDataGridViewTextBoxColumn,
            this.regNDataGridViewTextBoxColumn,
            this.regADataGridViewTextBoxColumn,
            this.regBDataGridViewTextBoxColumn});
            this.mCpuPanel.Location = new System.Drawing.Point(0, 33);
            this.mCpuPanel.Name = "mCpuPanel";
            this.mCpuPanel.RowHeadersVisible = false;
            this.mCpuPanel.Size = new System.Drawing.Size(445, 65);
            this.mCpuPanel.TabIndex = 2;
            // 
            // regPDataGridViewTextBoxColumn
            // 
            this.regPDataGridViewTextBoxColumn.DataPropertyName = "Reg_P";
            this.regPDataGridViewTextBoxColumn.HeaderText = "Reg_P";
            this.regPDataGridViewTextBoxColumn.Name = "regPDataGridViewTextBoxColumn";
            this.regPDataGridViewTextBoxColumn.Width = 70;
            // 
            // priDataGridViewTextBoxColumn
            // 
            this.priDataGridViewTextBoxColumn.DataPropertyName = "PRI";
            this.priDataGridViewTextBoxColumn.HeaderText = "PRI";
            this.priDataGridViewTextBoxColumn.Name = "priDataGridViewTextBoxColumn";
            this.priDataGridViewTextBoxColumn.Width = 70;
            // 
            // regNORDataGridViewTextBoxColumn
            // 
            this.regNORDataGridViewTextBoxColumn.DataPropertyName = "Reg_NOR";
            this.regNORDataGridViewTextBoxColumn.HeaderText = "Reg_NOR";
            this.regNORDataGridViewTextBoxColumn.Name = "regNORDataGridViewTextBoxColumn";
            this.regNORDataGridViewTextBoxColumn.Width = 70;
            // 
            // regNDataGridViewTextBoxColumn
            // 
            this.regNDataGridViewTextBoxColumn.DataPropertyName = "Reg_N";
            this.regNDataGridViewTextBoxColumn.HeaderText = "Reg_N";
            this.regNDataGridViewTextBoxColumn.Name = "regNDataGridViewTextBoxColumn";
            this.regNDataGridViewTextBoxColumn.Width = 70;
            // 
            // regADataGridViewTextBoxColumn
            // 
            this.regADataGridViewTextBoxColumn.DataPropertyName = "Reg_A";
            this.regADataGridViewTextBoxColumn.HeaderText = "Reg_A";
            this.regADataGridViewTextBoxColumn.Name = "regADataGridViewTextBoxColumn";
            this.regADataGridViewTextBoxColumn.Width = 70;
            // 
            // regBDataGridViewTextBoxColumn
            // 
            this.regBDataGridViewTextBoxColumn.DataPropertyName = "Reg_B";
            this.regBDataGridViewTextBoxColumn.HeaderText = "Reg_B";
            this.regBDataGridViewTextBoxColumn.Name = "regBDataGridViewTextBoxColumn";
            this.regBDataGridViewTextBoxColumn.Width = 70;
            // 
            // mOutputPanel
            // 
            this.mOutputPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mOutputPanel.Location = new System.Drawing.Point(0, 33);
            this.mOutputPanel.Name = "mOutputPanel";
            this.mOutputPanel.Size = new System.Drawing.Size(212, 65);
            this.mOutputPanel.TabIndex = 4;
            // 
            // cpuBindingSource
            // 
            this.cpuBindingSource.DataSource = typeof(Rca301Emulator.Emulator.CPU);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1245, 861);
            this.Controls.Add(this.splitAll);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolsPanel);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(400, 500);
            this.Name = "MainForm";
            this.Text = "RCA301 Emulator";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.splitAll.Panel1.ResumeLayout(false);
            this.splitAll.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitAll)).EndInit();
            this.splitAll.ResumeLayout(false);
            this.assemblySourceContextMenu.ResumeLayout(false);
            this.splitRightFill.Panel1.ResumeLayout(false);
            this.splitRightFill.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitRightFill)).EndInit();
            this.splitRightFill.ResumeLayout(false);
            this.splitRightBottom.Panel1.ResumeLayout(false);
            this.splitRightBottom.Panel1.PerformLayout();
            this.splitRightBottom.Panel2.ResumeLayout(false);
            this.splitRightBottom.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitRightBottom)).EndInit();
            this.splitRightBottom.ResumeLayout(false);
            this.splitRightTop.Panel1.ResumeLayout(false);
            this.splitRightTop.Panel1.PerformLayout();
            this.splitRightTop.Panel2.ResumeLayout(false);
            this.splitRightTop.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitRightTop)).EndInit();
            this.splitRightTop.ResumeLayout(false);
            this.toolsPanel.ResumeLayout(false);
            this.toolsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tickPeriodEdit)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mWatchPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mLabelPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mCpuPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cpuBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitAll;
        private Rca301Emulator.UserInterface.RichTextBoxEx tbAssemblySource;
        private System.Windows.Forms.ContextMenuStrip assemblySourceContextMenu;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem assembleToolStripMenuItem;
        private System.Windows.Forms.Label assemblySourceLabel;
        private System.Windows.Forms.Panel toolsPanel;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnAssemble;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.Label tickPeriodLabel;
        private System.Windows.Forms.NumericUpDown tickPeriodEdit;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel status;
        private Rca301Emulator.UserInterface.MemoryPanel mMemoryPanel;
        private Rca301Emulator.UserInterface.WatchPanel mWatchPanel;
        private Rca301Emulator.UserInterface.CpuPanel mCpuPanel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Reg_P;
        private System.Windows.Forms.DataGridViewTextBoxColumn Reg_NOR;
        private System.Windows.Forms.DataGridViewTextBoxColumn Reg_N;
        private System.Windows.Forms.DataGridViewTextBoxColumn Reg_A;
        private System.Windows.Forms.DataGridViewTextBoxColumn Reg_B;
        private System.Windows.Forms.SplitContainer splitRightBottom;
        private System.Windows.Forms.Label mVariablesLabel;
        private System.Windows.Forms.Label mLabelsLabel;
        private Rca301Emulator.UserInterface.LabelsPanel mLabelPanel;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn mLabelName;
        private System.Windows.Forms.DataGridViewTextBoxColumn mLabelAddress;
        private System.Windows.Forms.BindingSource cpuBindingSource;
        private Rca301Emulator.UserInterface.OutputPanel mOutputPanel;
        private System.Windows.Forms.Button btnImportMemory;
        private System.Windows.Forms.Button btnExportMemory;
        private System.Windows.Forms.OpenFileDialog importMemoryDialog;
        private System.Windows.Forms.SaveFileDialog exportMemoryDialog;
        private System.Windows.Forms.SplitContainer splitRightTop;
        private System.Windows.Forms.Label lOutput;
        private System.Windows.Forms.Label lCPU;
        private System.Windows.Forms.DataGridViewTextBoxColumn regPDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn priDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn regNORDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn regNDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn regADataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn regBDataGridViewTextBoxColumn;
        private System.Windows.Forms.ToolStripMenuItem goToAssemblyLineToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitRightFill;
    }
}

