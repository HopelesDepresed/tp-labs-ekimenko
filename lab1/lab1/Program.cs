using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    internal class Program
    {

        public static int Factorial(long i)
        {
            int res = 1;
            for (int j = 1; j <= i; j++)
            {
                res *= j;
            }
            return res;
        }

        public static double A(double x)
        {
            // 1. Проверка для логарифма: x должен быть строго больше 0
            if (x <= 0)
            {
                throw new ArgumentException("Ошибка: x должен быть строго больше 0 (логарифм от неположительного числа не определен).");
            }

            // 2. Проверка на деление на ноль: x + 3 != 0 => x != -3
            if (x == -3)
            {
                throw new ArgumentException("Ошибка: деление на ноль (x не может быть равен -3).");
            }

            // Вычисление выражения под корнем
            double fraction = (x - 3.0) / (x + 3.0);
            double cosPart = Math.Cos(x - 32);
            double innerSqrt = cosPart * fraction;

            // 3. Проверка на отрицательное число под корнем
            if (innerSqrt < 0)
            {
                throw new ArgumentException($"Ошибка: отрицательное число ({innerSqrt:F4}) под корнем.");
            }

            double sqrtPart = Math.Sqrt(innerSqrt);

            // Вычисление e^(log_5 x). 
            // В C# нет прямого метода log по основанию 5, используем формулу: log_5(x) = ln(x) / ln(5)
            double log5x = Math.Log(x) / Math.Log(5.0);
            double expPart = Math.Exp(log5x);

            // Итоговая формула: √(cos(x − 32) · (x − 3)/(x + 3)) − e^(log₅ x)
            double result = sqrtPart - expPart;

            return result;
        }

        static double SinTaylor(double x, double epsilon, out int termsCount)
        {
            double sum = 0.0;
            double term = x;
            int n = 0;

            termsCount = 0;

            // Суммируем члены, пока очередной член по модулю больше ε
            while (Math.Abs(term) > epsilon)
            {
                sum += term;
                termsCount++;

                // Переход к следующему члену:
                // следующий член = текущий * (-x²) / ((2n + 2)(2n + 3))
                term *= -x * x / ((2 * n + 2) * (2 * n + 3));

                n++;
            }

            return sum;
        }

        public static void FirstTask()
        {
            // Задание 1. Вычислить факториал числа n, введённого пользователем. Предусмотреть проверку ввода.
            try
            {
                Console.WriteLine("Введите число для возведения его в факториал (число не должно быть отрицательным или больше 19)");
                long user_num = long.Parse(Console.ReadLine());
                if (user_num < 0 || user_num > 19)
                {
                    throw new Exception("Заданное пользователем число не подходит под условие.");
                }
                int result = Factorial(user_num);
                Console.WriteLine("Результат: " + result);
            }
            catch (Exception e)
            {
                Console.WriteLine("При вводе числа возникло исключение: " + e.Message);
            }
        }

        public static void SecondTask()
        {
            // Задание 2. Вычислить последовательность чисел Фибоначчи от 0 до n и вывести её в одну строку через запятую.
            try
            {
                Console.WriteLine("Введите, сколько чисел из последовательности Фибоначчи вывести? (число не должно быть отрицательным или больше 70)");
                long user_num = long.Parse(Console.ReadLine());
                if (user_num < 0 || user_num > 70)
                {
                    throw new Exception("Заданное пользователем число не подходит под условие.");
                }
                long first = 0;
                long second = 1;
                for (int i = 0; i < user_num; i++)
                {
                    Console.Write(first + (i == user_num - 1 ? "" : ", "));
                    long next = first + second;
                    first = second;
                    second = next;
                }
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("При вводе числа возникло исключение: " + e.Message);
            }
        }

        public static void ThirdTask()
        {
            // Задание 3. Вычислить значение функции согласно варианту (значение x задаёт пользователь).
            // Если при данном x функция не определена — вывести понятное сообщение об ошибке.
            try
            {
                Console.WriteLine("Введите число x для подсчёта результата функции:");
                double user_num = double.Parse(Console.ReadLine());

                double result = A(user_num);

                Console.WriteLine("Результат функции: " + result);
            }
            catch (FormatException)
            {
                Console.WriteLine("Ошибка: введено некорректное число. Пожалуйста, используйте точку для дробной части (например, 2.5).");
            }
            catch (Exception e)
            {
                // Здесь выведется наше понятное сообщение из ArgumentException
                Console.WriteLine(e.Message);
            }
        }

        public static void FourthTask()
        {
            //Задание 4. Вычислить сумму ряда Тейлора согласно варианту с точностью ε = 10⁻⁶
            try
            {
                Console.Write("Введите x: ");
                double x = double.Parse(Console.ReadLine());

                const double epsilon = 1e-6;

                double taylorResult = SinTaylor(x, epsilon, out int termsCount);
                double mathResult = Math.Sin(x);

                Console.WriteLine($"Сумма ряда Тейлора: {taylorResult:F10}");
                Console.WriteLine($"Math.Sin(x):        {mathResult:F10}");
                Console.WriteLine($"Количество членов:  {termsCount}");
                Console.WriteLine($"Погрешность:        {Math.Abs(taylorResult - mathResult):E}");
            }
            catch (Exception e)
            {
                Console.WriteLine("При вводе числа возникло исключение: " + e.Message);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Вариант 2.");
            Console.WriteLine(new string('-', 40));

            FirstTask();
            Console.WriteLine(new string('-', 40));

            SecondTask();
            Console.WriteLine(new string('-', 40));

            ThirdTask();
            Console.WriteLine(new string('-', 40));

            FourthTask();
            Console.WriteLine(new string('-', 40));
        }
    }
}