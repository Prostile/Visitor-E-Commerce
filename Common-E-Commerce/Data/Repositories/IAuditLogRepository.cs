// Файл: Data/Repositories/IAuditLogRepository.cs
using System;
using System.Threading.Tasks; // Для асинхронных операций
using EcommerceConditionalLogic.Data.Entities; // Нужна сущность DbAuditLogEntry

namespace EcommerceConditionalLogic
{
    namespace Data.Repositories
    {
        /// <summary>
        /// Интерфейс репозитория для работы с записями журнала аудита.
        /// Определяет контракт для операций с данными аудита.
        /// </summary>
        public interface IAuditLogRepository
        {
            /// <summary>
            /// Асинхронно добавляет новую запись в журнал аудита.
            /// </summary>
            /// <param name="logEntry">Запись для добавления.</param>
            /// <returns>Задача, представляющая асинхронную операцию.</returns>
            Task AddLogEntryAsync(DbAuditLogEntry logEntry);

            // Примечание: Можно добавить другие методы, если потребуется, например:
            // Task<IEnumerable<DbAuditLogEntry>> GetLogsByEventIdAsync(Guid eventId);
            // Task<IEnumerable<DbAuditLogEntry>> GetLogsInDateRangeAsync(DateTime start, DateTime end);
        }
    }
}