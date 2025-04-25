using EcommerceAcyclicVisitor.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAcyclicVisitor
{
    namespace Data.Repositories
    {
        /// <summary>
        /// Реализация репозитория для работы с заказами, использующая EF Core.
        /// </summary>
        public class OrderRepository : IOrderRepository
        {
            private readonly AppDbContext _context;

            public OrderRepository(AppDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            /// <summary>
            /// Асинхронно добавляет новый заказ и сохраняет изменения.
            /// </summary>
            public async Task AddOrderAsync(DbOrder order)
            {
                if (order == null) throw new ArgumentNullException(nameof(order));

                // Проверка на дубликат OrderId перед добавлением
                bool exists = await _context.Orders.AnyAsync(o => o.OrderId == order.OrderId);
                if (exists)
                {
                    throw new InvalidOperationException($"Order with OrderId '{order.OrderId}' already exists.");
                }

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
            }

            /// <summary>
            /// Асинхронно находит заказ по его бизнес-идентификатору (OrderId).
            /// </summary>
            public async Task<DbOrder?> GetOrderByOrderIdAsync(string orderId)
            {
                if (string.IsNullOrWhiteSpace(orderId)) return null;

                return await _context.Orders
                                     .FirstOrDefaultAsync(o => o.OrderId == orderId);
            }

            /// <summary>
            /// Асинхронно сохраняет изменения для существующего заказа.
            /// EF Core автоматически отслеживает изменения у полученных из контекста сущностей.
            /// </summary>
            public async Task UpdateOrderAsync(DbOrder order)
            {
                if (order == null) throw new ArgumentNullException(nameof(order));

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine($"Concurrency error updating order {order.OrderId}: {ex.Message}");
                    throw;
                }
            }
        }
    }
}