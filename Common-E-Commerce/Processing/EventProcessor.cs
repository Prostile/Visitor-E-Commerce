// Файл: Processing/EventProcessor.cs (в проекте EcommerceConditionalLogic - Измененная версия)
using System;
using System.Collections.Generic;
using System.Threading.Tasks; // Для Task
using EcommerceConditionalLogic.Interfaces;
using EcommerceConditionalLogic.Events;
using EcommerceConditionalLogic.Services;

namespace EcommerceConditionalLogic
{
    namespace Processing
    {
        /// <summary>
        /// Центральный процессор событий (Версия без Посетителя - Измененная).
        /// Содержит асинхронную условную логику для вызова методов Handle.
        /// </summary>
        public class EventProcessor
        {
            private readonly List<object> _handlers = new List<object>();

            public void RegisterHandler(object handler)
            {
                if (handler == null) throw new ArgumentNullException(nameof(handler));
                if (!_handlers.Contains(handler))
                {
                    _handlers.Add(handler);
                    Console.WriteLine($"[PROCESSOR] Handler '{handler.GetType().Name}' registered.");
                }
            }

            /// <summary>
            /// Асинхронно обрабатывает входящее доменное событие, используя условную логику
            /// для асинхронного вызова методов Handle у соответствующих обработчиков.
            /// </summary>
            /// <param name="ev">Доменное событие для обработки.</param>
            /// <returns>Задача, представляющая асинхронную операцию обработки.</returns>
            public async Task ProcessAsync(IDomainEvent ev) // Метод теперь ProcessAsync и async Task
            {
                if (ev == null)
                {
                    Console.WriteLine("[PROCESSOR] Received a null event. Ignoring.");
                    return;
                }

                Console.WriteLine($"\n[PROCESSOR] Processing event: {ev.GetType().Name} (ID: {ev.EventId})");
                Console.WriteLine($"           Timestamp: {ev.Timestamp}");

                foreach (var handler in _handlers)
                {
                    try
                    {
                        // Используем await при вызове методов Handle, так как они теперь Task

                        if (ev is UserRegisteredEvent ure)
                        {
                            if (handler is AuditLogService als) await als.Handle(ure); // await
                            if (handler is NotificationService ns) await ns.Handle(ure); // await
                        }
                        else if (ev is OrderPlacedEvent ope)
                        {
                            if (handler is AuditLogService als) await als.Handle(ope); // await
                            if (handler is OrderManagementService oms) await oms.Handle(ope); // await
                        }
                        else if (ev is PaymentReceivedEvent pre)
                        {
                            if (handler is AuditLogService als) await als.Handle(pre); // await
                            if (handler is OrderManagementService oms) await oms.Handle(pre); // await
                            if (handler is NotificationService ns) await ns.Handle(pre); // await
                        }
                        else
                        {
                            // Тип события не обрабатывается
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[PROCESSOR ERROR] Handler '{handler.GetType().Name}' failed processing event '{ev.GetType().Name}' (ID: {ev.EventId}). Error: {ex.Message}");
                        Console.ResetColor();
                    }
                }
                Console.WriteLine($"[PROCESSOR] Finished processing event: {ev.GetType().Name} (ID: {ev.EventId})");
            }
        }
    }
}