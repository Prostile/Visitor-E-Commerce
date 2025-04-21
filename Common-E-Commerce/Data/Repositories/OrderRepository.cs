// Файл: Data/Repositories/OrderRepository.cs
using System;
using System.Linq; // Для FirstOrDefaultAsync
using System.Threading.Tasks;
using EcommerceConditionalLogic.Data.Entities; // Нужна сущность DbOrder
using Microsoft.EntityFrameworkCore; // Для DbContext, DbSet, EF методов

namespace EcommerceConditionalLogic
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

                // Проверка на дубликат OrderId перед добавлением (опционально, но полезно)
                // Уникальный индекс в БД сам вызовет исключение, но лучше проверить заранее.
                bool exists = await _context.Orders.AnyAsync(o => o.OrderId == order.OrderId);
                if (exists)
                {
                    // Можно выбрать: выбросить исключение, вернуть флаг, проигнорировать и т.д.
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

                // Ищем первую запись, у которой OrderId совпадает.
                // FirstOrDefaultAsync вернет null, если ничего не найдено.
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

                // Убедимся, что EF Core отслеживает этот объект (если он был получен из другого контекста,
                // или если мы хотим быть уверены). В нашем случае, если мы получили заказ через
                // GetOrderByOrderIdAsync из того же контекста, EF уже его отслеживает.
                // _context.Orders.Update(order); // Можно использовать этот метод для явного указания на обновление
                // или для прикрепления неотслеживаемого объекта.

                // Просто сохраняем изменения. Если свойства заказа были изменены после
                // его получения из этого контекста, EF Core обнаружит это и обновит БД.
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Обработка конфликтов параллелизма, если они настроены
                    Console.WriteLine($"Concurrency error updating order {order.OrderId}: {ex.Message}");
                    // Можно перезагрузить сущность, показать ошибку пользователю и т.д.
                    throw; // Перебрасываем для дальнейшей обработки, если нужно
                }
            }
        }
    }
}