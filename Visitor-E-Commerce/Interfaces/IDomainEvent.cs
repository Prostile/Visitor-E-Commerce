// Файл: Interfaces/IDomainEvent.cs (Измененная версия)
using System;
using System.Threading.Tasks; // Добавляем для Task

namespace EcommerceAcyclicVisitor
{
    namespace Interfaces
    {
        /// <summary>
        /// Базовый интерфейс для всех доменных событий в системе (Измененная версия).
        /// Метод Accept теперь возвращает Task.
        /// </summary>
        public interface IDomainEvent
        {
            Guid EventId { get; }
            DateTime Timestamp { get; }

            /// <summary>
            /// Метод, который позволяет Посетителю асинхронно "посетить" конкретный объект события.
            /// Реализация этого метода в конкретных классах событий будет содержать
            /// проверку типа посетителя и асинхронный вызов соответствующего метода Visit.
            /// </summary>
            /// <param name="visitor">Объект-посетитель (обработчик события).</param>
            /// <returns>Задача, представляющая асинхронную операцию посещения.</returns>
            Task Accept(IVisitor visitor); // Изменено с void на Task
        }
    }
}