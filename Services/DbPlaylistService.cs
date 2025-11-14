using Microsoft.EntityFrameworkCore;
using MusicPlayerWeb.Data;
using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Services;

public class DbPlaylistService : IPlaylistService
{
    private readonly AppDbContext _db;

    public DbPlaylistService(AppDbContext db) => _db = db;

    public IEnumerable<Playlist> GetForUser(string username)
    {
        return _db.Playlists
            .Where(p => p.OwnerUsername == username)
            .OrderBy(p => p.CreatedAt)
            .ToList();
    }

    public Playlist? GetById(string username, Guid id)
    {
        return _db.Playlists
            .Include(p => p.PlaylistTracks)
            .ThenInclude(pt => pt.Track)
            .FirstOrDefault(p => p.Id == id && p.OwnerUsername == username);
    }

    public Playlist Create(string username, string title)
    {
        var pl = new Playlist
        {
            Title = title.Trim(),
            OwnerUsername = username
        };

        _db.Playlists.Add(pl);
        _db.SaveChanges();
        return pl;
    }

    public IEnumerable<Track> GetTracks(string username, Guid playlistId)
    {
        return _db.PlaylistTracks
            .Include(pt => pt.Track)
            .Include(pt => pt.Playlist)
            .Where(pt => pt.PlaylistId == playlistId &&
                         pt.Playlist.OwnerUsername == username)
            .OrderBy(pt => pt.Order)
            .Select(pt => pt.Track)
            .ToList();
    }

    public bool AddTrack(string username, Guid playlistId, Guid trackId)
    {
        var playlist = _db.Playlists.FirstOrDefault(p => p.Id == playlistId && p.OwnerUsername == username);
        if (playlist == null) return false;

        var exists = _db.PlaylistTracks.Any(pt => pt.PlaylistId == playlistId && pt.TrackId == trackId);
        if (exists) return true;

        int maxOrder = _db.PlaylistTracks
            .Where(pt => pt.PlaylistId == playlistId)
            .Select(pt => (int?)pt.Order)
            .Max() ?? 0;

        _db.PlaylistTracks.Add(new PlaylistTrack
        {
            PlaylistId = playlistId,
            TrackId = trackId,
            Order = maxOrder + 1
        });

        _db.SaveChanges();
        return true;
    }

    public bool RemoveTrack(string username, Guid playlistId, Guid trackId)
    {
        var pt = _db.PlaylistTracks
            .Include(x => x.Playlist)
            .FirstOrDefault(x =>
                x.PlaylistId == playlistId &&
                x.TrackId == trackId &&
                x.Playlist.OwnerUsername == username);

        if (pt == null) return false;

        _db.PlaylistTracks.Remove(pt);
        _db.SaveChanges();
        return true;
    }

    public bool Rename(string username, Guid playlistId, string newTitle)
    {
        var pl = _db.Playlists.FirstOrDefault(p => p.Id == playlistId && p.OwnerUsername == username);
        if (pl == null) return false;

        pl.Title = newTitle.Trim();
        _db.SaveChanges();
        return true;
    }

    public bool Delete(string username, Guid playlistId)
    {
        var pl = _db.Playlists.FirstOrDefault(p => p.Id == playlistId && p.OwnerUsername == username);
        if (pl == null) return false;

        _db.Playlists.Remove(pl);
        _db.SaveChanges();
        return true;
    }
}
