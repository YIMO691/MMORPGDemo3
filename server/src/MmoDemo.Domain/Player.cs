namespace MmoDemo.Domain;

public class Player
{
    public string PlayerId { get; init; } = string.Empty;
    public List<PlayerSession> Sessions { get; init; } = [];
    public List<string> RoleIds { get; init; } = [];
    public string? SelectedRoleId { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public class PlayerSession
{
    public string Token { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
