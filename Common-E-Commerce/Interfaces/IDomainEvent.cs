namespace EcommerceConditionalLogic
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
        }
    }
}