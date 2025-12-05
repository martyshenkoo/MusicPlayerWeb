namespace MusicPlayerWeb.Models;

public interface IPlaylistVisitor
{
    void VisitPlaylist(Playlist playlist);
    void VisitTrack(Track track);
}

public interface IPlaylistVisitable
{
    void Accept(IPlaylistVisitor visitor);
}
