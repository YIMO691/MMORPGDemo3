using MmoDemo.Domain;

namespace MmoDemo.Application;

public interface IRoleRepository
{
    Role Create(string roleId, string playerId, string name, int classId);
    Role? Get(string roleId);
    List<Role> GetByPlayer(string playerId);
    int CountByPlayer(string playerId);
}
