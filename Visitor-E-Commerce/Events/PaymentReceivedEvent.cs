using EcommerceAcyclicVisitor.Interfaces;
using EcommerceAcyclicVisitor.Models;

namespace EcommerceAcyclicVisitor
{
    namespace Events
    {
        /// <summary>
        /// Конкретное событие: Получен результат обработки платежа.
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
            public async Task Accept(IVisitor visitor)
            {
                if (visitor is IPaymentReceivedVisitor specificVisitor)
                {
                    await specificVisitor.Visit(this);
                }
            }
        }
    }
}