// Файл: Simulation/FrontendSimulator.cs (в проекте EcommerceConditionalLogic - Измененная версия)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks; // Для Task и Task.Delay
using EcommerceConditionalLogic.Events;
using EcommerceConditionalLogic.Interfaces;
using EcommerceConditionalLogic.Models;
using EcommerceConditionalLogic.Processing; // Используем EventProcessor из этого проекта

namespace EcommerceConditionalLogic
{
    namespace Simulation
    {
        /// <summary>
        /// Симулирует действия пользователя или системы (Версия без Посетителя - Измененная).
        /// Асинхронно генерирует события и передает их в EventProcessor.
        /// </summary>
        public class FrontendSimulator
        {
            private readonly EventProcessor _eventProcessor;
            private readonly Random _random = new Random();
            private int _userCounter = 0;
            private int _orderCounter = 0;
            private int _paymentCounter = 0;
            private readonly List<string> _productIds = new List<string> { "prod-abc", "prod-xyz", "prod-def", "prod-123", "prod-789" };
            private readonly List<string> _userIds = new List<string>();

            public FrontendSimulator(EventProcessor eventProcessor)
            {
                _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
            }

            /// <summary>
            /// Асинхронно запускает симуляцию генерации событий.
            /// </summary>
            public async Task RunSimulationAsync(int numberOfEvents, int delayMilliseconds = 500)
            {
                LogSimulatorMessage($"--- Starting Frontend Simulation ({numberOfEvents} events, {delayMilliseconds}ms delay) ---");

                for (int i = 0; i < numberOfEvents; i++)
                {
                    IDomainEvent generatedEvent = GenerateRandomEvent();

                    if (generatedEvent != null)
                    {
                        LogSimulatorMessage($"Generated event: {generatedEvent.GetType().Name} (ID: {generatedEvent.EventId})");
                        await _eventProcessor.ProcessAsync(generatedEvent);
                    }
                    else
                    {
                        LogSimulatorMessage("Skipped event generation (e.g., no users yet).");
                    }

                    if (delayMilliseconds > 0)
                    {
                        await Task.Delay(delayMilliseconds);
                    }
                }

                LogSimulatorMessage("--- Frontend Simulation Complete ---");
            }

            private IDomainEvent GenerateRandomEvent()
            {
                int eventType = _random.Next(0, 3);

                switch (eventType)
                {
                    case 0: // UserRegisteredEvent
                        _userCounter++;
                        string newUserId = $"user-{_userCounter}";
                        string email = $"user{_userCounter}@example.com";
                        _userIds.Add(newUserId);
                        return new UserRegisteredEvent(newUserId, email);

                    case 1: // OrderPlacedEvent
                        if (!_userIds.Any()) return null;
                        _orderCounter++;
                        string orderId = $"order-{_orderCounter}";
                        string randomUserId = _userIds[_random.Next(_userIds.Count)];
                        int itemsCount = _random.Next(1, 4);
                        List<OrderItem> items = new List<OrderItem>();
                        decimal totalAmount = 0;
                        for (int j = 0; j < itemsCount; j++)
                        {
                            string productId = _productIds[_random.Next(_productIds.Count)];
                            int quantity = _random.Next(1, 6);
                            decimal price = (decimal)(_random.Next(10, 1000) * _random.NextDouble());
                            price = Math.Round(price, 2);
                            items.Add(new OrderItem(productId, quantity, price));
                            totalAmount += quantity * price;
                        }
                        return new OrderPlacedEvent(orderId, randomUserId, totalAmount, items);

                    case 2: // PaymentReceivedEvent
                        if (_orderCounter == 0) return null;
                        _paymentCounter++;
                        string paymentOrderId = $"order-{_orderCounter}";
                        string paymentId = $"pay-{_paymentCounter}-{_random.Next(1000, 9999)}";
                        decimal paymentAmount = (decimal)(_random.Next(10, 1500) * _random.NextDouble());
                        paymentAmount = Math.Round(paymentAmount, 2);
                        PaymentStatus status = _random.Next(0, 10) < 8 ? PaymentStatus.Success : PaymentStatus.Failure;
                        return new PaymentReceivedEvent(paymentOrderId, paymentId, paymentAmount, status);

                    default:
                        return null;
                }
            }

            private void LogSimulatorMessage(string message)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[SIMULATOR] {message}");
                Console.ResetColor();
            }
        }
    }
}