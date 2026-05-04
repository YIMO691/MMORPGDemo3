namespace MmoDemo.Contracts;

public static class ErrorCodes
{
    public const int Ok = 0;
    public const int Unauthorized = 1001;
    public const int InvalidRoleName = 1002;
    public const int RoleLimitReached = 1003;
    public const int InvalidClassId = 1004;
    public const int RoleNotFound = 1005;
    public const int ServerError = 1006;
    public const int SceneAccessDenied = 1007;
    public const int InvalidRequest = 1008;

    public static readonly int[] ValidClassIds = [1, 2, 3];
    public const int MaxRolesPerPlayer = 4;
    public const int MinRoleNameLength = 1;
    public const int MaxRoleNameLength = 12;
}
