using EcommerceAcyclicVisitor.Interfaces;
using EcommerceAcyclicVisitor.Events;
using EcommerceAcyclicVisitor.Models;

namespace EcommerceAcyclicVisitor
{
    namespace Services
    {
        /// <summary>
        /// Сервис для отправки уведомлений пользователям.
        /// Методы Visit теперь возвращают Task для соответствия интерфейсам.
        /// </summary>
        public class NotificationService :
            IVisitor,
            IUserRegisteredVisitor,
            IPaymentReceivedVisitor
        {
            public NotificationService() { }

            public Task Visit(UserRegisteredEvent ev)
            {
                LogProcessorMessage($"Sending welcome email to {ev.Email} for User ID={ev.UserId}.");
                return Task.CompletedTask;
            }

            public Task Visit(PaymentReceivedEvent ev)
            {
                if (ev.Status == PaymentStatus.Success)
                {
                    LogProcessorMessage($"Sending payment confirmation email for Order ID={ev.OrderId}. Payment ID={ev.PaymentId}.");
                }
                else
                {
                    LogProcessorMessage($"Sending payment failure notification for Order ID={ev.OrderId}. Payment ID={ev.PaymentId}.");
                }

                return Task.CompletedTask;
            }

            private void LogProcessorMessage(string message)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine($"[NOTIFICATION] {message}");
                Console.ResetColor();
            }
        }
    }
}