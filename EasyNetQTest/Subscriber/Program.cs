﻿    using System;
    using EasyNetQ;
    using Messages;

    namespace Subscriber 
    {
        class Program 
        {
            static void Main(string[] args) 
            {
                using var bus = RabbitHutch.CreateBus("host=localhost");
                bus.PubSub.Subscribe<TextMessage>("test", HandleTextMessage);
                Console.WriteLine("Listening for messages. Hit <return> to quit.");
                Console.ReadLine();
            }

            static void HandleTextMessage(TextMessage textMessage) 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Got message: {0}", textMessage.Text);
                Console.ResetColor();
            }
        }
    }
