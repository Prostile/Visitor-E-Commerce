using System.Text.Json; 
using EcommerceConditionalLogic.Events;
using EcommerceConditionalLogic.Data.Repositories; 
using EcommerceConditionalLogic.Data.Entities; 

namespace EcommerceConditionalLogic
{
    namespace Services
    {
        /// <summary>
        /// Сервис для логирования аудита событий в БД (Версия без Посетителя - Измененная).
        /// Зависит от IAuditLogRepository. Методы Handle асинхронные.
        /// </summary>
        public class AuditLogService
        {
            private readonly IAuditLogRepository _auditLogRepository;

            /// <summary>
            /// Конструктор, принимающий репозиторий через Dependency Injection.
            /// </summary>
            public AuditLogService(IAuditLogRepository auditLogRepository)
            {
                _auditLogRepository = auditLogRepository ?? throw new ArgumentNullException(nameof(auditLogRepository));
            }


            public async Task Handle(UserRegisteredEvent ev) 
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

            public async Task Handle(OrderPlacedEvent ev)
            {
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

            public async Task Handle(PaymentReceivedEvent ev) 
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