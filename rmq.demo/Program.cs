using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nest;
using RabbitMQ.Client;
using rmq.demo;
using rmq.demo.Controllers;
using rmq.demo.Models;
using rmq.demo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions();
builder.Services.Configure<Connections>(builder.Configuration.GetSection(nameof(Connections)).Bind);

// Add services to the container.
builder.Services.AddSingleton<IConnectionService, ConnectionService>();
builder.Services.AddSingleton<IForumDataProvider, ForumDataProvider>();
builder.Services.AddSingleton<IForumSearchIndexer, ForumSearchIndexer>();

builder.Services.AddSingleton<ConnectionSettings>(sp =>
{
    var connectionString = sp.GetRequiredService<IOptions<Connections>>().Value.ElasticSearch;
    return new ConnectionSettings(new Uri(connectionString))
        .DefaultMappingFor<SearchEntity>(m => m
            .IndexName("search"));
});
builder.Services.AddSingleton<IElasticClient>(sp =>
    new ElasticClient(sp.GetRequiredService<ConnectionSettings>()));

builder.Services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
{
    Endpoint = new AmqpTcpEndpoint(),
    DispatchConsumersAsync = true
});
builder.Services.AddHostedService<SearchEngineListener>();
builder.Services.AddHostedService<NotificationsListener>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/whatsup");

app.Run();