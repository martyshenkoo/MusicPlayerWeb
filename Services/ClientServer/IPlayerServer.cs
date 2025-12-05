using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Services.ClientServer;

public interface IPlayerServer
{
    PlayerOperationResult Play(string username, string title, string url);
    PlayerOperationResult Pause(string username);
    PlayerOperationResult Stop(string username);
}
