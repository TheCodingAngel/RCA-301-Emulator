using Rca301Emulator.Assembler;
using Rca301Emulator.Emulator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rca301Emulator.UserInterface
{
    class WatchPanel : BaseListPanel
    {
        Memory mMemory;
        IAssemblerEnvironment mAsmEnvironment;

        public WatchPanel()
        {
            AutoGenerateColumns = false;
        }

        public void SetVariables(Variable[] variables)
        {
            DataSource = variables;
        }

        public void Init(Memory memory, IAssemblerEnvironment asmEnvironment)
        {
            mMemory = memory;
            mAsmEnvironment = asmEnvironment;
        }

        public override void RefreshList()
        {
            Variable[] variables = (Variable[])DataSource;
            if (variables == null)
                return;

            for (int i = 0; i < variables.Length; i++)
            {
                Variable v = variables[i];
                Character[] data = mMemory.GetCharactersAt(v.Offset, v.Size);
                Rows[i].Cells[1].Value = GetStringRepresentation(data);
            }
        }

        protected override void OnCellDoubleClick(DataGridViewCellEventArgs e)
        {
            base.OnCellDoubleClick(e);

            Variable[] variables = (Variable[])DataSource;
            if (variables == null)
                return;

            if (e.RowIndex >= variables.Length)
                return;

            if (e.ColumnIndex == 2)
                mAsmEnvironment?.ShowMemoryAddress(variables[e.RowIndex].Offset);
            else if (e.ColumnIndex == 0)
                mAsmEnvironment?.SelectAssemblySourceLine(variables[e.RowIndex].LineIndex);
        }

        protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
        {
            base.OnCellValueChanged(e);

            Variable[] variables = (Variable[])DataSource;
            if (variables == null)
                return;

            if (e.RowIndex >= variables.Length)
                return;

            Variable variable = variables[e.RowIndex];
            if (variable.Value is Quad)
            {
                string newValue = (string)Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                mMemory.SetStringValue(variable.Offset, newValue.PadLeft(variable.Size, '0'));
            }
            else if (variable.Value is string)
            {
                string newValue = (string)Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                mMemory.SetStringValue(variable.Offset, newValue);
            }

            mAsmEnvironment?.RefreshMemory();
        }
    }
}
