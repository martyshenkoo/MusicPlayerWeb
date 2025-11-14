namespace MusicPlayerWeb.Models;

public interface ITrackIterator
{
    bool HasNext();
    Track Next();
    bool HasPrevious();
    Track Previous();
    Track Current();
}
