using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using rmq.demo.Models;

namespace rmq.demo.Services;

public class SearchEngineListener : BackgroundService
{
    private readonly IConnectionFactory connectionFactory;
    private readonly IForumSearchIndexer searchIndexer;
    private readonly ILogger<SearchEngineListener> logger;

    public SearchEngineListener(
        IConnectionFactory connectionFactory,
        IForumSearchIndexer searchIndexer,
        ILogger<SearchEngineListener> logger)
    {
        this.connectionFactory = connectionFactory;
        this.searchIndexer = searchIndexer;
        this.logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        var connection = connectionFactory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(Topology.ForumEventsExchange, ExchangeType.Topic, true);
        channel.QueueDeclare(Topology.SearchEngineQueue, true);
        channel.QueueBind(Topology.SearchEngineQueue, Topology.ForumEventsExchange, "forum.comment.*");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += (_, args) =>
        {
            var comment = JsonSerializer.Deserialize<Comment>(args.Body.ToArray());
            if (comment is not null)
            {
                logger.LogInformation("Received comment: {Comment}", comment);

                searchIndexer.Index(new SearchEntity
                {
                    Id = comment.Id,
                    EntityType = SearchEntityType.Comment,
                    Text = comment.Text
                });
            }

            channel.BasicAck(args.DeliveryTag, false);
            return Task.CompletedTask;
        };
        channel.BasicConsume(consumer, "search-engine", consumerTag: "search-engine-consumer");
    }
}