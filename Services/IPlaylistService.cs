using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Services;

public interface IPlaylistService
{
    IEnumerable<Playlist> GetForUser(string username);
    Playlist? GetById(string username, Guid id);
    Playlist Create(string username, string title);
    IEnumerable<Track> GetTracks(string username, Guid playlistId);

    bool AddTrack(string username, Guid playlistId, Guid trackId);
    bool RemoveTrack(string username, Guid playlistId, Guid trackId);
    bool Restore(string username, Guid playlistId, string title);

    bool Rename(string username, Guid playlistId, string newTitle);
    bool Delete(string username, Guid playlistId);
}
