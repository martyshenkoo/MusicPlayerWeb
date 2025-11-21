namespace MusicPlayerWeb.Models;

public class PlaylistPageViewModel
{
    public IEnumerable<Playlist> Playlists { get; set; } = Enumerable.Empty<Playlist>();
    public Guid? SelectedPlaylistId { get; set; }
    public string? SelectedPlaylistTitle { get; set; }
    public IEnumerable<Track> Tracks { get; set; } = Enumerable.Empty<Track>();
    public PlayerStateSnapshot PlayerState { get; set; } = PlayerStateSnapshot.Empty;
    public IReadOnlyCollection<PlayerCommandHistoryItem> CommandHistory { get; set; } = Array.Empty<PlayerCommandHistoryItem>();
}
