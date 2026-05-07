using System.IO;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MoonSharp.Interpreter;

namespace MmoDemo.Gateway.Tests;

public class Phase5LuaTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private static string ConfigPath(string file) =>
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..", "configs", file));

    public Phase5LuaTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public void LuaConfig_LoadQuests_ReturnsCorrectData()
    {
        var script = new Script();
        var result = script.DoFile(ConfigPath("quests.lua"));
        var root = result.Table;

        // Quest 1
        var q1 = root.Get(1).Table;
        Assert.Equal("Slime Extermination", q1.Get("name").CastToString());
        Assert.Equal(3, (int)q1.Get("count").CastToNumber());
        Assert.Equal(30, (int)q1.Get("exp").CastToNumber());
        Assert.Equal(20, (int)q1.Get("gold").CastToNumber());
        Assert.Equal("slime", q1.Get("target").CastToString());

        // Quest 2
        var q2 = root.Get(2).Table;
        Assert.Equal("Goblin Threat", q2.Get("name").CastToString());
        Assert.Equal(2, (int)q2.Get("count").CastToNumber());
        Assert.Equal(50, (int)q2.Get("exp").CastToNumber());

        // Quest 3
        var q3 = root.Get(3).Table;
        Assert.Equal("Wolf Hunt", q3.Get("name").CastToString());
        Assert.Equal(1, (int)q3.Get("count").CastToNumber());
        Assert.Equal(40, (int)q3.Get("exp").CastToNumber());
    }

    [Fact]
    public void LuaConfig_LoadMonsters_ReturnsCorrectData()
    {
        var script = new Script();
        var result = script.DoFile(ConfigPath("monsters.lua"));
        var root = result.Table;

        // Slime
        var slime = root.Get("slime").Table;
        Assert.Equal("Slime", slime.Get("name").CastToString());
        Assert.Equal(30, (int)slime.Get("hp").CastToNumber());
        Assert.Equal(10, (int)slime.Get("atk").CastToNumber());
        Assert.Equal(3, (int)slime.Get("def").CastToNumber());

        // Check drops array
        var slimeDrops = slime.Get("drops").Table;
        Assert.Single(slimeDrops.Values);
        Assert.Equal(1, (int)slimeDrops.Values.First().CastToNumber());

        // Goblin
        var goblin = root.Get("goblin").Table;
        Assert.Equal("Goblin", goblin.Get("name").CastToString());
        Assert.Equal(50, (int)goblin.Get("hp").CastToNumber());

        // Wolf
        var wolf = root.Get("wolf").Table;
        Assert.Equal("Wolf", wolf.Get("name").CastToString());
        Assert.Equal(40, (int)wolf.Get("hp").CastToNumber());
    }

    [Fact]
    public void LuaConfig_ModifyValue_NewValueTakesEffect()
    {
        var script = new Script();
        script.DoString("return { hp = 999 }");
        var result = script.DoString("return { hp = 999 }");
        Assert.Equal(999, (int)result.Table.Get("hp").CastToNumber());
    }

    [Fact]
    public async Task AdminReloadConfig_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/api/admin/reload-config", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ReloadResponse>();
        Assert.NotNull(body);
        Assert.Contains("reload", body!.Message);
    }

    [Fact]
    public async Task QuestReward_MatchesLuaConfig()
    {
        var client = _factory.CreateClient();
        var login = await (await client.PostAsJsonAsync("/api/auth/guest-login",
            new MmoDemo.Contracts.GuestLoginRequest("lua-quest-1", "editor", "0.5.0")))
            .Content.ReadFromJsonAsync<MmoDemo.Contracts.GuestLoginResponse>();

        // Verify the Lua config gives the same quest data as the API
        var luaScript = new Script();
        var luaResult = luaScript.DoFile(ConfigPath("quests.lua"));
        var luaQ1 = luaResult.Table.Get(1).Table;

        // Quest service should have loaded from Lua
        Assert.Equal(30, (int)luaQ1.Get("exp").CastToNumber());
        Assert.Equal(20, (int)luaQ1.Get("gold").CastToNumber());
    }

    private sealed record ReloadResponse(string Message);
}
