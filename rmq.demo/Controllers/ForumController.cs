using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using rmq.demo.Services;

namespace rmq.demo.Controllers;

[ApiController]
[Route("fora")]
public class ForumController : ControllerBase
{
    [HttpGet("")]
    public IActionResult Get([FromServices] IForumDataProvider dataProvider) =>
        Ok(dataProvider.GetFora());

    [HttpGet("{forumId:guid}")]
    public IActionResult Get(Guid forumId, [FromServices] IForumDataProvider dataProvider)
    {
        var forum = dataProvider.GetFora().FirstOrDefault(f => f.Id == forumId);
        return forum is null ? NotFound() : Ok(forum);
    }

    [HttpGet("{forumId:guid}/topics/{topicId:guid}")]
    public IActionResult GetTopic(Guid forumId, Guid topicId,
        [FromServices] IForumDataProvider dataProvider)
    {
        var topic = dataProvider.GetFora().FirstOrDefault(f => f.Id == forumId)
            ?.Topics.FirstOrDefault(t => t.Id == topicId);
        return topic is null ? NotFound() : Ok(topic);
    }

    [HttpGet("{forumId:guid}/topics/{topicId:guid}/comments/{commentId:guid}", Name = nameof(GetComment))]
    public IActionResult GetComment(Guid forumId, Guid topicId, Guid commentId,
        [FromServices] IForumDataProvider dataProvider)
    {
        var comment = dataProvider.GetFora().FirstOrDefault(f => f.Id == forumId)
            ?.Topics.FirstOrDefault(t => t.Id == topicId)
            ?.Comments.FirstOrDefault(c => c.Id == commentId);
        return comment is null ? NotFound() : Ok(comment);
    }

    [HttpPost("{forumId:guid}/topics/{topicId:guid}/comments")]
    public IActionResult PostComment(Guid forumId, Guid topicId,
        [FromBody] string text,
        [FromServices] IForumDataProvider dataProvider,
        [FromServices] IConnectionFactory connectionFactory)
    {
        var topic = dataProvider.GetFora().FirstOrDefault(f => f.Id == forumId)
            ?.Topics.FirstOrDefault(t => t.Id == topicId);
        if (topic is null) return NotFound();

        var comment = dataProvider.CreateComment(text);
        topic.Comments.Add(comment);

        using var connection = connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(Topology.ForumEventsExchange, ExchangeType.Topic);

        var basicProperties = channel.CreateBasicProperties();
        channel.BasicPublish(
            new PublicationAddress(ExchangeType.Topic, Topology.ForumEventsExchange, "forum.comment.created"),
            basicProperties,
            JsonSerializer.SerializeToUtf8Bytes(comment));

        return CreatedAtRoute(nameof(GetComment), new { forumId, topicId, commentId = comment.Id }, comment);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query,
        [FromServices] IForumSearchIndexer search) =>
        Ok(await search.Find(query));
}