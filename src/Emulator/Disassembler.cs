using Rca301Emulator.Assembler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca301Emulator.Emulator
{
    class DisassembleInfo
    {
        public string SourceCode { get; }
        public InstructionBase[] Instructions { get; }

        public DisassembleInfo(string sourceCode, InstructionBase[] instructions)
        {
            SourceCode = sourceCode;
            Instructions = instructions;
        }
    }

    class DumpLineData
    {
        public int Address { get; }
        public Character[] Characters { get; }

        public DumpLineData(int address, Character[] characters)
        {
            Address = address;
            Characters = characters;
        }
    }

    static class DumpSupport
    {
        const char MEMORY_EXPORT_DELIMITER = ',';

        public static string GetExportLine(Character[] data, int address)
        {
            return address.ToString() + MEMORY_EXPORT_DELIMITER +
                data.DataToString(MEMORY_EXPORT_DELIMITER);
        }

        public static DumpLineData ParseLine(string line, out InstructionData instructionData)
        {
            string[] chars = line.Split(MEMORY_EXPORT_DELIMITER).Trim();
            int address = int.Parse(chars[0]);

            int charCount = chars.Length - 1;
            Character[] data = new Character[charCount];
            for (int i = 1; i < chars.Length; i++)
                data[i - 1] = chars[i][0];

            instructionData = data.Length == InstructionBase.INSTRUCTION_SIZE ?
                new InstructionData(data) : null;

            return new DumpLineData(address, data);
        }
    }

    class Disassembler
    {
        const string VARIABLE_NAME_PREFIX = "Var";
        const string LABEL_NAME_PREFIX = "label";

        Memory mMemory;
        IAssemblerEnvironment mAsmEnvironment;

        List<InstructionBase> mInstructions = new List<InstructionBase>();
        List<DumpLineData> mVariables = new List<DumpLineData>();
        Dictionary<int, string> mLabels = new Dictionary<int, string>();

        int mCodeOffset;
        int mDataOffset;
        int mDataSize;

        public Disassembler(Memory memory, IAssemblerEnvironment asmEnvironment)
        {
            mMemory = memory;
            mAsmEnvironment = asmEnvironment;
        }

        public DisassembleInfo ImportAndDisassemble(string dumpFileName)
        {
            Import(dumpFileName);

            StringBuilder sourceCode = new StringBuilder();

            for (int i = 0; i < mVariables.Count; i++)
                sourceCode.AppendLine(DisassembleVariable(GetVariableName(i), mVariables[i]));

            sourceCode.AppendLine();
            sourceCode.AppendLine($"{EmulatorIdentifier.ORG} {mCodeOffset}");
            sourceCode.AppendLine();

            for (int i = 0; i < mInstructions.Count; i++)
            {
                InstructionBase instruction = mInstructions[i];

                string labelName;
                if (mLabels.TryGetValue(instruction.Address, out labelName))
                {
                    sourceCode.AppendLine();
                    sourceCode.AppendLine($"{labelName}:");
                }

                sourceCode.AppendLine(DisassembleInstruction(instruction));
            }

            mAsmEnvironment?.ShowMemoryAddress(mCodeOffset);

            return new DisassembleInfo(sourceCode.ToString(), mInstructions.ToArray());
        }

        void Import(string dumpFileName)
        {
            mCodeOffset = Memory.FIRST_AVAILABLE_ADDRESS;
            mDataOffset = 0;
            mDataSize = 0;

            mMemory.ClearMemory();

            using (StreamReader reader = new StreamReader(dumpFileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    InstructionData instructionData;
                    DumpLineData lineData = DumpSupport.ParseLine(line, out instructionData);

                    InstructionBase instruction = instructionData != null
                        ? instruction = InstructionCreator.CreateInstruction(instructionData, lineData.Address, false)
                        : null;

                    if (instruction != null)
                    {
                        if (mInstructions.Count == 0)
                            mCodeOffset = instruction.Address;
                        mInstructions.Add(instruction);
                        AddLabels(lineData, instructionData, instruction.Identifier);
                    }
                    else
                    {
                        if (mDataOffset == 0)
                            mDataOffset = lineData.Address; // store the address of the first variable
                        mVariables.Add(lineData);
                    }

                    if (mDataOffset != 0)
                        mDataSize += lineData.Characters.Length;

                    mMemory.SetCharacters(lineData.Address, lineData.Characters);
                }
            }

            mMemory.ApplyMemoryParameters(mCodeOffset,
                mDataOffset > 0 ? mDataOffset : mCodeOffset + mInstructions.Count * InstructionBase.INSTRUCTION_SIZE,
                mDataSize);
        }

        void AddLabels(DumpLineData lineData, InstructionData instructionData, InstructionIdentifier id)
        {
            switch (id)
            {
                case InstructionIdentifier.CTC:
                    AddNextLabel(instructionData.A_Value);
                    AddNextLabel(instructionData.B_Value);
                    break;
                case InstructionIdentifier.Reg:
                    if (instructionData.N_Value.Value == 1) // Store P register
                        AddNextLabel(instructionData.B_Value);
                    break;
            }
        }

        void AddNextLabel(Quad address)
        {
            if (address.IsRelativeAddress)
                return;

            if (!mLabels.ContainsKey(address.Value))
                mLabels.Add(address.Value, LABEL_NAME_PREFIX + (mLabels.Count + 1).ToString());
        }

        string GetLabel(Quad address)
        {
            if (address.IsRelativeAddress)
                return address.DisplayString;

            string labelName;
            if (!mLabels.TryGetValue(address.Value, out labelName))
                return address.DisplayString;

            return labelName;
        }

        string DisassembleInstruction(InstructionBase instruction)
        {
            if (instruction.Identifier != InstructionIdentifier.CTC &&
                instruction.Identifier != InstructionIdentifier.Reg)
            {
                return instruction.ToString();
            }

            InstructionData instructionData = instruction.GetData();

            string regA = instructionData.A_Value.DisplayString;
            string regB = instructionData.B_Value.DisplayString;

            switch (instruction.Identifier)
            {
                case InstructionIdentifier.CTC:
                    regA = GetLabel(instructionData.A_Value);
                    regB = GetLabel(instructionData.B_Value);
                    break;
                case InstructionIdentifier.Reg:
                    if (instructionData.N_Value.Value == 1) // Store P register
                        regB = GetLabel(instructionData.B_Value);
                    break;
            }

            return InstructionBase.ToString(instruction.Identifier, instructionData.N_Value, regA, regB);
        }

        string DisassembleVariable(string variableName, DumpLineData lineData)
        {
            string value;

            switch (lineData.Characters.Length)
            {
                case 2:
                    value = new Diad(lineData.Characters).DisplayString;
                    return $"DD {variableName} @ {lineData.Address} = {value}";
                case 4:
                    value = new Quad(lineData.Characters).DisplayString;
                    return $"DQ {variableName} @ {lineData.Address} = {value}";
                default:
                    value = lineData.Characters.DataToString();
                    return $"DC {variableName} @ {lineData.Address} = '{value}'";
            }
        }

        string GetVariableName(int index)
        {
            return VARIABLE_NAME_PREFIX + (index + 1).ToString();
        }
    }
}
