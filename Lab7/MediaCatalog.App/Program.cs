using MediaCatalog.Core;
using MediaCatalog.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using ConsoleTables;

namespace MediaCatalog.App;

internal class Program
{
    static void Main(string[] args)
    {
        // Настройка контейнера зависимостей (DI)
        // Мы говорим программе: "Когда понадобится сканер, создавай MediaScanner" и т.д.
        var services = new ServiceCollection();
        services.AddSingleton<IFileExtensionClassifier, FileExtensionClassifier>();
        services.AddSingleton<IMediaScanner, MediaScanner>();
        services.AddSingleton<IMediaRepository, InMemoryMediaRepository>();

        var provider = services.BuildServiceProvider();

        // Получаем готовые объекты
        var scanner = provider.GetRequiredService<IMediaScanner>();
        var repository = provider.GetRequiredService<IMediaRepository>();

        Console.WriteLine("=== МЕДИА-КАТАЛОГ ===\n");
        bool isRunning = true;

        while (isRunning)
        {
            Console.WriteLine("\nМЕНЮ:");
            Console.WriteLine("1. Сканировать папку");
            Console.WriteLine("2. Показать все файлы");
            Console.WriteLine("3. Фильтр: Аудио");
            Console.WriteLine("4. Фильтр: Видео");
            Console.WriteLine("5. Фильтр: Изображения");
            Console.WriteLine("6. Поиск по ключевому слову");
            Console.WriteLine("7. Очистить каталог");
            Console.WriteLine("0. Выход");
            Console.Write("\nВаш выбор: ");

            string? choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1": HandleScan(scanner, repository); break;
                    case "2": PrintResults(repository.GetAll(), "Все файлы"); break;
                    case "3": PrintResults(repository.FilterByType(MediaType.Audio), "Аудио"); break;
                    case "4": PrintResults(repository.FilterByType(MediaType.Video), "Видео"); break;
                    case "5": PrintResults(repository.FilterByType(MediaType.Image), "Изображения"); break;
                    case "6": HandleSearch(repository); break;
                    case "7":
                        repository.Clear();
                        Console.WriteLine("✓ Каталог очищен.");
                        break;
                    case "0": isRunning = false; break;
                    default: Console.WriteLine("✗ Неизвестная команда."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            if (isRunning)
            {
                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();
            }
        }
    }

    static void HandleScan(IMediaScanner scanner, IMediaRepository repository)
    {
        Console.Write("Введите путь к папке: ");
        string? path = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(path)) return;

        Console.Write("Сканировать вложенные папки? (д/н): ");
        bool recursive = Console.ReadLine()?.Trim().ToLower() == "д";

        Console.WriteLine("Сканирование...");
        var items = scanner.ScanDirectory(path, recursive);

        // Очищаем старое и добавляем новое
        repository.Clear();
        repository.AddRange(items);

        Console.WriteLine($"✓ Найдено файлов: {items.Count}");
    }

    static void HandleSearch(IMediaRepository repository)
    {
        Console.Write("Введите ключевое слово: ");
        string? keyword = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(keyword)) return;

        var results = repository.SearchByKeyword(keyword);
        PrintResults(results, $"Поиск '{keyword}'");
    }

    static void PrintResults(IReadOnlyList<MediaItem> items, string title)
    {
        Console.WriteLine($"\n--- {title} ({items.Count}) ---");
        if (items.Count == 0)
        {
            Console.WriteLine("(пусто)");
            return;
        }

        // Красивая таблица в консоли
        var table = new ConsoleTable("Тип", "Имя", "Размер (КБ)", "Путь");
        foreach (var item in items.Take(20)) // Показываем первые 20, чтобы не засорять экран
        {
            table.AddRow(item.Type, item.FileName, item.SizeBytes / 1024, item.FilePath);
        }
        table.Write();

        if (items.Count > 20)
            Console.WriteLine($"... и ещё {items.Count - 20} файлов");
    }
}
