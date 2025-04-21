// Файл: Models/PaymentStatus.cs
namespace EcommerceAcyclicVisitor
{
    namespace Models
    {
        /// <summary>
        /// Возможные статусы получения платежа.
        /// </summary>
        public enum PaymentStatus
        {
            /// <summary>
            /// Платеж успешно получен и подтвержден.
            /// </summary>
            Success,

            /// <summary>
            /// Платеж не удался (отклонен, ошибка и т.д.).
            /// </summary>
            Failure
        }
    }
}
