namespace MusicPlayerWeb.Models;

public class Track
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public string RelativeUrl { get; set; } = default!; // напр. /uploads/xxx.mp3
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public string? OwnerUsername { get; set; }
}
