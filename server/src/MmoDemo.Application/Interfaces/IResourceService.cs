namespace MmoDemo.Application;

public interface IResourceService
{
    List<ResourceManifestEntry> GetManifest();
    byte[]? GetResource(string path);
}

public record ResourceManifestEntry(string Name, string Hash, long Size);
