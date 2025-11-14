using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicPlayerWeb.Models;

public class PlaylistIterator : ITrackIterator
{
    private readonly IList<Track> _tracks;
    private int _index;

    public PlaylistIterator(IEnumerable<Track> tracks)
    {
        _tracks = tracks.ToList();
        _index = _tracks.Count > 0 ? 0 : -1;
    }

    public bool HasNext() => _index >= 0 && _index < _tracks.Count - 1;

    public Track Next()
    {
        if (HasNext())
        {
            _index++;
        }
        return Current();
    }

    public bool HasPrevious() => _index > 0;

    public Track Previous()
    {
        if (HasPrevious())
        {
            _index--;
        }
        return Current();
    }

    public Track Current()
    {
        if (_index < 0 || _index >= _tracks.Count)
            throw new InvalidOperationException("Немає поточного треку.");

        return _tracks[_index];
    }
}
