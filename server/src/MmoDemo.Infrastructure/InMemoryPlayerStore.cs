using System.Collections.Concurrent;
using MmoDemo.Application;
using MmoDemo.Domain;

namespace MmoDemo.Infrastructure;

public class InMemoryPlayerStore : IPlayerRepository
{
    private readonly ConcurrentDictionary<string, Player> _players = new();

    public Player GetOrCreate(string playerId)
    {
        return _players.GetOrAdd(playerId, _ => new Player { PlayerId = playerId });
    }

    public Player? Get(string playerId)
    {
        _players.TryGetValue(playerId, out var player);
        return player;
    }

    public void AddSession(string playerId, PlayerSession session)
    {
        var player = GetOrCreate(playerId);
        player.Sessions.Add(session);
    }

    public bool ValidateToken(string playerId, string token)
    {
        var player = Get(playerId);
        return player?.Sessions.Any(s => s.Token == token) ?? false;
    }
}
