using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolishNotation
{
    class PolishCondition
    {
        // Возвращает выражение, переведенное в обратную польскую запись
        public static List<string> GetPolishNote(string expression)
        {
            List<string> polishExpression = new List<string>();

            // if a > 0 then a = ~a else if b > 2 then a = ~b else a = b + 1

            // a 0 > t1 !F a a ~ = t0 !! (t1) b 2 > t2 !F a b ~ = t0 !! (t2) a b 1 + = (t0)$

            string expressionBuffer = "";
            expression += "$";

            for (int i = 0; i < expression.Length; i ++)    
            {
                if (expression[i] == '$')
                {
                    // конвертируем выражение
                    expressionBuffer = PolishArithmetic.GetPolishNotation(expressionBuffer).Replace("  ", " ");

                    polishExpression.AddRange(expressionBuffer.Split(' '));
                    polishExpression.Add("$");

                    continue;
                }
                else if (i != expression.Length - 1 && expression[i] == 'i' && expression[i + 1] == 'f')
                {
                    // очищаем буфер
                    i++;
                    expressionBuffer = "";
                    continue;
                }
                else if (i != expression.Length - 1 && expression[i] == 't' && expression[i + 1] == 'h')
                {
                    // конвертируем условием, добавляем метки, очищаем буфер
                    // a > 0
                    //индекс оператора сравнения
                    int compOpIndex = 0;
                    for (int j = 0; j < expressionBuffer.Length; j++)
                        if (expressionBuffer[j] == '>' || expressionBuffer[j] == '<' || expressionBuffer[j] == '~' || expressionBuffer[j] == '}' || expressionBuffer[j] == '{')
                        {
                            compOpIndex = j;
                            break;
                        }

                    string logicLeftPart = expressionBuffer.Substring(0, compOpIndex);
                    string logicRightPart = expressionBuffer.Substring(compOpIndex + 1);

                    expressionBuffer = PolishArithmetic.GetPolishNotation(logicLeftPart) + " " + PolishArithmetic.GetPolishNotation(logicRightPart) + " " + expressionBuffer[compOpIndex];

                    polishExpression.AddRange(expressionBuffer.Split(' '));
                    polishExpression.Add("#");
                    polishExpression.Add("!F");

                    i += 3;
                    expressionBuffer = "";
                    continue;
                }
                else if (i != expression.Length - 1 && expression[i] == 'e' && expression[i + 1] == 'l')
                {
                    // конвертируем выражение, добавляем метки, очищаем буфер
                    expressionBuffer = PolishArithmetic.GetPolishNotation(expressionBuffer).Replace("  ", " ");

                    polishExpression.AddRange(expressionBuffer.Split(' '));
                    polishExpression.Add("#");
                    polishExpression.Add("!!");

                    i += 3;
                    expressionBuffer = "";
                    continue;
                }
                else
                {
                    // запись очередного элемента
                    expressionBuffer += expression[i];
                }
            }

            // if a > 0 then if b > 0 then a = 1 else b = 1
            // a 0 > # !F b 0 > # !F a 1 = # !! b 1 = $
            // a 0 > # !F b 0 > # !F a 1 = n !! b 1 = $
            //
            // if a > 0 then a = 1
            // a 0 > # !F a 1 = $
            // 
            // if a > 0 then a = 1 else a = 2
            // a 0 > # !F a 1 = # !! a 2 = $
            // 
            // if a > 0 then a = 1 else if b > 2 then a = 2 else a = 3
            // a 0 > # !F a 1 = # !! a 2 ~ # !F a 2 = # !! a 3 = $
            // a 0 > # !F a 1 = n !! a 2 ~ # !F a 2 = n !! a 3 = $
            // 
            // if a > 0 then if b > 0 then a = 1 else if b < 0 then a = 42 else b = 1 else a = 14
            // a 0 > 30 !F b 0 > 15 !F a 1 = 33 !! b 0 < 25 !F a 42 = 33 !! b 1 = 33 !! a 14 = $

            for (int i = 0; i < polishExpression.Count; i ++)
            {
                if (polishExpression[i] == "!!")
                {
                    polishExpression[i - 1] = (polishExpression.Count - 1).ToString();
                }
            }

            for (int i = polishExpression.Count - 1; i >= 0; i --)
            {
                if (polishExpression[i] == "!F")
                {
                    int index = polishExpression.IndexOf("!!", i);
                    if (index != -1)
                    {
                        polishExpression[i - 1] = (index + 1).ToString();
                        polishExpression[index] = "!!!";
                    }
                    else
                        polishExpression[i - 1] = (polishExpression.Count - 1).ToString();
                }
            }

            for (int i = 0; i < polishExpression.Count; i++)
            {
                if (polishExpression[i] == "!!!")
                    polishExpression[i] = "!!";
            }

            return polishExpression;
        }

        // Вычисляет выражение, переведенное в обратную польскую запись
        public static string CalculatePolishExpression(List<string> polishExpression)
        {
            // a 0 > 11 !F a a ~ = 16 !! a b 1 + = $
            int currentPosition = 0;
            List<string> expressionBuffer = new List<string>();

            string Result = "";

            while (currentPosition < polishExpression.Count)
            {
                if (polishExpression[currentPosition] == "$")
                {
                    if (expressionBuffer.Count != 0)
                    {
                        string arithmeticExpression = "";
                        foreach (string s in expressionBuffer)
                            arithmeticExpression += s + " ";
                        arithmeticExpression = arithmeticExpression.Remove(arithmeticExpression.Length - 1);

                        Result = PolishArithmetic.CalculatePolishExpression(arithmeticExpression);
                    }

                    break;
                }
                else if (polishExpression[currentPosition] == "!F")
                {
                    string transition = expressionBuffer[expressionBuffer.Count - 1];
                    expressionBuffer.RemoveAt(expressionBuffer.Count - 1);

                    string logicExpression = "";
                    foreach (string s in expressionBuffer)
                        logicExpression += s + " ";
                    logicExpression = logicExpression.Remove(logicExpression.Length - 1);

                    string logicResult = PolishArithmetic.CalculatePolishExpression(logicExpression, true);                    

                    expressionBuffer.Clear();

                    if (logicResult == "0")
                    {
                        currentPosition = Convert.ToInt32(transition);
                        continue;
                    }
                }
                else if (polishExpression[currentPosition] == "!!")
                {
                    string transition = expressionBuffer[expressionBuffer.Count - 1];
                    expressionBuffer.RemoveAt(expressionBuffer.Count - 1);

                    string arithmeticExpression = "";
                    foreach (string s in expressionBuffer)
                        arithmeticExpression += s + " ";
                    arithmeticExpression = arithmeticExpression.Remove(arithmeticExpression.Length - 1);

                    Result = PolishArithmetic.CalculatePolishExpression(arithmeticExpression);

                    currentPosition = Convert.ToInt32(transition);
                    expressionBuffer.Clear();
                    continue;
                }
                else
                {
                    expressionBuffer.Add(polishExpression[currentPosition]);
                }

                currentPosition++;
            }

            return Result;
        }
    }
}
