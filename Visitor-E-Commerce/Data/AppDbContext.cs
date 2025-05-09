﻿using Microsoft.EntityFrameworkCore; // Нужен для DbContext и DbSet<>
using EcommerceAcyclicVisitor.Data.Entities; // Нужны наши сущности

namespace EcommerceAcyclicVisitor
{
    namespace Data
    {
        /// <summary>
        /// Контекст базы данных для приложения, использующий EF Core.
        /// Управляет сессиями с БД и предоставляет доступ к таблицам через DbSet<>.
        /// </summary>
        public class AppDbContext : DbContext
        {
            /// <summary>
            /// Коллекция сущностей для таблицы AuditLogEntries.
            /// Позволяет выполнять CRUD-операции над записями аудита.
            /// </summary>
            public DbSet<DbAuditLogEntry> AuditLogEntries { get; set; }

            /// <summary>
            /// Коллекция сущностей для таблицы Orders.
            /// Позволяет выполнять CRUD-операции над заказами.
            /// </summary>
            public DbSet<DbOrder> Orders { get; set; }

            /// <summary>
            /// Конструктор, принимающий опции конфигурации DbContext.
            /// Эти опции (например, строка подключения или использование In-Memory DB)
            /// будут передаваться через Dependency Injection.
            /// </summary>
            public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options)
            {
            }

            /// <summary>
            /// Метод для дополнительной конфигурации модели данных (Fluent API).
            /// Вызывается EF Core при создании модели.
            /// Здесь можно настроить индексы, связи, ограничения и т.д.
            /// </summary>
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<DbOrder>()
                    .HasIndex(o => o.OrderId)
                    .IsUnique();

                modelBuilder.Entity<DbOrder>()
                    .HasIndex(o => o.UserId);

                modelBuilder.Entity<DbAuditLogEntry>()
                   .HasIndex(log => log.OriginalEventId);
            }
        }
    }
}