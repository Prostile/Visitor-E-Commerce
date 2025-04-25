using EcommerceAcyclicVisitor.Interfaces;
using EcommerceAcyclicVisitor.Models;

namespace EcommerceAcyclicVisitor
{
    namespace Events
    {
        /// <summary>
        /// Конкретное событие: Заказ размещен.
        /// Реализует IDomainEvent с асинхронным Accept.
        /// </summary>
        public class OrderPlacedEvent : IDomainEvent
        {
            public Guid EventId { get; }
            public DateTime Timestamp { get; }
            public string OrderId { get; }
            public string UserId { get; }
            public decimal TotalAmount { get; }
            public IReadOnlyList<OrderItem> Items { get; }

            public OrderPlacedEvent(string orderId, string userId, decimal totalAmount, IEnumerable<OrderItem> items)
            {
                EventId = Guid.NewGuid();
                Timestamp = DateTime.UtcNow;
                OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
                UserId = userId ?? throw new ArgumentNullException(nameof(userId));
                if (totalAmount < 0)
                    throw new ArgumentOutOfRangeException(nameof(totalAmount), "Total amount cannot be negative.");
                TotalAmount = totalAmount;
                Items = items?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(items));
                if (!Items.Any())
                    throw new ArgumentException("Order must contain at least one item.", nameof(items));
            }

            /// <summary>
            /// Асинхронная реализация метода Accept.
            /// Проверяет тип посетителя и асинхронно вызывает Visit, если тип совпадает.
            /// </summary>
            public async Task Accept(IVisitor visitor)
            {
                if (visitor is IOrderPlacedVisitor specificVisitor)
                {
                    await specificVisitor.Visit(this);
                }
            }
        }
    }
}