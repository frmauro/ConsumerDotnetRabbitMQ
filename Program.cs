using System.Text;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

var logger = new LoggerConfiguration().WriteTo.Console(theme: AnsiConsoleTheme.Literate)
.CreateLogger();

logger.Information("Testando o recebimento de mensagens para uma Fila do RabbitMQ");

var queueName = "purshaseok";

try
{
    var factory = new ConnectionFactory() { HostName = "localhost" };

    using var connection = factory.CreateConnection();
    using var channel = connection.CreateModel();



    channel.QueueDeclare(queue: queueName,
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);
    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        logger.Information($"current message(s) received: {message}");
    };

    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    Console.ReadLine();

}
catch (System.Exception ex)
{
    logger.Error($"Exceção: {ex.GetType().FullName} | " + $"Mensagem: {ex.Message}");
}


