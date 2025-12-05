using System;
using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Services.Visitors;

public class PlaylistVisitorService
{
    private readonly IPlaylistService _playlists;

    public PlaylistVisitorService(IPlaylistService playlists)
    {
        _playlists = playlists;
    }

    public PlaylistStatistics GetStatistics(string username, Guid playlistId)
    {
        var playlist = _playlists.GetById(username, playlistId);
        if (playlist == null)
            return PlaylistStatistics.Empty;

        var visitor = new PlaylistStatisticsVisitor();
        playlist.Accept(visitor);
        return visitor.ToStatistics();
    }
}
