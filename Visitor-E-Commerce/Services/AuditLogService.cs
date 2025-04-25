using System.Text.Json;
using EcommerceAcyclicVisitor.Interfaces;
using EcommerceAcyclicVisitor.Events;
using EcommerceAcyclicVisitor.Data.Repositories;
using EcommerceAcyclicVisitor.Data.Entities;  

namespace EcommerceAcyclicVisitor
{
    namespace Services
    {
        /// <summary>
        /// Сервис для логирования аудита событий в базу данных.
        /// Зависит от IAuditLogRepository.
        /// </summary>
        public class AuditLogService :
            IVisitor,
            IUserRegisteredVisitor,
            IOrderPlacedVisitor,
            IPaymentReceivedVisitor
        {
            private readonly IAuditLogRepository _auditLogRepository;

            /// <summary>
            /// Конструктор, принимающий репозиторий через Dependency Injection.
            /// </summary>
            public AuditLogService(IAuditLogRepository auditLogRepository)
            {
                _auditLogRepository = auditLogRepository ?? throw new ArgumentNullException(nameof(auditLogRepository));
            }

            public async Task Visit(UserRegisteredEvent ev) 
            {
                string details = $"User Registered: ID={ev.UserId}, Email={ev.Email}";
                var logEntry = new DbAuditLogEntry(ev.EventId, ev.Timestamp, nameof(UserRegisteredEvent), details);

                try
                {
                    await _auditLogRepository.AddLogEntryAsync(logEntry);
                    Console.WriteLine($"[AUDIT DB] Logged: {details}");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[AUDIT DB ERROR] Failed to log UserRegisteredEvent (ID: {ev.EventId}). Error: {ex.Message}");
                    Console.ResetColor();
                }
            }

            public async Task Visit(OrderPlacedEvent ev) 
            {
                // Сериализуем основные детали для хранения
                var detailsObject = new { ev.OrderId, ev.UserId, ev.TotalAmount, ItemCount = ev.Items.Count };
                string details = JsonSerializer.Serialize(detailsObject);

                var logEntry = new DbAuditLogEntry(ev.EventId, ev.Timestamp, nameof(OrderPlacedEvent), details);

                try
                {
                    await _auditLogRepository.AddLogEntryAsync(logEntry);
                    Console.WriteLine($"[AUDIT DB] Logged: Order Placed: ID={ev.OrderId}, UserID={ev.UserId}, Amount={ev.TotalAmount:C}, Items={ev.Items.Count}");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[AUDIT DB ERROR] Failed to log OrderPlacedEvent (ID: {ev.EventId}). Error: {ex.Message}");
                    Console.ResetColor();
                }
            }
            public async Task Visit(PaymentReceivedEvent ev)
            {
                string details = $"Payment Received: OrderID={ev.OrderId}, PaymentID={ev.PaymentId}, Amount={ev.Amount:C}, Status={ev.Status}";
                var logEntry = new DbAuditLogEntry(ev.EventId, ev.Timestamp, nameof(PaymentReceivedEvent), details);

                try
                {
                    await _auditLogRepository.AddLogEntryAsync(logEntry);
                    Console.WriteLine($"[AUDIT DB] Logged: {details}");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[AUDIT DB ERROR] Failed to log PaymentReceivedEvent (ID: {ev.EventId}). Error: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
    }
}