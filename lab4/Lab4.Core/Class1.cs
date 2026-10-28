using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab4.Core
{
    public static class Algo
    {
        // 1. Метод из ЛР1: Факториал
        public static int Factorial(long i)
        {
            if (i < 0 || i > 20) throw new ArgumentOutOfRangeException(nameof(i), "Число должно быть от 0 до 20");
            int res = 1;
            for (int j = 1; j <= i; j++)
            {
                res *= j;
            }
            return res;
        }

        // 2. Метод из ЛР1: Последовательность Фибоначчи
        public static long[] Fibonacci(int n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), "Число не может быть отрицательным");

            long[] result = new long[n];
            if (n > 0) result[0] = 0;
            if (n > 1) result[1] = 1;

            for (int i = 2; i < n; i++)
            {
                result[i] = result[i - 1] + result[i - 2];
            }

            return result;
        }

        // 3. Метод из ЛР1: Функция варианта
        public static double A(double x)
        {
            double first_s = Math.Sqrt(Math.Log(4.0 / 3.0));
            double second_s = x + (9.0 / 7.0);
            double third_s = Math.Exp(Math.Sin(1.3 * x - 0.7));
            return first_s + second_s - third_s;
        }

        // 4. Метод из ЛР1: Ряд Тейлора
        public static double SinTaylor(double x)
        {
            double epsilon = 1e-6;
            double sum = 0.0;
            double term = x;
            int n = 0;

            while (Math.Abs(term) > epsilon)
            {
                sum += term;
                term *= -x * x / ((2 * n + 2) * (2 * n + 3));
                n++;
            }

            return sum;
        }
    }
}

namespace Lab4.Core.PC
{
    // 5. Интерфейс из ЛР2
    public interface IComponent
    {
        string Name { get; }
        double Cost { get; }
        double PowerConsumption { get; }
        string GetInfo();
    }

    // 6. Базовый класс из ЛР2
    public abstract class PCComponent : IComponent
    {
        public string Name { get; }
        public double Cost { get; }
        public double PowerConsumption { get; }

        protected PCComponent(string name, double cost, double powerConsumption)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "Название не может быть пустым");
            if (cost < 0) throw new ArgumentOutOfRangeException(nameof(cost), "Стоимость не может быть отрицательной");
            if (powerConsumption < 0) throw new ArgumentOutOfRangeException(nameof(powerConsumption), "Энергопотребление не может быть отрицательным");

            Name = name;
            Cost = cost;
            PowerConsumption = powerConsumption;
        }

        public abstract string GetInfo();
    }

    // 7. Класс Процессор из ЛР2
    public class Processor : PCComponent
    {
        public int Cores { get; }
        public double FrequencyGHz { get; }

        public Processor(string name, double cost, double powerConsumption, int cores, double frequencyGHz)
            : base(name, cost, powerConsumption)
        {
            if (cores <= 0) throw new ArgumentOutOfRangeException(nameof(cores), "Количество ядер должно быть больше 0");
            if (frequencyGHz <= 0) throw new ArgumentOutOfRangeException(nameof(frequencyGHz), "Частота должна быть больше 0");

            Cores = cores;
            FrequencyGHz = frequencyGHz;
        }

        public override string GetInfo() => $"[Процессор] {Name} | Ядер: {Cores} | Частота: {FrequencyGHz} ГГц | Цена: {Cost:F2} руб. | TDP: {PowerConsumption} Вт";
    }

    // 8. Класс Память из ЛР2
    public class Memory : PCComponent
    {
        public int CapacityGB { get; }

        public Memory(string name, double cost, double powerConsumption, int capacityGB)
            : base(name, cost, powerConsumption)
        {
            if (capacityGB <= 0) throw new ArgumentOutOfRangeException(nameof(capacityGB), "Объем памяти должен быть больше 0");
            CapacityGB = capacityGB;
        }

        public override string GetInfo() => $"[Память] {Name} | Объем: {CapacityGB} ГБ | Цена: {Cost:F2} руб. | Потребление: {PowerConsumption} Вт";
    }

    // 9. Класс Сборка ПК из ЛР2
    public class PCBuild
    {
        private readonly List<IComponent> _components;

        public PCBuild()
        {
            _components = new List<IComponent>();
        }

        public void AddComponent(IComponent component)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            _components.Add(component);
        }

        public void RemoveComponent(string componentName)
        {
            var component = _components.FirstOrDefault(c => c.Name.Equals(componentName, StringComparison.OrdinalIgnoreCase));
            if (component != null)
            {
                _components.Remove(component);
            }
        }

        public double GetTotalCost()
        {
            return _components.Sum(c => c.Cost);
        }

        public double GetTotalPowerConsumption()
        {
            return _components.Sum(c => c.PowerConsumption);
        }

        public int GetComponentCount()
        {
            return _components.Count;
        }
    }
}