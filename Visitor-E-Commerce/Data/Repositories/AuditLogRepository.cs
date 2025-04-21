// Файл: Data/Repositories/AuditLogRepository.cs
using System;
using System.Threading.Tasks;
using EcommerceAcyclicVisitor.Data.Entities; // Нужна сущность
using Microsoft.EntityFrameworkCore; // Для работы с DbSet и SaveChangesAsync

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
            private readonly AppDbContext _context; // Зависимость от DbContext

            /// <summary>
            /// Конструктор, принимающий AppDbContext через Dependency Injection.
            /// </summary>
            /// <param name="context">Контекст базы данных.</param>
            public AuditLogRepository(AppDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            /// <summary>
            /// Асинхронно добавляет новую запись в журнал аудита и сохраняет изменения в БД.
            /// </summary>
            /// <param name="logEntry">Запись для добавления.</param>
            public async Task AddLogEntryAsync(DbAuditLogEntry logEntry)
            {
                if (logEntry == null)
                {
                    throw new ArgumentNullException(nameof(logEntry));
                }

                // Добавляем сущность в DbSet. EF Core начнет отслеживать ее.
                await _context.AuditLogEntries.AddAsync(logEntry);

                // Сохраняем все отслеживаемые изменения (включая добавление нашей записи) в БД.
                await _context.SaveChangesAsync();
            }

            // Здесь были бы реализации других методов интерфейса, если бы они были.
        }
    }
}