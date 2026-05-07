using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MmoDemo.Gateway.Tests;

public class Phase6ResourceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public Phase6ResourceTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task ResourceManifest_ReturnsFileList()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/resources/manifest");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var files = doc.RootElement.GetProperty("files");
        Assert.True(files.GetArrayLength() > 0, "Manifest should contain at least one file");

        var firstName = files[0].GetProperty("name").GetString();
        var firstHash = files[0].GetProperty("hash").GetString();
        Assert.NotNull(firstName);
        Assert.NotNull(firstHash);
    }

    [Fact]
    public async Task ResourceDownload_ReturnsFile()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/resources/welcome.txt");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("MMORPG", content);
    }

    [Fact]
    public async Task ResourceDownload_VersionFile()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/resources/version.txt");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.True(content.Trim().Length > 0);
    }

    [Fact]
    public async Task ResourceDownload_InvalidPath_Returns404()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/resources/../.env");
        // Should be 404 — directory traversal blocked
        Assert.NotEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}
