using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MmoDemo.Contracts;

namespace MmoDemo.Gateway.Tests;

public class Phase1IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public Phase1IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task FullFlow_Login_CreateRole_SelectRole_EnterCity()
    {
        // ── Step 1: Guest Login ──
        var loginReq = new GuestLoginRequest("test-device-001", "editor", "0.1.0");
        var loginRes = await _client.PostAsJsonAsync("/api/auth/guest-login", loginReq);
        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        var login = await loginRes.Content.ReadFromJsonAsync<GuestLoginResponse>();
        Assert.NotNull(login);
        Assert.Equal(0, login.Code);
        Assert.NotEmpty(login.PlayerId);
        Assert.NotEmpty(login.Token);

        var playerId = login.PlayerId;
        var token = login.Token;

        // ── Step 2: Role List (empty) ──
        var listReq = new GetRoleListRequest(playerId, token);
        var listRes = await _client.PostAsJsonAsync("/api/roles/list", listReq);
        Assert.Equal(HttpStatusCode.OK, listRes.StatusCode);

        var list = await listRes.Content.ReadFromJsonAsync<GetRoleListResponse>();
        Assert.NotNull(list);
        Assert.Equal(0, list.Code);
        Assert.NotNull(list.Roles);
        Assert.Empty(list.Roles);

        // ── Step 3: Create Role ──
        var createReq = new CreateRoleRequest(playerId, token, "TestHero", 1);
        var createRes = await _client.PostAsJsonAsync("/api/roles/create", createReq);
        Assert.Equal(HttpStatusCode.OK, createRes.StatusCode);

        var create = await createRes.Content.ReadFromJsonAsync<CreateRoleResponse>();
        Assert.NotNull(create);
        Assert.Equal(0, create.Code);
        Assert.NotNull(create.Role);
        Assert.Equal("TestHero", create.Role.Name);
        Assert.Equal(1, create.Role.Level);
        Assert.Equal(1, create.Role.ClassId);
        Assert.Equal(1001, create.Role.SceneId);
        Assert.Equal(100, create.Role.Gold);
        Assert.NotEmpty(create.Role.RoleId);

        var roleId = create.Role.RoleId;

        // ── Step 4: Role List (has 1 role) ──
        listRes = await _client.PostAsJsonAsync("/api/roles/list", listReq);
        list = await listRes.Content.ReadFromJsonAsync<GetRoleListResponse>();
        Assert.NotNull(list);
        Assert.Single(list.Roles!);

        // ── Step 5: Select Role ──
        var selectReq = new SelectRoleRequest(playerId, token, roleId);
        var selectRes = await _client.PostAsJsonAsync("/api/roles/select", selectReq);
        Assert.Equal(HttpStatusCode.OK, selectRes.StatusCode);

        var select = await selectRes.Content.ReadFromJsonAsync<SelectRoleResponse>();
        Assert.NotNull(select);
        Assert.Equal(0, select.Code);
        Assert.NotNull(select.Role);
        Assert.Equal(roleId, select.Role.RoleId);

        // ── Step 6: Enter City ──
        var cityReq = new EnterCityRequest(playerId, token, roleId);
        var cityRes = await _client.PostAsJsonAsync("/api/scene/enter-city", cityReq);
        Assert.Equal(HttpStatusCode.OK, cityRes.StatusCode);

        var city = await cityRes.Content.ReadFromJsonAsync<EnterCityResponse>();
        Assert.NotNull(city);
        Assert.Equal(0, city.Code);
        Assert.NotNull(city.Role);
        Assert.Equal("TestHero", city.Role.Name);
        Assert.True(city.ServerTime > 0);
    }

    [Fact]
    public async Task Login_InvalidDeviceId_ReturnsError()
    {
        var req = new GuestLoginRequest("", "editor", "0.1.0");
        var res = await _client.PostAsJsonAsync("/api/auth/guest-login", req);
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task RoleList_InvalidToken_ReturnsUnauthorized()
    {
        var req = new GetRoleListRequest("fake-player", "fake-token");
        var res = await _client.PostAsJsonAsync("/api/roles/list", req);
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<GetRoleListResponse>();
        Assert.Equal(ErrorCodes.Unauthorized, body!.Code);
    }

    [Fact]
    public async Task CreateRole_InvalidName_ReturnsError()
    {
        // Login first
        var loginReq = new GuestLoginRequest("test-device-002", "editor", "0.1.0");
        var loginRes = await _client.PostAsJsonAsync("/api/auth/guest-login", loginReq);
        var login = await loginRes.Content.ReadFromJsonAsync<GuestLoginResponse>();

        // Empty name
        var req = new CreateRoleRequest(login!.PlayerId, login.Token, "", 1);
        var res = await _client.PostAsJsonAsync("/api/roles/create", req);
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<CreateRoleResponse>();
        Assert.Equal(ErrorCodes.InvalidRoleName, body!.Code);

        // Name too long
        req = new CreateRoleRequest(login.PlayerId, login.Token, new string('A', 20), 1);
        res = await _client.PostAsJsonAsync("/api/roles/create", req);
        body = await res.Content.ReadFromJsonAsync<CreateRoleResponse>();
        Assert.Equal(ErrorCodes.InvalidRoleName, body!.Code);
    }

    [Fact]
    public async Task CreateRole_InvalidClass_ReturnsError()
    {
        var loginReq = new GuestLoginRequest("test-device-003", "editor", "0.1.0");
        var loginRes = await _client.PostAsJsonAsync("/api/auth/guest-login", loginReq);
        var login = await loginRes.Content.ReadFromJsonAsync<GuestLoginResponse>();

        var req = new CreateRoleRequest(login!.PlayerId, login.Token, "Hero", 99);
        var res = await _client.PostAsJsonAsync("/api/roles/create", req);

        var body = await res.Content.ReadFromJsonAsync<CreateRoleResponse>();
        Assert.Equal(ErrorCodes.InvalidClassId, body!.Code);
    }

    [Fact]
    public async Task CreateRole_RoleLimitReached_ReturnsError()
    {
        var loginReq = new GuestLoginRequest("test-device-004", "editor", "0.1.0");
        var loginRes = await _client.PostAsJsonAsync("/api/auth/guest-login", loginReq);
        var login = await loginRes.Content.ReadFromJsonAsync<GuestLoginResponse>();
        var pid = login!.PlayerId;
        var token = login.Token;

        // Create max roles
        for (int i = 0; i < ErrorCodes.MaxRolesPerPlayer; i++)
        {
            var req = new CreateRoleRequest(pid, token, $"Hero{i}", 1);
            var res = await _client.PostAsJsonAsync("/api/roles/create", req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        // Next one should fail
        var failReq = new CreateRoleRequest(pid, token, "ExtraHero", 1);
        var failRes = await _client.PostAsJsonAsync("/api/roles/create", failReq);
        Assert.Equal(HttpStatusCode.BadRequest, failRes.StatusCode);

        var body = await failRes.Content.ReadFromJsonAsync<CreateRoleResponse>();
        Assert.Equal(ErrorCodes.RoleLimitReached, body!.Code);
    }

    [Fact]
    public async Task SelectRole_InvalidRole_ReturnsError()
    {
        var loginReq = new GuestLoginRequest("test-device-005", "editor", "0.1.0");
        var loginRes = await _client.PostAsJsonAsync("/api/auth/guest-login", loginReq);
        var login = await loginRes.Content.ReadFromJsonAsync<GuestLoginResponse>();

        var req = new SelectRoleRequest(login!.PlayerId, login.Token, "non-existent-role");
        var res = await _client.PostAsJsonAsync("/api/roles/select", req);

        var body = await res.Content.ReadFromJsonAsync<SelectRoleResponse>();
        Assert.Equal(ErrorCodes.RoleNotFound, body!.Code);
    }

    [Fact]
    public async Task EnterCity_InvalidRole_ReturnsError()
    {
        var loginReq = new GuestLoginRequest("test-device-006", "editor", "0.1.0");
        var loginRes = await _client.PostAsJsonAsync("/api/auth/guest-login", loginReq);
        var login = await loginRes.Content.ReadFromJsonAsync<GuestLoginResponse>();

        var req = new EnterCityRequest(login!.PlayerId, login.Token, "non-existent-role");
        var res = await _client.PostAsJsonAsync("/api/scene/enter-city", req);

        var body = await res.Content.ReadFromJsonAsync<EnterCityResponse>();
        Assert.Equal(ErrorCodes.RoleNotFound, body!.Code);
    }
}
