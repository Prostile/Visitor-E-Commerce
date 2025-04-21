// Файл: Services/AuditLogService.cs (Измененная версия)
using System;
using System.Text.Json; // Для простой сериализации деталей события
using System.Threading.Tasks; // Для Task в методах Visit
using EcommerceAcyclicVisitor.Interfaces;
using EcommerceAcyclicVisitor.Events;
using EcommerceAcyclicVisitor.Data.Repositories; // Нужен IAuditLogRepository
using EcommerceAcyclicVisitor.Data.Entities;    // Нужна сущность DbAuditLogEntry

namespace EcommerceAcyclicVisitor
{
    namespace Services
    {
        /// <summary>
        /// Сервис для логирования аудита событий в базу данных (Измененная версия).
        /// Зависит от IAuditLogRepository.
        /// </summary>
        public class AuditLogService :
            IVisitor,
            IUserRegisteredVisitor,
            IOrderPlacedVisitor,
            IPaymentReceivedVisitor
        // IReviewSubmittedVisitor // Если будет добавлен
        {
            private readonly IAuditLogRepository _auditLogRepository;

            /// <summary>
            /// Конструктор, принимающий репозиторий через Dependency Injection.
            /// </summary>
            /// <param name="auditLogRepository">Репозиторий для работы с логами аудита.</param>
            public AuditLogService(IAuditLogRepository auditLogRepository)
            {
                _auditLogRepository = auditLogRepository ?? throw new ArgumentNullException(nameof(auditLogRepository));
            }

            // Реализация метода Visit для UserRegisteredEvent
            // Метод теперь асинхронный, так как вызывает асинхронный репозиторий
            public async Task Visit(UserRegisteredEvent ev) // Возвращает Task
            {
                // Вместо Console.WriteLine создаем сущность для БД
                string details = $"User Registered: ID={ev.UserId}, Email={ev.Email}";
                var logEntry = new DbAuditLogEntry(ev.EventId, ev.Timestamp, nameof(UserRegisteredEvent), details);

                // Сохраняем через репозиторий
                try
                {
                    await _auditLogRepository.AddLogEntryAsync(logEntry);
                    Console.WriteLine($"[AUDIT DB] Logged: {details}"); // Оставляем лог в консоли для отладки
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[AUDIT DB ERROR] Failed to log UserRegisteredEvent (ID: {ev.EventId}). Error: {ex.Message}");
                    Console.ResetColor();
                    // В реальной системе - более надежное логирование ошибок
                }
            }

            // Реализация метода Visit для OrderPlacedEvent
            public async Task Visit(OrderPlacedEvent ev) // Возвращает Task
            {
                // Сериализуем основные детали для хранения
                // В реальном приложении можно использовать более сложную сериализацию или хранить ID
                var detailsObject = new { ev.OrderId, ev.UserId, ev.TotalAmount, ItemCount = ev.Items.Count };
                string details = JsonSerializer.Serialize(detailsObject); // Простая JSON сериализация

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

            // Реализация метода Visit для PaymentReceivedEvent
            public async Task Visit(PaymentReceivedEvent ev) // Возвращает Task
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

            // --- Важное изменение интерфейсов IVisitor ---
            // Так как методы Visit стали асинхронными (возвращают Task),
            // нам нужно обновить и сами интерфейсы IVisitor!
        }
    }
}