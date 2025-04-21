// Файл: Program.cs (в проекте EcommerceConditionalLogic - Финальная версия с DI и EF Core)
using System;
using System.Threading; // Для CancellationToken
using System.Threading.Tasks; // Для Task и async Main
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

// Импортируем все необходимые пространства имен нашего проекта
using EcommerceConditionalLogic.Data;
using EcommerceConditionalLogic.Data.Repositories;
// using EcommerceConditionalLogic.Interfaces; // Не нужен напрямую для регистрации
using EcommerceConditionalLogic.Processing;
using EcommerceConditionalLogic.Services;
using EcommerceConditionalLogic.Simulation;

namespace EcommerceConditionalLogic
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // --- Настройка EF Core ---
                    // (Идентично версии с Посетителем)
                    string dbName = $"EcommerceDb_Conditional_{Guid.NewGuid()}";
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase(dbName));

                    // --- Регистрация Репозиториев ---
                    // (Идентично версии с Посетителем)
                    services.AddScoped<IAuditLogRepository, AuditLogRepository>();
                    services.AddScoped<IOrderRepository, OrderRepository>();

                    // --- Регистрация Обработчиков ---
                    // Регистрируем конкретные классы обработчиков. DI позаботится о зависимостях.
                    services.AddScoped<AuditLogService>();
                    services.AddScoped<OrderManagementService>();
                    services.AddScoped<NotificationService>();

                    // --- Регистрация Процессора и Симулятора ---
                    // (Идентично версии с Посетителем)
                    services.AddSingleton<EventProcessor>(); // Singleton
                    services.AddScoped<FrontendSimulator>();  // Scoped или Transient

                    // --- Добавляем Hosted Service для запуска ---
                    // (Идентично версии с Посетителем)
                    services.AddHostedService<SimulationRunner>();

                })
                .Build();

            Console.WriteLine("Starting application host (Conditional Logic version)...");
            await host.RunAsync();
            Console.WriteLine("Application host stopped (Conditional Logic version).");
        }
    }

    // --- Класс для запуска симуляции ---
    // (Структурно идентичен версии с Посетителем, но использует типы из этого проекта)
    public class SimulationRunner : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SimulationRunner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("--- E-commerce Event Processing Simulation (Conditional Logic + EF Core + DI) ---");

            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedProvider = scope.ServiceProvider;

                // 1. Получаем EventProcessor
                var eventProcessor = scopedProvider.GetRequiredService<EventProcessor>();

                // 2. Получаем Обработчики (сервисы) из DI контейнера
                var auditLogService = scopedProvider.GetRequiredService<AuditLogService>();
                var orderManagementService = scopedProvider.GetRequiredService<OrderManagementService>();
                var notificationService = scopedProvider.GetRequiredService<NotificationService>();

                // 3. Регистрируем обработчики в процессоре
                // Метод RegisterHandler принимает object
                eventProcessor.RegisterHandler(auditLogService);
                eventProcessor.RegisterHandler(orderManagementService);
                eventProcessor.RegisterHandler(notificationService);

                // 4. Получаем FrontendSimulator
                var simulator = scopedProvider.GetRequiredService<FrontendSimulator>();

                // 5. Запускаем асинхронную симуляцию
                int numberOfEventsToSimulate = 10;
                int delayBetweenEvents = 750;

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
            }

            Console.WriteLine("\n--- Simulation Complete (Conditional Logic version) ---");
            Console.WriteLine("\nApplication running. Press Ctrl+C to exit.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("SimulationRunner stopping (Conditional Logic version).");
            return Task.CompletedTask;
        }
    }
}