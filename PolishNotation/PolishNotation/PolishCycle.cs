using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolishNotation
{
    class PolishCycle
    {
        // Возвращает выражение, переведенное в обратную польскую запись
        public static List<string> GetPolishNote(string expression)
        {
            List<string> polishExpression = new List<string>();

            expression = expression.Replace(" ", "");

            if (expression[0] == 'w') // while (a > 0) t += 2
            {
                string modifiedExpression = "if ";

                int index = expression.IndexOf('(') + 1;
                int bracketCounter = 1;
                while (bracketCounter != 0)
                {
                    modifiedExpression += expression[index++];
                    if (expression[index] == '(')
                        bracketCounter++;
                    else if (expression[index] == ')')
                        bracketCounter--;
                }

                modifiedExpression += " then ";
                modifiedExpression += expression.Substring(index + 1);

                polishExpression = PolishCondition.GetPolishNote(modifiedExpression);

                polishExpression.Remove("$");
                polishExpression.Add("0");
                polishExpression.Add("!");
                polishExpression.Add("$");
                polishExpression[polishExpression.IndexOf("!F") - 1] = (Convert.ToInt32(polishExpression[polishExpression.IndexOf("!F") - 1]) + 2).ToString();
            }
            else if (expression[0] == 'd') // do t += 2 while (a > 0)
            {
                // do b = 2 while (a > 0)
                // b 2 = a 0 > 0 !T $

                expression = expression.Substring(2);

                string action = expression.Substring(0, expression.IndexOf("while"));
                polishExpression.AddRange(PolishArithmetic.GetPolishNotation(action).Split(' '));

                expression = expression.Substring(expression.IndexOf("while"));
                expression = expression.Substring(expression.IndexOf("(") + 1);
                string condition = expression.Substring(0, expression.LastIndexOf(")"));

                int compOpIndex = 0;
                for (int j = 0; j < condition.Length; j++)
                    if (condition[j] == '>' || condition[j] == '<' || condition[j] == '~' || condition[j] == '}' || condition[j] == '{')
                    {
                        compOpIndex = j;
                        break;
                    }

                string logicLeftPart = condition.Substring(0, compOpIndex);
                string logicRightPart = condition.Substring(compOpIndex + 1);

                condition = PolishArithmetic.GetPolishNotation(logicLeftPart) + " " + PolishArithmetic.GetPolishNotation(logicRightPart) + " " + condition[compOpIndex];

                polishExpression.AddRange(condition.Split(' '));

                polishExpression.Add("0");
                polishExpression.Add("!T");
                polishExpression.Add("$");
            }
            else if (expression[0] == 'f') // for (i = 0; i < a; i += 1) t += 2
            {
                // for (i = 0; i < a; i += 1) t += 2
                // i 0 = i a < 24 !F 17 !! i i 1 + = 3 !! t t 2 + = 10 !! $

                string[] parts = expression.Split(';'); // [for(i=0][i<a][i+=1)t+=2]

                parts[0] = parts[0].Substring(parts[0].IndexOf("(") + 1);
                polishExpression.AddRange(PolishArithmetic.GetPolishNotation(parts[0]).Split(' '));

                int Bindex = polishExpression.Count;

                string condition = parts[1];
                int compOpIndex = 0;
                for (int j = 0; j < condition.Length; j++)
                    if (condition[j] == '>' || condition[j] == '<' || condition[j] == '~' || condition[j] == '}' || condition[j] == '{')
                    {
                        compOpIndex = j;
                        break;
                    }
                string logicLeftPart = condition.Substring(0, compOpIndex);
                string logicRightPart = condition.Substring(compOpIndex + 1);
                condition = PolishArithmetic.GetPolishNotation(logicLeftPart) + " " + PolishArithmetic.GetPolishNotation(logicRightPart) + " " + condition[compOpIndex];
                polishExpression.AddRange(condition.Split(' '));

                polishExpression.Add("#");
                polishExpression.Add("!F");
                polishExpression.Add("#");
                polishExpression.Add("!!");

                int Cindex = polishExpression.Count;

                string counter = parts[2].Substring(0, parts[2].IndexOf(")"));
                polishExpression.AddRange(PolishArithmetic.GetPolishNotation(counter).Split(' '));

                polishExpression.Add(Bindex.ToString());
                polishExpression.Add("!!");

                int Aindex = polishExpression.Count();

                string action = parts[2].Substring(parts[2].IndexOf(")") + 1);
                polishExpression.AddRange(PolishArithmetic.GetPolishNotation(action).Split(' '));

                polishExpression.Add(Cindex.ToString());
                polishExpression.Add("!!");
                polishExpression.Add("$");

                polishExpression[polishExpression.IndexOf("#")] = (polishExpression.Count - 1).ToString();
                polishExpression[polishExpression.IndexOf("#")] = Aindex.ToString();
            }

            return polishExpression;
        }
    }
}
