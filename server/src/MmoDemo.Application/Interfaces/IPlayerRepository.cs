using MmoDemo.Domain;

namespace MmoDemo.Application;

public interface IPlayerRepository
{
    Player GetOrCreate(string playerId);
    Player? Get(string playerId);
    void AddSession(string playerId, PlayerSession session);
    bool ValidateToken(string playerId, string token);
}
