using Rca301Emulator.Emulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca301Emulator.Assembler
{
    class BaseIdentifier
    {
        public string Name { get; }
        public int LineIndex { get; }

        protected BaseIdentifier(string name, int lineIndex)
        {
            Name = name;
            LineIndex = lineIndex;
        }
    }

    enum EmulatorIdentifier
    {
        ORG, // Absolute address where the code starts
    }

    class EmulatorParameter : BaseIdentifier
    {
        public int Value { get; }

        public EmulatorParameter(AssemblyLine asmLine)
            : base(asmLine.Identifier, asmLine.LineIndex)
        {
            if (asmLine.Expressions.Length != 1)
                throw new AssembleException($"Emulator declaration requires one expression", asmLine.LineIndex);

            Expression inputValue = asmLine.Expressions[0];

            if (inputValue is IntExpression)
                Value = ((IntExpression)inputValue).GetValue().Value;
            else
                throw new AssembleException($"Expression \"{inputValue.Text}\" is not an integer constant", LineIndex);
        }
    }

    class Label : BaseIdentifier
    {
        public int Offset { get; set; }

        public Label(int offset, AssemblyLine asmLine)
            : base(asmLine.Identifier, asmLine.LineIndex)
        {
            Offset = offset;
        }
    }

    class ValueIdentifier : BaseIdentifier
    {
        object mValue;

        public readonly Expression InputValue;

        public object Value => mValue;

        protected ValueIdentifier(string name, AssemblyLine asmLine)
            : base(name, asmLine.LineIndex)
        {
            if (asmLine.Expressions.Length < 2)
                throw new AssembleException($"At least one expression for identifier's value is required", asmLine.LineIndex);

            InputValue = asmLine.Expressions[asmLine.Expressions.Length - 1];
        }

        public virtual void CalculateValue(ExpressionCalculator expressionCalculator)
        {
            Expression valueExpression = expressionCalculator.Calculate(InputValue, LineIndex);
            if (valueExpression is IntExpression)
                mValue = ((IntExpression)valueExpression).GetValue();
            else if (valueExpression is StringExpression)
                mValue = ((StringExpression)valueExpression).GetValue();
            else
                throw new AssembleException($"Could not evaluate expression \"{valueExpression.Text}\"", LineIndex);
        }
    }

    enum ConstantIdentifier
    {
        CStr, // String constant
        CInt, // Integer constant
    }

    class Constant : ValueIdentifier
    {
        public Constant(ConstantIdentifier id, AssemblyLine asmLine)
            : base(asmLine.Expressions[0].Text, asmLine)
        {
            if (asmLine.Expressions.Length != 2)
                throw new AssembleException($"Constant doesn't have exactly one expression for its value", asmLine.LineIndex);
        }
    }

    enum VariableIdentifier
    {
        DC, // Define Character (or string if using '...' syntax)
        DD, // Define Diad
        DQ, // Define Quad
    }

    class Variable : ValueIdentifier
    {
        public readonly Expression AbsoluteOffset;

        public int Size { get; }
        public int Offset { get; set; }

        public Variable(VariableIdentifier id, int variableOffset, AssemblyLine asmLine)
            : base(asmLine.Expressions[0].Text, asmLine)
        {
            if (asmLine.Expressions.Length > 3)
                throw new AssembleException($"Too many expressions for the value identifier", asmLine.LineIndex);

            AbsoluteOffset = asmLine.Expressions.Length == 3 ? asmLine.Expressions[1] : null;

            Offset = variableOffset;

            switch (id)
            {
                case VariableIdentifier.DD:
                    Size = 2;
                    break;
                case VariableIdentifier.DQ:
                    Size = 4;
                    break;
                case VariableIdentifier.DC:
                    if (!(InputValue is StringExpression))
                        Size = 1;
                    else
                        Size = ((StringExpression)InputValue).GetValue().Length;
                    break;
                default:
                    throw new AssembleException($"Not implemented variable identifier {id}", asmLine.LineIndex);
            }
        }

        public override void CalculateValue(ExpressionCalculator expressionCalculator)
        {
            base.CalculateValue(expressionCalculator);

            if (AbsoluteOffset == null)
                return;

            Expression valueExpression = expressionCalculator.Calculate(AbsoluteOffset, LineIndex);
            if (valueExpression is IntExpression)
                Offset = ((IntExpression)valueExpression).GetValue().Value;
            else 
                throw new AssembleException($"Could not evaluate expression \"{valueExpression.Text}\"", LineIndex);
        }

        public Character[] GetData()
        {
            if (Value is Quad)
            {
                Quad v = (Quad)Value;

                switch (Size)
                {
                    case 1:
                        return new Character[] { v.Lo.Lo };
                    case 2:
                        return v.Lo.GetData();
                    case 4:
                        return v.GetData();
                    default:
                        throw new AssembleException($"Incorrect size for numeric variable", LineIndex);
                }
            }

            if (Value is string)
            {
                string str = (string)Value;

                Character[] res = new Character[str.Length];
                for (int i = 0; i < str.Length; i++)
                    res[i] = str[i];

                return res;
            }

            throw new AssembleException($"Unsupported data type", LineIndex);
        }
    }
}
