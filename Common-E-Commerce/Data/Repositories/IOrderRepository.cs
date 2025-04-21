// Файл: Data/Repositories/IOrderRepository.cs
using System.Threading.Tasks; // Для асинхронных операций
using EcommerceConditionalLogic.Data.Entities; // Нужна сущность DbOrder

namespace EcommerceConditionalLogic
{
    namespace Data.Repositories
    {
        /// <summary>
        /// Интерфейс репозитория для работы с заказами.
        /// Определяет контракт для операций с данными заказов.
        /// </summary>
        public interface IOrderRepository
        {
            /// <summary>
            /// Асинхронно добавляет новый заказ в базу данных.
            /// </summary>
            /// <param name="order">Заказ для добавления.</param>
            /// <returns>Задача, представляющая асинхронную операцию.</returns>
            Task AddOrderAsync(DbOrder order);

            /// <summary>
            /// Асинхронно находит заказ по его бизнес-идентификатору (OrderId).
            /// </summary>
            /// <param name="orderId">Бизнес-идентификатор заказа.</param>
            /// <returns>Найденный заказ или null, если заказ не найден.</returns>
            Task<DbOrder?> GetOrderByOrderIdAsync(string orderId); // Возвращаем nullable DbOrder

            /// <summary>
            /// Асинхронно обновляет существующий заказ в базе данных.
            /// EF Core отслеживает изменения, поэтому часто достаточно вызвать SaveChangesAsync.
            /// Этот метод может быть полезен для явного указания намерения обновления.
            /// </summary>
            /// <param name="order">Заказ с обновленными данными.</param>
            /// <returns>Задача, представляющая асинхронную операцию.</returns>
            Task UpdateOrderAsync(DbOrder order);

            // Можно добавить другие методы, например:
            // Task<IEnumerable<DbOrder>> GetOrdersByUserIdAsync(string userId);
            // Task<IEnumerable<DbOrder>> GetOrdersByStatusAsync(string status);
        }
    }
}