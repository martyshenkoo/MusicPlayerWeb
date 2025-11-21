namespace MusicPlayerWeb.Models;

public class PlayerCommandHistoryItem
{
    public required string CommandName { get; init; }
    public DateTime ExecutedAtUtc { get; init; } = DateTime.UtcNow;
    public string ExecutedBy { get; init; } = "system";
}
