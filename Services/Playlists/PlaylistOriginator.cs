using System;
using System.Collections.Generic;
using System.Linq;
using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Services.Playlists;

public class PlaylistOriginator
{
    public Guid PlaylistId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public IReadOnlyList<Guid> TrackIds { get; private set; } = Array.Empty<Guid>();

    public void Load(Playlist playlist, IEnumerable<Track> tracks)
    {
        PlaylistId = playlist.Id;
        Title = playlist.Title;
        TrackIds = tracks.Select(t => t.Id).ToArray();
    }

    public PlaylistMemento Save()
    {
        return new PlaylistMemento(PlaylistId, Title, TrackIds.ToArray());
    }

    public void Restore(PlaylistMemento memento)
    {
        PlaylistId = memento.PlaylistId;
        Title = memento.Title;
        TrackIds = memento.TrackIds.ToArray();
    }
}
