using EcommerceAcyclicVisitor.Interfaces;
using EcommerceAcyclicVisitor.Events;
using EcommerceAcyclicVisitor.Models;
using EcommerceAcyclicVisitor.Data.Repositories;
using EcommerceAcyclicVisitor.Data.Entities;    

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
            IOrderPlacedVisitor,    
            IPaymentReceivedVisitor 
        {
            private readonly IOrderRepository _orderRepository;

            private const string StatusPendingPayment = "PendingPayment";
            private const string StatusProcessing = "Processing";
            private const string StatusPaymentFailed = "PaymentFailed";

            /// <summary>
            /// Конструктор, принимающий репозиторий через Dependency Injection.
            /// </summary>
            public OrderManagementService(IOrderRepository orderRepository)
            {
                _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            }

            /// <summary>
            /// Обрабатывает событие OrderPlacedEvent: создает запись о заказе в БД.
            /// </summary>
            public async Task Visit(OrderPlacedEvent ev)
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
                    await _orderRepository.AddOrderAsync(dbOrder);
                    Console.WriteLine($"[ORDER DB] Created Order: ID={dbOrder.OrderId}, Status={dbOrder.Status}");
                }
                catch (InvalidOperationException ioex) // Обрабатываем возможный дубликат OrderId
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[ORDER DB WARN] Order with ID '{ev.OrderId}' already exists. Skipping creation. Message: {ioex.Message}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ORDER DB ERROR] Failed to create order (ID: {ev.OrderId}). Error: {ex.Message}");
                    Console.ResetColor();
                }
            }

            /// <summary>
            /// Обрабатывает событие PaymentReceivedEvent: находит заказ и обновляет его статус.
            /// </summary>
            public async Task Visit(PaymentReceivedEvent ev) 
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

                    // Проверяем, изменился ли статус, чтобы избежать лишних обновлений
                    if (order.Status != newStatus)
                    {
                        order.Status = newStatus; // Обновляем статус у сущности

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