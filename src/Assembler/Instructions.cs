using Rca301Emulator.Emulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca301Emulator.Assembler
{
    class InstructionHalt : InstructionBase
    {
        public InstructionHalt(AssemblyLine asmLine)
            : base(InstructionIdentifier.Hlt, asmLine)
        {
        }

        public InstructionHalt(InstructionData data, int address)
            : base(InstructionIdentifier.Hlt, data, address)
        {
        }

        protected override void Advance(CPU cpu, Memory memory)
        {
            // Avoid instruction pointer advance.
        }
    }

    class InstructionOutput : InstructionBase
    {
        Character[] mOutputCharacters;

        // The emulator should handle the output characters
        public Character[] OutputCharacters => mOutputCharacters;

        public InstructionOutput(InstructionIdentifier id, AssemblyLine asmLine)
            : base(id, asmLine)
        {
        }

        public InstructionOutput(InstructionIdentifier id, InstructionData data, int address)
            : base(id, data, address)
        {
        }

        protected override void Perform(CPU cpu, Memory memory)
        {
            int startAddress = cpu.Reg_A.Value;
            int endAddress = cpu.Reg_B.Value; // the character at that address must be included

            mOutputCharacters = memory.GetCharactersAt(cpu.Reg_A.Value, endAddress - startAddress + 1);

            cpu.Reg_A = endAddress + 1;
        }
    }

    class InstructionSymbolFill : InstructionBase
    {
        public InstructionSymbolFill(AssemblyLine asmLine)
            : base(InstructionIdentifier.SF, asmLine)
        {
        }

        public InstructionSymbolFill(InstructionData data, int address)
            : base(InstructionIdentifier.SF, data, address)
        {
        }

        protected override void Perform(CPU cpu, Memory memory)
        {
            int startAddress = cpu.Reg_A.Value;
            int endAddress = cpu.Reg_B.Value;

            for (int i = startAddress; i <= endAddress; i++)
                memory.SetCharacters(i, new Character[] { cpu.Reg_N });

            cpu.Reg_A = endAddress + 1;
        }
    }

    class InstructionCompare : InstructionBase
    {
        public InstructionCompare(AssemblyLine asmLine)
            : base(InstructionIdentifier.COM, asmLine)
        {
        }

        public InstructionCompare(InstructionData data, int address)
            : base(InstructionIdentifier.COM, data, address)
        {
        }

        protected override void Perform(CPU cpu, Memory memory)
        {
            int firstAddress = cpu.Reg_A.Value;
            int secondAddress = cpu.Reg_B.Value;
            int charCount = cpu.Reg_N.Value;

            cpu.PRI = PreviousResultIndicator.Zero;

            Character first = memory.GetCharacterAt(firstAddress);
            Character second = memory.GetCharacterAt(secondAddress);
            if (first.OverflownValue != second.OverflownValue)
            {
                cpu.PRI = first.OverflownValue > second.OverflownValue ?
                    PreviousResultIndicator.Positive : PreviousResultIndicator.Negative;
                cpu.Reg_A = firstAddress + 1;
                cpu.Reg_B = secondAddress + 1;
                return;
            }

            for (int i = 1; i < charCount; i++)
            {
                Character f = memory.GetCharacterAt(firstAddress + i);
                Character s = memory.GetCharacterAt(secondAddress + i);
                if (f.Value != s.Value)
                {
                    cpu.PRI = f.Value > s.Value ?
                        PreviousResultIndicator.Positive : PreviousResultIndicator.Negative;
                    cpu.Reg_A = firstAddress + i + 1;
                    cpu.Reg_B = secondAddress + i + 1;
                    return;
                }
            }

            cpu.Reg_A = firstAddress + charCount;
            cpu.Reg_B = secondAddress + charCount;
        }
    }

    class InstructionCtc : InstructionBase
    {
        public InstructionCtc(AssemblyLine asmLine)
            : base(InstructionIdentifier.CTC, asmLine)
        {
        }

        public InstructionCtc(InstructionData data, int address)
            : base(InstructionIdentifier.CTC, data, address)
        {
        }

        protected override void Advance(CPU cpu, Memory memory)
        {
            switch (cpu.Reg_N.Value)
            {
                case 0:
                    //AutomaticStore_P(cpu, memory);
                    //cpu.Reg_P = cpu.Reg_A;
                    base.Advance(cpu, memory);
                    return;
                case 1:
                    if (cpu.PRI == PreviousResultIndicator.Zero)
                    {
                        base.Advance(cpu, memory);
                        return;
                    }

                    AutomaticStore_P(cpu, memory);
                    cpu.Reg_P = cpu.PRI == PreviousResultIndicator.Positive ? cpu.Reg_A : cpu.Reg_B;
                    return;
                default:
                    throw new EmulatorException($"Parameter N canot be {cpu.Reg_N.Value} for CTC", Address);
            }
        }
    }

    class InstructionTransferData : InstructionBase
    {
        public InstructionTransferData(InstructionIdentifier id, AssemblyLine asmLine)
            : base(id, asmLine)
        {
        }

        public InstructionTransferData(InstructionIdentifier id, InstructionData data, int address)
            : base(id, data, address)
        {
        }

        protected override void Perform(CPU cpu, Memory memory)
        {
            int sourceAddress = cpu.Reg_A.Value;
            int destAddress = cpu.Reg_B.Value;
            int charCount = cpu.Reg_N.Value;

            int directionSign = Identifier == InstructionIdentifier.DL ? 1 : -1;

            for (int i = 0; i < charCount; i++)
            {
                Character charToCopy = memory.GetCharacterAt(sourceAddress + directionSign * i);
                memory.SetCharacter(destAddress + directionSign * i, charToCopy);
            }

            cpu.Reg_A = sourceAddress + directionSign * charCount;
            cpu.Reg_B = destAddress + directionSign * charCount;
            cpu.Reg_N = 0;
        }
    }

    class InstructionLocateSymbol : InstructionBase
    {
        public InstructionLocateSymbol(InstructionIdentifier id, AssemblyLine asmLine)
            : base(id, asmLine)
        {
        }

        public InstructionLocateSymbol(InstructionIdentifier id, InstructionData data, int address)
            : base(id, data, address)
        {
        }

        protected override void Perform(CPU cpu, Memory memory)
        {
            int startAddress = cpu.Reg_A.Value;
            int endAddress = cpu.Reg_B.Value;
            Character charToLocate = cpu.Reg_N;

            int directionSign = Identifier == InstructionIdentifier.LSL ? 1 : -1;

            cpu.PRI = PreviousResultIndicator.Zero;

            Character ch = memory.GetCharacterAt(startAddress);
            if (ch.Value != charToLocate.Value)
            {
                cpu.PRI = PreviousResultIndicator.Negative;
                cpu.Reg_A = startAddress - directionSign;
                return;
            }

            while (startAddress != endAddress)
            {
                startAddress += directionSign;

                ch = memory.GetCharacterAt(startAddress);
                if (ch.Value != charToLocate.Value)
                {
                    cpu.PRI = PreviousResultIndicator.Positive;
                    cpu.Reg_A = startAddress - directionSign;
                    return;
                }
            }

            cpu.Reg_A = endAddress;
        }
    }

    class InstructionStoreRegister : InstructionBase
    {
        public InstructionStoreRegister(AssemblyLine asmLine)
            : base(InstructionIdentifier.Reg, asmLine)
        {
        }

        public InstructionStoreRegister(InstructionData data, int address)
            : base(InstructionIdentifier.Reg, data, address)
        {
        }

        protected override void Perform(CPU cpu, Memory memory)
        {
            switch (cpu.Reg_N.Value)
            {
                case 0:
                    cpu.Reg_A = cpu.PrevReg_A;
                    cpu.Reg_B = cpu.PrevReg_B;
                    return;
                case 2: // Store A
                    AutomaticStore_Prev_A(cpu, memory);
                    cpu.Reg_A = cpu.PrevReg_A;
                    cpu.Reg_B = cpu.PrevReg_B;
                    break;
                case 4: // Store B
                    memory.SetCharacters(cpu.Reg_A.Value - 3, cpu.PrevReg_B.GetData());
                    cpu.Reg_A = cpu.Reg_A.Value - 2;
                    cpu.Reg_B = cpu.PrevReg_B;
                    break;
                case 1: // Store P
                    memory.SetCharacters(cpu.Reg_A.Value - 3, cpu.Reg_P.GetData());
                    cpu.Reg_A = cpu.Reg_A.Value - 2;
                    break;
                default:
                    throw new EmulatorException($"Parameter N canot be {cpu.Reg_N.Value} for REG", Address);
            }
        }

        protected override void Advance(CPU cpu, Memory memory)
        {
            switch (cpu.Reg_N.Value)
            {
                case 1: // Store P
                    cpu.Reg_P = cpu.Reg_B;
                    return;
            }

            base.Advance(cpu, memory);
        }
    }

    class InstructionBitOperation : InstructionBase
    {
        public InstructionBitOperation(InstructionIdentifier id, AssemblyLine asmLine)
            : base(id, asmLine)
        {
        }

        public InstructionBitOperation(InstructionIdentifier id, InstructionData data, int address)
            : base(id, data, address)
        {
        }

        protected override void Perform(CPU cpu, Memory memory)
        {
            int firstAddress = cpu.Reg_A.Value;
            int secondAddress = cpu.Reg_B.Value;
            int charCount = cpu.Reg_N.Value;

            for (int i = 0; i < charCount; i++)
            {
                Character f = memory.GetCharacterAt(firstAddress);
                Character s = memory.GetCharacterAt(secondAddress);
                switch (Identifier)
                {
                    case InstructionIdentifier.OR:
                        memory.SetCharacter(firstAddress, f.GetOr(s));
                        break;
                    case InstructionIdentifier.AND:
                        memory.SetCharacter(firstAddress, f.GetAnd(s));
                        break;
                }

                firstAddress--;
                secondAddress--;
            }

            cpu.Reg_A = firstAddress;
            cpu.Reg_B = secondAddress;
        }
    }

    /*class InstructionTally : InstructionBase
    {
        public InstructionTally(AssemblyLine asmLine)
            : base(InstructionIdentifier.TA, asmLine)
        {
        }

        public InstructionTally(InstructionData data, int address)
            : base(InstructionIdentifier.TA, data, address)
        {
        }

        protected override void Perform(CPU cpu, Memory memory)
        {
        }
    }*/
}
