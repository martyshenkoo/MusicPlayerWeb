using System;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using MusicPlayerWeb.Services;

namespace MusicPlayerWeb.Services.Playlists;

public class PlaylistHistoryService : IPlaylistHistoryService
{
    private readonly IPlaylistService _playlists;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan HistoryLifetime = TimeSpan.FromMinutes(30);

    public PlaylistHistoryService(IPlaylistService playlists, IMemoryCache cache)
    {
        _playlists = playlists;
        _cache = cache;
    }

    public bool Backup(string username, Guid playlistId)
    {
        var playlist = _playlists.GetById(username, playlistId);
        if (playlist == null)
            return false;

        var tracks = _playlists.GetTracks(username, playlistId).ToList();
        var originator = new PlaylistOriginator();
        originator.Load(playlist, tracks);

        var caretaker = _cache.GetOrCreate(BuildKey(username, playlistId), entry =>
        {
            entry.SlidingExpiration = HistoryLifetime;
            return new PlaylistHistoryCaretaker();
        });

        if (caretaker == null)
            return false;

        caretaker.Push(originator.Save());
        return true;
    }

    public bool Undo(string username, Guid playlistId)
    {
        if (!_cache.TryGetValue(BuildKey(username, playlistId), out PlaylistHistoryCaretaker? caretaker) || caretaker == null)
            return false;

        var snapshot = caretaker.Pop();
        if (snapshot == null)
            return false;

        var originator = new PlaylistOriginator();
        originator.Restore(snapshot);
        return ApplyState(username, originator);
    }

    public bool HasHistory(string username, Guid playlistId)
    {
        if (!_cache.TryGetValue(BuildKey(username, playlistId), out PlaylistHistoryCaretaker? caretaker) || caretaker == null)
            return false;

        return caretaker.HasHistory;
    }

    private bool ApplyState(string username, PlaylistOriginator originator)
    {
        var restored = _playlists.Restore(username, originator.PlaylistId, originator.Title);
        if (!restored)
            return false;

        var existing = _playlists.GetTracks(username, originator.PlaylistId)
            .Select(t => t.Id)
            .ToList();

        foreach (var trackId in existing)
        {
            _playlists.RemoveTrack(username, originator.PlaylistId, trackId);
        }

        foreach (var trackId in originator.TrackIds)
        {
            _playlists.AddTrack(username, originator.PlaylistId, trackId);
        }

        return true;
    }

    private static string BuildKey(string username, Guid playlistId)
    {
        return $"playlist-history:{username}:{playlistId}";
    }
}
