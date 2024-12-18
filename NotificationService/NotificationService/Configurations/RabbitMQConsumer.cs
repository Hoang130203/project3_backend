

using Microsoft.Extensions.Options;
using NotificationService.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
namespace NotificationService.Configurations
{
    //public class RabbitMQConsumer : BackgroundService
    //{
        //private readonly IConnection _connection;
        //private readonly IModel _channel;
        //private readonly INotificationProcessor _processor;
        //private readonly ILogger<RabbitMQConsumer> _logger;

        //public RabbitMQConsumer(
        //    IOptions<RabbitMQConfig> options,
        //    INotificationProcessor processor,
        //    ILogger<RabbitMQConsumer> logger)
        //{
        //    _processor = processor;
        //    _logger = logger;

        //    var factory = new ConnectionFactory
        //    {
        //        HostName = options.Value.HostName,
        //        UserName = options.Value.UserName,
        //        Password = options.Value.Password,
        //        VirtualHost = options.Value.VirtualHost
        //    };

        //    _connection = factory.CreateConnection();
        //    _channel = _connection.CreateModel();

        //    _channel.ExchangeDeclare("notifications", ExchangeType.Topic);
        //    _channel.QueueDeclare("notifications", true, false, false, null);
        //    _channel.QueueBind("notifications", "notifications", "notification.*");
        //}

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    var consumer = new EventingBasicConsumer(_channel);

        //    consumer.Received += async (model, ea) =>
        //    {
        //        try
        //        {
        //            var body = ea.Body.ToArray();
        //            var message = JsonSerializer.Deserialize<NotificationEvent>(
        //                Encoding.UTF8.GetString(body));

        //            await _processor.ProcessAsync(message);
        //            _channel.BasicAck(ea.DeliveryTag, false);
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Error processing message");
        //            _channel.BasicNack(ea.DeliveryTag, false, true);
        //        }
        //    };

        //    _channel.BasicConsume(
        //        queue: "notifications",
        //        autoAck: false,
        //        consumer: consumer);

        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        await Task.Delay(1000, stoppingToken);
        //    }
        //}

        //public override void Dispose()
        //{
        //    _channel?.Dispose();
        //    _connection?.Dispose();
        //    base.Dispose();
        //}
    //}
}
