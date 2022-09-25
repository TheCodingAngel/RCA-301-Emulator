using Rca301Emulator.Assembler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca301Emulator.Emulator
{
    // PRI - page 19
    enum PreviousResultIndicator : int
    {
        Negative = -1,
        Zero = 0,
        Positive = 1,
    }

    class CPU : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        Quad mReg_P; // Instruction pointer
        //Quad mReg_MA; // Memory addressing register
        //Diad mReg_MR; // Memory register

        PreviousResultIndicator mPRI;
        
        // Normal mode registers
        Character mReg_NOR, mReg_N;
        Quad mReg_A, mReg_B;
        Quad mPrevReg_A, mPrevReg_B;

        // Simultaneous mode registers
        //Character mReg_SOR, mReg_M;
        //Quad mReg_S, mReg_T;

        // Record File mode registers
        //Character mReg_FOR, mReg_L;
        //Quad mReg_U, mReg_V;


        public Quad PrevReg_A => mPrevReg_A;
        public Quad PrevReg_B => mPrevReg_B;

        public Quad Reg_P
        {
            get { return mReg_P; }
            set
            {
                if (mReg_P.CompareTo(value) == 0)
                    return;

                mReg_P = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Reg_P)));
            }
        }

        public PreviousResultIndicator PRI
        {
            get { return mPRI; }
            set
            {
                if (mPRI.CompareTo(value) == 0)
                    return;

                mPRI = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(mPRI)));
            }
        }

        public Character Reg_NOR
        {
            get { return mReg_NOR; }
            set
            {
                if (mReg_NOR.CompareTo(value) == 0)
                    return;

                mReg_NOR = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Reg_NOR)));
            }
        }

        public Character Reg_N
        {
            get { return mReg_N; }
            set
            {
                if (mReg_N.CompareTo(value) == 0)
                    return;

                mReg_N = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Reg_N)));
            }
        }

        public Quad Reg_A
        {
            get { return mReg_A; }
            set
            {
                mPrevReg_A = mReg_A;

                if (mReg_A.CompareTo(value) == 0)
                    return;

                mReg_A = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Reg_A)));
            }
        }

        public Quad Reg_B
        {
            get { return mReg_B; }
            set
            {
                mPrevReg_B = mReg_B;

                if (mReg_B.CompareTo(value) == 0)
                    return;

                mReg_B = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Reg_B)));
            }
        }

        public void Reset(int initialInstructionPointer)
        {
            mReg_P = initialInstructionPointer;
            mPRI = PreviousResultIndicator.Zero;

            mReg_NOR = 0;
            mReg_N = 0;
            mReg_A = 0;
            mReg_B = 0;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }

    class Emulator : IDisposable
    {
        const string DUMP_FILE_EXTENSION = ".dmp";
        IAssemblerEnvironment mAsmEnvironment;

        Memory mMemory;
        CPU mCPU;
        InstructionBase[] mInstructions;

        public Memory Memory => mMemory;
        public CPU CPU => mCPU;

        public Emulator(int memoryCapacity, IAssemblerEnvironment asmEnvironment)
        {
            mCPU = new CPU();
            mMemory = new Memory(memoryCapacity);
            mAsmEnvironment = asmEnvironment;
        }

        public void Dispose()
        {
            if (mMemory != null)
            {
                mMemory.Dispose();
                mMemory = null;
            }
        }

        public void PrepareForExecution(InstructionBase[] instructions)
        {
            mCPU.Reset(mMemory.ProgramOffset);
            mInstructions = instructions;

            if (mInstructions != null && mInstructions.Length > 0)
                mAsmEnvironment?.SelectAssemblySourceLine(GetInstructionLine(0));
        }

        int GetInstructionLine(int instructionIndex)
        {
            int line = mInstructions[instructionIndex].LineIndex;
            // Imported instructions have no line index.
            // We use though the fact that the disassembled source has only instructions,
            // so their index is the same as the line on which they stay.
            return line >= 0 ? line : instructionIndex;
        }

        public void SetNextInstruction(int instructionAddress)
        {
            CPU.Reg_P = instructionAddress;
            TrySelectNextInstructionInSource();
        }

        public void Step()
        {
            int nextInstructionAddress = (int)mCPU.Reg_P;
            InstructionData instructionData = mMemory.GetInstructionDataAt(nextInstructionAddress);

            InstructionBase instruction = InstructionCreator.CreateInstruction(instructionData, nextInstructionAddress, true);
            instruction.Execute(mCPU, mMemory);

            if (instruction is InstructionHalt)
                mAsmEnvironment?.Halt();

            if (instruction is InstructionOutput)
                mAsmEnvironment?.OutputCharacters(((InstructionOutput)instruction).OutputCharacters);

            TrySelectNextInstructionInSource();
        }

        public void TrySelectNextInstructionInSource()
        {
            if (mInstructions == null)
                return;

            int nextInstructionAddress = (int)mCPU.Reg_P;
            int nextInstructionOffset = nextInstructionAddress - mMemory.ProgramOffset;

            int nextInstructionIndex = -1;
            if ((nextInstructionOffset % InstructionBase.INSTRUCTION_SIZE) == 0)
            {
                nextInstructionIndex = nextInstructionOffset / InstructionBase.INSTRUCTION_SIZE;
                if (nextInstructionIndex >= 0 && nextInstructionIndex < mInstructions.Length)
                {
                    mAsmEnvironment?.SelectAssemblySourceLine(GetInstructionLine(nextInstructionIndex));
                    return;
                }
            }

            mAsmEnvironment?.ClearAssemblySourceSelection();
        }

        public DisassembleInfo ImportMemoryAndDisassemble(string fileName)
        {
            Disassembler disasm = new Disassembler(mMemory, mAsmEnvironment);
            return disasm.ImportAndDisassemble(fileName);
        }

        public void ExportMemory(string fileName, Variable[] vars)
        {
            string fileExtension = Path.GetExtension(fileName);

            if (string.Compare(fileExtension, DUMP_FILE_EXTENSION, true) == 0)
                DumpMemory(fileName, vars);
            else
                ExportMemoryWithSize(fileName, vars);
        }

        void DumpMemory(string fileName, Variable[] vars)
        {
            using (StreamWriter writer = new StreamWriter(fileName, false))
            {
                int sizeWithTables = mMemory.DataSize + mMemory.DataOffset;
                int totalSize = sizeWithTables - mMemory.ProgramOffset;

                int address = mMemory.ProgramOffset;
                while (address < sizeWithTables)
                {
                    Character[] row = mMemory.GetCharactersAt(address, mMemory.CharactersInRow);
                    writer.WriteLine(DumpSupport.GetExportLine(row, address));
                    address += row.Length;
                }
            }
        }

        void ExportMemoryWithSize(string fileName, Variable[] vars)
        {
            using (StreamWriter writer = new StreamWriter(fileName, false))
            {
                int address = mMemory.ProgramOffset;
                while (address < mMemory.DataOffset)
                {
                    Character[] instruction = mMemory.GetCharactersAt(address, InstructionBase.INSTRUCTION_SIZE);
                    writer.WriteLine(DumpSupport.GetExportLine(instruction, address));
                    address += instruction.Length;
                }

                foreach (string line in CollectVariableExports(vars))
                {
                    writer.WriteLine(line);
                }
            }
        }

        string[] CollectVariableExports(Variable[] vars)
        {
            if (vars == null || vars.Length <= 0)
                return new string[0];

            Array.Sort(vars, (v1, v2) => v1.Offset.CompareTo(v2.Offset));

            List<string> variableLines = new List<string>(vars.Length);
            foreach (Variable v in vars)
            {
                Character[] recentVariableData = mMemory.GetCharactersAt(v.Offset, v.Size);
                variableLines.Add(DumpSupport.GetExportLine(recentVariableData, v.Offset));
            }

            return variableLines.ToArray();
        }
    }
}
