using MmoDemo.Contracts;

namespace MmoDemo.Application;

public class RoleService : IRoleService
{
    private readonly IAuthService _authService;
    private readonly IPlayerRepository _players;
    private readonly IRoleRepository _roles;

    public RoleService(IAuthService authService, IPlayerRepository players, IRoleRepository roles)
    {
        _authService = authService;
        _players = players;
        _roles = roles;
    }

    public GetRoleListResponse GetRoleList(GetRoleListRequest request)
    {
        if (!_authService.ValidateToken(request.PlayerId, request.Token))
        {
            return new GetRoleListResponse(ErrorCodes.Unauthorized, "Player not authenticated", null);
        }

        var roles = _roles.GetByPlayer(request.PlayerId)
            .Select(r => new RoleInfo(r.RoleId, r.Name, r.Level, r.ClassId, r.SceneId, r.Gold))
            .ToList();

        return new GetRoleListResponse(ErrorCodes.Ok, "OK", roles);
    }

    public CreateRoleResponse CreateRole(CreateRoleRequest request)
    {
        if (!_authService.ValidateToken(request.PlayerId, request.Token))
        {
            return new CreateRoleResponse(ErrorCodes.Unauthorized, "Player not authenticated", null);
        }

        var name = request.Name?.Trim() ?? string.Empty;
        if (name.Length < ErrorCodes.MinRoleNameLength || name.Length > ErrorCodes.MaxRoleNameLength)
        {
            return new CreateRoleResponse(ErrorCodes.InvalidRoleName,
                $"Role name must be between {ErrorCodes.MinRoleNameLength} and {ErrorCodes.MaxRoleNameLength} characters", null);
        }

        if (!Array.Exists(ErrorCodes.ValidClassIds, id => id == request.ClassId))
        {
            return new CreateRoleResponse(ErrorCodes.InvalidClassId, "Class ID invalid", null);
        }

        var count = _roles.CountByPlayer(request.PlayerId);
        if (count >= ErrorCodes.MaxRolesPerPlayer)
        {
            return new CreateRoleResponse(ErrorCodes.RoleLimitReached, "Role limit reached", null);
        }

        var roleId = $"role_{Guid.NewGuid():N}";
        var role = _roles.Create(roleId, request.PlayerId, name, request.ClassId);

        var player = _players.Get(request.PlayerId);
        player?.RoleIds.Add(roleId);

        var roleInfo = new RoleInfo(role.RoleId, role.Name, role.Level, role.ClassId, role.SceneId, role.Gold);
        return new CreateRoleResponse(ErrorCodes.Ok, "OK", roleInfo);
    }

    public SelectRoleResponse SelectRole(SelectRoleRequest request)
    {
        if (!_authService.ValidateToken(request.PlayerId, request.Token))
        {
            return new SelectRoleResponse(ErrorCodes.Unauthorized, "Player not authenticated", null);
        }

        var role = _roles.Get(request.RoleId);
        if (role == null || role.PlayerId != request.PlayerId)
        {
            return new SelectRoleResponse(ErrorCodes.RoleNotFound, "Role not found", null);
        }

        var player = _players.Get(request.PlayerId);
        if (player != null)
        {
            player.SelectedRoleId = request.RoleId;
        }

        var roleInfo = new RoleInfo(role.RoleId, role.Name, role.Level, role.ClassId, role.SceneId, role.Gold);
        return new SelectRoleResponse(ErrorCodes.Ok, "OK", roleInfo);
    }
}
