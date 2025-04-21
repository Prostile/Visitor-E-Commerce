// Файл: Services/NotificationService.cs (в проекте EcommerceConditionalLogic - Измененная версия)
using System;
using System.Threading.Tasks; // Для Task и Task.CompletedTask
using EcommerceConditionalLogic.Events;
using EcommerceConditionalLogic.Models; // Нужен PaymentStatus

namespace EcommerceConditionalLogic
{
    namespace Services
    {
        /// <summary>
        /// Сервис для отправки уведомлений пользователям (Версия без Посетителя - Измененная).
        /// Методы Handle теперь возвращают Task для совместимости с асинхронным процессором.
        /// </summary>
        public class NotificationService
        {
            public NotificationService() { }

            // --- Асинхронные методы Handle ---

            public Task Handle(UserRegisteredEvent ev) // Теперь Task
            {
                // Синхронная логика
                Console.WriteLine($"[NOTIFICATION] Sending welcome email to {ev.Email} for User ID={ev.UserId}.");

                // Возвращаем завершенную задачу
                return Task.CompletedTask;
            }

            public Task Handle(PaymentReceivedEvent ev) // Теперь Task
            {
                // Синхронная логика
                if (ev.Status == PaymentStatus.Success)
                {
                    Console.WriteLine($"[NOTIFICATION] Sending payment confirmation email for Order ID={ev.OrderId}. Payment ID={ev.PaymentId}.");
                }
                else // PaymentStatus.Failure
                {
                    Console.WriteLine($"[NOTIFICATION] Sending payment failure notification for Order ID={ev.OrderId}. Payment ID={ev.PaymentId}.");
                }

                // Возвращаем завершенную задачу
                return Task.CompletedTask;
            }
        }
    }
}