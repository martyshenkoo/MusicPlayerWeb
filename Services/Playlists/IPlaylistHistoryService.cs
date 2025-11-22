namespace MusicPlayerWeb.Services.Playlists;

public interface IPlaylistHistoryService
{
    bool Backup(string username, Guid playlistId);
    bool Undo(string username, Guid playlistId);
    bool HasHistory(string username, Guid playlistId);
}
