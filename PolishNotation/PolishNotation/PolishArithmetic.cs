using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolishNotation
{
    public static class PolishArithmetic
    {
        public static Dictionary<string, string> Vars = new Dictionary<string, string>();

        // Возвращает арифметическое выражение, переведенное в обратную польскую запись
        public static string GetPolishNotation(string expression)
        {
            // Удаление пробелов из конвертируемого выражения
            expression = expression.Replace(" ", "");

            // Обработка операторов вида +=
            if (expression.Contains("=") && IsOperator(expression[expression.IndexOf("=") - 1]))
            {
                expression = expression[0] + "=" + expression[0] + expression[1] + "(" + expression.Substring(3) + ")";
            }

            // Выражение, переведенное в польскую запись
            string convertedExpression = "";

            // Объявление стека и установка начальных параметров выражения для перевода в обратную польскую нотацию
            Stack<char> stack = new Stack<char>();

            stack.Push('(');
            
            expression += ")";

            int i = 0;

            while (stack.Count != 0)
            {
                // Текущий символ выражения - число d=1+2
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
                    if (convertedExpression.Length != 0)
                        convertedExpression += " ";

                    // Пока верхний элемент стека содержит операцию и 
                    // ее приоритет меньше приоритета операции, прочитанной из выражения, 
                    // извлекать операции из стека в convertedExpression
                    while (stack.Peek().IsOperator() && CompareOperators(expression[i], stack.Peek()))
                    {
                        convertedExpression += stack.Pop() + " ";
                    }

                    if (expression[i] == '-')
                    {
                        if (i == 0)
                        {
                            convertedExpression += "0 ";
                        }
                        else if (expression[i - 1] == '(')
                        {
                            convertedExpression += "0 ";
                        }
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


        // Возвращает результат арифметического выражения, представленного в обратной польской записи
        public static string CalculatePolishExpression(string polishExpression, bool useLogic = false)
        {
            string logicOperator = "";
            if (useLogic)
            {
                logicOperator = polishExpression[polishExpression.Length - 1].ToString();
                polishExpression = polishExpression.Remove(polishExpression.Length - 1);
            }

            Stack<string> stack = new Stack<string>();

            string number = "";
            
            for (int i = 0; i < polishExpression.Length; i ++)
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

                        if (i >= polishExpression.Length)
                        {
                            return number;
                        }
                    }

                    if (Char.IsLetter(polishExpression[i]))
                    {
                        if (!Vars.ContainsKey(polishExpression[i].ToString()))
                        {
                            Console.Write("Введите значение для {0}: ", polishExpression[i]);
                            number = Console.ReadLine();
                            //if (Char.IsLetter(polishExpression[i]))
                                Vars.Add(polishExpression[i].ToString(), number);
                        }

                        number = polishExpression[i].ToString();
                    }

                    stack.Push(number);

                    number = "";
                }
                // Текущий символ выражения - оператор            
                else if (polishExpression[i].IsOperator())
                {
                    int expResult = 0;
                    // Из стека извлекаются два числа и над ними производится действие оператора
                    if (polishExpression[i] == '~')
                    {
                        if (Char.IsLetter(stack.Peek()[0]))
                            expResult = Calculate(polishExpression[i], Convert.ToInt32(Vars[stack.Pop()]));
                        else
                            expResult = Calculate(polishExpression[i], Convert.ToInt32(stack.Pop()));

                        // Результат вычисления помещается в стек
                        stack.Push(expResult.ToString());
                    }
                    else if (polishExpression[i] == '=')
                    {
                        string arg1 = stack.Pop();
                        string arg2 = stack.Pop();
                        Vars[arg2] = arg1;

                        stack.Push(arg1);
                    }
                    else
                    {
                        string arg1 = stack.Pop();
                        string arg2 = stack.Pop();
                        if (Char.IsLetter(arg1[0]))
                            arg1 = Vars[arg1];
                        if (Char.IsLetter(arg2[0]))
                            arg2 = Vars[arg2];
                        expResult = Calculate(polishExpression[i], Convert.ToInt32(arg1), Convert.ToInt32(arg2));

                        // Результат вычисления помещается в стек
                        stack.Push(expResult.ToString());
                    }                    
                }
            }

            if (useLogic)
            {
                string arg2 = stack.Pop();
                string arg1 = stack.Pop();

                string result = PolishLogic.CalculatePolishExpression(arg1 + " " + arg2 + " " + logicOperator).ToString();

                return result;
            }
            else
            {
                return stack.Pop();
            }
        }


        // Функция, вычисляющая выражение [arg2] [operator] [arg1]
        // Обратный порядок аргументов используется из-за обратного порядка взятия этих аргументов из стека
        private static int Calculate(char op, int arg1, int arg2 = 1)
        {
            switch (op)
            {
                case '+': return arg2 + arg1;
                case '-': return arg2 - arg1;
                case '*': return arg2 * arg1;
                case '/': return arg2 / arg1;
                case '%': return arg2 % arg1;
                case '~': return -arg1;
                default: return 1;
            }
        }

        // Функция сравнения приоритетов двух операторов
        private static bool CompareOperators(char operator_1, char operator_2)
        {
            switch (operator_1)
            {
                case '=': operator_1 = '1'; break;
                case '+': operator_1 = '2'; break;
                case '-': operator_1 = '2'; break;
                case '*': operator_1 = '5'; break;
                case '/': operator_1 = '4'; break;
                case '%': operator_1 = '3'; break;
                case '~': operator_1 = '6'; break;
            }

            switch (operator_2)
            {
                case '=': operator_2 = '1'; break;
                case '+': operator_2 = '2'; break;
                case '-': operator_2 = '2'; break;
                case '*': operator_2 = '5'; break;
                case '/': operator_2 = '4'; break;
                case '%': operator_2 = '3'; break;
                case '~': operator_2 = '6'; break;
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
            if (c == '+' || c == '-' || c == '*' || c == '/' || c == '%' || c == '~' || c == '=')
                return true;
            return false;
        }

        public static void SetVars()
        {
            for (int i = 0; i < Vars.Count; i++)
            {
                KeyValuePair<string, string> kp;

                kp = Vars.ElementAt(i);

                if (Char.IsLetter(kp.Value[0]))
                    SetVar(kp.Key, kp.Value);
            }
        }

        private static void SetVar(string key, string currentKey)
        {
            if (Char.IsLetter(currentKey[0]))
            {
                SetVar(key, Vars[currentKey]);
            }
            else
            {
                Vars[key] = currentKey;
            }
        }
    }
}
