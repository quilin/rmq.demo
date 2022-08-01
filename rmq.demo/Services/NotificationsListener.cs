using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using rmq.demo.Models;

namespace rmq.demo.Services;

public class NotificationsListener : BackgroundService
{
    private readonly IConnectionFactory connectionFactory;
    private readonly ILogger<NotificationsListener> logger;

    public NotificationsListener(
        IConnectionFactory connectionFactory,
        ILogger<NotificationsListener> logger)
    {
        this.connectionFactory = connectionFactory;
        this.logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        var connection = connectionFactory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(Topology.ForumEventsExchange, ExchangeType.Topic, true);
        channel.QueueDeclare(Topology.NotificationsQueue, true);
        channel.QueueBind(Topology.NotificationsQueue, Topology.ForumEventsExchange, "forum.comment.created");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += (_, args) =>
        {
            var comment = JsonSerializer.Deserialize<Comment>(args.Body.ToArray());
            if (comment is not null)
            {
                logger.LogInformation("Received comment from {Author}, let's notify everyone!", comment.Author);
            }

            channel.BasicAck(args.DeliveryTag, false);
            return Task.CompletedTask;
        };
        channel.BasicConsume(consumer, "notifications");
    }
}