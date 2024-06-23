using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fibonacci;
using EasyNetQ;
using System.Threading;
using System.Diagnostics;

namespace HttpClient
{

    class Program
    {
        /// <summary>
        /// Запуск клиента для формирвоания запроса к WebApi.
        /// Соединение устанавливается к RabbitMQ серверу через Message Bus.
        /// Задается предыдущее значение, по заданию, для примера работы программы я выбрал число 34 - 9 по номеру в последовательности.
        /// </summary>
        /// <param name="args"></param> - указывает на количество поток, что обязательно должно быть по заданию.
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            var AsyncCount = int.Parse(args[0]);

            var toServer = new Transfer2Server();
            var locServer = new FibonacciClass();
            var subId = $"fib_{Environment.ProcessId}_{Environment.MachineName}";

            try
            {
                using var bus = RabbitHutch.CreateBus("host=localhost:5672;username=guest;password=guest");
                await bus.PubSub.SubscribeAsync<FibonnaciValue>(subId, toServer.Recieve);
            }
            catch (AggregateException exp)
            {
                foreach (Exception e in exp.InnerExceptions)
                {
                    if (e is TaskCanceledException)
                        Console.WriteLine("Операция прервана");
                    else
                        Console.WriteLine(e.Message);
                }
            }
            
            var tasks = Enumerable
                .Range(0, AsyncCount)
                .Select(jobNumber => Task.Run(() => Run(jobNumber, toServer, locServer)))
                .ToArray();

            await Task.WhenAll(tasks);
        }

        static async Task Run(int jobId, Transfer2Server toServer, FibonacciClass locServer)
        {

            var value = new FibonnaciValue(Guid.NewGuid(), 9, 34);

            while (true)
            {
                try
                {
                    var tcs = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    value = await toServer.RequestNextValue(value, tcs.Token);
                }
                catch (AggregateException exp)
                {
                    foreach (Exception e in exp.InnerExceptions)
                    {
                        if (e is TaskCanceledException)
                            Console.WriteLine("Операция прервана");
                        else
                            Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}