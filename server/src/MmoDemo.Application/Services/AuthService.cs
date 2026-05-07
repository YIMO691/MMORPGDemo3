using MmoDemo.Contracts;
using MmoDemo.Domain;

namespace MmoDemo.Application;

public class AuthService : IAuthService
{
    private readonly IPlayerRepository _players;

    public AuthService(IPlayerRepository players)
    {
        _players = players;
    }

    public GuestLoginResponse GuestLogin(GuestLoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.DeviceId))
        {
            return new GuestLoginResponse(
                ErrorCodes.InvalidRequest,
                "Device ID is required",
                string.Empty,
                string.Empty,
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            );
        }

        var playerId = $"player_{Guid.NewGuid():N}";
        var token = $"token_{Guid.NewGuid():N}";

        var session = new PlayerSession { Token = token };
        _players.AddSession(playerId, session);

        return new GuestLoginResponse(
            ErrorCodes.Ok,
            "OK",
            playerId,
            token,
            DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        );
    }

    public bool ValidateToken(string playerId, string token)
    {
        return _players.ValidateToken(playerId, token);
    }
}
