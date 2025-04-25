using EcommerceAcyclicVisitor.Data.Entities; 

namespace EcommerceAcyclicVisitor
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
            Task AddOrderAsync(DbOrder order);

            /// <summary>
            /// Асинхронно находит заказ по его бизнес-идентификатору (OrderId).
            /// </summary>
            Task<DbOrder?> GetOrderByOrderIdAsync(string orderId);

            /// <summary>
            /// Асинхронно обновляет существующий заказ в базе данных.
            /// EF Core отслеживает изменения, поэтому часто достаточно вызвать SaveChangesAsync.
            /// Этот метод может быть полезен для явного указания намерения обновления.
            /// </summary>
            Task UpdateOrderAsync(DbOrder order);
        }
    }
}