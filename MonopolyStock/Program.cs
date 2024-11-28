using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StockDataAccess;
using Microsoft.EntityFrameworkCore;
using StockCommonUtils;
using Spectre.Console;
using System.Linq.Expressions;
using StockCore.Abstractions;
using StockBusinessLogic.Services;
using StockDataAccess.Repositories;

namespace YourNamespace
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Создаем сервис-провайдер
            var serviceProvider = ConfigureServices();
            
            
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider; 
                var boxService = services.GetRequiredService<IBoxService>();
                var dbContext = services.GetRequiredService<DatabaseContext>();
                // Применяем миграции
                dbContext.Database.Migrate();

                AnsiConsole.Write(new FigletText("Monopoly Stock").Centered().Color(Color.Green));
                AnsiConsole.Write(new Rule("[yellow]Добро пожаловать на склад![/]").RuleStyle("yellow"));
                while (true)
                {
                    var option = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Выберите действие:").AddChoices("Операции с данными о коробках", "Операции с данными о паллетах", "Выход")); switch (option)
                    {
                        case "Операции с данными о коробках":
                            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Выберите операцию:").AddChoices("Загрузить файл с коробками", "Получить данные о коробке", "Получить все коробки", "Удалить коробку", "Назад"));
                            switch (choice)
                            {
                                case "Загрузить файл с коробками":
                                    Console.WriteLine("Введите путь к CSV-файлу: ");
                                    try
                                    {
                                        var csvFilePath = Console.ReadLine();
                                        var importer = new CsvDataImporter(dbContext);
                                        importer.ImportBoxes(csvFilePath);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Произошла ошибка: {ex.Message}");
                                    }
                                    break;
                                case "Получить данные о коробке":
                                    Console.WriteLine("Введите GUID:");
                                    string input = Console.ReadLine();
                                    if (Guid.TryParse(input, out Guid parsedGuid))
                                    {
                                        var box = await boxService.GetBoxesByIdAsync(parsedGuid);
                                        bool exists = await dbContext.Boxes.AnyAsync(b => b.BoxId == parsedGuid);
                                        if (exists)
                                        {
                                            Console.WriteLine($"Ширина: {box.Width}, Высота: {box.Height}, Глубина: {box.Depth}, Объём: {box.Volume}, Дата производства: {box.productionDate}, Срок годности: {box.expirationDate}");
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Запись с указанным GUID не найдена в базе данных.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Ошибка: введена некорректная строка GUID.");
                                    }
                                    break;
                                case "Получить все коробки":
                                    var boxes = await boxService.GetAllBoxesAsync();
                                    foreach (var box in boxes)
                                    {
                                        Console.WriteLine($"BoxId: {box.BoxId}, Width: {box.Width}, Height: {box.Height}, Depth: {box.Depth}, Volume: {box.Volume}, ProductionDate: {box.productionDate}, ExpirationDate: {box.expirationDate}");
                                    }
                                    break;

                                case "Удалить коробку":
                                    Console.WriteLine("Введите GUID:");

                                    string deleteInput = Console.ReadLine();
                                    if (Guid.TryParse(deleteInput, out Guid deleteGuid))
                                    {
                                        await boxService.DeleteBoxAsync(deleteGuid);
                                        Console.WriteLine("Коробка успешно удалена.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Ошибка: введена некорректная строка GUID.");
                                    }
                                    break;
                                case "Назад":
                                    break;
                            }
                            break;
                        case "Операции с данными о паллетах":
                            var choice_pallet = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Выберите операцию:").AddChoices("Загрузить файл с паллетами", "Добавить все коробки в паллеты", "Группировка и сортировка паллетов", "Топ-3 паллеты по сроку годности", "Назад"));
                            switch (choice_pallet)
                            {
                                case "Загрузить файл с паллетами":
                                    Console.WriteLine("Введите путь к CSV-файлу: ");
                                    try
                                    {
                                        var csvFilePath = Console.ReadLine();
                                        var importer = new CsvDataImporter(dbContext);
                                        importer.ImportPallets(csvFilePath);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Произошла ошибка: {ex.Message}");
                                    }
                                    break;
                                case "Добавить все коробки в паллеты":
                                    try 
                                    { 
                                        await PalletUtility.AddAllBoxesToPallets(dbContext); 
                                    } 
                                    catch (Exception ex) 
                                    { 
                                        Console.WriteLine($"Произошла ошибка: {ex.Message}"); 
                                    } 
                                    break;

                                case "Группировка и сортировка паллетов": 
                                    PalletUtility.GroupAndSortPallets(dbContext); 
                                    break;
                                case "Топ-3 паллеты по сроку годности":
                                    PalletUtility.GetTopThreePalletsByExpirationDate(dbContext); 
                                    break;
                                case "Назад":
                                    break;
                            }                         
                            break;                            
                        case "Выход": 
                            return; } } } }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Регистрация DatabaseContext
            services.AddDbContext<DatabaseContext>();
            services.AddScoped<IBoxService, BoxService>();
            services.AddScoped<IBoxRepository, BoxRepository>();

            services.AddLogging(configure => configure.AddConsole());

            return services.BuildServiceProvider();
        }

    }
}
