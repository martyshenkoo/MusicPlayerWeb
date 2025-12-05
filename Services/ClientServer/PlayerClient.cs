using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Services.ClientServer;

public class PlayerClient
{
    private readonly IPlayerServer _server;

    public PlayerClient(IPlayerServer server)
    {
        _server = server;
    }

    public PlayerOperationResult RequestPlay(string username, string title, string url)
        => _server.Play(username, title, url);

    public PlayerOperationResult RequestPause(string username)
        => _server.Pause(username);

    public PlayerOperationResult RequestStop(string username)
        => _server.Stop(username);
}
