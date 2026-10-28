using System;
using System.IO;
using System.Text;

namespace FileEncodingViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. РЕГИСТРАЦИЯ ПРОВАЙДЕРА КОДИРОВОК (Обязательно для .NET Core / .NET 5+)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Настройка консоли для корректного отображения Unicode
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("=== Просмотр файла с выбором кодировки ===\n");

            string filePath = GetFilePath();

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден. Программа завершена.");
                return;
            }

            DemonstrateEncodings(filePath);

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static string GetFilePath()
        {
            Console.WriteLine("Введите полный путь к файлу (или оставьте пустым для '1.txt' в текущей папке):");
            Console.Write("Путь: ");
            string path = Console.ReadLine()?.Trim().Trim('"');

            return string.IsNullOrEmpty(path) ? "1.txt" : path;
        }

        static void DemonstrateEncodings(string filePath)
        {
            Console.WriteLine($"\nФайл: {Path.GetFileName(filePath)}");
            Console.WriteLine(new string('-', 60));

            // Чтение в UTF-8
            Console.WriteLine("\n1. Кодировка UTF-8:");
            ReadAndDisplayFile(filePath, Encoding.UTF8);

            // Чтение в Windows-1251
            Console.WriteLine("\n2. Кодировка Windows-1251:");
            ReadAndDisplayFile(filePath, Encoding.GetEncoding(1251));

            // Чтение в CP866
            Console.WriteLine("\n3. Кодировка CP866:");
            ReadAndDisplayFile(filePath, Encoding.GetEncoding(866));

            InteractiveRead(filePath);
        }

        static void ReadAndDisplayFile(string filePath, Encoding encoding)
        {
            try
            {
                string content = File.ReadAllText(filePath, encoding);
                Console.WriteLine(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения файла: {ex.Message}");
            }
        }

        static void InteractiveRead(string filePath)
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("Интерактивный выбор кодировки:");
            Console.WriteLine("1 - UTF-8");
            Console.WriteLine("2 - Windows-1251");
            Console.WriteLine("3 - CP866");
            Console.WriteLine("0 - Выход");

            while (true)
            {
                Console.Write("\nВыберите кодировку (0-3): ");
                string choice = Console.ReadLine();

                Encoding selectedEncoding = null;
                string encodingName = "";

                switch (choice)
                {
                    case "1":
                        selectedEncoding = Encoding.UTF8;
                        encodingName = "UTF-8";
                        break;
                    case "2":
                        selectedEncoding = Encoding.GetEncoding(1251);
                        encodingName = "Windows-1251";
                        break;
                    case "3":
                        selectedEncoding = Encoding.GetEncoding(866);
                        encodingName = "CP866";
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        continue;
                }

                Console.WriteLine($"\nЧтение в кодировке {encodingName}:");
                Console.WriteLine(new string('-', 60));
                ReadAndDisplayFile(filePath, selectedEncoding);
            }
        }
    }
}