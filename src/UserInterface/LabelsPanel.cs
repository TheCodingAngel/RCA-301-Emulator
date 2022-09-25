using Rca301Emulator.Emulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rca301Emulator.UserInterface
{
    class LabelsPanel : BaseListPanel
    {
        IAssemblerEnvironment mAsmEnvironment;

        public LabelsPanel()
        {
        }

        public void Init(IAssemblerEnvironment asmEnvironment)
        {
            mAsmEnvironment = asmEnvironment;
        }

        public void SetLabels(Assembler.Label[] labels)
        {
            DataSource = labels;
        }

        public override void RefreshList()
        {
            Assembler.Label[] labels = (Assembler.Label[])DataSource;
            if (labels == null)
                return;

            for (int i = 0; i < labels.Length; i++)
            {
                Assembler.Label l = labels[i];
                Quad address = l.Offset;
                Rows[i].Cells[1].Value = GetStringRepresentation(address.GetData());
            }
        }

        protected override void OnCellDoubleClick(DataGridViewCellEventArgs e)
        {
            base.OnCellDoubleClick(e);

            Assembler.Label[] lables = (Assembler.Label[])DataSource;
            if (lables == null)
                return;

            if (e.RowIndex >= lables.Length)
                return;

            if (e.ColumnIndex == 1)
                mAsmEnvironment?.ShowMemoryAddress(lables[e.RowIndex].Offset);
            else if (e.ColumnIndex == 0)
                mAsmEnvironment?.SelectAssemblySourceLine(lables[e.RowIndex].LineIndex);
        }
    }
}
