using Microsoft.EntityFrameworkCore;
using MusicPlayerWeb.Data;
using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Services;

public class DbTrackService : ITrackService
{
    private readonly AppDbContext _db;

    public DbTrackService(AppDbContext db) => _db = db;

    public IEnumerable<Track> GetForUser(string username)
    {
        return _db.Tracks
                  .AsNoTracking()
                  .Where(t => t.OwnerUsername == username)
                  .OrderByDescending(t => t.AddedAt)
                  .ToList();
    }

    public Track AddForUser(string username, string title, string fileName, string relUrl)
    {
        var track = new Track
        {
            Title = string.IsNullOrWhiteSpace(title) ? fileName : title.Trim(),
            FileName = fileName,
            RelativeUrl = relUrl,
            OwnerUsername = username,
            AddedAt = DateTime.UtcNow
        };
        _db.Tracks.Add(track);
        _db.SaveChanges();
        return track;
    }

    public bool Delete(string username, Guid id)
    {
        var tr = _db.Tracks.FirstOrDefault(t => t.Id == id && t.OwnerUsername == username);
        if (tr == null) return false;

        _db.Tracks.Remove(tr);
        _db.SaveChanges();
        return true;
    }
}
