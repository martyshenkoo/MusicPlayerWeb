using System.Collections.Generic;

namespace MusicPlayerWeb.Services.Playlists;

public class PlaylistMemento
{
    public PlaylistMemento(Guid playlistId, string title, IReadOnlyList<Guid> trackIds)
    {
        PlaylistId = playlistId;
        Title = title;
        TrackIds = trackIds;
        SavedAtUtc = DateTime.UtcNow;
    }

    public Guid PlaylistId { get; }
    public string Title { get; }
    public IReadOnlyList<Guid> TrackIds { get; }
    public DateTime SavedAtUtc { get; }
}
