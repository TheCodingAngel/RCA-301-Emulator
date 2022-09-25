using Rca301Emulator.Assembler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca301Emulator.Emulator
{
    class AddressPosition
    {
        public static AddressPosition Empty = new AddressPosition(0, 0);

        public int Row { get; }
        public int Column { get; }

        public AddressPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public AddressPosition Offest(int rows, int columns)
        {
            return new AddressPosition(Row + rows, Column + columns);
        }
    }

    class Memory : IDisposable
    {
        // The first 225 characters are reserved by spec; extend it with some padding
        public const int FIRST_AVAILABLE_ADDRESS = 226 + Quad.NUM_CHARACTERS;

        const int DEFAULT_CHARACTERS_IN_ROW = InstructionBase.INSTRUCTION_SIZE;
        const int MIN_CHARACTERS_IN_ROW = 1;

        Character[] mContainer;
        int mCharactersInRow;

        int mNumRows;
        int mLastRowCharacters;

        int mProgramOffset;

        int mDataOffset;
        int mDataSize;


        public int Capacity => mContainer.Length;
        public int CharactersInRow => mCharactersInRow;

        public int NumRows => mNumRows;
        public int LastRowCharacter => mLastRowCharacters;

        public int ProgramOffset => mProgramOffset;
        public int DataOffset => mDataOffset;
        public int DataSize => mDataSize;

        public AddressPosition ProgramOffsetPosition => GetAddressPosition(mProgramOffset);
        public AddressPosition DataOffsetPosition => GetAddressPosition(mDataOffset);


        public Memory(int capacity)
        {
            mContainer = new Character[capacity];

            for (int i = 0; i < capacity; i++)
                mContainer[i] = (byte)i;

            mCharactersInRow = DEFAULT_CHARACTERS_IN_ROW;
        }

        public void Dispose()
        {
            mContainer = null;
        }

        public void ClearMemory()
        {
            Array.Clear(mContainer, 0, mContainer.Length);
        }

        public void ApplyMemoryParameters(int programOffset, int dataOffset, int dataSize)
        {
            mProgramOffset = programOffset;
            mDataOffset = dataOffset;
            mDataSize = dataSize;

            Update();
        }

        public void SetCharactersInRow(int charactersInRow)
        {
            int newValue = Math.Max(charactersInRow, MIN_CHARACTERS_IN_ROW);
            if (mCharactersInRow == newValue)
                return;

            mCharactersInRow = newValue;

            Update();
        }

        public Character GetCharacterAt(int address)
        {
            return mContainer[address];
        }

        public Character[] GetCharactersAt(int address, int numCharacters)
        {
            if (address < 0 || numCharacters < 1 || address + numCharacters >= mContainer.Length)
                return new Character[0];

            Character[] res = new Character[numCharacters];
            Array.Copy(mContainer, address, res, 0, numCharacters);
            return res;
        }

        public InstructionData GetInstructionDataAt(int address)
        {
            return new InstructionData(GetCharactersAt(address, InstructionBase.INSTRUCTION_SIZE));
        }

        public Character[] GetRowOfCharacters(int row)
        {
            if (row >= mNumRows)
                return new Character[0];

            Character[] res;

            if (row == mNumRows - 1)
            {
                res = new Character[mLastRowCharacters];
                Array.Copy(mContainer, row * mCharactersInRow, res, 0, mLastRowCharacters);
            }
            else
            {
                res = new Character[mCharactersInRow];
                Array.Copy(mContainer, row * mCharactersInRow, res, 0, mCharactersInRow);
            }

            return res;
        }

        public void SetCharacter(int address, Character ch)
        {
            mContainer[address] = ch;
        }

        public void SetCharacters(int address, Character[] chars)
        {
            Array.Copy(chars, 0, mContainer, address, chars.Length);
        }

        public void SetStringValue(int address, string value)
        {
            if (string.IsNullOrEmpty(value) || address + value.Length > mContainer.Length)
                return;

            for (int i = 0; i < value.Length; i++)
                mContainer[i + address] = value[i];
        }

        void Update()
        {
            mNumRows = mContainer.Length / mCharactersInRow;
            mLastRowCharacters = mContainer.Length % mCharactersInRow;

            if (mLastRowCharacters == 0)
                mLastRowCharacters = mCharactersInRow;
            else
                mNumRows++;
        }

        AddressPosition GetAddressPosition(int address)
        {
            return new AddressPosition(address / mCharactersInRow, address % mCharactersInRow);
        }
    }
}
