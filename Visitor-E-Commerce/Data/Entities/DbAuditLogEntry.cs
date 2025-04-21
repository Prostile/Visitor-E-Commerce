// Файл: Data/Entities/DbAuditLogEntry.cs
using System;
using System.ComponentModel.DataAnnotations; // Для атрибута [Key]

namespace EcommerceAcyclicVisitor
{
    namespace Data.Entities
    {
        /// <summary>
        /// Сущность базы данных, представляющая запись в журнале аудита событий.
        /// </summary>
        public class DbAuditLogEntry
        {
            /// <summary>
            /// Первичный ключ записи в журнале (автоинкремент).
            /// Атрибут [Key] или соглашение по именованию (Id или <ClassName>Id)
            /// используются EF Core для определения первичного ключа.
            /// </summary>
            [Key]
            public long Id { get; set; } // Используем long для потенциально большого кол-ва записей

            /// <summary>
            /// Уникальный идентификатор исходного доменного события.
            /// </summary>
            public Guid OriginalEventId { get; set; }

            /// <summary>
            /// Время записи в лог (или время исходного события, если нужно).
            /// </summary>
            public DateTime Timestamp { get; set; }

            /// <summary>
            /// Тип произошедшего события (например, "UserRegistered", "OrderPlaced").
            /// </summary>
            [Required] // Делаем поле обязательным на уровне БД (если поддерживается провайдером)
            [MaxLength(100)] // Ограничиваем длину строки
            public string EventType { get; set; }

            /// <summary>
            /// Сериализованные детали события или краткое описание.
            /// </summary>
            [Required]
            public string Details { get; set; } // Длину можно увеличить при необходимости

            // Можно добавить другие поля, например:
            // public string UserId { get; set; } // ID пользователя, связанного с событием (если применимо)
            // public string? RelatedEntityId { get; set; } // ID связанной сущности (заказ, продукт)

            // Конструктор без параметров требуется EF Core (хотя он может использовать и другие)
            public DbAuditLogEntry()
            {
                // Инициализация строк, чтобы избежать null reference, если не установлены
                EventType = string.Empty;
                Details = string.Empty;
            }

            // Удобный конструктор для создания объекта
            public DbAuditLogEntry(Guid originalEventId, DateTime timestamp, string eventType, string details)
            {
                OriginalEventId = originalEventId;
                Timestamp = timestamp;
                EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
                Details = details ?? throw new ArgumentNullException(nameof(details));
            }
        }
    }
}