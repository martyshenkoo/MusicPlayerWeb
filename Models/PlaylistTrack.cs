namespace MusicPlayerWeb.Models;

public class PlaylistTrack
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PlaylistId { get; set; }
    public Playlist Playlist { get; set; } = default!;

    public Guid TrackId { get; set; }
    public Track Track { get; set; } = default!;

    public int Order { get; set; }
}
