using EcommerceConditionalLogic.Events;
using EcommerceConditionalLogic.Models;

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


            public Task Handle(UserRegisteredEvent ev)
            {
                Console.WriteLine($"[NOTIFICATION] Sending welcome email to {ev.Email} for User ID={ev.UserId}.");

                return Task.CompletedTask;
            }

            public Task Handle(PaymentReceivedEvent ev)
            {
                if (ev.Status == PaymentStatus.Success)
                {
                    Console.WriteLine($"[NOTIFICATION] Sending payment confirmation email for Order ID={ev.OrderId}. Payment ID={ev.PaymentId}.");
                }
                else
                {
                    Console.WriteLine($"[NOTIFICATION] Sending payment failure notification for Order ID={ev.OrderId}. Payment ID={ev.PaymentId}.");
                }

                return Task.CompletedTask;
            }
        }
    }
}