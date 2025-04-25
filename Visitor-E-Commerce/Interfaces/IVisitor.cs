using EcommerceAcyclicVisitor.Events;

namespace EcommerceAcyclicVisitor
{
    namespace Interfaces
    {
        /// <summary>
        /// Базовый маркерный интерфейс для всех Посетителей (Обработчиков событий).
        /// </summary>
        public interface IVisitor { }

        /// <summary>
        /// Интерфейс для Посетителей, которые могут асинхронно обрабатывать событие UserRegisteredEvent.
        /// </summary>
        public interface IUserRegisteredVisitor : IVisitor
        {
            Task Visit(UserRegisteredEvent ev);
        }

        /// <summary>
        /// Интерфейс для Посетителей, которые могут асинхронно обрабатывать событие OrderPlacedEvent.
        /// </summary>
        public interface IOrderPlacedVisitor : IVisitor
        {
            Task Visit(OrderPlacedEvent ev);
        }

        /// <summary>
        /// Интерфейс для Посетителей, которые могут асинхронно обрабатывать событие PaymentReceivedEvent.
        /// </summary>
        public interface IPaymentReceivedVisitor : IVisitor
        {
            Task Visit(PaymentReceivedEvent ev);
        }
    }
}