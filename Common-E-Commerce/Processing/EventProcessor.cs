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
            public async Task ProcessAsync(IDomainEvent ev)
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
                        if (ev is UserRegisteredEvent ure)
                        {
                            if (handler is AuditLogService als) await als.Handle(ure);
                            if (handler is NotificationService ns) await ns.Handle(ure);
                        }
                        else if (ev is OrderPlacedEvent ope)
                        {
                            if (handler is AuditLogService als) await als.Handle(ope);
                            if (handler is OrderManagementService oms) await oms.Handle(ope);
                        }
                        else if (ev is PaymentReceivedEvent pre)
                        {
                            if (handler is AuditLogService als) await als.Handle(pre);
                            if (handler is OrderManagementService oms) await oms.Handle(pre);
                            if (handler is NotificationService ns) await ns.Handle(pre); 
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