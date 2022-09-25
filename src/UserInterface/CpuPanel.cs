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
    class CpuPanel : DataGridView
    {
        IAssemblerEnvironment mAsmEnvironment;
        CPU mCPU;

        public CpuPanel()
        {
            AutoGenerateColumns = false;
        }

        public void Init(CPU cpu, IAssemblerEnvironment asmEnvironment)
        {
            mCPU = cpu;
            mAsmEnvironment = asmEnvironment;
            DataSource = new BindingList<CPU>(new[] { mCPU });
        }

        protected override void OnCellParsing(DataGridViewCellParsingEventArgs e)
        {
            if (e.RowIndex != 0 || mCPU == null)
            {
                base.OnCellParsing(e);
                return;
            }

            switch (e.ColumnIndex)
            {
                case 0:
                    mCPU.Reg_P = (string)e.Value;
                    e.Value = mCPU.Reg_P;
                    mAsmEnvironment?.SelectNextAssemblyInstructionInSource();
                    e.ParsingApplied = true;
                    break;
                case 1:
                    mCPU.Reg_NOR = ((string)e.Value)[0];
                    e.Value = mCPU.Reg_NOR;
                    mAsmEnvironment?.SelectNextAssemblyInstructionInSource();
                    e.ParsingApplied = true;
                    break;
                case 2:
                    mCPU.Reg_N = ((string)e.Value)[0];
                    e.Value = mCPU.Reg_N;
                    mAsmEnvironment?.SelectNextAssemblyInstructionInSource();
                    e.ParsingApplied = true;
                    break;
                case 3:
                    mCPU.Reg_A = (string)e.Value;
                    e.Value = mCPU.Reg_A;
                    mAsmEnvironment?.SelectNextAssemblyInstructionInSource();
                    e.ParsingApplied = true;
                    break;
                case 4:
                    mCPU.Reg_B = (string)e.Value;
                    e.Value = mCPU.Reg_B;
                    mAsmEnvironment?.SelectNextAssemblyInstructionInSource();
                    e.ParsingApplied = true;
                    break;
                default:
                    base.OnCellParsing(e); ;
                    break;
            }
        }
    }
}
