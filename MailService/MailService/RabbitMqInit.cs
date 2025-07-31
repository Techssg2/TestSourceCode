using RabbitMQ.Client;

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
                Password = "guest"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare("demo.exchange", ExchangeType.Topic, durable: true);
            channel.QueueDeclare("demo.queue", durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind("demo.queue", "demo.exchange", "demo.key");
        }
    }
}