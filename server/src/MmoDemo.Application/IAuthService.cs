using MmoDemo.Contracts;

namespace MmoDemo.Application;

public interface IAuthService
{
    GuestLoginResponse GuestLogin(GuestLoginRequest request);
    bool ValidateToken(string playerId, string token);
}
