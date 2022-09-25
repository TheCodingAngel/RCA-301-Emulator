using Rca301Emulator.Emulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca301Emulator.Assembler
{
    enum InstructionIdentifier
    {
        Unknown,
        Hlt,
        TWN, // Tape Write Normal
        SF, // Transfer Symbol to Fill
        Add,
        Sub,
        COM, // Compare Left
        CTC, // Conditional Transfer of Control
        LSL, // Locate Symbol Left
        LSR, // Locate Symbol Right
        DL, // Transfer Data Left
        DR, // Transfer Data Right
        Reg, // Store Register
        OR, // Logical OR
        AND, // Logical AND
        //TA, // Loop to address given number of times
    }

    static class InstructionCreator
    {
        static Dictionary<InstructionIdentifier, Character> OPCODE = new Dictionary<InstructionIdentifier, Character>
        {
            { InstructionIdentifier.Hlt, '.' },
            { InstructionIdentifier.TWN, '8' },
            { InstructionIdentifier.SF, 'J' },
            { InstructionIdentifier.Add, '+' },
            { InstructionIdentifier.Sub, '-' },
            { InstructionIdentifier.COM, 'Y' },
            { InstructionIdentifier.CTC, 'W' },
            { InstructionIdentifier.LSL, 'K' },
            { InstructionIdentifier.LSR, 'L' },
            { InstructionIdentifier.DL, 'M' },
            { InstructionIdentifier.DR, 'N' },
            { InstructionIdentifier.Reg, 'V' },
            { InstructionIdentifier.OR, 'Q' },
            { InstructionIdentifier.AND, 'T' },
            //{ InstructionIdentifier.TA, 'X' },
        };

        public static bool TryGetOpcode(InstructionIdentifier id, out Character opcode)
        {
            return OPCODE.TryGetValue(id, out opcode);
        }

        public static InstructionBase CreateInstruction(InstructionIdentifier id, AssemblyLine asmLine)
        {
            switch (id)
            {
                case InstructionIdentifier.Hlt:
                    return new InstructionHalt(asmLine);
                case InstructionIdentifier.SF:
                    return new InstructionSymbolFill(asmLine);
                case InstructionIdentifier.Add:
                case InstructionIdentifier.Sub:
                    return new InstructionMath(id, asmLine);
                case InstructionIdentifier.CTC:
                    return new InstructionCtc(asmLine);
                case InstructionIdentifier.TWN:
                    return new InstructionOutput(id, asmLine);
                case InstructionIdentifier.DL:
                case InstructionIdentifier.DR:
                    return new InstructionTransferData(id, asmLine);
                case InstructionIdentifier.LSL:
                case InstructionIdentifier.LSR:
                    return new InstructionLocateSymbol(id, asmLine);
                case InstructionIdentifier.COM:
                    return new InstructionCompare(asmLine);
                case InstructionIdentifier.Reg:
                    return new InstructionStoreRegister(asmLine);
                case InstructionIdentifier.OR:
                case InstructionIdentifier.AND:
                    return new InstructionBitOperation(id, asmLine);
                //case InstructionIdentifier.TA:
                //    return new InstructionTally(asmLine);
            }
            throw new AssembleException($"Unsupported instruction {asmLine.Identifier}", asmLine.LineIndex);
        }

        public static InstructionBase CreateInstruction(InstructionData data, int instructionAddress,
            bool throwOnError)
        {
            InstructionIdentifier id;
            if (!TryGetIdentifier(data.OperationCode, out id))
            {
                if (throwOnError)
                    throw new EmulatorException($"Unknown instruction opcode \"{data.OperationCode}\"", instructionAddress);
                else
                    return null;
            }

            switch (id)
            {
                case InstructionIdentifier.Hlt:
                    return new InstructionHalt(data, instructionAddress);
                case InstructionIdentifier.SF:
                    return new InstructionSymbolFill(data, instructionAddress);
                case InstructionIdentifier.Add:
                case InstructionIdentifier.Sub:
                    return new InstructionMath(id, data, instructionAddress);
                case InstructionIdentifier.CTC:
                    return new InstructionCtc(data, instructionAddress);
                case InstructionIdentifier.TWN:
                    return new InstructionOutput(id, data, instructionAddress);
                case InstructionIdentifier.DL:
                case InstructionIdentifier.DR:
                    return new InstructionTransferData(id, data, instructionAddress);
                case InstructionIdentifier.LSL:
                case InstructionIdentifier.LSR:
                    return new InstructionLocateSymbol(id, data, instructionAddress);
                case InstructionIdentifier.COM:
                    return new InstructionCompare(data, instructionAddress);
                case InstructionIdentifier.Reg:
                    return new InstructionStoreRegister(data, instructionAddress);
                case InstructionIdentifier.OR:
                case InstructionIdentifier.AND:
                    return new InstructionBitOperation(id, data, instructionAddress);
                //case InstructionIdentifier.TA:
                //    return new InstructionTally(data, instructionAddress);
            }

            if (throwOnError)
                throw new EmulatorException($"Unsupported instruction \"{id}\"", instructionAddress);

            return null;
        }

        static bool TryGetIdentifier(Character opcode, out InstructionIdentifier id)
        {
            foreach (KeyValuePair<InstructionIdentifier, Character> kvp in OPCODE)
            {
                if (kvp.Value.CompareTo(opcode) == 0)
                {
                    id = kvp.Key;
                    return true;
                }
            }

            id = InstructionIdentifier.Unknown;
            return false;
        }
    }

    class InstructionData
    {
        public Character OperationCode { get; }
        public Character N_Value { get; }
        public Quad A_Value { get; }
        public Quad B_Value { get; }

        public InstructionData(Character operationCode,
            Character n, Quad a, Quad b)
        {
            OperationCode = operationCode;
            N_Value = n;
            A_Value = a;
            B_Value = b;
        }

        public InstructionData(Character[] data)
        {
            OperationCode = data[0];
            N_Value = data[1];
            A_Value = new Quad(data[2], data[3], data[4], data[5]);
            B_Value = new Quad(data[6], data[7], data[8], data[9]);
        }

        public Character[] GetCharacterSequence()
        {
            Character[] res = new Character[InstructionBase.INSTRUCTION_SIZE];

            res[0] = OperationCode;

            res[1] = N_Value;

            res[2] = A_Value.Hi.Hi;
            res[3] = A_Value.Hi.Lo;
            res[4] = A_Value.Lo.Hi;
            res[5] = A_Value.Lo.Lo;

            res[6] = B_Value.Hi.Hi;
            res[7] = B_Value.Hi.Lo;
            res[8] = B_Value.Lo.Hi;
            res[9] = B_Value.Lo.Lo;

            return res;
        }
    }

    abstract class InstructionBase
    {
        public const int INSTRUCTION_SIZE = 10; // in characters
        public static Quad STA_ADDRESS = new Quad(212); // page 19 - locate / transfer left / right
        public static Quad STP_ADDRESS = new Quad(216); // page 19 - CTC, Tally

        const int INSTRUCTION_LINE_LENGTH = 20; // Length of the line for mnemonic representation

        bool mRelativeAddressing;

        readonly Expression mExpression_N;
        readonly Expression mExpression_A;
        readonly Expression mExpression_B;

        Character mOpcode;
        Character mN;
        Quad mA;
        Quad mB;

        protected bool RelativeAddressing => mRelativeAddressing;

        public readonly InstructionIdentifier Identifier;
        public readonly int LineIndex;
        public readonly int Address;

        protected InstructionBase(InstructionIdentifier id, AssemblyLine asmLine)
        {
            Identifier = id;
            LineIndex = asmLine.LineIndex;
            Address = -1;

            if (asmLine.Expressions.Length != 3)
                throw new AssembleException($"Invalid number of expressions", asmLine.LineIndex);

            mExpression_N = asmLine.Expressions[0];
            mExpression_A = asmLine.Expressions[1];
            mExpression_B = asmLine.Expressions[2];
        }

        protected InstructionBase(InstructionIdentifier id, InstructionData data, int address)
        {
            Identifier = id;
            LineIndex = -1;
            Address = address;

            mOpcode = data.OperationCode;
            mN = data.N_Value;
            mA = data.A_Value;
            mB = data.B_Value;
        }

        public void Parse(int baseDataAddress, ExpressionCalculator expressionCalculator)
        {
            if (!InstructionCreator.TryGetOpcode(Identifier, out mOpcode))
                throw new AssembleException($"Cannot find the opcode for the instruction", LineIndex);

            Quad n = GetExpressionValue(expressionCalculator, mExpression_N, "N Character");
            if (n.Value > Character.Max.OverflownValue)
                throw new AssembleException($"The expression for N Character is too big", LineIndex);

            mN = n.Lo.Lo;

            mA = GetExpressionValue(expressionCalculator, mExpression_A, "A address");
            mB = GetExpressionValue(expressionCalculator, mExpression_B, "B address");
        }

        public InstructionData GetData()
        {
            return new InstructionData(mOpcode, mN, mA, mB);
        }

        Quad GetExpressionValue(ExpressionCalculator expressionCalculator, Expression expression, string description)
        {
            Expression val = expressionCalculator.Calculate(expression, LineIndex);

            if (val is IntExpression)
                return ((IntExpression)val).GetValue();

            if (val is StringExpression)
                return ((StringExpression)val).GetValue();

            throw new AssembleException($"The expression for {description} could not be evaluated", LineIndex);
        }

        public void Execute(CPU cpu, Memory memory)
        {
            cpu.Reg_N = mN;
            cpu.Reg_NOR = mOpcode;
            cpu.Reg_A = SolveAddress(mA, memory);
            cpu.Reg_B = SolveAddress(mB, memory);

            Perform(cpu, memory);
            Advance(cpu, memory);
        }

        protected virtual void Perform(CPU cpu, Memory memory)
        {
        }

        protected virtual void Advance(CPU cpu, Memory memory)
        {
            cpu.Reg_P = (int)cpu.Reg_P + INSTRUCTION_SIZE;
        }

        protected Quad SolveAddress(Quad address, Memory memory)
        {
            if (address.IsRelativeAddress)
                mRelativeAddressing = true;

            while (address.IsRelativeAddress)
            {
                Character[] newAddress = memory.GetCharactersAt((int)address.GetNonRelativeVariant(), 4);
                address = new Quad(newAddress);
            }

            return address;
        }

        protected void AutomaticStore_Prev_A(CPU cpu, Memory memory)
        {
            memory.SetCharacters(STA_ADDRESS.Value, cpu.PrevReg_A.GetData());
        }

        protected void AutomaticStore_P(CPU cpu, Memory memory)
        {
            Quad nextP = cpu.Reg_P.Value + INSTRUCTION_SIZE;
            memory.SetCharacters(STP_ADDRESS.Value, nextP.GetData());
        }

        public override string ToString()
        {
            return ToString(Identifier, mN, mA.DisplayString, mB.DisplayString);
        }

        public static string ToString(InstructionIdentifier id, Character N, string A, string B)
        {
            StringBuilder res = new StringBuilder(INSTRUCTION_LINE_LENGTH);
            res.Append(id.ToString().ToUpper());
            res.Append(" ");
            res.Append(N.DisplayString);
            res.Append(", ");
            res.Append(A);
            res.Append(", ");
            res.Append(B);
            return res.ToString();
        }
    }
}
