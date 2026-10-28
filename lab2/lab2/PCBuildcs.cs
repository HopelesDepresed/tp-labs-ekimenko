using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    // Интерфейс для любого компонента ПК
    public interface IComponent
    {
        string Name { get; }
        double Cost { get; }                 // Стоимость в рублях
        double PowerConsumption { get; }     // Энергопотребление в Ваттах (Вт)
        string GetInfo();                    // Информация о компоненте
    }

    // Базовый класс компонента (аналог Vehicle)
    public abstract class PCComponent : IComponent
    {
        public string Name { get; }
        public double Cost { get; }
        public double PowerConsumption { get; }

        protected PCComponent(string name, double cost, double powerConsumption)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), "Название компонента не может быть пустым");
            if (cost < 0)
                throw new ArgumentOutOfRangeException(nameof(cost), "Стоимость не может быть отрицательной");
            if (powerConsumption < 0)
                throw new ArgumentOutOfRangeException(nameof(powerConsumption), "Энергопотребление не может быть отрицательным");

            Name = name;
            Cost = cost;
            PowerConsumption = powerConsumption;
        }

        public abstract string GetInfo();
    }

    // Класс Процессор
    public class Processor : PCComponent
    {
        public int Cores { get; }
        public double FrequencyGHz { get; }
        public string Socket { get; }

        public Processor(string name, double cost, double powerConsumption, int cores, double frequencyGHz, string socket)
            : base(name, cost, powerConsumption)
        {
            if (cores <= 0) throw new ArgumentOutOfRangeException(nameof(cores), "Количество ядер должно быть больше 0");
            if (frequencyGHz <= 0) throw new ArgumentOutOfRangeException(nameof(frequencyGHz), "Частота должна быть больше 0");

            Cores = cores;
            FrequencyGHz = frequencyGHz;
            Socket = socket;
        }

        public override string GetInfo()
        {
            return $"[Процессор] {Name} | Ядер: {Cores} | Частота: {FrequencyGHz} ГГц | Сокет: {Socket} | Цена: {Cost:F2} руб. | TDP: {PowerConsumption} Вт";
        }
    }

    // Класс Оперативная память
    public class Memory : PCComponent
    {
        public string Type { get; }      // Например, DDR4 или DDR5
        public int CapacityGB { get; }
        public int FrequencyMHz { get; }

        public Memory(string name, double cost, double powerConsumption, string type, int capacityGB, int frequencyMHz)
            : base(name, cost, powerConsumption)
        {
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException(nameof(type));
            if (capacityGB <= 0) throw new ArgumentOutOfRangeException(nameof(capacityGB), "Объем памяти должен быть больше 0");

            Type = type;
            CapacityGB = capacityGB;
            FrequencyMHz = frequencyMHz;
        }

        public override string GetInfo()
        {
            return $"[Память] {Name} | Тип: {Type} | Объем: {CapacityGB} ГБ | Частота: {FrequencyMHz} МГц | Цена: {Cost:F2} руб. | Потребление: {PowerConsumption} Вт";
        }
    }

    // Класс Накопитель (Диск)
    public class Disk : PCComponent
    {
        public string DiskType { get; }  // Например, SSD, HDD, NVMe
        public int CapacityGB { get; }

        public Disk(string name, double cost, double powerConsumption, string diskType, int capacityGB)
            : base(name, cost, powerConsumption)
        {
            if (string.IsNullOrWhiteSpace(diskType)) throw new ArgumentNullException(nameof(diskType));
            if (capacityGB <= 0) throw new ArgumentOutOfRangeException(nameof(capacityGB), "Объем накопителя должен быть больше 0");

            DiskType = diskType;
            CapacityGB = capacityGB;
        }

        public override string GetInfo()
        {
            return $"[Накопитель] {Name} | Тип: {DiskType} | Объем: {CapacityGB} ГБ | Цена: {Cost:F2} руб. | Потребление: {PowerConsumption} Вт";
        }
    }

    // Класс Сборка ПК (управляет компонентами и считает итоги)
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
            Console.WriteLine($"✓ Добавлен компонент: {component.Name}");
        }

        public void RemoveComponent(string componentName)
        {
            var component = _components.FirstOrDefault(c => c.Name.Equals(componentName, StringComparison.OrdinalIgnoreCase));
            if (component != null)
            {
                _components.Remove(component);
                Console.WriteLine($"✗ Удален компонент: {componentName}");
            }
            else
            {
                Console.WriteLine($"Компонент '{componentName}' не найден в сборке.");
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

        public void PrintBuildInfo()
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine(" КОНФИГУРАЦИЯ ПЕРСОНАЛЬНОГО КОМПЬЮТЕРА");
            Console.WriteLine(new string('=', 50));

            if (_components.Count == 0)
            {
                Console.WriteLine("Сборка пуста. Добавьте компоненты.");
            }
            else
            {
                foreach (var comp in _components)
                {
                    Console.WriteLine(comp.GetInfo());
                }
            }

            Console.WriteLine(new string('-', 50));
            Console.WriteLine($" СУММАРНАЯ СТОИМОСТЬ:       {GetTotalCost():F2} руб.");
            Console.WriteLine($" СУММАРНОЕ ЭНЕРГОПОТРЕБЛЕНИЕ: {GetTotalPowerConsumption():F2} Вт");
            Console.WriteLine(new string('=', 50) + "\n");
        }
    }
}