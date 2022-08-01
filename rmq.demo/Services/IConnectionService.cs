using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rmq.demo.Services;

public interface IConnectionService
{
    Task Add(string user, string connectionId);
    Task Remove(string user, string connectionId);
    IReadOnlyDictionary<string, IEnumerable<string>> GetConnectedUsers();
}

internal class ConnectionService : IConnectionService
{
    private static readonly ConcurrentDictionary<string, HashSet<string>> Connections = new();

    public Task Add(string user, string connectionId)
    {
        Connections.AddOrUpdate(
            user,
            _ => new HashSet<string> { connectionId },
            (_, connectionIds) =>
            {
                connectionIds.Add(connectionId);
                return connectionIds;
            });
        return Task.CompletedTask;
    }

    public Task Remove(string user, string connectionId)
    {
        if (!Connections.TryGetValue(user, out var connectionIds))
        {
            return Task.CompletedTask;
        }

        connectionIds.Remove(connectionId);
        if (connectionIds.Count == 0)
        {
            Connections.TryRemove(user, out _);
        }

        return Task.CompletedTask;
    }

    public IReadOnlyDictionary<string, IEnumerable<string>> GetConnectedUsers() =>
        Connections.ToDictionary(
            k => k.Key,
            v => v.Value.Select(c => c));
}