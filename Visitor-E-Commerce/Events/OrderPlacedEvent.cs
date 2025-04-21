// Файл: Events/OrderPlacedEvent.cs (Измененная версия)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Для Task
using EcommerceAcyclicVisitor.Interfaces;
using EcommerceAcyclicVisitor.Models;

namespace EcommerceAcyclicVisitor
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

            /// <summary>
            /// Асинхронная реализация метода Accept.
            /// Проверяет тип посетителя и асинхронно вызывает Visit, если тип совпадает.
            /// </summary>
            /// <param name="visitor">Посетитель (обработчик).</param>
            /// <returns>Задача, представляющая асинхронную операцию.</returns>
            public async Task Accept(IVisitor visitor) // Метод теперь async Task
            {
                if (visitor is IOrderPlacedVisitor specificVisitor)
                {
                    // Асинхронно вызываем и ожидаем Visit
                    await specificVisitor.Visit(this);
                }
                // Если тип не совпал, async метод вернет завершенную Task
            }
        }
    }
}