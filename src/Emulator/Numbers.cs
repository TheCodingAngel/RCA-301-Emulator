using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca301Emulator.Emulator
{
    interface INumber<T>
    {
        int Value { get; }

        bool IsNegative { get; }
        T GetNegative();
    }

    struct Diad : IComparable<Diad>, INumber<Diad>
    {
        public const int NUM_CHARACTERS = 2;

        public static Diad Zero = new Diad(0, 0);
        public static Diad Max = new Diad(Character.Max, 9);

        Character mHi;
        Character mLo;

        public int Value => Number.GetValue(mHi, mLo);
        public string DisplayString => mHi.DisplayString + mLo.DisplayString;

        public Character Hi => mHi;
        public Character Lo => mLo;
        public bool IsNegative => mLo.IsNegative;

        public Diad(Character[] value)
        {
            if (value == null)
            {
                mHi = Character.Zero;
                mLo = Character.Zero;
                return;
            }

            mHi = value.Length > 1 ? value[0] : 0;
            mLo = value.Length > 0 ? value[1] : 0;
        }

        public Diad(Character hi, Character lo)
        {
            mHi = hi;
            mLo = lo;
        }

        public Diad(int val)
        {
            Character[] chars = Number.SplitToCharacters(val, 2);
            mHi = chars[0];
            mLo = chars[1];
        }

        public Diad GetNegative()
        {
            return new Diad(mHi, mLo.GetNegative());
        }

        public static implicit operator Diad(int val)
        {
            return new Diad(val);
        }

        public static implicit operator Diad(string str)
        {
            if (string.IsNullOrEmpty(str))
                return Zero;

            str = str.PadLeft(2, '0');
            Character[] ch = new Character[2];
            for (int i = 0; i < ch.Length; i++)
                ch[i] = str[i];

            return new Diad(ch[0], ch[1]);
        }

        public static explicit operator int(Diad d)
        {
            return d.Value;
        }

        public Character[] GetData()
        {
            return new Character[] { mHi, mLo };
        }

        public override string ToString()
        {
            return DisplayString;
        }

        public int CompareTo(Diad other)
        {
            int res = mHi.CompareTo(other.mHi);

            if (res == 0)
                return mLo.CompareTo(other.mLo);

            return res;
        }
    }

    struct Quad : IComparable<Quad>, INumber<Quad>
    {
        public const int NUM_CHARACTERS = 4;

        const byte RELATIVE_ADDRESS_MASK = 0x10;
        const byte RELATIVE_ZONE_MASK = 0x01;

        public static Quad Zero = new Quad(Diad.Zero, Diad.Zero);
        public static Quad Max = new Quad(Character.Max, 9, 9, 9);

        Diad mHi;
        Diad mLo;

        public int Value => Number.GetValue(mHi.Hi, mHi.Lo, mLo.Hi, mLo.Lo);
        public string DisplayString => mHi.DisplayString + mLo.DisplayString;

        public Diad Hi => mHi;
        public Diad Lo => mLo;
        public bool IsRelativeAddress => (mLo.Lo.Zone & RELATIVE_ZONE_MASK) > 0;
        public bool IsNegative => mLo.IsNegative;

        public Quad(Character[] value)
        {
            if (value == null)
            {
                mHi = Diad.Zero;
                mLo = Diad.Zero;
                return;
            }

            Character hi = value.Length > 1 ? value[0] : 0;
            Character lo = value.Length > 0 ? value[1] : 0;
            mHi = new Diad(hi, lo);

            hi = value.Length > 3 ? value[2] : 0;
            lo = value.Length > 2 ? value[3] : 0;
            mLo = new Diad(hi, lo);
        }

        public Quad(Character c0, Character c1, Character c2, Character c3)
        {
            mHi = new Diad(c0, c1);
            mLo = new Diad(c2, c3);
        }

        public Quad(Diad hi, Diad lo)
        {
            mHi = hi;
            mLo = lo;
        }

        public Quad(int val)
        {
            Character[] chars = Number.SplitToCharacters(val, 4);
            mHi = new Diad(chars[0], chars[1]);
            mLo = new Diad(chars[2], chars[3]);
        }

        public Quad GetNegative()
        {
            return new Quad(mHi, mLo.GetNegative());
        }

        public Quad GetRelativeVariant()
        {
            return new Quad(mHi.Hi, mHi.Lo, mLo.Hi,
                (byte)(mLo.Lo.Digit | RELATIVE_ADDRESS_MASK));
        }

        public Quad GetNonRelativeVariant()
        {
            return new Quad(mHi.Hi, mHi.Lo, mLo.Hi, mLo.Lo.Digit);
        }

        public static implicit operator Quad(string str)
        {
            if (string.IsNullOrEmpty(str))
                return Zero;

            str = str.PadLeft(4, '0');
            Character[] ch = new Character[4];
            for (int i = 0; i < ch.Length; i++)
                ch[i] = str[i];

            return new Quad(ch);
        }

        public static implicit operator Quad(int val)
        {
            return new Quad(val);
        }

        public static explicit operator int(Quad q)
        {
            return q.Value;
        }

        public Character[] GetData()
        {
            return new Character[] { mHi.Hi, mHi.Lo, mLo.Hi, mLo.Lo };
        }

        public override string ToString()
        {
            return DisplayString;
        }

        public int CompareTo(Quad other)
        {
            int res = mHi.CompareTo(other.mHi);

            if (res == 0)
                return mLo.CompareTo(other.mLo);

            return res;
        }
    }

    class Number
    {
        const int CHARACTER_BITS = 6;
        const int DIGIT_MASK = 0xF; // the first 4 bits define the decimal digit

        public static int GetLeastSixBits(int val)
        {
            return val & 0x3F;
        }

        public static int GetMaxValue(int numCharacters)
        {
            int res = 1 << (numCharacters * CHARACTER_BITS);
            return res - 1;
        }

        public static int GetValue(params Character[] characters)
        {
            Debug.Assert(characters != null && characters.Length > 1 && characters.Length <= 4);

            int res = characters[0].OverflownValue;

            for (int i = 1; i < characters.Length; i++)
            {
                res *= 10;
                res += characters[i].Digit;
            }

            return !characters[characters.Length - 1].IsNegative ? res : -res;
        }

        public static Character[] SplitToCharacters(int val, int maxCharacterCount, bool trimHiZeroes = false)
        {
            List<Character> res = new List<Character>(maxCharacterCount);
            bool isInputNegative = val < 0;
            int x = Math.Abs(val);

            for (int i = 0; i < res.Capacity - 1; i++)
            {
                res.Insert(0, new Character(x % 10));
                x /= 10;
            }

            // Leave the most significant character to be bigger than 9
            res.Insert(0, CharacterMap.FromOverflownValue(x));

            if (trimHiZeroes)
            {
                // Keep the last zero - we want to return at least one digit
                while (res.Count > 1 && res[0].Value == 0)
                    res.RemoveAt(0);
            }

            if (isInputNegative)
                res[res.Count - 1] = res[res.Count - 1].GetNegative();

            return res.ToArray();
        }
    }
}
