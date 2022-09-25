using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca301Emulator.Emulator
{
    struct Character : IComparable<Character>, INumber<Character>
    {
        const byte NEGATIVE_MASK = 0x20;

        public static Character Zero = new Character(0);
        public static Character Max = new Character(Number.GetMaxValue(1));

        byte mValue;

        public int Value => !IsNegative ? mValue : -(int)GetNegative();
        public byte Zone => (byte)(mValue >> 4);
        public byte Digit => (byte)(mValue & 0xF);

        public bool IsNegative => (mValue & NEGATIVE_MASK) > 0;
        public int OverflownValue => Zone * 10 + Digit; // Used for most significant characters

        public char Char => CharacterMap.GetChar(mValue);
        public string DisplayString => Char.ToDisplayString();
        //public string Hex => mValue.ToString("X");
        //public string Dec => mValue.ToString();

        public Character(char val)
        {
            mValue = CharacterMap.GetValue(val);
        }

        public Character(int val)
        {
            mValue = (byte)(val >= 0 ?
                Number.GetLeastSixBits(val) :
                Number.GetLeastSixBits(-val) | NEGATIVE_MASK);
        }

        public Character GetNegative()
        {
            if (!IsNegative)
                return new Character(Number.GetLeastSixBits(mValue | NEGATIVE_MASK));

            return new Character(Number.GetLeastSixBits(mValue & (~NEGATIVE_MASK)));
        }

        public Character GetOr(Character ch)
        {
            return new Character(mValue | ch.mValue);
        }

        public Character GetAnd(Character ch)
        {
            return new Character(mValue & ch.mValue);
        }

        public static implicit operator Character(char ch)
        {
            return new Character(ch);
        }

        public static implicit operator Character(int val)
        {
            return new Character(val);
        }

        public static explicit operator char(Character ch)
        {
            return ch.Char;
        }

        public static explicit operator byte(Character ch)
        {
            return ch.mValue;
        }

        public static explicit operator int(Character ch)
        {
            return ch.Value;
        }

        public override string ToString()
        {
            return DisplayString;
        }

        public int CompareTo(Character other)
        {
            return mValue.CompareTo(other.mValue);
        }
    }

    static class CharacterMap
    {
        const char MISSING_CHARACTER = '\0';
        const byte MISSING_CHAR_VALUE = 63;

        static Dictionary<byte, char> BYTE_TO_CHAR = new Dictionary<byte, char>{
            {00, '0'}, {01, '1'}, {02, '2'}, {03, '3'}, {04, '4'}, {05, '5'}, {06, '6'}, {07, '7'}, {08, '8'}, {09, '9'},
            {10, ' '}, {11, '#'}, {12, '@'}, {13, '('}, {14, ')'},

            {15, MISSING_CHARACTER},

            {16, '&'}, {17, 'A'}, {18, 'B'}, {19, 'C'}, {20, 'D'}, {21, 'E'}, {22, 'F'}, {23, 'G'}, {24, 'H'}, {25, 'I'},
            {26, '+'}, {27, '.'}, {28, ';'}, {29, ':'}, {30, '\''},

            {31, MISSING_CHARACTER},

            {32, '-'}, {33, 'J'}, {34, 'K'}, {35, 'L'}, {36, 'M'}, {37, 'N'}, {38, 'O'}, {39, 'P'}, {40, 'Q'}, {41, 'R'},
            {42, '\x1f'},  // End of Item (EI) - represented as Unit Separator in the ASCII table
            {43, '$'}, {44, '*'},
            {45, '\x1e'}, // End of Data (ED) - represented as Record Separator in the ASCII table
            {46, '\x1c'}, // End of File (EF) - represneted as File Separator in the ASCII table

            {47, MISSING_CHARACTER},

            {48, '"'}, {49, '/'}, {50, 'S'}, {51, 'T'}, {52, 'U'}, {53, 'V'}, {54, 'W'}, {55, 'X'}, {56, 'Y'}, {57, 'Z'},
            {58, '\x1d'}, // End of Block (EB) - represneted as Group Separator in the ASCII table
            {59, ','}, {60, '%'},
            {61, '\x1'}, // Item Separator Symbol (ISS) - represented as Start Of Heading in the ASCII table
            {62, '='},

            {63, MISSING_CHARACTER},
        };

        public static char GetChar(byte val)
        {
            char res;
            if (BYTE_TO_CHAR.TryGetValue(val, out res))
                return res;

            return MISSING_CHARACTER;
        }

        public static string ToDisplayString(this char ch)
        {
            switch (ch)
            {
                case '\0':
                    return "?";
                case '\x1f':
                    return "EI";
                case '\x1e':
                    return "ED";
                case '\x1c':
                    return "EF";
                case '\x1d':
                    return "EB";
                case '\x1':
                    return "ISS";
            }
            return ch.ToString();
        }

        public static byte GetValue(char ch)
        {
            if (ch == MISSING_CHARACTER)
                return MISSING_CHAR_VALUE;

            foreach (KeyValuePair<byte, char> kvp in BYTE_TO_CHAR)
            {
                if (kvp.Value == ch)
                    return kvp.Key;
            }

            return MISSING_CHAR_VALUE;
        }

        public static Character FromOverflownValue(int overflownValue)
        {
            int zone = overflownValue / 10;
            int digit = overflownValue % 10;
            return new Character((zone << 4) + digit);
        }

        public static string ToDisplayString(int overflownValue)
        {
            Character ch = FromOverflownValue(overflownValue);
            return ch.DisplayString;
        }
    }
}
