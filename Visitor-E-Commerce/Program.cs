using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using EcommerceAcyclicVisitor.Data;
using EcommerceAcyclicVisitor.Data.Repositories;
using EcommerceAcyclicVisitor.Interfaces;
using EcommerceAcyclicVisitor.Processing;
using EcommerceAcyclicVisitor.Services;
using EcommerceAcyclicVisitor.Simulation;

namespace EcommerceAcyclicVisitor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    string dbName = $"EcommerceDb_{Guid.NewGuid()}";
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase(dbName));

                    services.AddScoped<IAuditLogRepository, AuditLogRepository>();
                    services.AddScoped<IOrderRepository, OrderRepository>();

                    services.AddScoped<AuditLogService>();
                    services.AddScoped<OrderManagementService>();
                    services.AddScoped<NotificationService>();

                    services.AddSingleton<EventProcessor>();
                    services.AddScoped<FrontendSimulator>();

                    services.AddHostedService<SimulationRunner>();

                })
                .Build();

            Console.WriteLine("Starting application host...");
            await host.RunAsync();
            Console.WriteLine("Application host stopped.");
        }
    }

    public class SimulationRunner : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SimulationRunner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("--- E-commerce Event Processing Simulation (Acyclic Visitor + EF Core + DI) ---");

            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedProvider = scope.ServiceProvider;

                var eventProcessor = scopedProvider.GetRequiredService<EventProcessor>();

                var handlers = scopedProvider.GetServices<IVisitor>();

                var auditLogService = scopedProvider.GetRequiredService<AuditLogService>();
                var orderManagementService = scopedProvider.GetRequiredService<OrderManagementService>();
                var notificationService = scopedProvider.GetRequiredService<NotificationService>();

                eventProcessor.RegisterHandler(auditLogService);
                eventProcessor.RegisterHandler(orderManagementService);
                eventProcessor.RegisterHandler(notificationService);

                var simulator = scopedProvider.GetRequiredService<FrontendSimulator>();

                int numberOfEventsToSimulate = 10;
                int delayBetweenEvents = 750;

                await simulator.RunSimulationAsync(numberOfEventsToSimulate, delayBetweenEvents);

                Console.WriteLine("\n-----------------------------------------");
                Console.WriteLine("--- Database Contents After Simulation ---");
                Console.WriteLine("-----------------------------------------");

                // Получаем DbContext
                var dbContext = scopedProvider.GetRequiredService<AppDbContext>();

                Console.WriteLine("\n>>> Audit Log Entries:");

                var auditLogs = await dbContext.AuditLogEntries.OrderBy(l => l.Timestamp).ToListAsync(cancellationToken); // Добавляем OrderBy для порядка
                if (!auditLogs.Any())
                {
                    Console.WriteLine("   (No audit log entries found)");
                }
                else
                {
                    foreach (var log in auditLogs)
                    {
                        Console.WriteLine($"  - Id: {log.Id}, Time: {log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}, EventId: {log.OriginalEventId}, Type: {log.EventType}, Details: \"{log.Details}\"");
                    }
                }

                // Выводим записи из таблицы заказов
                Console.WriteLine("\n>>> Orders:");
                var orders = await dbContext.Orders.OrderBy(o => o.OrderTimestamp).ToListAsync(cancellationToken);
                if (!orders.Any())
                {
                    Console.WriteLine("   (No orders found)");
                }
                else
                {
                    foreach (var order in orders)
                    {
                        Console.WriteLine($"  - DbId: {order.Id}, OrderId: {order.OrderId}, UserId: {order.UserId}, Status: {order.Status}, Amount: {order.TotalAmount:C}, Time: {order.OrderTimestamp.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }
                }

                Console.WriteLine("\n-----------------------------------------");

            }

            Console.WriteLine("\n--- Simulation Complete ---");
            Console.WriteLine("\nApplication running. Press Ctrl+C to exit.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("SimulationRunner stopping.");
            return Task.CompletedTask;
        }
    }
}