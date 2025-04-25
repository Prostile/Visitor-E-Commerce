using EcommerceAcyclicVisitor.Data.Entities;

namespace EcommerceAcyclicVisitor
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
            Task AddLogEntryAsync(DbAuditLogEntry logEntry);
        }
    }
}