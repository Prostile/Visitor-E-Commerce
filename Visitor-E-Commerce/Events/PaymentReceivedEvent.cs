// Файл: Events/PaymentReceivedEvent.cs (Измененная версия)
using System;
using System.Threading.Tasks; // Для Task
using EcommerceAcyclicVisitor.Interfaces;
using EcommerceAcyclicVisitor.Models;

namespace EcommerceAcyclicVisitor
{
    namespace Events
    {
        /// <summary>
        /// Конкретное событие: Получен результат обработки платежа (Измененная версия).
        /// Реализует IDomainEvent с асинхронным Accept.
        /// </summary>
        public class PaymentReceivedEvent : IDomainEvent
        {
            public Guid EventId { get; }
            public DateTime Timestamp { get; }
            public string OrderId { get; }
            public string PaymentId { get; }
            public decimal Amount { get; }
            public PaymentStatus Status { get; }

            public PaymentReceivedEvent(string orderId, string paymentId, decimal amount, PaymentStatus status)
            {
                EventId = Guid.NewGuid();
                Timestamp = DateTime.UtcNow;
                OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
                PaymentId = paymentId ?? throw new ArgumentNullException(nameof(paymentId));
                if (amount < 0)
                    throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");
                Amount = amount;
                if (!Enum.IsDefined(typeof(PaymentStatus), status))
                    throw new ArgumentOutOfRangeException(nameof(status), "Invalid payment status value.");
                Status = status;
            }

            /// <summary>
            /// Асинхронная реализация метода Accept.
            /// Проверяет тип посетителя и асинхронно вызывает Visit, если тип совпадает.
            /// </summary>
            /// <param name="visitor">Посетитель (обработчик).</param>
            /// <returns>Задача, представляющая асинхронную операцию.</returns>
            public async Task Accept(IVisitor visitor) // Метод теперь async Task
            {
                if (visitor is IPaymentReceivedVisitor specificVisitor)
                {
                    // Асинхронно вызываем и ожидаем Visit
                    await specificVisitor.Visit(this);
                }
                // Если тип не совпал, async метод вернет завершенную Task
            }
        }
    }
}