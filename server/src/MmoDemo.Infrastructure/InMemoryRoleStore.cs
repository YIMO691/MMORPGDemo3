using System.Collections.Concurrent;
using MmoDemo.Application;
using MmoDemo.Domain;

namespace MmoDemo.Infrastructure;

public class InMemoryRoleStore : IRoleRepository
{
    private readonly ConcurrentDictionary<string, Role> _roles = new();

    public Role Create(string roleId, string playerId, string name, int classId)
    {
        var role = new Role
        {
            RoleId = roleId,
            PlayerId = playerId,
            Name = name,
            ClassId = classId,
            Level = 1,
            SceneId = 1001,
            Gold = 100
        };
        _roles[roleId] = role;
        return role;
    }

    public Role? Get(string roleId)
    {
        _roles.TryGetValue(roleId, out var role);
        return role;
    }

    public List<Role> GetByPlayer(string playerId)
    {
        return _roles.Values
            .Where(r => r.PlayerId == playerId)
            .OrderBy(r => r.CreatedAt)
            .ToList();
    }

    public int CountByPlayer(string playerId)
    {
        return _roles.Values.Count(r => r.PlayerId == playerId);
    }
}
