// Файл: Services/OrderManagementService.cs (Измененная версия)
using System;
using System.Threading.Tasks; // Для Task
using EcommerceAcyclicVisitor.Interfaces;
using EcommerceAcyclicVisitor.Events;
using EcommerceAcyclicVisitor.Models; // Для PaymentStatus
using EcommerceAcyclicVisitor.Data.Repositories; // Нужен IOrderRepository
using EcommerceAcyclicVisitor.Data.Entities;    // Нужна сущность DbOrder

namespace EcommerceAcyclicVisitor
{
    namespace Services
    {
        /// <summary>
        /// Сервис для управления состоянием заказов в базе данных (Измененная версия).
        /// Зависит от IOrderRepository.
        /// </summary>
        public class OrderManagementService :
            IVisitor,
            IOrderPlacedVisitor,    // Обрабатывает размещение заказа
            IPaymentReceivedVisitor // Обрабатывает получение платежа
        {
            private readonly IOrderRepository _orderRepository;

            // Статусы заказа (лучше вынести в константы или enum, но для простоты оставим строками)
            private const string StatusPendingPayment = "PendingPayment";
            private const string StatusProcessing = "Processing";
            private const string StatusPaymentFailed = "PaymentFailed";

            /// <summary>
            /// Конструктор, принимающий репозиторий через Dependency Injection.
            /// </summary>
            /// <param name="orderRepository">Репозиторий для работы с заказами.</param>
            public OrderManagementService(IOrderRepository orderRepository)
            {
                _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            }

            /// <summary>
            /// Обрабатывает событие OrderPlacedEvent: создает запись о заказе в БД.
            /// </summary>
            public async Task Visit(OrderPlacedEvent ev) // Возвращает Task
            {
                Console.WriteLine($"[ORDER MGMT] Received Order Placed: ID={ev.OrderId}. Attempting to create order in DB.");

                // Создаем сущность DbOrder на основе данных события
                var dbOrder = new DbOrder(
                    ev.OrderId,
                    ev.UserId,
                    ev.Timestamp, // Используем время события как время размещения
                    ev.TotalAmount,
                    StatusPendingPayment // Начальный статус
                );

                try
                {
                    // Сохраняем через репозиторий
                    await _orderRepository.AddOrderAsync(dbOrder);
                    Console.WriteLine($"[ORDER DB] Created Order: ID={dbOrder.OrderId}, Status={dbOrder.Status}");
                }
                catch (InvalidOperationException ioex) // Обрабатываем возможный дубликат OrderId
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[ORDER DB WARN] Order with ID '{ev.OrderId}' already exists. Skipping creation. Message: {ioex.Message}");
                    Console.ResetColor();
                }
                catch (Exception ex) // Общая ошибка при сохранении
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ORDER DB ERROR] Failed to create order (ID: {ev.OrderId}). Error: {ex.Message}");
                    Console.ResetColor();
                    // В реальной системе нужна более сложная обработка: компенсационные транзакции, очередь ошибок и т.д.
                }
            }

            /// <summary>
            /// Обрабатывает событие PaymentReceivedEvent: находит заказ и обновляет его статус.
            /// </summary>
            public async Task Visit(PaymentReceivedEvent ev) // Возвращает Task
            {
                Console.WriteLine($"[ORDER MGMT] Received Payment Result for Order ID={ev.OrderId}. Status={ev.Status}. Attempting to update order in DB.");

                try
                {
                    // Находим заказ в БД по OrderId из события
                    DbOrder? order = await _orderRepository.GetOrderByOrderIdAsync(ev.OrderId);

                    if (order == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[ORDER DB WARN] Order not found for payment event: OrderID={ev.OrderId}. Cannot update status.");
                        Console.ResetColor();
                        return; // Выходим, если заказ не найден
                    }

                    // Определяем новый статус на основе статуса платежа
                    string newStatus = (ev.Status == PaymentStatus.Success)
                                        ? StatusProcessing
                                        : StatusPaymentFailed;

                    // Проверяем, изменился ли статус, чтобы избежать лишних обновлений (опционально)
                    if (order.Status != newStatus)
                    {
                        order.Status = newStatus; // Обновляем статус у сущности

                        // Сохраняем изменения через репозиторий
                        await _orderRepository.UpdateOrderAsync(order);
                        Console.WriteLine($"[ORDER DB] Updated Order: ID={order.OrderId}, New Status={order.Status}");
                    }
                    else
                    {
                        Console.WriteLine($"[ORDER DB] Order ID={order.OrderId} already has status '{newStatus}'. No update needed.");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ORDER DB ERROR] Failed to update order status for Order ID={ev.OrderId}. Error: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
    }
}