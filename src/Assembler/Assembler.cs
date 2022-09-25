using Rca301Emulator.Emulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca301Emulator.Assembler
{
    class Assembler : IDisposable
    {
        Dictionary<EmulatorIdentifier, EmulatorParameter> mEmulatorParams =
            new Dictionary<EmulatorIdentifier, EmulatorParameter>();

        Dictionary<string, Constant> mConstants = new Dictionary<string, Constant>();
        Dictionary<string, Variable> mVariables = new Dictionary<string, Variable>();
        List<InstructionBase> mInstructions = new List<InstructionBase>();
        Dictionary<string, Label> mLabels = new Dictionary<string, Label>();

        int mCodeStart;
        int mCodeSize;
        int mDataStart;
        int mDataSize;

        public Assembler()
        {
        }

        public void Dispose()
        {
            Reset();
        }

        public Variable[] GetVariables()
        {
            return mVariables.Values.ToArray();
        }

        public Label[] GetLabels()
        {
            return mLabels.Values.ToArray();
        }

        public InstructionBase[] GetInstructions()
        {
            return mInstructions.ToArray();
        }

        public void SetInstructions(InstructionBase[] instructions)
        {
            Reset();

            mInstructions.AddRange(instructions);
        }

        public void Assemble(string[] assembleCodeLines, Memory memory)
        {
            Reset();

            if (assembleCodeLines == null || assembleCodeLines.Length <= 0)
                return;

            for (int i = 0; i < assembleCodeLines.Length; i++)
            {
                AssemblyLine asmLine = new AssemblyLine(assembleCodeLines[i], i);
                if (string.IsNullOrEmpty(asmLine.Identifier))
                    continue;

                ExtractIdentifier(asmLine);
            }

            Compile();

            FillMemory(memory);
        }

        void Reset()
        {
            mEmulatorParams.Clear();
            mConstants.Clear();
            mVariables.Clear();
            mInstructions.Clear();
            mLabels.Clear();

            mCodeSize = 0;
            mDataStart = 0;
            mDataSize = 0;
        }

        void ExtractIdentifier(AssemblyLine asmLine)
        {
            InstructionIdentifier instructionId;
            if (asmLine.Identifier.TryGetEnumValue(out instructionId))
            {
                mInstructions.Add(InstructionCreator.CreateInstruction(instructionId, asmLine));
                return;
            }

            ConstantIdentifier constantId;
            if (asmLine.Identifier.TryGetEnumValue(out constantId))
            {
                if (!(asmLine.Expressions[0] is IdentifierExpression))
                    throw new AssembleException($"Incorrect constant name", asmLine.LineIndex);

                Constant constant = new Constant(constantId, asmLine);
                mConstants.Add(constant.Name, constant);
                return;
            }

            VariableIdentifier variableId;
            if (asmLine.Identifier.TryGetEnumValue(out variableId))
            {
                if (!(asmLine.Expressions[0] is IdentifierExpression))
                    throw new AssembleException($"Incorrect variable name", asmLine.LineIndex);

                Variable variable = new Variable(variableId, mDataSize, asmLine);

                if (variable.AbsoluteOffset == null)
                {
                    mDataSize += variable.Size;

                    // Ensure even addressing for easier High speed memory access.
                    if (mDataSize % 2 != 0)
                        mDataSize++;
                }

                mVariables.Add(variable.Name, variable);
                return;
            }

            EmulatorIdentifier emulatorId;
            if (asmLine.Identifier.TryGetEnumValue(out emulatorId))
            {
                EmulatorParameter param = new EmulatorParameter(asmLine);
                mEmulatorParams[emulatorId] = param; // In more than one declarations the last wins
                return;
            }

            if (asmLine.Expressions != null && asmLine.Expressions.Length == 1 &&
                asmLine.Expressions[0] is LabelExpression)
            {
                Label label = new Label(mInstructions.Count * InstructionBase.INSTRUCTION_SIZE, asmLine);
                mLabels.Add(label.Name, label);
                return;
            }

            throw new AssembleException($"Unknown identifier \"{asmLine.Identifier}\"", asmLine.LineIndex);
        }

        void Compile()
        {
            EmulatorParameter param;
            if (mEmulatorParams.TryGetValue(EmulatorIdentifier.ORG, out param))
                mCodeStart = param.Value;
            else
                mCodeStart = Memory.FIRST_AVAILABLE_ADDRESS;

            mCodeSize = mInstructions.Count * InstructionBase.INSTRUCTION_SIZE;

            // Use one instruction size as a padding between code and data
            mDataStart = mCodeStart + mCodeSize + InstructionBase.INSTRUCTION_SIZE;

            foreach (Variable variable in mVariables.Values)
            {
                if (variable.AbsoluteOffset == null)
                    variable.Offset += mDataStart;
            }

            // Note - the data size is already calculated (done when the variables were extracted)

            ExpressionCalculator expressionCalculator = new ExpressionCalculator(mConstants, mVariables, mLabels);

            foreach (Constant constant in mConstants.Values)
                constant.CalculateValue(expressionCalculator);

            foreach (Variable variable in mVariables.Values)
                variable.CalculateValue(expressionCalculator);

            foreach (Label label in mLabels.Values)
                label.Offset += mCodeStart;

            for (int i = 0; i < mInstructions.Count; i++)
            {
                InstructionBase instruction = mInstructions[i];
                instruction.Parse(mDataStart, expressionCalculator);
            }
        }

        void FillMemory(Memory memory)
        {
            memory.ClearMemory();
            memory.ApplyMemoryParameters(mCodeStart, mDataStart, mDataSize);

            int instructionAddress = mCodeStart;
            for (int i = 0; i < mInstructions.Count; i++)
            {
                InstructionData data = mInstructions[i].GetData();
                Character[] chars = data.GetCharacterSequence();
                memory.SetCharacters(instructionAddress, chars);
                instructionAddress += chars.Length;
            }

            Debug.Assert(instructionAddress - mCodeStart == mCodeSize);

            foreach (Variable v in mVariables.Values)
            {
                memory.SetCharacters(v.Offset, v.GetData());
            }
        }
    }
}
