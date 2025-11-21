namespace MusicPlayerWeb.Models;

public class PlayerStateSnapshot
{
    public string? Title { get; init; }
    public string? Url { get; init; }
    public PlayerPlaybackState State { get; init; }
    public DateTime UpdatedAtUtc { get; init; }

    public bool HasTrack => !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Url);

    public static PlayerStateSnapshot Create(string? title, string? url, PlayerPlaybackState state)
    {
        return new PlayerStateSnapshot
        {
            Title = title,
            Url = url,
            State = state,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }

    public static PlayerStateSnapshot Empty => Create(null, null, PlayerPlaybackState.Stopped);
}
