using RabbitMQ.Client;
using System;

namespace MailService
{
    public static class RabbitMqInit
    {
        public static void Initialize()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                // Add connection efficiency settings for initialization
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                AutomaticRecoveryEnabled = true
            };

            try
            {
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                // Declare exchange with proper error handling
                channel.ExchangeDeclare("demo.exchange", ExchangeType.Topic, durable: true);
                
                // Declare queue with message TTL for efficiency (optional optimization)
                var queueArgs = new System.Collections.Generic.Dictionary<string, object>();
                channel.QueueDeclare("demo.queue", durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
                
                channel.QueueBind("demo.queue", "demo.exchange", "demo.key");
                
                Console.WriteLine("🔧 RabbitMQ infrastructure initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to initialize RabbitMQ: {ex.Message}");
                throw;
            }
        }
    }
}