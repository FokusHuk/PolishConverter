using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolishNotation
{
    public static class PolishLogic
    {
        /// Возвращает арифметическое выражение, переведенное в обратную польскую запись
        public static string GetPolishNotation(string expression)
        {
            // Удаление пробелов из конвертируемого выражения
            expression = expression.Replace(" ", "");

            // Выражение, переведенное в польскую запись
            string convertedExpression = "";

            // Объявление стека и установка начальных параметров выражения для перевода в обратную польскую нотацию
            Stack<char> stack = new Stack<char>();

            stack.Push('(');

            expression += ")";

            int i = 0;

            while (stack.Count != 0)
            {
                // Текущий символ выражения - число
                if (Char.IsDigit(expression[i]) || Char.IsLetter(expression[i]))
                {
                    convertedExpression += expression[i];
                }
                // Текущий символ выражения - открывающая скобка
                else if (expression[i] == '(')
                {
                    stack.Push('(');
                }
                // Текущий символ выражения - оператор
                else if (expression[i].IsOperator())
                {
                    if (convertedExpression.Length != 0 && convertedExpression[convertedExpression.Length - 1] != ' ')
                        convertedExpression += " ";

                    // Пока верхний элемент стека содержит операцию и 
                    // ее приоритет не меньше приоритета операции, прочитанной из выражения, 
                    // извлекать операции из стека в convertedExpression
                    while (stack.Peek().IsOperator() && CompareOperators(expression[i], stack.Peek()))
                    {
                        convertedExpression += stack.Pop() + " ";
                    }

                    stack.Push(expression[i]);
                }
                // Текущий символ выражения - закрывающая скобка
                if (expression[i] == ')')
                {
                    // Пока на вершине стека не окажется открывающая скобка, 
                    // извлекать из стека элементы и записывать их в выходную строку
                    while (stack.Peek() != '(')
                    {
                        convertedExpression += " ";
                        convertedExpression += stack.Pop();
                    }
                    // Из стека извлекается открывающая скобка
                    stack.Pop();
                }
                i++;
            }

            return convertedExpression;
        }


        /// Возвращает результат арифметического выражения, представленного в обратной польской записи
        public static int CalculatePolishExpression(string polishExpression)
        {
            Stack<int> stack = new Stack<int>();

            string number = "";

            for (int i = 0; i < polishExpression.Length; i++)
            {
                // Текущий символ выражения - число
                if (Char.IsDigit(polishExpression[i]) || Char.IsLetter(polishExpression[i]))
                {
                    // Если число многозначное, то оно посимвольно считывается в буфер number
                    // А затем конвертируется в int и помещается в стек
                    while (Char.IsDigit(polishExpression[i]))
                    {
                        number += polishExpression[i];
                        i++;
                    }

                    if (Char.IsLetter(polishExpression[i]))
                    {
                        if (!PolishArithmetic.Vars.ContainsKey(polishExpression[i].ToString()))
                        {
                            Console.Write("Введите значение для {0}: ", polishExpression[i]);
                            number = Console.ReadLine();
                            PolishArithmetic.Vars.Add(polishExpression[i].ToString(), number);
                        }
                        else
                        {
                            number = PolishArithmetic.Vars[polishExpression[i].ToString()];
                        }
                    }

                    stack.Push(Convert.ToInt32(number));

                    number = "";
                }
                // Текущий символ выражения - оператор            
                else if (polishExpression[i].IsOperator())
                {
                    int expResult = 0;

                    // Из стека извлекаются два числа и над ними производится действие оператора
                    if (polishExpression[i] == '!')
                        expResult = Calculate(polishExpression[i], stack.Pop());
                    else
                        expResult = Calculate(polishExpression[i], stack.Pop(), stack.Pop());

                    // Результат вычисления помещается в стек
                    stack.Push(expResult);
                }
            }

            return stack.Pop();
        }


        // Функция, вычисляющая выражение [arg2] [operator] [arg1]
        // Обратный порядок аргументов используется из-за обратного порядка взятия этих аргументов из стека
        private static int Calculate(char op, int arg1, int arg2 = 1)
        {
            switch (op)
            {
                case '=': return OperatorEQUIVALENCE(arg2, arg1);
                case '~': return OperatorIMPLICATION(arg2, arg1);
                case '|': return OperatorOR(arg2, arg1);
                case '+': return OperatorXOR(arg2, arg1);
                case '&': return OperatorAND(arg2, arg1);
                case '!': return OperatorNOT(arg1);
                case '>': return OperatorMORE(arg2, arg1);
                case '<': return OperatorLESS(arg2, arg1);
                case '}': return OperatorMOREorEQUALS(arg2, arg1);
                case '{': return OperatorLESSorEQUALS(arg2, arg1);
                default: return 1;
            }
        }

        // Функции, описывающие работу логических операторов
        private static int OperatorAND(int x, int y)
        {
            if (x == y && x == 1)
                return 1;
            else
                return 0;
        }

        private static int OperatorOR(int x, int y)
        {
            if (x == y && x == 0)
                return 0;
            else
                return 1;
        }

        private static int OperatorNOT(int x)
        {
            return x == 1 ? 0 : 1;
        }

        private static int OperatorXOR(int x, int y)
        {
            if (x != y)
                return 1;
            else
                return 0;
        }

        private static int OperatorIMPLICATION(int x, int y)
        {
            if (x == 1 && y == 0)
                return 0;
            else
                return 1;
        }

        private static int OperatorEQUIVALENCE(int x, int y)
        {
            if (x == y)
                return 1;
            else
                return 0;
        }

        private static int OperatorMORE(int x, int y)
        {
            if (x > y)
                return 1;
            else
                return 0;
        }

        private static int OperatorLESS(int x, int y)
        {
            if (x < y)
                return 1;
            else
                return 0;
        }

        private static int OperatorMOREorEQUALS(int x, int y)
        {
            if (x >= y)
                return 1;
            else
                return 0;
        }

        private static int OperatorLESSorEQUALS(int x, int y)
        {
            if (x <= y)
                return 1;
            else
                return 0;
        }

        // Функция сравнения приоритетов двух операторов
        private static bool CompareOperators(char operator_1, char operator_2)
        {
            switch (operator_1)
            {
                case '~': case '>': case '<': case '}': case '{': operator_1 = '1'; break;
                case '-': operator_1 = '2'; break;
                case '|': case '+': operator_1 = '3'; break;
                case '&': operator_1 = '4'; break;
                case '!': operator_1 = '5'; break;
            }

            switch (operator_2)
            {
                case '~': case '>': case '<': case '}': case '{': operator_2 = '1'; break;
                case '-': operator_2 = '2'; break;
                case '|': case '+': operator_2 = '3'; break;
                case '&': operator_2 = '4'; break;
                case '!': operator_2 = '5'; break;
            }

            if ((int)operator_1 <= (int)operator_2)
            {
                return true;
            }

            return false;
        }

        // Функция-расширение, определяющая, является ли данный символ арифметическим оператором
        private static bool IsOperator(this char c)
        {
            // !   НЕ
            // &   И
            // |   ИЛИ
            // +   ИСКЛЮЧАЮЩЕЕ ИЛИ
            // ~  ЭКВИВАЛЕНЦИЯ
            // -   СЛЕДОВАНИЕ
            // >   БОЛЬШЕ
            // <   МЕНЬШЕ
            // }  БОЛЬШЕ ИЛИ РАВНО
            // {  МЕНЬШЕ ИЛИ РАВНО
            if (c == '!' || c == '&' || c == '|' || c == '+' || c == '~' || c == '-' ||
                c == '>' || c == '<' || c == '}' || c == '{')
                return true;
            return false;
        }
    }
}