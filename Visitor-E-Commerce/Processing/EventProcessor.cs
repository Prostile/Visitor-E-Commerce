using EcommerceAcyclicVisitor.Interfaces;

namespace EcommerceAcyclicVisitor
{
    namespace Processing
    {
        /// <summary>
        /// Центральный процессор событий.
        /// Вызывает асинхронный метод Accept события для каждого обработчика.
        /// </summary>
        public class EventProcessor
        {
            private readonly List<IVisitor> _handlers = new List<IVisitor>();

            /// <summary>
            /// Регистрирует новый обработчик событий (Посетитель).
            /// </summary>
            public void RegisterHandler(IVisitor handler)
            {
                if (handler == null) throw new ArgumentNullException(nameof(handler));
                if (!_handlers.Contains(handler))
                {
                    _handlers.Add(handler);
                    LogProcessorMessage($"Handler '{handler.GetType().Name}' registered.");
                }
            }

            /// <summary>
            /// Асинхронно обрабатывает входящее доменное событие.
            /// Вызывает и ожидает метод Accept события для каждого обработчика.
            /// </summary>
            public async Task ProcessAsync(IDomainEvent ev)
            {
                if (ev == null)
                {
                    LogProcessorMessage("Received a null event. Ignoring.");
                    return;
                }

                LogProcessorMessage($"\nProcessing event: {ev.GetType().Name} (ID: {ev.EventId})");
                LogProcessorMessage($"           Timestamp: {ev.Timestamp}");

                foreach (var handler in _handlers)
                {
                    try
                    {
                        await ev.Accept(handler);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        LogProcessorMessage($"[ERROR] Handler '{handler.GetType().Name}' failed processing event '{ev.GetType().Name}' (ID: {ev.EventId}). Error: {ex.Message}");
                        Console.ResetColor();
                    }
                }
                LogProcessorMessage($"Finished processing event: {ev.GetType().Name} (ID: {ev.EventId})");
            }

            private void LogProcessorMessage(string message)
            {
                Console.WriteLine($"[PROCESSOR] {message}");
            }
        }
    }
}