using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolishNotation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Выберите тип выражения:\n1) арифметическое\n2) логическое\n3) с условным оператором\n4) с циклом\n>> (1/2/3/4) ");
            string choose = Console.ReadLine();
            if (choose == "1")
                TestArithmetic();
            else if (choose == "2")
                TestLogic();
            else if (choose == "3")
                TestCondition();
            else
                TestCycle();
        }

        private static void TestArithmetic()
        {
            do
            {
                Console.Clear();

                Console.Write("Введите арифметическое выражение: ");

                string expression = Console.ReadLine();

                string convertedExpression = PolishArithmetic.GetPolishNotation(expression);

                Console.WriteLine("\nВыражение в обратной польской нотации: {0}", convertedExpression);

                string result = PolishArithmetic.CalculatePolishExpression(convertedExpression);

                Console.Write("\nРезультат вычисления: ");
                if (!Char.IsLetter(result[0]))
                    Console.Write(result);
                Console.WriteLine();

                PolishArithmetic.SetVars();

                foreach (KeyValuePair<string, string> p in PolishArithmetic.Vars)
                    Console.WriteLine(p.Key + " = " + p.Value);

                PolishArithmetic.Vars.Clear();

                Console.WriteLine("\n(Нажмите ESC, чтобы выйти)");
            }
            while (Console.ReadKey().Key != ConsoleKey.Escape);
        }

        private static void TestLogic()
        {
            do
            {
                Console.Clear();

                Console.Write("Введите логическое выражение: ");

                string expression = Console.ReadLine();

                string convertedExpression = PolishLogic.GetPolishNotation(expression);

                Console.WriteLine("\nВыражение в обратной польской нотации: {0}", convertedExpression);

                int result = PolishLogic.CalculatePolishExpression(convertedExpression);

                Console.WriteLine("\nРезультат вычисления: {0}", result);

                PolishArithmetic.Vars.Clear();

                Console.WriteLine("\n(Нажмите ESC, чтобы выйти)");
            }
            while (Console.ReadKey().Key != ConsoleKey.Escape);
        }

        private static void TestCondition()
        {
            do
            {
                Console.Clear();

                Console.Write("Введите выражение с условным оператором: ");

                string expression = Console.ReadLine();

                List<string> polishExpression = PolishCondition.GetPolishNote(expression);

                Console.Write("\nВыражение в обратной польской нотации: ");
                foreach (string s in polishExpression)
                    Console.Write(s + " ");
                Console.WriteLine();

                string result = PolishCondition.CalculatePolishExpression(polishExpression);

                Console.WriteLine("\nРезультат вычисления: " + result);

                PolishArithmetic.SetVars();

                foreach (KeyValuePair<string, string> p in PolishArithmetic.Vars)
                    Console.WriteLine(p.Key + " = " + p.Value);

                PolishArithmetic.Vars.Clear();

                Console.WriteLine("\n(Нажмите ESC, чтобы выйти)");
            }
            while (Console.ReadKey().Key != ConsoleKey.Escape);
        }

        private static void TestCycle()
        {
            do
            {
                Console.Clear();

                Console.Write("Введите выражение: ");

                string expression = Console.ReadLine();

                List<string> polishExpression = PolishCycle.GetPolishNote(expression);

                Console.Write("\nВыражение в обратной польской нотации: ");
                foreach (string s in polishExpression)
                    Console.Write(s + " ");
                Console.WriteLine();

                Console.WriteLine("\n(Нажмите ESC, чтобы выйти)");
            }
            while (Console.ReadKey().Key != ConsoleKey.Escape);
        }
    }
}
