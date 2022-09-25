using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca301Emulator.Assembler
{
    class AssemblyLine
    {
        public readonly int LineIndex;
        public readonly string Identifier;
        public readonly Expression[] Expressions;

        public AssemblyLine(string assemblyLine, int lineIndex)
        {
            LineIndex = lineIndex;

            string line = assemblyLine.Trim();

            int commentPlace = line.IndexOf(';', 0);
            if (commentPlace >= 0)
                line = line.Substring(0, commentPlace);

            Identifier = GetNextAlphaNumericalPart(line);

            if (!string.IsNullOrEmpty(line))
            {
                if (string.IsNullOrEmpty(Identifier))
                    throw new AssembleException($"Incorrect syntax", lineIndex);

                string after_identifier = line.Substring(Identifier.Length);
                string[] string_expressions = after_identifier.Split('\'');
                if ((string_expressions.Length % 2) == 0)
                    throw new AssembleException($"Incomplete character sequence", lineIndex);
                string param_line = string_expressions[0];
                for (int i = 1; i < string_expressions.Length; i++)
                {
                  if ((i % 2) == 0)
                     param_line += string_expressions[i];
                  else
                     param_line += "^" + i.ToString();
                }

                string[] expressions = param_line.Split(',', '=', '@').Trim();
                Expressions = new Expression[expressions.Length];
                for (int i = 0; i < expressions.Length; i++)
                {
                    string e = expressions[i];
                    if (e[0] == '^')
                        e = "'" + string_expressions[int.Parse(e.Substring(1))] + "'";
                    Expressions[i] = CreateExpression(e);
               }
            }
        }

        static string GetNextAlphaNumericalPart(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsLetterOrDigit(str, i) && str[i] != '_')
                    return str.Substring(0, i);
            }

            return str;
        }

        Expression CreateExpression(string expression)
        {
            string input = expression.Trim();

            if (input[0] == '\'')
               return new StringExpression(input);

            if (input[0] == '[' && input[input.Length - 1] == ']')
            {
                return new AddressExpression(input,
                    CreateExpression(input.Substring(1, input.Length - 2)));
            }

            if (input[0] == '{' && input[input.Length - 1] == '}')
            {
                return new IndirectAddressExpression(input,
                    CreateExpression(input.Substring(1, input.Length - 2)));
            }

            if (input[0] == ':')
            {
                return new LabelExpression();
            }

            string[] operands = input.Split(new[] { '-' }, 2).Trim();
            if (operands.Length > 1)
            {
                return new BinaryExpression(input, BinaryExpressionOperation.Subtract,
                    CreateExpression(operands[0]), CreateExpression(operands[1]));
            }

            operands = input.Split(new[] { '+' }, 2).Trim();
            if (operands.Length > 1)
            {
                return new BinaryExpression(input, BinaryExpressionOperation.Sum,
                    CreateExpression(operands[0]), CreateExpression(operands[1]));
            }

            operands = input.Split(new[] { '/' }, 2).Trim();
            if (operands.Length > 1)
            {
                return new BinaryExpression(input, BinaryExpressionOperation.Divide,
                    CreateExpression(operands[0]), CreateExpression(operands[1]));
            }

            operands = input.Split(new[] { '*' }, 2).Trim();
            if (operands.Length > 1)
            {
                return new BinaryExpression(input, BinaryExpressionOperation.Multiply,
                    CreateExpression(operands[0]), CreateExpression(operands[1]));
            }

            int value;
            if (int.TryParse(input, out value))
                return new IntExpression(input);

            return new IdentifierExpression(input);
        }
    }
}
