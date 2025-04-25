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
            Task Accept(IVisitor visitor);
        }
    }
}