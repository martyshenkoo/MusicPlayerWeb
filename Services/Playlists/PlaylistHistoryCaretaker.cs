using System.Collections.Generic;

namespace MusicPlayerWeb.Services.Playlists;

public class PlaylistHistoryCaretaker
{
    private readonly Stack<PlaylistMemento> _history = new();
    private readonly object _sync = new();

    public void Push(PlaylistMemento memento)
    {
        lock (_sync)
        {
            _history.Push(memento);
        }
    }

    public PlaylistMemento? Pop()
    {
        lock (_sync)
        {
            if (_history.Count == 0)
                return null;

            return _history.Pop();
        }
    }

    public bool HasHistory
    {
        get
        {
            lock (_sync)
            {
                return _history.Count > 0;
            }
        }
    }
}
