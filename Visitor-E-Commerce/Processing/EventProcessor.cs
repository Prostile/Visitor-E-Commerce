// Файл: Processing/EventProcessor.cs (Измененная версия)
using System;
using System.Collections.Generic;
using System.Threading.Tasks; // Для Task и Task.WhenAll (или простой await в цикле)
using EcommerceAcyclicVisitor.Interfaces;

namespace EcommerceAcyclicVisitor
{
    namespace Processing
    {
        /// <summary>
        /// Центральный процессор событий (Измененная версия).
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
                    Console.WriteLine($"[PROCESSOR] Handler '{handler.GetType().Name}' registered.");
                }
            }

            /// <summary>
            /// Асинхронно обрабатывает входящее доменное событие.
            /// Вызывает и ожидает метод Accept события для каждого обработчика.
            /// </summary>
            /// <param name="ev">Доменное событие для обработки.</param>
            /// <returns>Задача, представляющая асинхронную операцию обработки.</returns>
            public async Task ProcessAsync(IDomainEvent ev) // Метод теперь ProcessAsync и возвращает Task
            {
                if (ev == null)
                {
                    Console.WriteLine("[PROCESSOR] Received a null event. Ignoring.");
                    return; // Возвращаем неявно Task.CompletedTask
                }

                Console.WriteLine($"\n[PROCESSOR] Processing event: {ev.GetType().Name} (ID: {ev.EventId})");
                Console.WriteLine($"           Timestamp: {ev.Timestamp}");

                // Вариант 1: Последовательный вызов обработчиков
                // Простой и надежный, если порядок важен или обработчики могут влиять друг на друга.
                foreach (var handler in _handlers)
                {
                    try
                    {
                        // Вызываем и ОЖИДАЕМ завершения Accept для каждого обработчика
                        await ev.Accept(handler);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[PROCESSOR ERROR] Handler '{handler.GetType().Name}' failed processing event '{ev.GetType().Name}' (ID: {ev.EventId}). Error: {ex.Message}");
                        Console.ResetColor();
                        // Можно добавить логирование стека: Console.WriteLine(ex.StackTrace);
                        // Решаем, прерывать ли обработку другими хендлерами (здесь - не прерываем)
                    }
                }

                // Вариант 2: Параллельный вызов обработчиков (если они независимы)
                // Может быть быстрее, но сложнее в отладке и обработке ошибок.
                /*
                List<Task> tasks = new List<Task>();
                foreach (var handler in _handlers)
                {
                    // Запускаем Accept для каждого обработчика, но не ждем сразу
                    tasks.Add(Task.Run(async () => // Запускаем в Task.Run для потенциального параллелизма
                    {
                         try
                         {
                             await ev.Accept(handler);
                         }
                         catch (Exception ex)
                         {
                              Console.ForegroundColor = ConsoleColor.Red;
                              Console.WriteLine($"[PROCESSOR ERROR (Parallel)] Handler '{handler.GetType().Name}' failed processing event '{ev.GetType().Name}' (ID: {ev.EventId}). Error: {ex.Message}");
                              Console.ResetColor();
                              // Важно: Нужно агрегировать ошибки или использовать более сложную логику
                         }
                    }));
                }
                // Ожидаем завершения всех запущенных задач
                await Task.WhenAll(tasks);
                */

                Console.WriteLine($"[PROCESSOR] Finished processing event: {ev.GetType().Name} (ID: {ev.EventId})");
            }
        }
    }
}