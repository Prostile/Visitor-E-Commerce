// Файл: Services/OrderManagementService.cs (в проекте EcommerceConditionalLogic - Измененная версия)
using System;
using System.Threading.Tasks; // Для Task
using EcommerceConditionalLogic.Events;
using EcommerceConditionalLogic.Models; // Нужен PaymentStatus
using EcommerceConditionalLogic.Data.Repositories; // Нужен репозиторий из этого проекта
using EcommerceConditionalLogic.Data.Entities;    // Нужна сущность из этого проекта

namespace EcommerceConditionalLogic
{
    namespace Services
    {
        /// <summary>
        /// Сервис для управления состоянием заказов в БД (Версия без Посетителя - Измененная).
        /// Зависит от IOrderRepository. Методы Handle асинхронные.
        /// </summary>
        public class OrderManagementService
        {
            private readonly IOrderRepository _orderRepository;

            // Статусы заказа
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

            // --- Асинхронные методы Handle ---

            public async Task Handle(OrderPlacedEvent ev) // Теперь async Task
            {
                Console.WriteLine($"[ORDER MGMT] Received Order Placed: ID={ev.OrderId}. Attempting to create order in DB.");

                // Используем DbOrder из EcommerceConditionalLogic.Data.Entities
                var dbOrder = new DbOrder(
                    ev.OrderId,
                    ev.UserId,
                    ev.Timestamp,
                    ev.TotalAmount,
                    StatusPendingPayment
                );

                try
                {
                    await _orderRepository.AddOrderAsync(dbOrder);
                    Console.WriteLine($"[ORDER DB] Created Order: ID={dbOrder.OrderId}, Status={dbOrder.Status}");
                }
                catch (InvalidOperationException ioex)
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

            public async Task Handle(PaymentReceivedEvent ev) // Теперь async Task
            {
                Console.WriteLine($"[ORDER MGMT] Received Payment Result for Order ID={ev.OrderId}. Status={ev.Status}. Attempting to update order in DB.");

                try
                {
                    // Используем методы репозитория
                    DbOrder? order = await _orderRepository.GetOrderByOrderIdAsync(ev.OrderId);

                    if (order == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[ORDER DB WARN] Order not found for payment event: OrderID={ev.OrderId}. Cannot update status.");
                        Console.ResetColor();
                        return;
                    }

                    string newStatus = (ev.Status == PaymentStatus.Success)
                                        ? StatusProcessing
                                        : StatusPaymentFailed;

                    if (order.Status != newStatus)
                    {
                        order.Status = newStatus;
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