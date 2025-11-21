using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Services;

public class PlayerStateService
{
    private readonly object _sync = new();
    private PlayerStateSnapshot _snapshot = PlayerStateSnapshot.Empty;

    public PlayerStateSnapshot Snapshot
    {
        get
        {
            lock (_sync)
            {
                return _snapshot;
            }
        }
    }

    public void Play(string title, string url)
    {
        lock (_sync)
        {
            _snapshot = PlayerStateSnapshot.Create(title, url, PlayerPlaybackState.Playing);
        }
    }

    public void Pause()
    {
        lock (_sync)
        {
            if (!_snapshot.HasTrack)
                return;

            _snapshot = PlayerStateSnapshot.Create(_snapshot.Title, _snapshot.Url, PlayerPlaybackState.Paused);
        }
    }

    public void Stop()
    {
        lock (_sync)
        {
            _snapshot = PlayerStateSnapshot.Create(null, null, PlayerPlaybackState.Stopped);
        }
    }
}
