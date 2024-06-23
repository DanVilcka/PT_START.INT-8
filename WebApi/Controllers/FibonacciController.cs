using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fibonacci;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FibonacciController : ControllerBase
    {
        private readonly ILogger<FibonacciController> logger;
        private readonly IBus bus;
        private readonly FibonacciClass fibonacci;

        public FibonacciController(
            ILogger<FibonacciController> logger, 
            IBus bus, 
            FibonacciClass fibonacci)
        {
            this.logger = logger;
            this.bus = bus;
            this.fibonacci = fibonacci;
        }

        [HttpPost]
        public IActionResult Post(FibonnaciValue current)
        {
            var calculate_Task = Task.Run(() => {
                var next = FibonacciClass.CalculateNext(current);
                logger.LogInformation($"Sending fib({next.N}) -> {next.Value}, id = {current.Id}");
                return bus.PubSub.PublishAsync(next);
            });
            
            calculate_Task.ConfigureAwait(false);
            var message = $"Requested fib({current.N + 1}), id = {current.Id}";
            logger.LogInformation(message);
            return StatusCode(200, message);
        }
    }
}