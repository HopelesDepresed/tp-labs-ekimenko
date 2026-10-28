using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5
{
    internal class Program
    {
        static void Main()
        {
            Console.Write("Введите количество точек N (рекомендуется от 10^7): ");
            long n = long.Parse(Console.ReadLine());

            Console.Write("Введите количество потоков: ");
            int threadCount = int.Parse(Console.ReadLine());

            if (n <= 0)
            {
                Console.WriteLine("N должно быть положительным.");
                return;
            }

            if (threadCount <= 0)
            {
                Console.WriteLine("Количество потоков должно быть положительным.");
                return;
            }

            if (threadCount > n)
                threadCount = (int)Math.Min(n, int.MaxValue);

            Console.WriteLine();
            Console.WriteLine($"Количество точек: {n:N0}");
            Console.WriteLine($"Количество потоков: {threadCount}");
            Console.WriteLine();

            // Последовательное вычисление
            Stopwatch stopwatch = Stopwatch.StartNew();
            double piSequential = CalculateSequential(n);
            stopwatch.Stop();
            long sequentialTime = stopwatch.ElapsedMilliseconds;

            // Параллельное вычисление
            stopwatch.Restart();
            double piParallel = CalculateParallel(n, threadCount);
            stopwatch.Stop();
            long parallelTime = stopwatch.ElapsedMilliseconds;

            double speedup;
            if (parallelTime == 0)
                speedup = double.PositiveInfinity;
            else
                speedup = (double)sequentialTime / parallelTime;

            double difference = Math.Abs(piSequential - piParallel);

            Console.WriteLine("Результаты:");
            Console.WriteLine($"Последовательный результат: {piSequential:R}");
            Console.WriteLine($"Параллельный результат:     {piParallel:R}");
            Console.WriteLine($"Эталонное значение Math.PI: {Math.PI:R}");
            Console.WriteLine();

            Console.WriteLine($"Время последовательной версии: {sequentialTime} мс");
            Console.WriteLine($"Время параллельной версии:     {parallelTime} мс");

            if (double.IsInfinity(speedup))
                Console.WriteLine("Ускорение: слишком маленькое время измерения");
            else
                Console.WriteLine($"Ускорение: {speedup:F2}");

            Console.WriteLine();
            Console.WriteLine($"Разница между версиями: {difference:E}");

            // Для Монте-Карло допуск 1e-12 недостижим при разумных N. 
            // Точность Монте-Карло ~ 1/sqrt(N). Для N=10^8 точность ~ 1e-4.
            const double tolerance = 1e-3;

            if (difference <= tolerance)
                Console.WriteLine($"Результаты совпадают с заданной точностью (допуск {tolerance}).");
            else
                Console.WriteLine($"Результаты отличаются больше заданной точности (допуск {tolerance}). Это нормально для малых N в методе Монте-Карло.");

            Console.WriteLine();
            Console.WriteLine("Ошибка последовательного результата: " + $"{Math.Abs(Math.PI - piSequential):E}");
            Console.WriteLine("Ошибка параллельного результата:     " + $"{Math.Abs(Math.PI - piParallel):E}");

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static double CalculateSequential(long n)
        {
            long hits = 0;
            // Один экземпляр Random для последовательной версии
            Random random = new Random();

            for (long i = 0; i < n; i++)
            {
                double x = random.NextDouble();
                double y = random.NextDouble();

                // Проверка попадания в четверть круга радиусом 1
                if (x * x + y * y <= 1.0)
                {
                    hits++;
                }
            }

            // Pi = 4 * (попадания / всего точек)
            return 4.0 * hits / (double)n;
        }

        static double CalculateParallel(long n, int threadCount)
        {
            long[] partialHits = new long[threadCount];
            Task[] tasks = new Task[threadCount];

            long blockSize = n / threadCount;
            long remainder = n % threadCount;

            for (int threadId = 0; threadId < threadCount; threadId++)
            {
                int localThreadId = threadId;

                long start = localThreadId * blockSize + Math.Min(localThreadId, remainder);
                long count = blockSize + (localThreadId < remainder ? 1 : 0);
                long end = start + count;

                tasks[localThreadId] = Task.Run(() =>
                {
                    long localHits = 0;

                    // ВАЖНО: Создаем свой экземпляр Random для каждого потока.
                    // Используем Guid для генерации уникального seed, чтобы потоки, 
                    // запущенные одновременно, не получили одинаковые последовательности чисел.
                    Random localRandom = new Random(Guid.NewGuid().GetHashCode());

                    for (long i = start; i < end; i++)
                    {
                        double x = localRandom.NextDouble();
                        double y = localRandom.NextDouble();

                        if (x * x + y * y <= 1.0)
                        {
                            localHits++;
                        }
                    }

                    partialHits[localThreadId] = localHits;
                });
            }

            Task.WaitAll(tasks);

            long totalHits = 0;
            for (int i = 0; i < threadCount; i++)
            {
                totalHits += partialHits[i];
            }

            return 4.0 * totalHits / (double)n;
        }
    }
}
