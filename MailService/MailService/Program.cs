using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MailService
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            RabbitMqInit.Initialize();

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                // Connection efficiency settings
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Use proper resource management with using statements
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Set prefetch count for better throughput control
            channel.BasicQos(prefetchSize: 0, prefetchCount: 20, global: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"📨 Received: {message}");

                    // Simulate message processing
                    ProcessMessage(message);

                    // Acknowledge message only after successful processing
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error processing message: {ex.Message}");
                    
                    // Reject message and requeue it for retry
                    channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            channel.BasicConsume(queue: "demo.queue", autoAck: false, consumer: consumer);

            Console.WriteLine("✅ Worker started. Press [Enter] to exit.");
            Console.ReadLine();

            // Resources will be automatically disposed by using statements
        }

        private static void ProcessMessage(string message)
        {
            // Simulate actual message processing work
            // In a real application, this would contain business logic
            System.Threading.Thread.Sleep(100); // Simulate processing time
        }
    }
}