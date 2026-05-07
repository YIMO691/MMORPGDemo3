using MmoDemo.Contracts;

namespace MmoDemo.Application;

public class SceneService : ISceneService
{
    private readonly IAuthService _authService;
    private readonly IRoleRepository _roles;

    public SceneService(IAuthService authService, IRoleRepository roles)
    {
        _authService = authService;
        _roles = roles;
    }

    public EnterCityResponse EnterCity(EnterCityRequest request)
    {
        if (!_authService.ValidateToken(request.PlayerId, request.Token))
        {
            return new EnterCityResponse(
                ErrorCodes.Unauthorized, "Player not authenticated", null,
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }

        var role = _roles.Get(request.RoleId);
        if (role == null || role.PlayerId != request.PlayerId)
        {
            return new EnterCityResponse(
                ErrorCodes.RoleNotFound, "Role not found", null,
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }

        var roleInfo = new RoleInfo(role.RoleId, role.Name, role.Level, role.ClassId, role.SceneId, role.Gold);
        return new EnterCityResponse(ErrorCodes.Ok, "OK", roleInfo,
            DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }
}
