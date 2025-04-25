using System.ComponentModel.DataAnnotations;

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
            public long Id { get; set; }

            /// <summary>
            /// Уникальный идентификатор исходного доменного события.
            /// </summary>
            public Guid OriginalEventId { get; set; }

            /// <summary>
            /// Время записи в лог (или время исходного события, если нужно).
            /// </summary>
            public DateTime Timestamp { get; set; }

            /// <summary>
            /// Тип произошедшего события (например, UserRegistered, OrderPlaced).
            /// </summary>
            [Required] // Делаем поле обязательным на уровне БД
            [MaxLength(100)] // Ограничиваем длину строки
            public string EventType { get; set; }

            /// <summary>
            /// Сериализованные детали события или краткое описание.
            /// </summary>
            [Required]
            public string Details { get; set; }

            public DbAuditLogEntry()
            {
                EventType = string.Empty;
                Details = string.Empty;
            }

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