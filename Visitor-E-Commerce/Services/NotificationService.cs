// Файл: Services/NotificationService.cs (Измененная версия)
using System;
using System.Threading.Tasks; // Для Task и Task.CompletedTask
using EcommerceAcyclicVisitor.Interfaces;
using EcommerceAcyclicVisitor.Events;
using EcommerceAcyclicVisitor.Models; // Для PaymentStatus

namespace EcommerceAcyclicVisitor
{
    namespace Services
    {
        /// <summary>
        /// Сервис для отправки уведомлений пользователям (Измененная версия).
        /// Методы Visit теперь возвращают Task для соответствия интерфейсам.
        /// </summary>
        public class NotificationService :
            IVisitor,
            IUserRegisteredVisitor,
            IPaymentReceivedVisitor
        // IReviewSubmittedVisitor // Если будет добавлен
        {
            // Конструктор остается прежним (если нет зависимостей)
            public NotificationService() { }

            // Реализация метода Visit для UserRegisteredEvent
            public Task Visit(UserRegisteredEvent ev) // Возвращает Task
            {
                // Синхронная логика (вывод в консоль)
                Console.WriteLine($"[NOTIFICATION] Sending welcome email to {ev.Email} for User ID={ev.UserId}.");

                // В реальном приложении здесь мог бы быть асинхронный вызов:
                // await _emailService.SendWelcomeEmailAsync(ev.Email, ev.UserId);

                // Возвращаем завершенную задачу, так как интерфейс требует Task
                return Task.CompletedTask;
            }

            // Реализация метода Visit для PaymentReceivedEvent
            public Task Visit(PaymentReceivedEvent ev) // Возвращает Task
            {
                // Синхронная логика
                if (ev.Status == PaymentStatus.Success)
                {
                    Console.WriteLine($"[NOTIFICATION] Sending payment confirmation email for Order ID={ev.OrderId}. Payment ID={ev.PaymentId}.");
                    // await _emailService.SendPaymentConfirmationAsync(order.UserEmail, ev.OrderId, ...);
                }
                else // PaymentStatus.Failure
                {
                    Console.WriteLine($"[NOTIFICATION] Sending payment failure notification for Order ID={ev.OrderId}. Payment ID={ev.PaymentId}.");
                    // await _emailService.SendPaymentFailureAsync(order.UserEmail, ev.OrderId, ...);
                }

                // Возвращаем завершенную задачу
                return Task.CompletedTask;
            }
        }
    }
}