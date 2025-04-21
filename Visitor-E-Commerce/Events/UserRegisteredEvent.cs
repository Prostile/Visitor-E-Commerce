// Файл: Events/UserRegisteredEvent.cs (Измененная версия)
using System;
using System.Threading.Tasks; // Для Task и Task.CompletedTask
using EcommerceAcyclicVisitor.Interfaces;

namespace EcommerceAcyclicVisitor
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

            /// <summary>
            /// Асинхронная реализация метода Accept.
            /// Проверяет тип посетителя и асинхронно вызывает Visit, если тип совпадает.
            /// </summary>
            /// <param name="visitor">Посетитель (обработчик).</param>
            /// <returns>Задача, представляющая асинхронную операцию.</returns>
            public async Task Accept(IVisitor visitor) // Метод теперь async Task
            {
                if (visitor is IUserRegisteredVisitor specificVisitor)
                {
                    // Асинхронно вызываем метод Visit и ожидаем его завершения
                    await specificVisitor.Visit(this);
                }
                // Если посетитель не реализует интерфейс, то не делаем ничего асинхронного,
                // поэтому можно было бы вернуть Task.CompletedTask, но async метод
                // сам обернет результат в Task, так что дополнительных действий не нужно.
                // Просто выходим из метода.
            }
        }
    }
}