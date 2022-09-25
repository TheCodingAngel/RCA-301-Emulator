using Rca301Emulator.Emulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca301Emulator.Assembler
{
    enum BinaryExpressionOperation
    {
        Multiply,
        Divide,
        Sum,
        Subtract,
    }

    // Not really abstract - used to prevent instances of this class (only its children should be instanciated).
    abstract class Expression
    {
        public readonly string Text;

        public Expression(string text)
        {
            Text = text;
        }
    }

    class IdentifierExpression : Expression
    {
        public IdentifierExpression(string text)
            : base(text)
        {
        }

        public override string ToString()
        {
            return "identifier " + Text;
        }
    }

    class LabelExpression : Expression
    {
        public LabelExpression()
            : base(string.Empty)
        {
        }

        public override string ToString()
        {
            return "label " + Text;
        }
    }

    class AddressExpression : Expression
    {
        public readonly Expression Pointer;

        public AddressExpression(string text, Expression pointer)
            : base(text)
        {
            Pointer = pointer;
        }

        public override string ToString()
        {
            return "value at address " + Text;
        }
    }

    class StringExpression : Expression
    {
        public StringExpression(string text)
            : base(text)
        {
        }

        public string GetValue()
        {
            return Text.Substring(1, Text.Length - 2);
        }

        public override string ToString()
        {
            return "literal " + Text;
        }
    }

    class IntExpression : Expression
    {
        public IntExpression(string text)
            : base(text)
        {
        }

        public virtual Quad GetValue()
        {
            return Text;
        }

        public override string ToString()
        {
            return "literal " + Text;
        }
    }

    class IndirectAddressExpression : IntExpression
    {
        public readonly Expression Address;

        public IndirectAddressExpression(string text, Expression address)
            : base(text)
        {
            Address = address;
        }

        public override Quad GetValue()
        {
            if (Address is IntExpression)
            {
                Quad address = ((IntExpression)Address).GetValue();
                return address.GetRelativeVariant();
            }

            return Quad.Zero;
        }

        public override string ToString()
        {
            return "indirect variant of address " + Text;
        }
    }

    class BinaryExpression : IntExpression
    {
        public readonly BinaryExpressionOperation Operation;

        public readonly Expression LeftOperand;
        public readonly Expression RightOperand;

        public BinaryExpression(string valueName, BinaryExpressionOperation operation,
            Expression leftOperand, Expression rightOperand)
            : base(valueName)
        {
            Operation = operation;
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        public override string ToString()
        {
            return $"{Operation} of {GetOperandDescription(LeftOperand)} and {GetOperandDescription(RightOperand)}";
        }

        static string GetOperandDescription(Expression operand)
        {
            return (operand is BinaryExpression) ? "(" + operand + ")" : "\"" + operand + "\"";
        }
    }

    class ExpressionCalculator
    {
        Dictionary<string, Constant> mConstants;
        Dictionary<string, Variable> mVariables;
        Dictionary<string, Label> mLabels;

        public ExpressionCalculator(Dictionary<string, Constant> constants,
            Dictionary<string, Variable> variables, Dictionary<string, Label> labels)
        {
            mConstants = constants;
            mVariables = variables;
            mLabels = labels;
        }

        public Expression Calculate(Expression expression, int lineIndex)
        {
            Dictionary<string, Expression> history = new Dictionary<string, Expression>();
            return Calculate(history, expression, lineIndex);
        }

        Expression Calculate(Dictionary<string, Expression> history, Expression expression, int lineIndex)
        {
            // Exclude child classes from the check!
            if (expression.GetType() == typeof(IntExpression))
                return expression;

            if (expression.GetType() == typeof(StringExpression))
                return expression;

            if (history.ContainsKey(expression.Text))
                throw new AssembleException($"Recursive usage of \"{expression.Text}\"", lineIndex);

            history.Add(expression.Text, expression);

            if (expression is AddressExpression)
            {
                AddressExpression ae = (AddressExpression)expression;

                Variable v;
                if (mVariables.TryGetValue(ae.Pointer.Text, out v))
                {
                    return Calculate(history, v.InputValue, lineIndex);
                }

                throw new AssembleException($"Undefined variable \"{ae.Pointer.Text}\"", lineIndex);
            }

            if (expression is IndirectAddressExpression)
            {
                IndirectAddressExpression iae = (IndirectAddressExpression)expression;
                return new IndirectAddressExpression(string.Empty, Calculate(history, iae.Address, lineIndex));
            }

            if (expression is IdentifierExpression)
            {
                Constant c;
                if (mConstants.TryGetValue(expression.Text, out c))
                {
                    return Calculate(history, c.InputValue, lineIndex);
                }

                Variable v;
                if (mVariables.TryGetValue(expression.Text, out v))
                {
                    return new IntExpression(v.Offset.ToString());
                }

                Label l;
                if (mLabels.TryGetValue(expression.Text, out l))
                {
                    return new IntExpression(l.Offset.ToString());
                }

                throw new AssembleException($"Undefined symbol \"{expression.Text}\"", lineIndex);
            }

            if (expression is BinaryExpression)
            {
                BinaryExpression be = (BinaryExpression)expression;

                Expression left = Calculate(history, be.LeftOperand, lineIndex);
                Expression right = Calculate(history, be.RightOperand, lineIndex);

                return PerfromBinaryOperation(left, right, be.Operation, lineIndex);
            }

            throw new AssembleException($"Unsupported expression \"{expression}\"", lineIndex);
        }

        Expression PerfromBinaryOperation(Expression left, Expression right,
            BinaryExpressionOperation operation, int lineIndex)
        {
            if (!(left is IntExpression))
                throw new AssembleException($"Left operand \"{left}\" must be integer", lineIndex);

            if (!(right is IntExpression))
                throw new AssembleException($"Right operand \"{right}\" must be integer", lineIndex);

            Quad l = ((IntExpression)left).GetValue();
            Quad r = ((IntExpression)right).GetValue();
            int calc;

            switch (operation)
            {
                case BinaryExpressionOperation.Sum:
                    calc = l.Value + r.Value;
                    break;
                case BinaryExpressionOperation.Subtract:
                    calc = l.Value - r.Value;
                    break;
                case BinaryExpressionOperation.Multiply:
                    calc = l.Value * r.Value;
                    break;
                case BinaryExpressionOperation.Divide:
                    calc = l.Value / r.Value;
                    break;
                default:
                    throw new AssembleException($"Unsupported binary operation \"{operation}\"", lineIndex);
            }

            IntExpression res = new IntExpression(calc.ToString());

            if (l.IsRelativeAddress || r.IsRelativeAddress)
                return new IndirectAddressExpression(string.Empty, res);

            return res;
        }
    }
}
