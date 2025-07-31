using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace ProducerMailService
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            int count = 1;

            while (true)
            {
                string message = $"Message {count++} at {DateTime.Now:HH:mm:ss}";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "demo.exchange",
                    routingKey: "demo.key",
                    basicProperties: null,
                    body: body
                );

                Console.WriteLine($"🚀 Sent: {message}");

                Thread.Sleep(10_000); // 10 giây
            }
        }
    }
}