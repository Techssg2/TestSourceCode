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
                Password = "guest",
                // Add connection efficiency settings
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            // Use proper resource management
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            int count = 1;

            try
            {
                while (true)
                {
                    string message = $"Message {count++} at {DateTime.Now:HH:mm:ss}";
                    var body = Encoding.UTF8.GetBytes(message);

                    // Add message properties for better efficiency
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true; // Make messages durable
                    properties.MessageId = Guid.NewGuid().ToString();
                    properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                    channel.BasicPublish(
                        exchange: "demo.exchange",
                        routingKey: "demo.key",
                        basicProperties: properties,
                        body: body
                    );

                    Console.WriteLine($"🚀 Sent: {message}");

                    Thread.Sleep(10_000); // 10 giây
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in producer: {ex.Message}");
                throw;
            }
        }
    }
}