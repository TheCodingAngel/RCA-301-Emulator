using Rca301Emulator.Assembler;
using Rca301Emulator.Emulator;
using Rca301Emulator.UserInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rca301Emulator
{
    partial class MainForm : Form, IAssemblerEnvironment
    {
        const string CAPTION_DELIMITER = " - ";
        const string CHANGE_PREFIX = "* ";

        string mInitialCaption;
        string mInitialStatus;
        Timer mProgramRunTimer;

        string mDefaultFileDirectory;
        string mAssemblySourceFileName;
        bool mIsSourceEdited;

        Assembler.Assembler mAssembler;
        Emulator.Emulator mEmulator;

        public MainForm()
        {
            InitializeComponent();

            mProgramRunTimer = new Timer();
            mProgramRunTimer.Tick += OnRunTimerTick;

            mAssembler = new Assembler.Assembler();
            mEmulator = new Emulator.Emulator(10000, this);

            mCpuPanel.Init(mEmulator.CPU, this);
            mMemoryPanel.Init(mEmulator.Memory, this);

            mWatchPanel.Init(mEmulator.Memory, this);
            mLabelPanel.Init(this);
        }

        void RefreshContent()
        {
            mMemoryPanel.RefreshMemory();
            mWatchPanel.RefreshList();
            mLabelPanel.RefreshList();
        }

        void PrepareForExecution(Assembler.Assembler asm)
        {
            mEmulator.PrepareForExecution(asm.GetInstructions());

            mMemoryPanel.ScrollToAddress(mEmulator.Memory.ProgramOffset);
            mWatchPanel.SetVariables(asm.GetVariables());
            mLabelPanel.SetLabels(asm.GetLabels());

            mOutputPanel.ResetOutput();
        }

        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);

            mInitialCaption = Text;
            mInitialStatus = status.Text;

            mDefaultFileDirectory = Path.GetDirectoryName(GetExecutableFullName());
            openFileDialog.InitialDirectory = mDefaultFileDirectory;
            saveFileDialog.InitialDirectory = mDefaultFileDirectory;

            tbAssemblySource.ShowLineNumbers = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!SaveIfChanged())
                e.Cancel = true;

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            mEmulator.Dispose();
            mAssembler.Dispose();
        }

        static string GetExecutableFullName()
        {
            // Entry assembly is always the executable that loads the dll
            Assembly app = Assembly.GetEntryAssembly();
            if (app != null)
                return app.Location;

            // When debugging the current process is the visual studio host (".vshost.exe")
            Process currentProcess = Process.GetCurrentProcess();
            return currentProcess.MainModule.FileName;
        }

        void SetStatus(string message)
        {
            status.Text = message;
        }

        void ResetStatus()
        {
            status.Text = mInitialStatus;
        }

        void ReportError(Exception e, bool showDialog = true)
        {
            ReportError(e.Message, showDialog);
        }

        void ReportError(string message, bool showDialog)
        {
            SetStatus(message);
            if (showDialog)
                MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #region IAssemblerEnvironment

        public void SelectAssemblySourceLine(int lineIndex)
        {
            if (lineIndex < 0 || lineIndex > tbAssemblySource.Lines.Length)
                return;

            int start = tbAssemblySource.GetFirstCharIndexFromLine(lineIndex);
            int length = tbAssemblySource.Lines[lineIndex].Length;

            tbAssemblySource.Focus();
            tbAssemblySource.Select(start, length);
        }

        public void SelectNextAssemblyInstructionInSource()
        {
            mEmulator.TrySelectNextInstructionInSource();
        }

        public void ClearAssemblySourceSelection()
        {
            tbAssemblySource.Select();
            tbAssemblySource.Select(tbAssemblySource.SelectionStart, 0);
        }

        public void ShowMemoryAddress(int address)
        {
            mMemoryPanel.Focus();
            mMemoryPanel.ScrollToAddress(address);
        }

        public void RefreshMemory()
        {
            mMemoryPanel.RefreshMemory();
        }

        public void OutputCharacters(Character[] chars)
        {
            mOutputPanel.AddCharacters(chars);
        }

        public void Halt()
        {
            OnStopClick(this, EventArgs.Empty);

            if (MessageBox.Show(this,
                "System halted. Do you want to reassemble and reset execution?", "System Halted",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                OnAssembleClick(this, EventArgs.Empty);
            }
        }

        #endregion

        void UpdateCaption()
        {
            StringBuilder caption = new StringBuilder();

            if (mIsSourceEdited)
                caption.Append(CHANGE_PREFIX);

            caption.Append(mInitialCaption);

            if (!string.IsNullOrEmpty(mAssemblySourceFileName))
                caption.Append(CAPTION_DELIMITER + mAssemblySourceFileName);

            Text = caption.ToString();
        }

        void OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        void OnDragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Array fileNames = (Array)e.Data.GetData(DataFormats.FileDrop);

                if (fileNames != null)
                {
                    mAssemblySourceFileName = fileNames.GetValue(0).ToString();

                    // Explorer instance from which file is dropped is not responding
                    // all the time when DragDrop handler is active, so we need to return
                    // immediately (especially if we show message boxes).
                    Task.Factory.StartNew(LoadFileAsync);
                }
            }
            catch (Exception ex)
            {
                ReportError("Error handling drag and drop from Explorer: " + ex.Message, false);
            }
        }

        void LoadFileAsync()
        {
            if (InvokeRequired)
                Invoke(new Action(LoadFile));
            else
                LoadFile();
        }

        void OnLoadClick(object sender, EventArgs args)
        {
            openFileDialog.InitialDirectory = mDefaultFileDirectory;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                mAssemblySourceFileName = openFileDialog.FileName;
                LoadFile();
            }
        }

        void LoadFile()
        {
            try
            {
                SaveIfChanged();

                if (!string.IsNullOrEmpty(mAssemblySourceFileName))
                {
                    mDefaultFileDirectory = Path.GetDirectoryName(mAssemblySourceFileName);

                    tbAssemblySource.LoadFile(mAssemblySourceFileName, RichTextBoxStreamType.PlainText);
                    mIsSourceEdited = false;
                }

                UpdateCaption();

                ResetStatus();
            }
            catch (Exception e)
            {
                ReportError(e);
            }
        }

        bool SaveIfChanged()
        {
            if (mIsSourceEdited && MessageBox.Show(this, "Do you want to save the changes?",
                "Do you want to save the changes",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                return SaveFile();
            }

            return true;
        }

        bool SaveFile()
        {
            try
            {
                if (string.IsNullOrEmpty(mAssemblySourceFileName))
                {
                    saveFileDialog.InitialDirectory = mDefaultFileDirectory;
                    saveFileDialog.FileName = null;
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        mDefaultFileDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                        mAssemblySourceFileName = saveFileDialog.FileName;
                    }
                }

                if (!string.IsNullOrEmpty(mAssemblySourceFileName))
                {
                    tbAssemblySource.SaveFile(mAssemblySourceFileName, RichTextBoxStreamType.PlainText);
                    mIsSourceEdited = false;
                    UpdateCaption();
                    ResetStatus();
                    return true;
                }
            }
            catch (Exception e)
            {
                ReportError(e);
            }

            return false;
        }

        void OnSaveClick(object sender, EventArgs args)
        {
            SaveFile();
        }

        void OnExportMemoryClick(object sender, EventArgs args)
        {
            if (string.IsNullOrEmpty(exportMemoryDialog.InitialDirectory))
                exportMemoryDialog.InitialDirectory = mDefaultFileDirectory;

            if (exportMemoryDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                mEmulator.ExportMemory(exportMemoryDialog.FileName, mAssembler.GetVariables());
            }
            catch (Exception e)
            {
                ReportError(e);
            }
        }

        void OnImportMemoryClick(object sender, EventArgs args)
        {
            if (string.IsNullOrEmpty(importMemoryDialog.InitialDirectory))
                importMemoryDialog.InitialDirectory = mDefaultFileDirectory;

            if (importMemoryDialog.ShowDialog() != DialogResult.OK)
                return;

            Cursor oldCursor = Cursor;
            Cursor = Cursors.WaitCursor;

            try
            {
                SetStatus("Importing...");

                DisassembleInfo info = mEmulator.ImportMemoryAndDisassemble(importMemoryDialog.FileName);
                SetAssemblySource(info.SourceCode);
                mAssembler.SetInstructions(info.Instructions);
                PrepareForExecution(mAssembler);

                ResetStatus();
            }
            catch (Exception e)
            {
                ReportError(e);
            }
            finally
            {
                RefreshContent();
                Cursor = oldCursor;
            }
        }

        void SetAssemblySource(string sourceCode)
        {
            tbAssemblySource.Text = sourceCode;

            mIsSourceEdited = true;
            UpdateCaption();
        }

        void OnAssemblySourceChanged(object sender, EventArgs args)
        {
            mIsSourceEdited = tbAssemblySource.CanUndo;
            UpdateCaption();
        }

        void OnAssembleClick(object sender, EventArgs args)
        {
            Cursor oldCursor = Cursor;
            Cursor = Cursors.WaitCursor;

            try
            {
                SetStatus("Assembling...");

                mAssembler.Assemble(tbAssemblySource.Lines, mEmulator.Memory);
                PrepareForExecution(mAssembler);

                ResetStatus();
            }
            catch (AssembleException ae)
            {
                ReportError(ae);
                SelectAssemblySourceLine(ae.LineNumber - 1);
            }
            catch (Exception e)
            {
                ReportError(e);
            }
            finally
            {
                RefreshContent();
                Cursor = oldCursor;
            }
        }

        void OnStartClick(object sender, EventArgs args)
        {
            startToolStripMenuItem.Visible = false;
            btnStart.Visible = false;

            stopToolStripMenuItem.Visible = true;
            btnStop.Visible = true;

            SetStatus("RUNNING");

            mProgramRunTimer.Interval = (int)tickPeriodEdit.Value;
            mProgramRunTimer.Start();
        }

        void OnStopClick(object sender, EventArgs args)
        {
            mProgramRunTimer.Stop();

            startToolStripMenuItem.Visible = true;
            btnStart.Visible = true;

            stopToolStripMenuItem.Visible = false;
            btnStop.Visible = false;

            ResetStatus();
        }

        void OnRunTimerTick(object sender, EventArgs args)
        {
            OnStepClick(sender, args);
        }

        void OnStepClick(object sender, EventArgs args)
        {
            try
            {
                mEmulator.Step();
                RefreshContent();
            }
            catch (EmulatorException ee)
            {
                if (mProgramRunTimer.Enabled)
                    mProgramRunTimer.Stop();

                ReportError(ee);
                ShowMemoryAddress(ee.Address);
            }
            catch (Exception e)
            {
                if (mProgramRunTimer.Enabled)
                    mProgramRunTimer.Stop();

                ReportError(e);
            }
        }

        void OnGoToAssemblyLineClick(object sender, EventArgs e)
        {
            int lineIndex = tbAssemblySource.GetLineFromCharIndex(tbAssemblySource.SelectionStart);
            int address = GetInstructionAddress(lineIndex);
            if (address >= 0)
            {
                mEmulator.SetNextInstruction(address);
                mMemoryPanel.ScrollToAddress(address);
            }
        }

        int GetInstructionAddress(int lineIndex)
        {
            InstructionBase[] instructions = mAssembler.GetInstructions();

            for (int i = 0; i < instructions.Length; i++)
            {
                InstructionBase instruction = instructions[i];
                if (instruction.LineIndex == lineIndex)
                    return instruction.Address >= 0 ? instruction.Address : InstructionIndexToAddress(i);
            }

            // Handle instructions without line information (such as imported ones)

            if (lineIndex >= instructions.Length)
                return -1;

            InstructionBase res = instructions[lineIndex];
            if (res.LineIndex >= 0)
                return -1;

            return res.Address >= 0 ? res.Address : InstructionIndexToAddress(lineIndex);
        }

        int InstructionIndexToAddress(int index)
        {
            return mEmulator.Memory.ProgramOffset + index * InstructionBase.INSTRUCTION_SIZE;
        }

        void OnSetNextInstruction(object sender, EventArgsAddress e)
        {
            mEmulator.SetNextInstruction(e.Address);
        }

        void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F2:
                case Keys.Control | Keys.S:
                    OnSaveClick(this, EventArgs.Empty);
                    break;
                case Keys.F3:
                case Keys.Control | Keys.O:
                    OnLoadClick(this, EventArgs.Empty);
                    break;
                case Keys.F4:
                    OnExportMemoryClick(this, EventArgs.Empty);
                    break;
                case Keys.F5:
                    OnStartClick(this, EventArgs.Empty);
                    break;
                case Keys.F6:
                    OnStopClick(this, EventArgs.Empty);
                    break;
                case Keys.F7:
                    OnImportMemoryClick(this, EventArgs.Empty);
                    break;
                case Keys.F8:
                    OnStepClick(this, EventArgs.Empty);
                    break;
                case Keys.F9:
                    OnAssembleClick(this, EventArgs.Empty);
                    break;
            }
        }
    }
}
