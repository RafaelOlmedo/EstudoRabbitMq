using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory
{
    HostName = "localhost",
    UserName = "admin",
    Password = "admin"
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

const string QueueName = "ExemploRabbitMq";

channel.QueueDeclare(queue: QueueName,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    try
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($" [x] Received {message}");
        channel.BasicAck(ea.DeliveryTag, false);
    }
    catch (Exception ex)
    {
        channel.BasicNack(ea.DeliveryTag, false, true);
        Console.WriteLine($"Erro ao processar mensagem {ea.RoutingKey}. Retorno: {ex.Message}.");
    }
  
};
channel.BasicConsume(queue: QueueName,
                     autoAck: false,
                     consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();