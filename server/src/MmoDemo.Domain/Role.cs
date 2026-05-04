namespace MmoDemo.Domain;

public class Role
{
    public string RoleId { get; init; } = string.Empty;
    public string PlayerId { get; init; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public int ClassId { get; set; }
    public int SceneId { get; set; } = 1001;
    public long Gold { get; set; } = 100;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
