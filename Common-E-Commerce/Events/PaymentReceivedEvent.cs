// Файл: Events/PaymentReceivedEvent.cs (Измененная версия)
using System;
using System.Threading.Tasks; // Для Task
using EcommerceConditionalLogic.Interfaces;
using EcommerceConditionalLogic.Models;

namespace EcommerceConditionalLogic
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
        }
    }
}