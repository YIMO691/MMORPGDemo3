using MmoDemo.Application;
using MmoDemo.Contracts;
using MmoDemo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── In-memory repositories ──
builder.Services.AddSingleton<IPlayerRepository, InMemoryPlayerStore>();
builder.Services.AddSingleton<IRoleRepository, InMemoryRoleStore>();

// ── Application services ──
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IRoleService, RoleService>();
builder.Services.AddSingleton<ISceneService, SceneService>();

var app = builder.Build();

// ── Health check ──
app.MapGet("/health", () => Results.Ok(new
{
    Status = "OK",
    Service = "MmoDemo.Gateway",
    Phase = "Phase 1"
}));

// ── Guest Login ──
app.MapPost("/api/auth/guest-login", (GuestLoginRequest request, IAuthService auth) =>
{
    var response = auth.GuestLogin(request);
    return response.Code == ErrorCodes.Ok
        ? Results.Ok(response)
        : Results.BadRequest(response);
});

// ── Role List ──
app.MapPost("/api/roles/list", (GetRoleListRequest request, IRoleService roles) =>
{
    var response = roles.GetRoleList(request);
    return response.Code == ErrorCodes.Ok
        ? Results.Ok(response)
        : Results.BadRequest(response);
});

// ── Create Role ──
app.MapPost("/api/roles/create", (CreateRoleRequest request, IRoleService roles) =>
{
    var response = roles.CreateRole(request);
    return response.Code == ErrorCodes.Ok
        ? Results.Ok(response)
        : Results.BadRequest(response);
});

// ── Select Role ──
app.MapPost("/api/roles/select", (SelectRoleRequest request, IRoleService roles) =>
{
    var response = roles.SelectRole(request);
    return response.Code == ErrorCodes.Ok
        ? Results.Ok(response)
        : Results.BadRequest(response);
});

// ── Enter City ──
app.MapPost("/api/scene/enter-city", (EnterCityRequest request, ISceneService scene) =>
{
    var response = scene.EnterCity(request);
    return response.Code == ErrorCodes.Ok
        ? Results.Ok(response)
        : Results.BadRequest(response);
});

app.Run();

public partial class Program;
