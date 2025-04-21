// Файл: Events/OrderPlacedEvent.cs (Измененная версия)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Для Task
using EcommerceConditionalLogic.Interfaces;
using EcommerceConditionalLogic.Models;

namespace EcommerceConditionalLogic
{
    namespace Events
    {
        /// <summary>
        /// Конкретное событие: Заказ размещен (Измененная версия).
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
        }
    }
}