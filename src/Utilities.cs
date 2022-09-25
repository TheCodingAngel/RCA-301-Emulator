using Rca301Emulator.Emulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca301Emulator
{
    class AssembleException : Exception
    {
        // The user treats line 0 as line 1 so increment the line index
        public readonly int LineNumber;

        public AssembleException(string message, int lineIndex)
            : base(message + $" at line {lineIndex + 1}.")
        {
            LineNumber = lineIndex + 1;
        }
    }

    class EmulatorException : Exception
    {
        public readonly int Address;

        public EmulatorException(string message, int address)
            : base(message + $" at address \"{address}\".")
        {
            Address = address;
        }
    }

    enum ViewType
    {
        Characters,
        Hexadecimals,
        Decimals,
    }

    interface IAssemblerEnvironment
    {
        void SelectAssemblySourceLine(int lineIndex);
        void SelectNextAssemblyInstructionInSource();
        void ClearAssemblySourceSelection();

        void ShowMemoryAddress(int address);
        void RefreshMemory();

        void OutputCharacters(Character[] chars);
        void Halt();
    }

    static class Extensions
    {
        public static string[] Trim(this string[] strings)
        {
            if (strings == null)
                return null;

            List<string> res = new List<string>(strings.Length);

            foreach (string str in strings)
            {
                string s = str.Trim();
                if (!string.IsNullOrEmpty(s))
                    res.Add(s);
            }

            return res.ToArray();
        }

        public static bool TryGetEnumValue<T>(this string str, out T enumValue)
            where T : struct
        {
            foreach (T id in typeof(T).GetEnumValues())
            {
                if (string.Compare(str, id.ToString(), true) == 0)
                {
                    enumValue = id;
                    return true;
                }
            }

            enumValue = default(T);
            return false;
        }

        public static string DataToString(this Character[] data, char? delimiter = null)
        {
            if (data == null || data.Length <= 0)
                return null;

            StringBuilder res = new StringBuilder(data.Length * 2);

            foreach (Character ch in data)
            {
                res.Append(ch.Char);
                if (delimiter != null)
                    res.Append(delimiter.Value);
            }

            return res.ToString();
        }

        public static int HIWORD(this int n)
        {
            return ((n >> 0x10) & 0xffff);
        }

        public static int HIWORD(this IntPtr n)
        {
            return HIWORD((int)((long)n));
        }

        public static int LOWORD(this int n)
        {
            return (n & 0xffff);
        }

        public static int LOWORD(this IntPtr n)
        {
            return LOWORD((int)((long)n));
        }
    }
}
