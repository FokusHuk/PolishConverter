using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolishNotation
{
    public static class PolishLogicRW
    {
        public static string GetPolishNotation(string expression)
        {
            string convertedExpression = "";

            Stack<char> stack = new Stack<char>();

            stack.Push('(');

            expression += ")";

            int i = 0;

            while (stack.Count != 0)
            {
                if (expression[i] == ')')
                {
                    while (stack.Peek() != '(')
                    {
                        convertedExpression += stack.Pop();
                    }
                    stack.Pop();
                }
                else if (Char.IsDigit(expression[i]))
                {
                    convertedExpression += expression[i] + " ";
                }
                else if (expression[i] == '(')
                {
                    stack.Push(expression[i]);
                }
                else if (expression[i].IsOperator())
                {
                    while (stack.Peek().IsOperator() && CompareOperators(expression[i], stack.Peek()))
                    {
                        convertedExpression += stack.Pop();
                    }
                    stack.Push(expression[i]);
                }

                i++;
            }


                return convertedExpression;
        }

        private static bool IsOperator(this char c)
        {
            if (c == '!' || c == '&' || c == '|' || c == '+' || c == '~' || c == '=')
                return true;
            return false;
        }

        private static bool CompareOperators(char operator_1, char operator_2)
        {
            switch (operator_1)
            {
                case '=': operator_1 = '1'; break;
                case '~': operator_1 = '2'; break;
                case '|': case '+': operator_1 = '3'; break;
                case '&': operator_1 = '4'; break;
                case '!': operator_1 = '5'; break;
            }

            switch (operator_2)
            {
                case '=': operator_2 = '1'; break;
                case '~': operator_2 = '2'; break;
                case '|': case '+': operator_2 = '3'; break;
                case '&': operator_2 = '4'; break;
                case '!': operator_2 = '5'; break;
            }

            if (Convert.ToInt32(operator_1.ToString()) <= Convert.ToInt32(operator_2.ToString()))
            {
                return true;
            }

            return false;
        }

    }
}
