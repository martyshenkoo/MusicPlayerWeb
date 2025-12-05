using System;
using System.Collections.Generic;

namespace MusicPlayerWeb.Models;

public class PlaylistStatistics
{
    public string? PlaylistTitle { get; init; }
    public int TrackCount { get; init; }
    public IReadOnlyCollection<string> TrackTitles { get; init; } = Array.Empty<string>();

    public static PlaylistStatistics Empty => new PlaylistStatistics
    {
        PlaylistTitle = null,
        TrackCount = 0,
        TrackTitles = Array.Empty<string>()
    };
}
