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
                Password = "guest"
            };
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"📨 Received: {message}");

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: "demo.queue", autoAck: false, consumer: consumer);

            Console.WriteLine("✅ Worker started. Press [Enter] to exit.");
            Console.ReadLine();

            channel.Close();
            connection.Close();
        }
    }
}