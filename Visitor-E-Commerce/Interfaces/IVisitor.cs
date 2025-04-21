// Файл: Interfaces/IVisitor.cs (Измененная версия)
using System.Threading.Tasks; // Добавляем для Task
using EcommerceAcyclicVisitor.Events;

namespace EcommerceAcyclicVisitor
{
    namespace Interfaces
    {
        /// <summary>
        /// Базовый маркерный интерфейс для всех Посетителей (Обработчиков событий).
        /// </summary>
        public interface IVisitor { }

        // --- Специфичные интерфейсы посетителей для каждого типа события ---
        // Методы Visit теперь возвращают Task для поддержки асинхронных операций.

        /// <summary>
        /// Интерфейс для Посетителей, которые могут асинхронно обрабатывать событие UserRegisteredEvent.
        /// </summary>
        public interface IUserRegisteredVisitor : IVisitor
        {
            // Возвращаемый тип изменен на Task
            Task Visit(UserRegisteredEvent ev);
        }

        /// <summary>
        /// Интерфейс для Посетителей, которые могут асинхронно обрабатывать событие OrderPlacedEvent.
        /// </summary>
        public interface IOrderPlacedVisitor : IVisitor
        {
            // Возвращаемый тип изменен на Task
            Task Visit(OrderPlacedEvent ev);
        }

        /// <summary>
        /// Интерфейс для Посетителей, которые могут асинхронно обрабатывать событие PaymentReceivedEvent.
        /// </summary>
        public interface IPaymentReceivedVisitor : IVisitor
        {
            // Возвращаемый тип изменен на Task
            Task Visit(PaymentReceivedEvent ev);
        }

        // !!! Когда мы добавим другие события, их интерфейсы посетителей
        // также должны будут объявлять метод Visit, возвращающий Task,
        // если соответствующие обработчики будут асинхронными. !!!

    }
}