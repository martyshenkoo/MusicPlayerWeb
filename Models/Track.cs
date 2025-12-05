namespace MusicPlayerWeb.Models;

public class Track : IPlaylistVisitable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public string RelativeUrl { get; set; } = default!;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public string? OwnerUsername { get; set; }

    public void Accept(IPlaylistVisitor visitor)
    {
        visitor.VisitTrack(this);
    }
}
