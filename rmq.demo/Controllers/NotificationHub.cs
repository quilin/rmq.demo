using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using rmq.demo.Services;

namespace rmq.demo.Controllers;

public class NotificationHub : Hub
{
    private readonly IConnectionService connectionService;

    public NotificationHub(
        IConnectionService connectionService)
    {
        this.connectionService = connectionService;
    }

    public override Task OnConnectedAsync()
    {
        var user = TryExtractUser();
        if (user is not null)
        {
            connectionService.Add(user, Context.ConnectionId);
        }

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var user = TryExtractUser();
        if (user is not null)
        {
            connectionService.Remove(user, Context.ConnectionId);
        }

        return base.OnDisconnectedAsync(exception);
    }

    private string? TryExtractUser()
    {
        string user;
        if (!Context.GetHttpContext().Request.Query.TryGetValue("access_token", out var queryValues) ||
            !queryValues.Any() || string.IsNullOrWhiteSpace(user = queryValues.First()))
        {
            return null;
        }

        return user;
    }
    
}