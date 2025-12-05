using System.Collections.Generic;
using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Services.Visitors;

public class PlaylistStatisticsVisitor : IPlaylistVisitor
{
    private readonly List<string> _titles = new();

    public string? PlaylistTitle { get; private set; }
    public int TrackCount { get; private set; }

    public void VisitPlaylist(Playlist playlist)
    {
        PlaylistTitle = playlist.Title;
    }

    public void VisitTrack(Track track)
    {
        TrackCount++;
        _titles.Add(track.Title);
    }

    public PlaylistStatistics ToStatistics()
    {
        return new PlaylistStatistics
        {
            PlaylistTitle = PlaylistTitle,
            TrackCount = TrackCount,
            TrackTitles = _titles.AsReadOnly()
        };
    }
}
