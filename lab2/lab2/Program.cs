using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Подключаем пространство имен, где находятся наши классы ПК 
// (если вы оставили их в namespace Lab2, как в предыдущем ответе)
using Lab2;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Демонстрация сборки персонального компьютера ===\n");

            // 1. Создаем компоненты (аналог создания транспортных средств)
            Processor cpu = new Processor("Intel Core i5-13400F", 18500.0, 65.0, 10, 4.6, "LGA1700");
            Memory ram1 = new Memory("Kingston Fury Beast", 6200.0, 5.0, "DDR4", 16, 3200);
            Memory ram2 = new Memory("Kingston Fury Beast", 6200.0, 5.0, "DDR4", 16, 3200); // Вторая планка для двухканального режима
            Disk ssd = new Disk("Samsung 980 PRO", 9500.0, 5.5, "NVMe SSD", 1000);
            Disk hdd = new Disk("Seagate Barracuda", 4500.0, 7.0, "HDD", 2000);

            // 2. Создаем сборку ПК (аналог List<Vehicle> autopark)
            PCBuild myPC = new PCBuild();

            Console.WriteLine("Демонстрация добавления компонентов в сборку:");
            myPC.AddComponent(cpu);
            myPC.AddComponent(ram1);
            myPC.AddComponent(ram2);
            myPC.AddComponent(ssd);
            myPC.AddComponent(hdd);

            Console.WriteLine("\nДемонстрация удаления компонента (передумали покупать HDD):");
            // Аналог выгрузки груза/пассажиров
            myPC.RemoveComponent("Seagate Barracuda");

            Console.WriteLine($"\nРасчет итоговых характеристик сборки:\n");
            Console.WriteLine(new string('-', 70));

            // 3. Полиморфизм в действии: 
            // Метод PrintBuildInfo() внутри себя проходит по списку IComponent 
            // и вызывает переопределенный GetInfo() для каждого конкретного типа 
            // (Processor, Memory, Disk), а также суммирует их характеристики.
            myPC.PrintBuildInfo();

            // Дополнительный явный вывод итогов (аналог вывода стоимости поездки)
            Console.WriteLine("Краткая сводка:");
            Console.WriteLine($"Суммарная стоимость сборки:      {myPC.GetTotalCost():F2} руб.");
            Console.WriteLine($"Суммарное энергопотребление:     {myPC.GetTotalPowerConsumption():F2} Вт");
            Console.WriteLine(new string('-', 70));

            Console.WriteLine("Готово. Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
