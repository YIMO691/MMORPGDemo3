using System.Security.Cryptography;

namespace MmoDemo.Application;

public class ResourceService : IResourceService
{
    private readonly string _resourceRoot;

    public ResourceService(string resourceRoot = "resources")
    {
        _resourceRoot = Path.GetFullPath(resourceRoot);
    }

    public List<ResourceManifestEntry> GetManifest()
    {
        if (!Directory.Exists(_resourceRoot))
            return [];

        var files = Directory.GetFiles(_resourceRoot, "*", SearchOption.AllDirectories);
        var entries = new List<ResourceManifestEntry>();
        foreach (var file in files)
        {
            var relativePath = Path.GetRelativePath(_resourceRoot, file).Replace('\\', '/');
            var info = new FileInfo(file);
            var hash = ComputeHash(file);
            entries.Add(new ResourceManifestEntry(relativePath, hash, info.Length));
        }
        return entries;
    }

    public byte[]? GetResource(string path)
    {
        var fullPath = Path.GetFullPath(Path.Combine(_resourceRoot, path));
        // Prevent directory traversal
        if (!fullPath.StartsWith(Path.GetFullPath(_resourceRoot) + Path.DirectorySeparatorChar)
            && fullPath != Path.GetFullPath(_resourceRoot) + Path.DirectorySeparatorChar.ToString().TrimEnd())
            return null;
        if (!File.Exists(fullPath)) return null;
        return File.ReadAllBytes(fullPath);
    }

    private static string ComputeHash(string filePath)
    {
        using var sha = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = sha.ComputeHash(stream);
        return Convert.ToHexStringLower(hash);
    }
}
