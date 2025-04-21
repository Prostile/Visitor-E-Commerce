// Файл: Events/UserRegisteredEvent.cs (Измененная версия)
using System;
using System.Threading.Tasks; // Для Task и Task.CompletedTask
using EcommerceConditionalLogic.Interfaces;

namespace EcommerceConditionalLogic
{
    namespace Events
    {
        /// <summary>
        /// Конкретное событие: Пользователь зарегистрировался (Измененная версия).
        /// Реализует IDomainEvent с асинхронным Accept.
        /// </summary>
        public class UserRegisteredEvent : IDomainEvent
        {
            public Guid EventId { get; }
            public DateTime Timestamp { get; }
            public string UserId { get; }
            public string Email { get; }

            public UserRegisteredEvent(string userId, string email)
            {
                EventId = Guid.NewGuid();
                Timestamp = DateTime.UtcNow;
                UserId = userId ?? throw new ArgumentNullException(nameof(userId));
                Email = email ?? throw new ArgumentNullException(nameof(email));
            }
        }
    }
}