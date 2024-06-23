using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fibonacci;
using System.Net.Http.Json;
using System.Threading;
using System.Collections.Concurrent;

namespace HttpClient
{
    class Transfer2Server
    {
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<FibonnaciValue>> pendingRequests = new();
        
        /// <summary>
        /// Создание клиента к WebApi и отправка запросов к нему идёт через http-запросы.
        /// </summary>
        private readonly System.Net.Http.HttpClient client = new System.Net.Http.HttpClient() { 
            BaseAddress = new Uri("http://localhost:5278") 
        };

        public void Recieve(FibonnaciValue result)
        {
            if (pendingRequests.TryRemove(result.Id, out var tcs))
            {
                Console.WriteLine($"Receiving {result}");
                tcs.SetResult(result);
            }
            else
            {
                Console.WriteLine($"Discarding unexpected {result}");
            }
        }

        /// <summary>
        /// Запрос следующего значения.Именно здесь выполняется запрос к серверу.
        /// </summary>
        /// <param name="fibVal"></param> - значение предыдущее, задается по заданию.
        /// <param name="stop"></param> - токен останавливающий вычисления;
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FibonnaciValue> RequestNextValue(
            FibonnaciValue fibVal, CancellationToken stop)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "fibonacci") 
            { 
                Content = JsonContent.Create(fibVal)
            };

            var tcs = new TaskCompletionSource<FibonnaciValue>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            
            stop.Register(() => tcs.TrySetCanceled(stop));

            if (pendingRequests.TryAdd(fibVal.Id, tcs))
            {
                var response = await client.SendAsync(request, stop);
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Unexpected status code {response.StatusCode}");
                else
                {
                    var result = await tcs.Task;
                    return result;
                }
            }
            else
            {
                throw new Exception($"Can't add tcs for {(fibVal.Id, fibVal.N)}");
            }
        }
    }
}