using EcommerceAcyclicVisitor.Data.Entities;

namespace EcommerceAcyclicVisitor
{
    namespace Data.Repositories
    {
        /// <summary>
        /// Реализация репозитория для работы с записями журнала аудита,
        /// использующая EF Core.
        /// </summary>
        public class AuditLogRepository : IAuditLogRepository
        {
            private readonly AppDbContext _context;

            /// <summary>
            /// Конструктор, принимающий AppDbContext через Dependency Injection.
            /// </summary>
            public AuditLogRepository(AppDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            /// <summary>
            /// Асинхронно добавляет новую запись в журнал аудита и сохраняет изменения в БД.
            /// </summary>
            public async Task AddLogEntryAsync(DbAuditLogEntry logEntry)
            {
                if (logEntry == null)
                {
                    throw new ArgumentNullException(nameof(logEntry));
                }

                await _context.AuditLogEntries.AddAsync(logEntry);

                await _context.SaveChangesAsync();
            }
        }
    }
}