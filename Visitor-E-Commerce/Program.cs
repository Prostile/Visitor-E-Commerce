// Файл: Program.cs (Финальная версия с DI и EF Core)
using System;
using System.Linq; // Для Enumerable.Empty<IVisitor>() и GetServices
using System.Threading.Tasks; // Для Task и async Main
using Microsoft.Extensions.DependencyInjection; // Для IServiceCollection, AddDbContext, etc.
using Microsoft.Extensions.Hosting; // Для Host, IHostedService
using Microsoft.EntityFrameworkCore; // Для UseInMemoryDatabase

// Импортируем все необходимые пространства имен нашего проекта
using EcommerceAcyclicVisitor.Data;
using EcommerceAcyclicVisitor.Data.Repositories;
using EcommerceAcyclicVisitor.Interfaces;
using EcommerceAcyclicVisitor.Processing;
using EcommerceAcyclicVisitor.Services;
using EcommerceAcyclicVisitor.Simulation;
using System.Threading;

namespace EcommerceAcyclicVisitor
{
    class Program
    {
        // Main теперь асинхронный, чтобы можно было использовать await
        static async Task Main(string[] args)
        {
            // Используем Generic Host Builder для настройки DI, конфигурации и т.д.
            // Это стандартный подход в современных .NET приложениях.
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // --- Настройка EF Core ---
                    // Добавляем AppDbContext в DI контейнер.
                    // Используем In-Memory базу данных.
                    // Важно: Даем базе данных уникальное имя при каждом запуске,
                    // чтобы избежать сохранения состояния между запусками в одном процессе.
                    string dbName = $"EcommerceDb_{Guid.NewGuid()}";
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase(dbName));

                    // --- Регистрация Репозиториев ---
                    // Регистрируем реализации репозиториев с их интерфейсами.
                    // Scoped: один экземпляр на 'scope'. В консольном приложении scope=lifetime.
                    services.AddScoped<IAuditLogRepository, AuditLogRepository>();
                    services.AddScoped<IOrderRepository, OrderRepository>();

                    // --- Регистрация Обработчиков (Посетителей) ---
                    // Регистрируем каждый сервис-обработчик.
                    // Они будут автоматически разрешены как IVisitor позже.
                    services.AddScoped<AuditLogService>();
                    services.AddScoped<OrderManagementService>();
                    services.AddScoped<NotificationService>();
                    // services.AddScoped<RecommendationEngineService>(); // Если бы он был

                    // --- Регистрация Процессора и Симулятора ---
                    // EventProcessor может быть Singleton, если он потокобезопасен
                    // (в нашем случае он просто хранит список, так что Singleton подходит).
                    services.AddSingleton<EventProcessor>();
                    // FrontendSimulator можно сделать Scoped или Transient.
                    services.AddScoped<FrontendSimulator>();

                    // --- Добавляем "фоновый" сервис для запуска симуляции ---
                    // Используем AddHostedService для запуска нашего основного кода
                    // после того, как хост будет построен и запущен.
                    services.AddHostedService<SimulationRunner>();

                })
                .Build(); // Строим хост

            Console.WriteLine("Starting application host...");
            await host.RunAsync(); // Запускаем хост и ждем его завершения (Ctrl+C)
            Console.WriteLine("Application host stopped.");
        }
    }

    // --- Класс для запуска симуляции как IHostedService ---
    // Этот класс будет автоматически запущен хостом после его старта.
    public class SimulationRunner : IHostedService
    {
        private readonly IServiceProvider _serviceProvider; // Для получения сервисов

        public SimulationRunner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("--- E-commerce Event Processing Simulation (Acyclic Visitor + EF Core + DI) ---");

            // Используем scope для корректного разрешения Scoped зависимостей (DbContext, Repositories)
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedProvider = scope.ServiceProvider;

                // 1. Получаем EventProcessor из DI контейнера
                var eventProcessor = scopedProvider.GetRequiredService<EventProcessor>();

                // 2. Получаем ВСЕ зарегистрированные сервисы, которые реализуют IVisitor
                var handlers = scopedProvider.GetServices<IVisitor>();

                // --- Регистрация альтернативным способом ---
                // Вместо регистрации каждого обработчика как IVisitor,
                // можно зарегистрировать их по типу (как сделано выше),
                // а затем получить их ИЗ КОНТЕЙНЕРА по типу и зарегистрировать в процессоре.
                // Этот способ часто предпочтительнее, так как не требует явной регистрации
                // одного класса под несколькими интерфейсами.
                var auditLogService = scopedProvider.GetRequiredService<AuditLogService>();
                var orderManagementService = scopedProvider.GetRequiredService<OrderManagementService>();
                var notificationService = scopedProvider.GetRequiredService<NotificationService>();

                eventProcessor.RegisterHandler(auditLogService);
                eventProcessor.RegisterHandler(orderManagementService);
                eventProcessor.RegisterHandler(notificationService);
                // -----------------------------------------


                // 3. Получаем FrontendSimulator из DI контейнера
                var simulator = scopedProvider.GetRequiredService<FrontendSimulator>();


                // 4. Запускаем асинхронную симуляцию
                int numberOfEventsToSimulate = 10;
                int delayBetweenEvents = 750;

                // Убедимся, что RunSimulation теперь асинхронный
                // (Нужно обновить FrontendSimulator, если еще не сделано)
                await simulator.RunSimulationAsync(numberOfEventsToSimulate, delayBetweenEvents);

                Console.WriteLine("\n-----------------------------------------");
                Console.WriteLine("--- Database Contents After Simulation ---");
                Console.WriteLine("-----------------------------------------");

                // Получаем DbContext из того же scope
                var dbContext = scopedProvider.GetRequiredService<AppDbContext>();

                // 1. Выводим записи из журнала аудита
                Console.WriteLine("\n>>> Audit Log Entries:");
                // Используем ToListAsync для асинхронного получения данных
                var auditLogs = await dbContext.AuditLogEntries.OrderBy(l => l.Timestamp).ToListAsync(cancellationToken); // Добавляем OrderBy для порядка
                if (!auditLogs.Any())
                {
                    Console.WriteLine("   (No audit log entries found)");
                }
                else
                {
                    foreach (var log in auditLogs)
                    {
                        // Выводим основные поля
                        Console.WriteLine($"  - Id: {log.Id}, Time: {log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}, EventId: {log.OriginalEventId}, Type: {log.EventType}, Details: \"{log.Details}\"");
                    }
                }

                // 2. Выводим записи из таблицы заказов
                Console.WriteLine("\n>>> Orders:");
                // Используем ToListAsync и OrderBy
                var orders = await dbContext.Orders.OrderBy(o => o.OrderTimestamp).ToListAsync(cancellationToken);
                if (!orders.Any())
                {
                    Console.WriteLine("   (No orders found)");
                }
                else
                {
                    foreach (var order in orders)
                    {
                        // Выводим основные поля
                        Console.WriteLine($"  - DbId: {order.Id}, OrderId: {order.OrderId}, UserId: {order.UserId}, Status: {order.Status}, Amount: {order.TotalAmount:C}, Time: {order.OrderTimestamp.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }
                }

                Console.WriteLine("\n-----------------------------------------");

            } // Scope и все Scoped зависимости (DbContext) будут освобождены здесь

            Console.WriteLine("\n--- Simulation Complete ---");

            // В реальном IHostedService здесь может быть логика ожидания или остановки.
            // Для консольного примера мы просто завершаем работу после симуляции.
            // Чтобы приложение не закрылось сразу, можно запросить остановку хоста:
            // var lifetime = _serviceProvider.GetRequiredService<IHostApplicationLifetime>();
            // lifetime.StopApplication();
            // Но так как мы в StartAsync, лучше дождаться Ctrl+C в host.RunAsync() в Main.
            Console.WriteLine("\nApplication running. Press Ctrl+C to exit.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Логика очистки при остановке хоста (если нужна)
            Console.WriteLine("SimulationRunner stopping.");
            return Task.CompletedTask;
        }
    }
}