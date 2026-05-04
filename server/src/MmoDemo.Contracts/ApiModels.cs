namespace MmoDemo.Contracts;

// ── Auth ──
public record GuestLoginRequest(string DeviceId, string Platform, string AppVersion);

public record GuestLoginResponse(
    int Code,
    string Message,
    string PlayerId,
    string Token,
    long ServerTime
);

// ── Roles ──
public record RoleInfo(
    string RoleId,
    string Name,
    int Level,
    int ClassId,
    int SceneId,
    long Gold
);

public record GetRoleListRequest(string PlayerId, string Token);

public record GetRoleListResponse(
    int Code,
    string Message,
    List<RoleInfo>? Roles
);

public record CreateRoleRequest(string PlayerId, string Token, string Name, int ClassId);

public record CreateRoleResponse(
    int Code,
    string Message,
    RoleInfo? Role
);

public record SelectRoleRequest(string PlayerId, string Token, string RoleId);

public record SelectRoleResponse(
    int Code,
    string Message,
    RoleInfo? Role
);

// ── Scene ──
public record EnterCityRequest(string PlayerId, string Token, string RoleId);

public record EnterCityResponse(
    int Code,
    string Message,
    RoleInfo? Role,
    long ServerTime
);
