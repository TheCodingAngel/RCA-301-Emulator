using Rca301Emulator.Emulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca301Emulator.Assembler
{
    class InstructionMath : InstructionBase
    {
        enum ComplementType
        {
            None,
            First,
            Second
        }

        public InstructionMath(InstructionIdentifier id, AssemblyLine asmLine)
            : base(id, asmLine)
        {
        }

        public InstructionMath(InstructionIdentifier id, InstructionData data, int address)
            : base(id, data, address)
        {
        }

        protected override void Perform(CPU cpu, Memory memory)
        {
            // The result must go in the first address
            int firstAddress = cpu.Reg_A.Value; // augend or minuend
            int secondAddress = cpu.Reg_B.Value; // addend or subtrahend
            int charNum = cpu.Reg_N.OverflownValue;

            Character first = memory.GetCharacterAt(firstAddress);
            Character second = memory.GetCharacterAt(secondAddress);

            // Default value is for sum of negatives or for subtract of negative and positive
            bool makeResultNegative = first.IsNegative;

            ComplementType ct = ComplementType.None;

            bool effectiveSum =
                (Identifier == InstructionIdentifier.Add && first.IsNegative == second.IsNegative) ||
                (Identifier == InstructionIdentifier.Sub && first.IsNegative != second.IsNegative);

            if (effectiveSum) // abs(a) + abs(b) or -(abs(a) + abs(b))
            {
                makeResultNegative = first.IsNegative;
                ct = ComplementType.None;
            }
            else // abs(a) - abs(b) or abs(b) - abs(a)
            {
                int cmp = Compare(memory, firstAddress, secondAddress, charNum);

                // Complement the smaller number to avoid complementing of all numbers in the result
                ct = cmp > 0 ? ComplementType.Second : ComplementType.First;

                if (first.IsNegative)
                    makeResultNegative = cmp > 0;
                else
                    makeResultNegative = cmp < 0;
            }

            if (ComplementaryAdd(ct, memory, firstAddress, secondAddress, charNum))
                cpu.PRI = PreviousResultIndicator.Zero;
            else
                cpu.PRI = makeResultNegative ? PreviousResultIndicator.Negative : PreviousResultIndicator.Positive;

            if (makeResultNegative)
                InvertResultSign(memory, firstAddress);

            // Possible address arithmetics - keep indirect addressing in the output
            if (charNum == 4)
            {
                Quad qFirst = new Quad(0, 0, 0, first);
                Quad qSecond = new Quad(0, 0, 0, second);

                if (qFirst.IsRelativeAddress || qSecond.IsRelativeAddress)
                    MakeResultRelative(memory, firstAddress);
            }

            cpu.Reg_A = firstAddress - charNum;
            cpu.Reg_B = secondAddress - charNum;
        }

        void InvertResultSign(Memory memory, int resultAddress)
        {
            Character ch = memory.GetCharacterAt(resultAddress);
            memory.SetCharacter(resultAddress, ch.GetNegative());
        }

        void MakeResultRelative(Memory memory, int resultAddress)
        {
            Quad q = new Quad(0, 0, 0, memory.GetCharacterAt(resultAddress));
            memory.SetCharacter(resultAddress, q.GetRelativeVariant().Lo.Lo);
        }

        // Compares absolute values (ignores the sign)
        int Compare(Memory memory, int firstAddress, int secondAddress, int charNum)
        {
            int firstStart = firstAddress - (charNum - 1);
            int secondStart = secondAddress - (charNum - 1);

            int first = memory.GetCharacterAt(firstStart).Zone;
            int second = memory.GetCharacterAt(secondStart).Zone;
            if (first != second)
                return first > second ? 1 : -1;

            for (int i = 0; i < charNum; i++)
            {
                first = memory.GetCharacterAt(firstStart + i).Digit;
                second = memory.GetCharacterAt(secondStart + i).Digit;
                if (first != second)
                    return first > second ? 1 : -1;
            }

            return 0;
        }

        // returns true if the result is zero
        bool ComplementaryAdd(ComplementType ct, Memory memory, int firstAddress, int secondAddress, int charNum)
        {
            bool isZero = true;
            int carry = 0;

            for (int i = 0; i < charNum - 1; i++)
            {
                int first = memory.GetCharacterAt(firstAddress).Digit;
                int second = memory.GetCharacterAt(secondAddress).Digit;

                //int sum = first + second + carry;
                int sum = SumComplement(ct, first, second, i == 0) + carry;
                
                carry = sum / 10;
                sum = sum % 10;
                if (sum != 0)
                    isZero = false;

                memory.SetCharacter(firstAddress, sum);
                firstAddress--;
                secondAddress--;
            }

            // The most significant character can be overflow.
            // The zone bits are the additional digit for that (0 to 3).
            Character mscFirst = memory.GetCharacterAt(firstAddress);
            Character mscSecond = memory.GetCharacterAt(secondAddress);

            int lowerDigit = SumComplement(ct, mscFirst.Digit, mscSecond.Digit, false) + carry;

            carry = lowerDigit / 10;
            lowerDigit = lowerDigit % 10;
            if (lowerDigit != 0)
                isZero = false;

            int higherDigit = SumComplement(ct, mscFirst.Zone, mscSecond.Zone, false) + carry;
            higherDigit = higherDigit % 10;
            if (higherDigit != 0)
                isZero = false;

            if (higherDigit <= 3)
                memory.SetCharacter(firstAddress, CharacterMap.FromOverflownValue(10* higherDigit + lowerDigit));
            else
                memory.SetCharacter(firstAddress, Character.Max);

            return isZero;
        }

        static int SumComplement(ComplementType ct, int first, int second, bool areLessSignificantDigit)
        {
            int complement = areLessSignificantDigit ? 10 : 9;
            switch (ct)
            {
                case ComplementType.First:
                    return complement - first + second;
                case ComplementType.Second:
                    return first + complement - second;
                default:
                    return first + second;
            }
        }
    }
}
