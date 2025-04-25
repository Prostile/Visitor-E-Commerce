using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace EcommerceAcyclicVisitor
{
    namespace Data.Entities
    {
        /// <summary>
        /// Сущность базы данных, представляющая заказ.
        /// </summary>
        public class DbOrder
        {
            /// <summary>
            /// Первичный ключ (автоинкремент).
            /// </summary>
            [Key]
            public long Id { get; set; }

            /// <summary>
            /// Бизнес-идентификатор заказа (например, "order-123").
            /// Должен быть уникальным. Можно добавить индекс в DbContext.
            /// </summary>
            [Required]
            [MaxLength(100)]
            public string OrderId { get; set; }

            /// <summary>
            /// Идентификатор пользователя, разместившего заказ.
            /// </summary>
            [Required]
            [MaxLength(100)]
            public string UserId { get; set; }

            /// <summary>
            /// Время размещения заказа (из события OrderPlacedEvent).
            /// </summary>
            public DateTime OrderTimestamp { get; set; }

            /// <summary>
            /// Общая сумма заказа.
            /// Атрибут для указания типа данных в БД (важно для денег).
            /// </summary>
            [Required]
            [Column(TypeName = "decimal(18, 2)")]
            public decimal TotalAmount { get; set; }

            /// <summary>
            /// Текущий статус заказа (например, "PendingPayment", "Processing", "PaymentFailed", "Shipped").
            /// Обновляется сервисом OrderManagementService.
            /// </summary>
            [Required]
            [MaxLength(50)]
            public string Status { get; set; }

            public DbOrder()
            {
                OrderId = string.Empty;
                UserId = string.Empty;
                Status = string.Empty;
            }

            public DbOrder(string orderId, string userId, DateTime orderTimestamp, decimal totalAmount, string initialStatus)
            {
                OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
                UserId = userId ?? throw new ArgumentNullException(nameof(userId));
                OrderTimestamp = orderTimestamp;
                TotalAmount = totalAmount;
                Status = initialStatus ?? throw new ArgumentNullException(nameof(initialStatus));
            }
        }
    }
}