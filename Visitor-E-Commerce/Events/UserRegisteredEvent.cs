using EcommerceAcyclicVisitor.Interfaces;

namespace EcommerceAcyclicVisitor
{
    namespace Events
    {
        /// <summary>
        /// Конкретное событие: Пользователь зарегистрировался.
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

            /// <summary>
            /// Асинхронная реализация метода Accept.
            /// Проверяет тип посетителя и асинхронно вызывает Visit, если тип совпадает.
            /// </summary>
            public async Task Accept(IVisitor visitor)
            {
                if (visitor is IUserRegisteredVisitor specificVisitor)
                {
                    await specificVisitor.Visit(this);
                }
            }
        }
    }
}