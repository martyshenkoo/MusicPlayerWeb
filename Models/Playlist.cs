namespace MusicPlayerWeb.Models;

public class Playlist
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = default!;
    public string OwnerUsername { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PlaylistTrack> PlaylistTracks { get; set; } = new List<PlaylistTrack>();

    public ITrackIterator CreateIterator(IEnumerable<Track> tracks)
    {
        return new PlaylistIterator(tracks);
    }
}
