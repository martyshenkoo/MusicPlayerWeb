using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Services;

public interface ITrackService
{
    IEnumerable<Track> GetForUser(string username);
    Track AddForUser(string username, string title, string fileName, string relUrl);
    bool Delete(string username, Guid id);
}
